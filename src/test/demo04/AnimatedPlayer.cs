using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace simple
    {
        namespace demo04
        {
            public class AnimatedPlayer
            {
                private readonly LittleGameEngine lge;

                public AnimatedPlayer(String resourceDir)
                {
                    // creamos el juego
                    Size winSize = new Size(640, 480);

                    lge = new LittleGameEngine(winSize, "Animated Player", Color.White);
                    lge.onMainUpdate = OnMainUpdate;

                    // cargamos los recursos que usaremos
                    lge.LoadImage("fondo", resourceDir + "/images/Backgrounds/FreeTileset/Fondo.png", false, false);
                    lge.LoadImage("heroe_idle_right", resourceDir + "/images/Swordsman/Idle/Idle_0*.png", 0.16f, false, false);
                    lge.LoadImage("heroe_idle_left", resourceDir + "/images/Swordsman/Idle/Idle_0*.png", 0.16f, true, false);
                    lge.LoadImage("heroe_run_right", resourceDir + "/images/Swordsman/Run/Run_0*.png", 0.16f, false, false);
                    lge.LoadImage("heroe_run_left", resourceDir + "/images/Swordsman/Run/Run_0*.png", 0.16f, true, false);
                    lge.LoadImage("mute", resourceDir + "/images/icons/sound-*.png", false, false);
                    lge.LoadSysFont("monospace", "Courier New", FontStyle.Regular, 12);

                    // agregamos el fondo
                    Sprite fondo = new Sprite("fondo", new PointF(0, 0), "fondo");
                    lge.AddGObject(fondo, 0);

                    // agregamos la barra de info
                    Canvas infobar = new Canvas(new PointF(0, 0), new Size(640, 20), "infobar");
                    lge.AddGObjectGUI(infobar);


                    MiHeroe heroe = new MiHeroe();
                    lge.AddGObject(heroe, 1);

                    // # configuramos la camara
                    lge.SetCameraBounds(new RectangleF(0, 0, 1920, 1056));

                    // establecemos que la camara siga al heroe
                    lge.SetCameraTarget(heroe, false);
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
                    infobar.DrawText(info, new PointF(100, 0), "monospace", Color.Black);
                }

                // main loop
                public void Run(int fps)
                {
                    lge.Run(fps);
                }

                // show time
                public static void Main()
                {
                    AnimatedPlayer game = new AnimatedPlayer(@"C:\Users\rcarrascor\Documents\MyProjects\CSLittleGameEngine\src\test\resources");
                    game.Run(60);
                    Console.WriteLine("Eso es todo!!!");
                }
            }
        }
    }
}
