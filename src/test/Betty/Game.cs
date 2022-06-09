using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace betty
    {
        public class Game
        {
            private readonly LittleGameEngine lge;

            private readonly int[,] mapa;

            public Game(String resourceDir)
            {
                // creamos el juego
                Size winSize = new Size(608, 736);

                lge = new LittleGameEngine(winSize, "Betty", Color.White);
                lge.onMainUpdate = OnMainUpdate;
                // lge.ShowColliders(Color.Red);

                // cargamos los recursos que usaremos
                lge.imagesManager.LoadImage("fondo", resourceDir + "/images/Betty/Fondo.png", false, false);
                lge.imagesManager.LoadImage("betty_idle", resourceDir + "/images/Betty/idle-0*.png", false, false);
                lge.imagesManager.LoadImage("betty_down", resourceDir + "/images/Betty/down-0*.png", false, false);
                lge.imagesManager.LoadImage("betty_up", resourceDir + "/images/Betty/up-0*.png", false, false);
                lge.imagesManager.LoadImage("betty_left", resourceDir + "/images/Betty/left-0*.png", false, false);
                lge.imagesManager.LoadImage("betty_right", resourceDir + "/images/Betty/right-0*.png", false, false);
                lge.imagesManager.LoadImage("zombie", resourceDir + "/images/Kenny/Zombie/zombie_walk*.png", false, false);
                lge.LoadSysFont("monospace", "Courier New", FontStyle.Regular, 10);

                // agregamos el fondo
                Sprite fondo = new Sprite("fondo", new PointF(0, 0), "fondo");
                lge.AddGObject(fondo, 0);

                // agregamos la barra de info
                Canvas infobar = new Canvas(new PointF(0, 0), new Size(640, 20), "infobar");
                lge.AddGObjectGUI(infobar);

                // cargamos el mapa en memoria
                string[] lines = System.IO.File.ReadAllLines(resourceDir + "/images/Betty/Mapa.txt");
                int x = 0, y = 0;
                mapa = new int[22, 19];
                foreach (string line in lines)
                {
                    String[] codes = line.Split(',');
                    for (x = 0; x < codes.Length; x++)
                        mapa[y, x] = Convert.ToInt32(codes[x]);
                    y++;
                }

                // agregamos a Betty
                Betty betty = new Betty("Betty", winSize);
                betty.SetPosition(32 * 9, 32 * 9);
                lge.AddGObject(betty, 1);

                // agregamos 3 zombies
                for (int i = 0; i < 3; i++)
                {
                    Zombie zombie = new Zombie("Zombie-" + i, winSize);
                    zombie.SetPosition(32 + 32 * 4 + 32 * (i * 4), 32 * 21);
                    lge.AddGObject(zombie, 1);
                }

                // agregamos los muros para las colisiones (segun el mapa)
                for (y = 0; y < 22; y++)
                    for (x = 0; x < 19; x++)
                        if (mapa[y, x] == 1)
                        {
                            GameObject muro = new GameObject(new PointF(x * 32, 32 + y * 32), new SizeF(32, 32));
                            muro.EnableCollider(true);
                            muro.SetTag("muro");
                            lge.AddGObject(muro, 1);
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
                infobar.DrawText(info, new PointF(40, 3), "monospace", Color.White);
            }

            // main loop
            public void Run(int fps)
            {
                lge.Run(fps);
            }

            // show time
            public static void Main()
            {
                Game game = new Game(@"C:\Users\rcarrascor\Documents\MyProjects\CSLittleGameEngine\src\test\resources");
                game.Run(60);
                Console.WriteLine("Eso es todo!!!");
            }

        }
    }
}
