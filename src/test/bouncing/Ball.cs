using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace bouncing
    {
        public class Ball : Canvas
        {
            private readonly LittleGameEngine lge;

            private float vx, vy;
            private float g, e;
            private GameObject ground;

            public Ball(float x, float y, float vx, float vy) :
                base(new PointF(x, y), new Size(20, 20))
            {
                // acceso al motor de juegos
                lge = LittleGameEngine.GetInstance();

                this.vx = vx;
                this.vy = vy;
                g = 240.0f;
                e = 0.4f;
                EnableCollider(true);
                ground = lge.GetGObject("ground");

                Color fillColor = Color.FromArgb(100, 255, 0, 64);
                Fill(fillColor);
            }

            public override void OnUpdate(float dt)
            {
                float x = GetX() + vx * dt;
                float y = GetY() + vy * dt;

                if (x < 0)
                {
                    lge.DelGObject(this);
                    return;
                }

                vy += g * dt;
                SetPosition(x, y);
            }

            public override void OnPostUpdate(float dt)
            {
                if (CollidesWith(ground))
                {
                    float x = GetX();
                    float y = ground.GetY() - GetHeight();
                    SetPosition(x, y);

                    vy = -vy * e;
                    if (Math.Abs(vy) < 50)
                    {
                        vy = 0;
                        vx = 0;
                        g = 0;
                    }
                }

            }

        }
    }
}
