using System;
using System.Drawing;
using System.Windows.Forms;
using rcr.lge;

namespace test
{
    namespace pong
    {

        public class Pong : IEvents
        {
            private readonly LittleGameEngine lge;
            private int paddleSpeed = 240;

            public Pong()
            {
                // creamos el juego
                Size winSize = new Size(640, 640);

                lge = new LittleGameEngine(winSize, "Ping", Color.Black);
                lge.SetOnMainUpdate(this);
                // lge.ShowColliders(Color.Red);

                // cargamos los recursos que usaremos
                //String resourceDir = @"../resources";

                lge.LoadSysFont("monospace", "FreeMono", FontStyle.Regular, 12);

                // agregamos la barra de info
                Canvas infobar = new Canvas(new PointF(0, 0), new Size(640, 20), "infobar");
                lge.AddGObjectGUI(infobar);

                // el campo de juego
                Canvas field = new Canvas(new PointF(24, 80), new Size(592, 526), "field");
                field.Fill(Color.FromArgb(0, 0, 100));
                lge.AddGObject(field, 0);

                // los bordes
                Canvas wall = new Canvas(new PointF(0, 76), new Size(640, 4));
                wall.Fill(Color.White);
                wall.SetTag("wall-horizontal");
                wall.EnableCollider(true);
                lge.AddGObject(wall, 1);

                wall = new Canvas(new PointF(0, 606), new Size(640, 4));
                wall.Fill(Color.White);
                wall.SetTag("wall-horizontal");
                wall.EnableCollider(true);
                lge.AddGObject(wall, 1);

                wall = new Canvas(new PointF(20, 80), new Size(4, 526));
                wall.Fill(Color.White);
                wall.SetTag("wall-vertical");
                wall.EnableCollider(true);
                lge.AddGObject(wall, 1);

                wall = new Canvas(new PointF(616, 80), new Size(4, 526));
                wall.Fill(Color.White);
                wall.SetTag("wall-vertical");
                wall.EnableCollider(true);
                lge.AddGObject(wall, 1);

                // los actores
                Ball ball = new Ball(new PointF(320, 400), new SizeF(8, 8), "ball");
                lge.AddGObject(ball, 1);

                Canvas paddle = new Canvas(new PointF(90, 270), new Size(8, 60), "user-paddle");
                paddle.Fill(Color.White);
                paddle.SetTag("paddle");
                paddle.EnableCollider(true);
                paddle.SetBounds(field.GetRectangle());
                lge.AddGObject(paddle, 1);

                paddle = new Canvas(new PointF(540, 270), new Size(8, 60), "system-paddle");
                paddle.Fill(Color.White);
                paddle.SetTag("paddle");
                paddle.EnableCollider(true);
                paddle.SetBounds(field.GetRectangle());
                lge.AddGObject(paddle, 1);
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
                infobar.Fill(Color.FromArgb(0x80, 0x80, 0x80, 0x80));
                infobar.DrawText(info, new PointF(100, 0), "monospace", Color.White);

                // user paddle
                Canvas userPaddle = (Canvas)lge.GetGObject("user-paddle");
                float speed = paddleSpeed * dt;
                float x = userPaddle.GetX();
                float y = userPaddle.GetY();

                if (lge.KeyPressed(Keys.Up))
                    userPaddle.SetPosition(x, y - speed);
                else if (lge.KeyPressed(Keys.Down))
                    userPaddle.SetPosition(x, y + speed);

                // la pelota
                Ball ball = (Ball)lge.GetGObject("ball");
                // float bx = ball.GetX();
                float by = ball.GetY();

                // system paddle
                Canvas systemPaddle = (Canvas)lge.GetGObject("system-paddle");
                float px = systemPaddle.GetX();
                float py = systemPaddle.GetY();
                // int pw = systemPaddle.GetWidth();
                float ph = systemPaddle.GetHeight();

                if (py + ph / 2.0 < by)
                    py = py + speed;
                else if (py + ph / 2.0 > by)
                    py = py - speed;
                systemPaddle.SetPosition(px, py);
            }

            // main loop
            public void Run(int fps)
            {
                lge.Run(fps);
            }

            // show time
            public static void Main(String[] args)
            {
                Pong game = new Pong();
                game.Run(60);
                Console.WriteLine("Eso es todo!!!");
            }
        }
    }
}