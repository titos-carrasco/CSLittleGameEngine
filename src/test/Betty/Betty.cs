using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace betty
    {
        public class Betty : Sprite
        {
            private readonly LittleGameEngine lge;

            private bool alive;
            private Size winSize;
            private PointF lastPoint;

            public Betty(String name, Size winSize) :
                base("betty_idle", new PointF(0, 0), name)
            {

                // acceso al motor de juegos
                lge = LittleGameEngine.GetInstance();

                SetTag("Betty");
                EnableCollider(true);
                alive = true;
                this.winSize = winSize;
            }

            public bool IsAlive()
            {
                return alive;
            }

            public void SetAlive(bool alive)
            {
                this.alive = alive;
                SetImage("betty_idle");
            }

            public override void OnUpdate(float dt)
            {
                // solo si estoy viva
                if (!alive)
                    return;

                // velocity = pixeles por segundo
                // float velocity = 120;
                // float pixels = velocity * dt;
                float pixels = 2;

                // nuestra posicion actual y tamano
                float x = GetX();
                float y = GetY();
                lastPoint = new PointF(x, y);

                // cambiamos sus coordenadas e imagen segun la tecla presionada
                int idx = GetImagesIndex();
                if (lge.KeyPressed(Keys.Right))
                {
                    SetImage("betty_right", idx);
                    x += pixels;
                }
                else if (lge.KeyPressed(Keys.Left))
                {
                    SetImage("betty_left", idx);
                    x -= pixels;
                }
                else if (lge.KeyPressed(Keys.Up))
                {
                    SetImage("betty_up", idx);
                    y -= pixels;
                }
                else if (lge.KeyPressed(Keys.Down))
                {
                    SetImage("betty_down", idx);
                    y += pixels;
                }
                else
                {
                    SetImage("betty_idle", idx);
                    x = (int)(x / 4) * 4;
                    y = (int)(y / 4) * 4;
                }

                // tunel?
                if (x < -16)
                    x = winSize.Width - 16;
                else if (x > winSize.Width - 16)
                    x = -16;

                // siguiente imagen de la secuencia
                SetPosition(x, y);
                NextImage(dt, 0.1f);
            }

            public override void OnPostUpdate(float dt)
            {
                if (!alive)
                    return;

                GameObject[] gobjs = lge.CollidesWith(this);
                foreach (GameObject gobj in gobjs)
                    if (gobj.GetTag().Equals("zombie"))
                    {
                        alive = false;
                        Console.WriteLine("Un zombie me mato");
                        return;
                    }
                    else if (gobj.GetTag().Equals("muro"))
                    {
                        float x = GetX();
                        float y = GetY();
                        float xo = gobj.GetX();
                        float yo = gobj.GetY();

                        if (lastPoint.X < x) x = xo - GetWidth();
                        else if (lastPoint.X > x) x = xo + gobj.GetWidth();

                        if (lastPoint.Y < y) y = yo - GetHeight();
                        else if (lastPoint.Y > y) y = yo + gobj.GetHeight();

                        SetPosition(x, y);
                    }
            }
        }
    }
}
