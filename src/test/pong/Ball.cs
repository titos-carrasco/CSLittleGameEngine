using System;
using System.Drawing;
using rcr.lge;

namespace test
{
    namespace pong
    {

        public class Ball : Canvas
        {
            private readonly LittleGameEngine lge;
            private float initX;
            private float initY;
            private float speedX = -120;
            private float speedY = 120;

            public Ball(PointF position, SizeF size, String name) :
                base(position, size, name)
            {
                lge = LittleGameEngine.GetInstance();
                EnableCollider(true);
                Fill(Color.White);

                initX = position.X;
                initY = position.Y;
            }

            public override void OnUpdate(float dt)
            {
                float dx = speedX * dt;
                float dy = speedY * dt;

                SetPosition(GetX() + dx, GetY() + dy);
            }

            public override void OnPostUpdate(float dt)
            {
                GameObject[] gobjs = lge.CollidesWith(this);
                if (gobjs != null)
                {
                    float x = GetX();
                    float y = GetY();
                    float dx = speedX * dt;
                    float dy = speedY * dt;

                    foreach (GameObject gobj in gobjs)
                    {
                        if (gobj.GetTag().Equals("wall-horizontal"))
                        {
                            speedY = -speedY;
                            dy = -dy;
                        }
                        else if (gobj.GetTag().Equals("paddle"))
                        {
                            speedX = -speedX;
                            dx = -dx;
                        }
                        else if (gobj.GetTag().Equals("wall-vertical"))
                        {
                            x = initX;
                            y = initY;
                        }
                    }
                    SetPosition(x + dx, y + dy);
                }
            }
        }
    }
}
