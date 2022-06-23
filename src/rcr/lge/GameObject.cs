using System;
using System.Drawing;

namespace rcr
{
    namespace lge
    {
        /// <summary>
        /// Objeto base del juego. En Little Game Engine casi todo es un GameObject
        /// <para>@author Roberto carrasco(titos.carrasco @gmail.com)</para>
        /// </summary>
        public class GameObject
        {
            /// <value>El rectangulo que delimita a este GameObject</value>
            protected internal RectangleF rect;
            /// <value>Los rectangulos que confirman el colisionador de este GameObject</value>
            protected internal RectangleF[] collider = { };
            /// <value>El nombre de este GameObject</value>
            protected internal String name;
            /// <value>La imagen que visibiliza este GameObject</value>
            protected internal Bitmap surface = null;
            /// <value>El rectangulo que limita el movimiento de este GameObject</value>
            protected internal Nullable<RectangleF> bounds = null;
            /// <value>El tag de este GameObject</value>
            protected internal String tag = "";
            /// <value>true si este GameObject participa del procesdamiento de colisiones</value>
            protected internal bool useColliders = false;
            /// <value>true si este GameObject sera incluido en el evento OnCollision</value>
            protected internal bool callOnCollision = false;
            /// <value></value>
            protected internal int layer = -1;

            /// <summary>
            /// Crea un objeto del juego
            /// </summary>
            /// <param name="origin">Posicion inicial (x,y) del GameObject</param>
            /// <param name="size">Dimension (ancho,alto) del GameObject</param>
            public GameObject(PointF origin, SizeF size) :
                this(origin.X, origin.Y, size.Width, size.Height, null)
            { }

            /// <summary>
            /// Crea un objeto del juego
            /// </summary>
            /// <param name="origin">Posicion inicial (x,y) del GameObject</param>
            /// <param name="size">Dimension (ancho,alto) del GameObject.</param>
            /// <param name="name">Nombre unico para este GameObject.</param>
            public GameObject(PointF origin, SizeF size, String name) :
                this(origin.X, origin.Y, size.Width, size.Height, name)
            { }

            /// <summary>
            /// Crea un objeto del juego
            /// </summary>
            /// <param name="x">Posicion inicial en X del GameObject.</param>
            /// <param name="y">Posicion inicial en X del GameObject</param>
            /// <param name="width">Ancho del GameObject</param>
            /// <param name="height">Alto del GameObject</param>
            /// <param name="name">Nombre unico para este GameObject</param>
            public GameObject(float x, float y, float width, float height, String name)
            {
                rect = new RectangleF(x, y, width, height);
                if (name == null)
                    name = "__no_name__" + System.Guid.NewGuid().ToString();
                this.name = name;
                SetCollider(new RectangleF(0, 0, width, height));
            }

            /// <summary>
            /// Retorna la posicion de este objeto
            /// </summary>
            /// <returns>La posicion</returns>
            public PointF GetPosition()
            {
                return new PointF(rect.X, rect.Y);
            }

            /// <summary>
            /// Obtiene la coordenada X del GameObject
            /// </summary>
            /// <returns>La coordenada X</returns>
            public float GetX()
            {
                return rect.X;
            }

            /// <summary>
            /// Obtiene la coordenada Y del GameObject
            /// </summary>
            /// <returns>La coordenada Y</returns>
            public float GetY()
            {
                return rect.Y;
            }

            /// <summary>
            /// Obtiene la dimension de este objeto
            /// </summary>
            /// <returns>La dimension</returns>
            public SizeF GetSize()
            {
                return new SizeF(rect.Width, rect.Height);
            }

            /// <summary>
            /// Obtiene el ancho de este objeto
            /// </summary>
            /// <returns>El ancho</returns>
            public float GetWidth()
            {
                return rect.Width;
            }

            /// <summary>
            /// Obtiene el alto de este objeto
            /// </summary>
            /// <returns>El alto</returns>
            public float GetHeight()
            {
                return rect.Height;
            }

            /// <summary>
            /// Obtiene una copia del rectangulo que rodea a este objeto
            /// </summary>
            /// <returns>El rectangulo</returns>
            public RectangleF GetRectangle()
            {
                return new RectangleF(GetPosition(), GetSize());
            }

            /// <summary>
            /// Obtiene el nombre de este objeto
            /// </summary>
            /// <returns>El nombre</returns>
            public String GetName()
            {
                return name;
            }

            /// <summary>
            /// Obtiene el numero de capa de este objeto
            /// </summary>
            /// <returns>El numero de capa</returns>
            public int GetLayer()
            {
                return layer;
            }

            /// <summary>
            /// Obtiene el tag de este objeto
            /// </summary>
            /// <returns>El tag</returns>
            public String GetTag()
            {
                return tag;
            }

            /// <summary>
            /// Obtiene el colisionador de este objeto.
            /// </summary>
            /// <returns>Un arreglo de rectangulos que definen al colisionador</returns>
            public RectangleF[] GetCollider()
            {
                int l = collider.Length;
                RectangleF[] rects = new RectangleF[l];

                for (int i = 0; i < l; i++)
                {
                    RectangleF r = new RectangleF(collider[i].Location, collider[i].Size);
                    r.X += rect.X;
                    r.Y += rect.Y;
                    rects[i] = r;
                }
                return rects;
            }

            /// <summary>
            /// Establece un rectangulo que limita el movimiento de este objeto
            /// </summary>
            /// <param name="bounds">El rectangulo en donde se permitira mover al objeto</param>
            public void SetBounds(RectangleF bounds)
            {
                this.bounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            }

            /// <summary>
            /// Establece la posicion de este objeto
            /// </summary>
            /// <param name="position">La posicion (x, y)</param>
            public void SetPosition(PointF position)
            {
                SetPosition(position.X, position.Y);
            }

            /// <summary>
            /// Establece la posicion de este objeto
            /// </summary>
            /// <param name="x">La coordenada X</param>
            /// <param name="y">La xcoordenada Y</param>
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

            /// <summary>
            /// Establece el tag de este objeto
            /// </summary>
            /// <param name="tag">El tag a asignar</param>
            public void SetTag(String tag)
            {
                this.tag = tag;
            }

            /// <summary>
            /// Establece el colisionador para este objeto
            /// </summary>
            /// <param name="rect">El rectangulo que define la zona de colision</param>
            public void SetCollider(RectangleF rect)
            {
                collider = new RectangleF[] { new RectangleF(rect.X, rect.Y, rect.Width, rect.Height) };
            }

            /// <summary>
            /// Establece el colisionador para este objeto
            /// </summary>
            /// <param name="rects">Los rectangulos que definen la zona de colision.</param>
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

            /// <summary>
            /// Establece si este objeto participara o no del procesamiento de colisiones
            /// </summary>
            /// <param name="useColliders">Si es verdadero participara del procesamiento de colisiones</param>
            /// <param name="oncollision">Si es verdadero se invocara al metodo OnCollision al detectar una colision</param>
            public void EnableCollider(bool useColliders, bool oncollision = false)
            {
                this.useColliders = useColliders;
                this.callOnCollision = oncollision;
            }

            /// <summary>
            /// Determina si este GameObject colisiona con otro
            /// </summary>
            /// <param name="gobj">El GameObject a procesar</param>
            /// <returns>Verdadero si colisiona con el GameObject especificado</returns>
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

            /// <summary>
            /// Invocada en el siguiente ciclo para todos los GameObjects marcados para eliminacion
            /// </summary>
            virtual public void OnDelete()
            {
            }

            /// <summary>
            /// Invocada en el siguiente ciclo para todos los GameObjects recien creados
            /// </summary>
            virtual public void OnStart()
            {
            }

            /// <summary>
            /// Invocada en el siguiente ciclo para todos los GameObjects previo al evento OnUpdate()
            /// </summary>
            /// <param name="dt">Tiempo en segundos desde el ultimo ciclo</param>
            virtual public void OnPreUpdate(float dt)
            {
            }

            /// <summary>
            /// Invocada en el siguiente ciclo para todos los GameObjects previo al evento OnPostUpdate()
            /// </summary>
            /// <param name="dt">Tiempo en segundos desde el ultimo ciclo</param>
            virtual public void OnUpdate(float dt)
            {
            }

            /// <summary>
            /// Invocada en el siguiente ciclo para todos los GameObjects previo al evento OnCollision()
            /// </summary>
            /// <param name="dt">Tiempo en segundos desde el ultimo ciclo</param>
            virtual public void OnPostUpdate(float dt)
            {
            }

            /// <summary>
            /// Invocada en el siguiente ciclo para todos los GameObjects previo al evento OnPreRender()
            /// </summary>
            /// <param name="dt">Tiempo en segundos desde el ultimo ciclo</param>
            /// <param name="gobjs">Los GameObjects que colisionan con este GameObject</param>
            virtual public void OnCollision(float dt, GameObject[] gobjs)
            {
            }

            /// <summary>
            /// Invocada en el siguiente ciclo para todos los GameObjects previo al rendering del juego en pantalla
            /// </summary>
            /// <param name="dt">Tiempo en segundos desde el ultimo ciclo</param>
            virtual public void OnPreRender(float dt)
            {
            }

            /// <summary>
            /// Es invocada para todos los GameObjects justo antes de finalizar el game loop
            /// </summary>
            virtual public void OnQuit()
            {
            }
        }
    }
}
