using System;
using System.Drawing;
using rcr.lge;

namespace test
{
    namespace betty
    {
        public class Zombie : Sprite
        {
            private readonly LittleGameEngine lge;

            private Size winSize;
            private char dir;
            private bool active;

            public Zombie(String name, Size winSize) :
                base("zombie", new PointF(0, 0), name)
            {

                // acceso al motor de juegos
                lge = LittleGameEngine.GetInstance();

                SetTag("zombie");
                EnableCollider(true);
                active = true;
                this.winSize = winSize;

                // direccion inicial - Right, Down, Left, Up
                var rand = new Random();
                int i = rand.Next(4);
                dir = "RDLU"[i];
            }

            public void SetActive(bool active)
            {
                this.active = active;
            }

            public override void OnUpdate(float dt)
            {
                if (!active)
                    return;

                // velocity = pixeles por segundo
                // float velocity = 120;
                // float pixels = velocity*dt;
                float pixels = 2;

                // las coordenadas de Betty
                Betty betty = (Betty)lge.GetGObject("Betty");
                float bx = betty.GetX();
                float by = betty.GetY();

                // nuestra posicion actual
                float x = GetX();
                float y = GetY();

                // posicion respecto a Betty
                bool abajo = y > by;
                bool arriba = y < by;
                bool izquierda = x < bx;
                bool derecha = x > bx;

                // estrategia de movimiento
                String estrategia = "";

                if (dir == 'R')
                {
                    if (abajo && izquierda)
                        estrategia = "URDL";
                    else if (abajo && derecha)
                        estrategia = "UDRL";
                    else if (arriba && izquierda)
                        estrategia = "DRUL";
                    else if (arriba && derecha)
                        estrategia = "DURL";
                    else if (arriba)
                        estrategia = "DRUL";
                    else if (abajo)
                        estrategia = "URDL";
                    else if (izquierda)
                        estrategia = "RUDL";
                    else if (derecha)
                        estrategia = "UDRL";
                }
                else if (dir == 'L')
                {
                    if (abajo && izquierda)
                        estrategia = "UDLR";
                    else if (abajo && derecha)
                        estrategia = "LUDR";
                    else if (arriba && izquierda)
                        estrategia = "DULR";
                    else if (arriba && derecha)
                        estrategia = "DLUR";
                    else if (arriba)
                        estrategia = "DLUR";
                    else if (abajo)
                        estrategia = "ULDR";
                    else if (izquierda)
                        estrategia = "LUDR";
                    else if (derecha)
                        estrategia = "UDLR";
                }
                else if (dir == 'U')
                {
                    if (abajo && izquierda)
                        estrategia = "URLD";
                    else if (abajo && derecha)
                        estrategia = "ULRD";
                    else if (arriba && izquierda)
                        estrategia = "RLUD";
                    else if (arriba && derecha)
                        estrategia = "LRUD";
                    else if (arriba)
                        estrategia = "LRUD";
                    else if (abajo)
                        estrategia = "ULRD";
                    else if (izquierda)
                        estrategia = "RULD";
                    else if (derecha)
                        estrategia = "LURD";
                }
                else if (dir == 'D')
                {
                    if (abajo && izquierda)
                        estrategia = "RLDU";
                    else if (abajo && derecha)
                        estrategia = "LRDU";
                    else if (arriba && izquierda)
                        estrategia = "RDLU";
                    else if (arriba && derecha)
                        estrategia = "LDRU";
                    else if (arriba)
                        estrategia = "DLRU";
                    else if (abajo)
                        estrategia = "LRDU";
                    else if (izquierda)
                        estrategia = "RDLU";
                    else if (derecha)
                        estrategia = "LDRU";
                }

                // probamos cada movimiento de la estrategia
                for (int i = 0; i < estrategia.Length; i++)
                {
                    char c = estrategia[i];
                    float nx = x, ny = y;

                    if (c == 'R')
                        nx += pixels;
                    else if (c == 'L')
                        nx -= pixels;
                    else if (c == 'U')
                        ny -= pixels;
                    else if (c == 'D')
                        ny += pixels;

                    // verificamos que no colisionemos con un muro u otro zombie
                    SetPosition(nx, ny);
                    GameObject[] gobjs = lge.CollidesWith(this);
                    bool collision = false;
                    foreach (GameObject gobj in gobjs)
                    {
                        String tag = gobj.GetTag();
                        if (tag.Equals("zombie") || tag.Equals("muro"))
                        {
                            collision = true;
                            break;
                        }
                    }

                    if (!collision)
                    {
                        dir = c;

                        // tunel?
                        if (nx < -16)
                            nx = winSize.Width - 16;
                        else if (nx > winSize.Width - 16)
                            nx = -16;
                        SetPosition(nx, ny);
                        break;
                    }

                    // otro intento
                    SetPosition(x, y);
                }

                // siguiente imagen de la secuencia
                NextImage(dt, 0.1f);
            }
        }
    }
}
