using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace bouncing
    {
        public class Bouncing
        {
            private readonly LittleGameEngine lge;
            private readonly Canvas ground;

            public Bouncing()
            {
                // creamos el juego
                Size winSize = new Size(800, 440);

                lge = new LittleGameEngine(winSize, "Bouncing Balls", Color.White);
                lge.onMainUpdate = OnMainUpdate;

                // cargamos los recursos que usaremos
                lge.fontManager.LoadSysFont("monospace", "Courier New", FontStyle.Regular, 10);

                // agregamos el suelo
                ground = new Canvas(new PointF(0, 340), new Size(800, 100), "ground");
                ground.Fill(Color.Gray);
                ground.SetTag("ground");
                ground.EnableCollider(true);
                lge.AddGObject(ground, 1);

                // los objetos a rebotar
                var rand = new Random();
                for (int i = 0; i < 200; i++)
                {
                    float x = 50.0f + (float)rand.Next(700);
                    float y = 50.0f + (float)rand.Next(150);
                    float vx = -50.0f + (float)rand.Next(100);
                    float vy = 0.0f;
                    Ball gobj = new Ball(x, y, vx, vy);
                    lge.AddGObject(gobj, 1);
                }

                // agregamos la barra de info
                Canvas infobar = new Canvas(new PointF(0, 0), new Size(800, 20), "infobar");
                lge.AddGObjectGUI(infobar);
            }

            public void OnMainUpdate(float dt)
            {
                // abortamos con la tecla Escape
                if (lge.KeyPressed(Keys.Escape))
                    lge.Quit();

                // mostramos la info
                Point mousePosition = lge.GetMousePosition();
                bool[] mouseButtons = lge.GetMouseButtons();

                String info = String.Format("FPS: {0,-6:f} - LPS: {1,-6:f} - gObjs: {2} - Mouse: ({3},{4}) ({5},{6},{7})",
                                            lge.GetFPS(),
                                            lge.GetLPS(),
                                            lge.GetCountGObjects(),
                                            mousePosition.X, mousePosition.Y,
                                            mouseButtons[0] ? 1 : 0,
                                            mouseButtons[1] ? 1 : 0,
                                            mouseButtons[2] ? 1 : 0
                                    );
                Canvas infobar = (Canvas)lge.GetGObject("infobar");
                infobar.Fill(Color.FromArgb(0x10, 0x20, 0x20, 0x20));
                infobar.DrawText(info, new PointF(40, 3), "monospace", Color.Black);
            }

            // main loop
            public void Run(int fps)
            {
                lge.Run(fps);
            }

            // show time
            public static void Main()
            {
                Bouncing game = new Bouncing();
                game.Run(60);
                Console.WriteLine("Eso es todo!!!");
            }
        }
    }
}
