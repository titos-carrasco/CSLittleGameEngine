using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace cementerio
    {
        public class Game
        {
            private readonly LittleGameEngine lge;

            public Game(String resourceDir)
            {
                Size winSize = new Size(640, 342);

                lge = new LittleGameEngine(winSize, "El Cementerio", Color.Black);
                lge.onMainUpdate = OnMainUpdate;

                // cargamos los recursos que usaremos
                lge.LoadImage("fondo", resourceDir + "/fondo.png", false, false);
                lge.LoadImage("ninja-idle-right", resourceDir + "/NinjaGirl/Idle_*.png", 0.1f, false, false);
                lge.LoadImage("ninja-idle-left", resourceDir + "/NinjaGirl/Idle_*.png", 0.1f, true, false);
                lge.LoadImage("ninja-run-right", resourceDir + "/NinjaGirl/Run_*.png", 0.1f, false, false);
                lge.LoadImage("ninja-run-left", resourceDir + "/NinjaGirl/Run_*.png", 0.1f, true, false);
                lge.LoadImage("platform", resourceDir + "/platform.png", 0.3f, false, false);

                // el fondo
                Sprite fondo = new Sprite("fondo", new PointF(0, 0));
                lge.AddGObject(fondo, 0);

                // los NonPlayer Characters (NPC)
                makeFloor();
                makePlatforms();

                // nuestra heroina
                Ninja ninja = new Ninja(90, 163);
                ninja.SetBounds(new RectangleF(new PointF(0, 0), new Size(winSize.Width, winSize.Height + 100)));
                lge.AddGObject(ninja, 1);
            }

            public void makeFloor()
            {
                Canvas[] suelos = new Canvas[] { new Canvas(new PointF(0, 85), new Size(170, 1)),
                        new Canvas(new PointF(0, 214), new Size(170, 1)),
                        new Canvas(new PointF(214, 300), new Size(128, 1)),
                        new Canvas(new PointF(342, 214), new Size(127, 1)),
                        new Canvas(new PointF(470, 257), new Size(127, 1)),
                        new Canvas(new PointF(513, 86), new Size(127, 1)) };

                foreach (Canvas s in suelos)
                {
                    s.EnableCollider(true);
                    s.SetTag("suelo");
                    lge.AddGObject(s, 1);
                }
            }

            public void makePlatforms()
            {
                Platform[] platforms = new Platform[] { new Platform(200, 200, 'U', 100, 60),
                        new Platform(400, 100, 'L', 100, 60) };
                foreach (Platform p in platforms)
                {
                    lge.AddGObject(p, 1);
                }
            }

            public void OnMainUpdate(float dt)
            {
                // abortamos con la tecla Escape
                if (lge.KeyPressed(Keys.Escape))
                    lge.Quit();
            }

            // main loop
            public void Run(int fps)
            {
                lge.Run(fps);
            }

            // show time
            public static void Main(String[] args)
            {
                Game game = new Game(@"C:\Users\rcarrascor\Documents\MyProjects\CSLittleGameEngine\src\test\cementerio\resources");
                game.Run(60);
                Console.WriteLine("Eso es todo!!!");
            }

        }
    }
}
