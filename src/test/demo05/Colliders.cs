using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace simple
    {
        namespace demo05
        {
            public class Colliders
            {
                private readonly LittleGameEngine lge;

                public Colliders(String resourceDir)
                {
                    // creamos el juego
                    Size winSize = new Size(640, 480);

                    lge = new LittleGameEngine(winSize, "Colliders", Color.White);
                    lge.ShowColliders(Color.Red);
                    lge.onMainUpdate = OnMainUpdate;

                    // cargamos los recursos que usaremos
                    lge.imageManager.LoadImages("fondo", resourceDir + "/images/Backgrounds/FreeTileset/Fondo.png", false, false);
                    lge.imageManager.LoadImages("heroe_idle_right", resourceDir + "/images/Swordsman/Idle/Idle_0*.png", 0.16f, false, false);
                    lge.imageManager.LoadImages("heroe_idle_left", resourceDir + "/images/Swordsman/Idle/Idle_0*.png", 0.16f, true, false);
                    lge.imageManager.LoadImages("heroe_run_right", resourceDir + "/images/Swordsman/Run/Run_0*.png", 0.16f, false, false);
                    lge.imageManager.LoadImages("heroe_run_left", resourceDir + "/images/Swordsman/Run/Run_0*.png", 0.16f, true, false);
                    lge.imageManager.LoadImages("ninja", resourceDir + "/images/Swordsman/Idle/Idle_000.png", 0.16f, false, false);
                    //lge.fontManager.LoadSysFont("monospace", "Courier New", FontStyle.Regular, 10);
                    lge.fontManager.LoadTTFont("monospace", resourceDir + "/fonts/FreeMono.ttf", FontStyle.Regular, 10);
                    lge.soundManager.LoadSound("fondo", resourceDir + "/sounds/happy-and-sad.wav");
                    lge.soundManager.LoadSound("aves", resourceDir + "/sounds/bird-thrush-nightingale.wav");
                    lge.soundManager.LoadSound("poing", resourceDir + "/sounds/cartoon-poing.wav");

                    // activamos la musica de fondo
                    lge.soundManager.PlaySound("fondo", true);

                    // agregamos el fondo
                    Sprite fondo = new Sprite("fondo", new PointF(0, 0), "fondo");
                    lge.AddGObject(fondo, 0);

                    // agregamos la barra de info
                    Canvas infobar = new Canvas(new PointF(0, 0), new Size(640, 20), "infobar");
                    lge.AddGObjectGUI(infobar);

                    // agregamos un ninja
                    Sprite ninja = new Sprite("ninja", new PointF(350, 720), "ninja");
                    ninja.EnableCollider(true, true);
                    ninja.SetCollider(new RectangleF[] { new RectangleF(36, 8, 36, 36), new RectangleF(28, 44, 44, 36) });
                    lge.AddGObject(ninja, 1);

                    // agregamos al heroe
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

                    // de manera aleatorio activamos sonido de aves
                    Random rand = new Random();
                    int n = rand.Next(1000);
                    if (n < 3)
                        lge.soundManager.PlaySound("aves", false);
                }

                // main loop
                public void Run(int fps)
                {
                    lge.Run(fps);
                }

                // show time
                public static void Main(String[] args)
                {
                    String resourceDir = args[0];
                    Colliders game = new Colliders(resourceDir);
                    game.Run(60);
                    Console.WriteLine("Eso es todo!!!");
                }
            }
        }
    }
}
