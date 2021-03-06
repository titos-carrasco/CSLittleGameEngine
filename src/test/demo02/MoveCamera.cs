using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace simple
    {
        namespace demo02
        {
            public class MoveCamera
            {
                private readonly LittleGameEngine lge;

                public MoveCamera(String resourceDir)
                {
                    // creamos el juego
                    Size winSize = new Size(640, 480);

                    lge = new LittleGameEngine(winSize, "Move Camera", Color.White);
                    lge.onMainUpdate = OnMainUpdate;

                    // cargamos los recursos que usaremos
                    lge.imageManager.LoadImages("fondo", resourceDir + "/images/Backgrounds/FreeTileset/Fondo.png", false, false);
                    lge.imageManager.LoadImages("heroe", resourceDir + "/images/Swordsman/Idle/Idle_000.png", 0.16f, false, false);
                    lge.imageManager.LoadImages("mute", resourceDir + "/images/icons/sound-*.png", false, false);
                    lge.fontManager.LoadSysFont("monospace", "Courier New", FontStyle.Regular, 10);
                    lge.soundManager.LoadSound("fondo", resourceDir + "/sounds/happy-and-sad.wav");

                    // activamos la musica de fondo
                    lge.soundManager.PlaySound("fondo", true);

                    // agregamos el fondo
                    Sprite fondo = new Sprite("fondo", new PointF(0, 0), "fondo");
                    lge.AddGObject(fondo, 0);

                    // agregamos la barra de info
                    Canvas infobar = new Canvas(new PointF(0, 0), new Size(640, 20), "infobar");
                    lge.AddGObjectGUI(infobar);

                    // agregamos al heroe
                    Sprite heroe = new Sprite("heroe", new PointF(550, 626), "Heroe");
                    lge.AddGObject(heroe, 1);

                    // # configuramos la camara
                    lge.SetCameraBounds(new RectangleF(0, 0, 1920, 1056));

                    // posicionamos la camara
                    PointF heroePosition = heroe.GetPosition();
                    SizeF heroeSize = heroe.GetSize();
                    SizeF cameraSize = lge.GetCameraSize();
                    float x = heroePosition.X + heroeSize.Width / 2 - cameraSize.Width / 2;
                    float y = heroePosition.Y + heroeSize.Height / 2 - cameraSize.Height / 2;
                    lge.SetCameraPosition(new PointF(x, y));
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

                    // velocity = pixeles por segundo
                    float velocity = 240;
                    float pixels = velocity * dt;

                    // la posiciona actual de la camara
                    PointF cameraPosition = lge.GetCameraPosition();

                    // cambiamos sus coordenadas segun la tecla presionada
                    if (lge.KeyPressed(Keys.Right))
                        cameraPosition.X += pixels;
                    else if (lge.KeyPressed(Keys.Left))
                        cameraPosition.X -= pixels;

                    if (lge.KeyPressed(Keys.Up))
                        cameraPosition.Y -= pixels;
                    else if (lge.KeyPressed(Keys.Down))
                        cameraPosition.Y += pixels;

                    // posicionamos la camara
                    lge.SetCameraPosition(cameraPosition);
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
                    MoveCamera game = new MoveCamera(resourceDir);
                    game.Run(60);
                    Console.WriteLine("Eso es todo!!!");
                }
            }
        }
    }
}
