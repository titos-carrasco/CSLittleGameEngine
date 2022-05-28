using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace simple
    {
        namespace demo03
        {
            public class MovePlayer : IEvents
            {
                private readonly LittleGameEngine lge;

                public MovePlayer()
                {
                    // creamos el juego
                    Size winSize = new Size(640, 480);

                    lge = new LittleGameEngine(winSize, "Move Player", Color.White);
                    lge.SetOnMainUpdate(this);

                    // cargamos los recursos que usaremos
                    String resourceDir = @"../../resources";

                    lge.LoadImage("fondo", resourceDir + "/images/Backgrounds/FreeTileset/Fondo.png", false, false);
                    lge.LoadImage("heroe_right", resourceDir + "/images/Swordsman/Idle/Idle_000.png", 0.16f, false, false);
                    lge.LoadImage("heroe_left", resourceDir + "/images/Swordsman/Idle/Idle_000.png", 0.16f, true, false);
                    lge.LoadSysFont("monospace", "FreeMono", FontStyle.Regular, 12);

                    // agregamos el fondo
                    Sprite fondo = new Sprite("fondo", new PointF(0, 0), "fondo");
                    lge.AddGObject(fondo, 0);

                    // agregamos la barra de info
                    Canvas infobar = new Canvas(new PointF(0, 0), new Size(640, 20), "infobar");
                    lge.AddGObjectGUI(infobar);

                    // agregamos al heroe
                    MiHeroe heroe = new MiHeroe();
                    lge.AddGObject(heroe, 1);

                    // # configuramos la camara
                    lge.SetCameraBounds(new Rectangle(0, 0, 1920, 1056));

                    // establecemos que la camara siga al heroe
                    lge.SetCameraTarget(heroe, true);
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
                    MovePlayer game = new MovePlayer();
                    game.Run(60);
                    Console.WriteLine("Eso es todo!!!");
                }
            }
        }
    }
}
