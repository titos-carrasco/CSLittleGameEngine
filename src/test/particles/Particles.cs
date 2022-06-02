using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace particles
    {
        public class Particles
        {
            private readonly LittleGameEngine lge;
            private Canvas panel;
            int numParticles = 500;
            Particle[] particles;

            public Particles()
            {
                // creamos el juego
                Size winSize = new Size(800, 440);

                lge = new LittleGameEngine(winSize, "Particles", Color.White);
                lge.onMainUpdate = OnMainUpdate;

                // cargamos los recursos que usaremos
                lge.LoadSysFont("monospace", "Courier New", FontStyle.Regular, 12);

                // agregamos la barra de info
                Canvas infobar = new Canvas(new PointF(0, 0), new Size(800, 20), "infobar");
                lge.AddGObjectGUI(infobar);

                // un canvas para plotear
                panel = new Canvas(new PointF(0, 0), new Size(800, 600), "Panel");
                panel.Fill(Color.White);
                lge.AddGObject(panel, 1);

                // las particulas
                numParticles = 500;
                particles = new Particle[numParticles];
                var rand = new Random();
                for (int i = 0; i < numParticles; i++)
                {
                    float x = 300 + (float)rand.Next(200);
                    float y = 100 + (float)rand.Next(100);
                    float vx = -120 + (float)rand.Next(240);
                    float vy = -120 + (float)rand.Next(240);
                    float m = 0.1f + (float)rand.NextDouble();
                    particles[i] = new Particle(x, y, vx, vy, m);
                }
            }

            public void OnMainUpdate(float dt)
            {
                // abortamos con la tecla Escape
                if (lge.KeyPressed(Keys.Escape))
                    lge.Quit();

                // mostramos la info
                Point mousePosition = lge.GetMousePosition();
                bool[] mouseButtons = lge.GetMouseButtons();

                String info = String.Format("FPS: {0,-6:f} - gObjs: {1} - Mouse: ({2},{3}) ({4},{5},{6})",
                                            lge.GetFPS(),
                                            lge.GetCountGObjects(),
                                            mousePosition.X, mousePosition.Y,
                                            mouseButtons[0] ? 1 : 0,
                                            mouseButtons[1] ? 1 : 0,
                                            mouseButtons[2] ? 1 : 0
                                    );
                Canvas infobar = (Canvas)lge.GetGObject("infobar");
                infobar.Fill(Color.FromArgb(0x10, 0x20, 0x20, 0x20));
                infobar.DrawText(info, new PointF(200, 0), "monospace", Color.Black);

                // las particulas
                for (int i = 0; i < numParticles; i++)
                {
                    Particle particle = particles[i];
                    particle.Update(dt);
                }

                panel.Fill(Color.White);
                for (int i = 0; i < numParticles; i++)
                {
                    Particle particle = particles[i];
                    float x = particle.x;
                    float y = particle.y;
                    int r = (int)(particle.m * 5);
                    panel.DrawRectangle(new PointF(x, y), new Size(r, r), Color.Black, false);
                }
            }

            // main loop
            public void Run(int fps)
            {
                lge.Run(fps);
            }

            // show time
            public static void Main(String[] args)
            {
                Particles game = new Particles();
                game.Run(60);
                Console.WriteLine("Eso es todo!!!");
            }

        }
    }
}
