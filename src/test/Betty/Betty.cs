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
                // float pixels = velocity*dt;
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
                    x = x + pixels;
                }
                else if (lge.KeyPressed(Keys.Left))
                {
                    SetImage("betty_left", idx);
                    x = x - pixels;
                }
                else if (lge.KeyPressed(Keys.Up))
                {
                    SetImage("betty_up", idx);
                    y = y - pixels;
                }
                else if (lge.KeyPressed(Keys.Down))
                {
                    SetImage("betty_down", idx);
                    y = y + pixels;
                }
                else
                {
                    SetImage("betty_idle", idx);
                    if (x % 32 < 4)
                        x = (float)Math.Round(x / 32) * 32;
                    else if (x % 32 > 28)
                        x = (float)Math.Round((x + 32) / 32) * 32;
                    if (y % 32 < 4)
                        y = (float)Math.Round(y / 32) * 32;
                    else if (y % 32 > 28)
                        y = (float)Math.Round((y + 32) / 32) * 32;
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
                        SetPosition(lastPoint);
                    }
            }
        }
    }
}
