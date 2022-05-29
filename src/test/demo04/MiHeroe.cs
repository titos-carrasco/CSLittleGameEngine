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
            public class MiHeroe : Sprite
            {
                private readonly LittleGameEngine lge;
                private int state;

                public MiHeroe() :
                    base("heroe_idle_right", new PointF(550, 626), "Heroe")
                {
                    // acceso al motor de juegos
                    lge = LittleGameEngine.GetInstance();

                    // sus atributos
                    state = 1;
                    SetBounds(new RectangleF(0, 0, 1920, 1056));
                }

                public override void OnUpdate(float dt)
                {
                    // velocity = pixeles por segundo
                    float velocity = 240;
                    float pixels = velocity * dt;

                    // la posiciona actual del heroe
                    float x = GetX();
                    float y = GetY();

                    // cambiamos sus coordenadas, orientacion e imagen segun la tecla presionada
                    if (lge.KeyPressed(Keys.Right))
                    {
                        x = x + pixels;
                        if (state != 2)
                        {
                            SetImage("heroe_run_right");
                            state = 2;
                        }
                    }
                    else if (lge.KeyPressed(Keys.Left))
                    {
                        x = x - pixels;
                        if (state != -2)
                        {
                            SetImage("heroe_run_left");
                            state = -2;
                        }
                    }
                    else if (state == 2)
                    {
                        if (state != 1)
                        {
                            SetImage("heroe_idle_right");
                            state = 1;
                        }
                    }
                    else if (state == -2)
                    {
                        if (state != -1)
                        {
                            SetImage("heroe_idle_left");
                            state = -1;
                        }
                    }

                    if (lge.KeyPressed(Keys.Up))
                        y = y - pixels;
                    else if (lge.KeyPressed(Keys.Down))
                        y = y + pixels;

                    // siguiente imagen de la secuencia
                    NextImage(dt, 0.050f);

                    // lo posicionamos
                    SetPosition(x, y);
                }
            }
        }
    }
}
