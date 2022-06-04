using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace birds
    {
        public class Birds
        {
            private readonly LittleGameEngine lge;

            public Birds(String resourceDir)
            {
                // creamos el juego
                Size winSize = new Size(800, 440);

                lge = new LittleGameEngine(winSize, "Birds", Color.White);
                lge.onMainUpdate = OnMainUpdate;

                // cargamos los recursos que usaremos
                lge.LoadImage("fondo", resourceDir + "/images/Backgrounds/FreeTileset/Fondo.png", winSize, false, false);
                lge.LoadImage("heroe", resourceDir + "/images/Swordsman/Idle/Idle_0*.png", 0.08f, false, false);
                lge.LoadImage("mute", resourceDir + "/images/icons/sound-*.png", false, false);
                lge.LoadImage("bird", resourceDir + "/images/BlueBird/frame-*.png", 0.04f, false, false);
                lge.LoadSysFont("monospace", "Courier New", FontStyle.Regular, 10);

                // agregamos el fondo
                Sprite fondo = new Sprite("fondo", new PointF(0, 0), "fondo");
                lge.AddGObject(fondo, 0);

                // agregamos la barra de info
                Canvas infobar = new Canvas(new PointF(0, 0), new Size(800, 20), "infobar");
                lge.AddGObjectGUI(infobar);

                // agregamos al heroe
                Sprite heroe = new Sprite("heroe", new PointF(226, 254), "Heroe");
                lge.AddGObject(heroe, 1);

                // agregamos pajaros
                var rand = new Random();
                for (int i = 0; i < 500; i++)
                {
                    float x = (float)rand.Next(winSize.Width);
                    float y = 15 + (float)rand.Next(winSize.Height);
                    Bird bird = new Bird("bird", new PointF(x, y));
                    lge.AddGObject(bird, 1);
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
                Birds game = new Birds(@"C:\Users\rcarrascor\Documents\MyProjects\CSLittleGameEngine\src\test\resources");
                game.Run(60);
                Console.WriteLine("Eso es todo!!!");
            }
        }
    }
}
