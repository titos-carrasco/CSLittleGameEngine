using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace simple
    {
        namespace demo01
        {
            public class TheWorld
            {
                private readonly LittleGameEngine lge;

                public TheWorld(String resourceDir)
                {
                    // creamos el juego
                    Size winSize = new Size(800, 440);

                    lge = new LittleGameEngine(winSize, "The World", Color.White);
                    lge.onMainUpdate = OnMainUpdate;
                   //lge.ShowColliders(Color.Red);

                    // cargamos los recursos que usaremos
                    lge.imageManager.LoadImages("fondo", resourceDir + "/images/Backgrounds/FreeTileset/Fondo.png", winSize, false, false);
                    lge.imageManager.LoadImages("heroe", resourceDir + "/images/Swordsman/Idle/Idle_0*.png", 0.08f, false, false);
                    lge.soundManager.LoadSound("fondo", resourceDir + "/sounds/happy-and-sad.wav");
                    lge.fontManager.LoadSysFont("banner", "Comic Sans MS", FontStyle.Regular, 30);
                    lge.fontManager.LoadSysFont("monospace", "Courier New", FontStyle.Regular, 10);

                    // agregamos el fondo
                    Sprite fondo = new Sprite("fondo", new PointF(0, 0));
                    lge.AddGObject(fondo, 0);

                    // activamos la nusica de fondo
                    lge.soundManager.PlaySound("fondo", true, 1);

                    // agregamos la barra de info
                    Canvas infobar = new Canvas(new PointF(0, 0), new Size(800, 20), "infobar");
                    lge.AddGObjectGUI(infobar);

                    // agregamos al heroe
                    Sprite heroe = new Sprite("heroe", new PointF(226, 254), "Heroe");
                    //heroe.EnableCollider(true);
                    lge.AddGObject(heroe, 1);

                    // agregamos un texto con transparencia
                    Canvas canvas = new Canvas(new PointF(200, 110), new Size(400, 200));
                    canvas.DrawText("Little Game Engine", new PointF(30, 90), "banner", Color.FromArgb(255, 20, 20, 20));
                    lge.AddGObjectGUI(canvas);
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

                    // animamos al heroe
                    Sprite heroe = (Sprite)lge.GetGObject("Heroe");
                    heroe.NextImage(dt, 0.060f);
                }

                // main loop
                public void Run(int fps)
                {
                    lge.Run(fps);
                }

                // show time
                public static void Main()
                {
                    TheWorld game = new TheWorld(@"/mnt/sda5/roberto/Projects/GitHub/CSLittleGameEngine/src/test/resources/");
                    game.Run(60);
                    Console.WriteLine("Eso es todo!!!");
                }
            }
        }
    }
}
