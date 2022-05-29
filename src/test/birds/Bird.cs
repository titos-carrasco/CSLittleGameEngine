using System;
using System.Drawing;
using rcr.lge;

namespace test
{
    namespace birds
    {
        public class Bird : Sprite
        {

            public Bird(String inames, PointF position) :
                base(inames, position)
            {
            }

            public override void OnUpdate(float dt)
            {
                NextImage(dt, 0.060f);
            }
        }
    }
}
