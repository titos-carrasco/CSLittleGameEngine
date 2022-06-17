using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace cementerio
    {
        public class Platform : Sprite
        {
            char dir;
            private readonly float pixels;
            private readonly float distance;
            private float travel = 0;

            public Platform(float x, float y, char dir, float distance, float speed) :
                base("platform", new PointF(x, y))
            {
                // los eventos que recibiremos
                SetCollider(new RectangleF(new PointF(0, 0), new SizeF(GetWidth(), 1)));
                EnableCollider(true);
                SetTag("plataforma");

                // mis atributos
                this.dir = dir;
                this.pixels = speed;
                this.distance = distance;
            }

            public char GetDir()
            {
                return dir;
            }

            public float GetSpeed()
            {
                return pixels;
            }

            public override void OnUpdate(float dt)
            {
                PointF position = GetPosition();
                float x = position.X;
                float y = position.Y;

                float d = pixels * dt;
                if (dir == 'R')
                    x += d;
                else if (dir == 'L')
                    x -= d;
                else if (dir == 'D')
                    y += d;
                else if (dir == 'U')
                    y -= d;

                SetPosition(x, y);

                travel += d;
                if (travel > distance)
                {
                    travel = 0;
                    if (dir == 'R')
                        dir = 'L';
                    else if (dir == 'L')
                        dir = 'R';
                    else if (dir == 'D')
                        dir = 'U';
                    else if (dir == 'U')
                        dir = 'D';
                }
            }
        }
    }
}
