namespace test
{
    namespace particles
    {
        public class Particle
        {

            protected internal float x, y, vx, vy, m;

            public Particle(float x, float y, float vx, float vy, float m)
            {
                this.x = x;
                this.y = y;
                this.vx = vx;
                this.vy = vy;
                this.m = m;
            }

            public float ComputeYForce()
            {
                // return m * -9.81;
                return m * -60;
            }

            public void Update(float dt)
            {
                float fx = 0;
                float fy = ComputeYForce();
                float ax = fx / m;
                float ay = fy / m;
                vx = vx + ax * dt;
                vy = vy + ay * dt;
                x = x + vx * dt;
                y = y - vy * dt;
            }
        }
    }
}
