using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace cementerio
    {
        public class Ninja : Sprite
        {
            private readonly LittleGameEngine lge;
            private RectangleF colisionador;
            private readonly float vx = 120;
            private float vy = 0;
            private readonly float vym = 500;
            private readonly float g = 480;
            private readonly float vsalto = 140;

            public Ninja(float x, float y) :
                base("ninja-idle-right", new PointF(x, y))
            {
                // acceso a LGE
                lge = LittleGameEngine.GetInstance();

                // los eventos que recibiremos
                EnableCollider(true);

                // el colisionador
                colisionador = new RectangleF(new PointF(20, 5), new SizeF(15, GetHeight() - 5));
                SetCollider(colisionador);
            }

            public bool FixPosition(float dx, float dy, float dt)
            {
                GameObject[] gobjs = lge.CollidesWith(this);
                foreach (GameObject gobj in gobjs)
                {
                    String tag = gobj.GetTag();
                    if (tag.Equals("suelo"))
                    {
                        SetPosition(GetX(), gobj.GetY() - GetHeight());
                        return true;
                    }
                    else if (tag.Equals("plataforma"))
                    {
                        Platform p = (Platform)gobj;
                        if (p.GetDir() == 'L')
                            SetPosition(GetX() - p.GetSpeed() * dt, p.GetY() - GetHeight() + 1);
                        else if (p.GetDir() == 'R')
                            SetPosition(GetX() + p.GetSpeed() * dt, p.GetY() - GetHeight() + 1);
                        else if (p.GetDir() == 'U')
                            SetPosition(GetX(), GetY() - p.GetSpeed() * dt);
                        else if (p.GetDir() == 'D')
                            SetPosition(GetX(), GetY() + p.GetSpeed() * dt);
                        return true;
                    }
                }
                return false;
            }

            // despues de que todo fue actualizado
            public override void OnPostUpdate(float dt)
            {
                // nuestra posicion actual
                PointF position = GetPosition();
                float x = position.X;
                float y = position.Y;
                float x0 = x;
                float y0 = y;

                // primero el movimiento en X
                int move_x = 0;
                if (lge.KeyPressed(Keys.Left))
                {
                    move_x = -1;
                    SetImage("ninja-run-left");
                }
                else if (lge.KeyPressed(Keys.Right))
                {
                    move_x = 1;
                    SetImage("ninja-run-right");
                }
                else
                {
                    SetImage("ninja-idle-right");
                }
                x += move_x * vx * dt;

                // siguiente imagen y su colisionador
                NextImage(dt, 0.04f);
                SetCollider(colisionador);

                // ahora el movimiento en Y
                y += vy * dt;
                vy += g * dt;

                // nueva posicion
                SetPosition(x, y);

                // estamos en un suelo?
                bool onfloor = FixPosition(x - x0, y - y0, dt);

                // nos piden saltar
                if (onfloor)
                {
                    if (lge.KeyPressed(Keys.Space))
                        vy = -vsalto;
                    else
                        vy = 0;
                }

                // limitamos a velocidad en Y
                if (vy > vym)
                    vy = vym;
            }
        }
    }
}
