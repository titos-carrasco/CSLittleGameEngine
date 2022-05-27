using System;
using System.Drawing;

namespace rcr
{
    namespace lge
    {
        /**
        * Objeto base del juego. En Little Game Engine casi todo es un GameObject
        *
        * @author Roberto carrasco (titos.carrasco@gmail.com)
        */
        public class GameObject
        {
            protected internal RectangleF rect;
            protected internal RectangleF[] collider = { };
            protected internal String name;
            protected internal Bitmap? surface = null;
            protected internal RectangleF? bounds = null;
            protected internal String tag = "";
            protected internal bool useColliders = false;
            protected internal bool callOnCollision = false;
            protected internal int layer = -1;

            /**
            * Crea un objeto del juego
            *
            * @param origin posicion inicial (x,y) del GameObject
            * @param size   dimension (ancho,alto) del GameObject
            * @param name   nombre unico para este GameObject
            */
            public GameObject(PointF origin, SizeF size, String? name) :
                this(origin.X, origin.Y, size.Width, size.Height, name)
            { }

            /**
            * Crea un objeto del juego
            *
            * @param x      posicion inicial en X del GameObject
            * @param y      posicion inicial en X del GameObject
            * @param Width  ancho del GameObject
            * @param Height alto del GameObject
            * @param name   nombre unico para este GameObject
            */
            public GameObject(float x, float y, float width, float height, String? name)
            {
                rect = new RectangleF(x, y, width, height);
                if (name == null)
                    name = "__no_name__" + System.Guid.NewGuid().ToString();
                this.name = name;
                SetCollider(new RectangleF(0, 0, width, height));
            }

            /**
            * Retorna la posicion de este objeto
            *
            * @return la posicion
            */
            public PointF GetPosition()
            {
                return new PointF(rect.X, rect.Y);
            }

            /**
            * Obtiene la coordenada X del GameObjec
            *
            * @return la coordenada X
            */
            public float GetX()
            {
                return rect.X;
            }

            /**
            * Obtiene la coordenada Y del GameObject
            *
            * @return la coordenada Y
            */
            public float GetY()
            {
                return rect.Y;
            }

            /**
            * Retorna la dimension de este objeto
            *
            * @return la dimension
            */
            public SizeF GetSize()
            {
                return new SizeF(rect.Width, rect.Height);
            }

            /**
            * Retorna el ancho de este objeto
            *
            * @return el ancho
            */
            public float GetWidth()
            {
                return rect.Width;
            }

            /**
            * Retorna el alto de este objeto
            *
            * @return el alto
            */
            public float GetHeight()
            {
                return rect.Height;
            }

            /**
            * Retorna una copia del rectangulo que rodea a este objeto
            *
            * @return el rectangulo
            */
            public RectangleF GetRectangle()
            {
                return new RectangleF(GetPosition(), GetSize());
            }

            /**
            * Retorna el nombre de este objeto
            *
            * @return el nombre
            */
            public String GetName()
            {
                return name;
            }

            /**
            * Retorna el layer de este objeto
            *
            * @return el layer
            */
            public int GetLayer()
            {
                return layer;
            }

            /**
            * Retorna el TAG de este objeto
            *
            * @return el tag
            */
            public String GetTag()
            {
                return tag;
            }

            /**
            * Retorna el colisionador de este objeto
            *
            * @return los rectangulos que definen su colisionador
            */
            public RectangleF[] GetCollider()
            {
                int l = collider.Length;
                RectangleF[] rects = new RectangleF[l];

                for (int i = 0; i < l; i++)
                {
                    RectangleF r = collider[i];
                    r.X += rect.X;
                    r.Y += rect.Y;
                    rects[i] = r;
                }
                return rects;
            }

            /**
            * Establece el rectangulo que limita el movimiento de este objeto
            *
            * @param bounds el rectangulo en donde se permitira mover al objeto
            */
            public void SetBounds(RectangleF bounds)
            {
                this.bounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }

            /**
            * Establece la posicion de este objeto
            *
            * @param position la posicion (x, y)
            */
            public void SetPosition(PointF position)
            {
                SetPosition(position.X, position.Y);
            }

            /**
            * Establece la posicion de este objeto
            *
            * @param x la cordenada x
            * @param y la coordenada y
            */
            public void SetPosition(float x, float y)
            {
                rect.X = x;
                rect.Y = y;

                if (bounds == null)
                    return;

                RectangleF b = (RectangleF)bounds; ;

                if (rect.Width <= b.Width && rect.Height <= b.Height)
                {
                    if (rect.X < b.X)
                        rect.X = b.X;
                    else if (rect.X + rect.Width >= b.X + b.Width)
                        rect.X = b.X + b.Width - rect.Width;
                    if (rect.Y < b.Y)
                        rect.Y = b.Y;
                    else if (rect.Y + rect.Height >= b.Y + b.Height)
                        rect.Y = b.Y + b.Height - rect.Height;
                }
            }

            /**
            * Establece el TAG para este objeto
            *
            * @param tag el tag a asignar
            */
            public void SetTag(String tag)
            {
                this.tag = new String(tag);
            }

            /**
            * Establece el colisionador para este objeto
            *
            * @param rect el rectangulo que define la zona de colision
            */
            public void SetCollider(RectangleF rect)
            {
                collider = new RectangleF[] { new RectangleF(rect.X, rect.Y, rect.Width, rect.Height) };
            }

            /**
            * Establece el colisionador para este objeto
            *
            * @param rects los rectangulos que definen la zona de colision
            */
            public void SetCollider(RectangleF[] rects)
            {
                int l = rects.Length;
                collider = new RectangleF[l];
                for (int i = 0; i < l; i++)
                {
                    RectangleF r = rects[i];
                    collider[i] = new RectangleF(r.X, r.Y, r.Width, r.Height);
                }
            }

            /**
            * Establece si este objeto participara o no del procesamiento de colisiones
            *
            * @param useColliders si es verdadero participara del procesamiento de
            *                     colisiones
            */
            public void EnableCollider(bool useColliders, bool oncollision=false)
            {
                this.useColliders = useColliders;
                this.callOnCollision = oncollision;
            }

            /**
            * Determina si un GameObject colisiona con otro
            *
            * @param gobj el gobject a comparar
            * @return Verdadero si colisiona con el GameObject especificado
            */
            public bool CollidesWith(GameObject gobj)
            {
                if (layer == gobj.layer)
                    foreach (RectangleF r1 in GetCollider())
                        foreach (RectangleF r2 in gobj.GetCollider())
                            if (r1.IntersectsWith(r2))
                                return true;
                return false;
            }

            // manejo de eventos

            /**
            * Es invocada en el siguiente ciclo para todos los
            * GameObjects marcados para eliminacion
            */
            virtual public void OnDelete()
            {
            }

            /**
            * Es invocada en el siguiente ciclo para todos los
            * GameObjects recien creados
            */
            virtual public void OnStart()
            {
            }

            /**
            * Es invocada en el siguiente ciclo para todos los
            * GameObjects previo al evento onUpdate()
            *
            * @param dt tiempo en segundos desde el ultimo ciclo
            */
            virtual public void OnPreUpdate(float dt)
            {
            }

            /**
            * Es invocada en el siguiente ciclo para todos los
            * GameObjects previo al evento onPostUpdate()
            *
            * @param dt tiempo en segundos desde el ultimo ciclo
            */
            virtual public void OnUpdate(float dt)
            {
            }

            /**
            * Es en el siguiente ciclo para todos los
            * GameObjects previo al evento onCollision()
            *
            * @param dt tiempo en segundos desde el ultimo ciclo
            */
            virtual public void OnPostUpdate(float dt)
            {
            }

            /**
            * Es invocada en el siguiente ciclo para todos los
            * GameObjects previo al evento onPreRender()
            *
            * @param dt tiempo en segundos desde el ultimo ciclo
            * @param gobjs los GameObjects que colisionan co este GameObject
            */
            virtual public void OnCollision(float dt, GameObject[] gobjs)
            {
            }

            /**
            * Es invocada en el siguiente ciclo para todos los
            * GameObjects previo al rendering del juego en pantalla
            *
            * @param dt tiempo en segundos desde el ultimo ciclo
            */
            virtual public void OnPreRender(float dt)
            {
            }

            /**
            * Es invocadapara todos los GameObjects jusrto antes de
            * finalizar el game loop
            */
            virtual public void OnQuit()
            {
            }

        }
    }
}
