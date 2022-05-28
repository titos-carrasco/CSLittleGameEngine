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
            public class MiHeroe : Sprite
            {
                private readonly LittleGameEngine lge;

                public MiHeroe() :
                    base("heroe_right", new PointF(550, 626), "Heroe")
                {
                    // acceso al motor de juegos
                    lge = LittleGameEngine.GetInstance();

                    // sus atributos
                    SetBounds(new Rectangle(0, 0, 1920, 1056));
                }

                public override void OnUpdate(float dt)
                {
                    // velocity = pixeles por segundo
                    float velocity = 240;
                    float pixels = velocity * dt;

                    // la posiciona actual del heroe
                    float x = GetX();
                    float y = GetY();

                    // cambiamos sus coordenadas segun la tecla presionada
                    if (lge.KeyPressed(Keys.Right))
                    {
                        x = x + pixels;
                        SetImage("heroe_right");
                    }
                    else if (lge.KeyPressed(Keys.Left))
                    {
                        x = x - pixels;
                        SetImage("heroe_left");
                    }

                    if (lge.KeyPressed(Keys.Up))
                        y = y - pixels;
                    else if (lge.KeyPressed(Keys.Down))
                        y = y + pixels;

                    // lo posicionamos
                    SetPosition(x, y);
                }
            }
        }
    }
}
