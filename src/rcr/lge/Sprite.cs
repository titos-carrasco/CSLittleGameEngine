using System;
using System.Drawing;

namespace rcr
{
    namespace lge
    {
        /// <summary>
        /// Clase para manejar GameObjects animados
        /// <para>@author Roberto carrasco (titos.carrasco@gmail.com)</para>
        /// </summary>
        /// <seealso cref="rcr.lge.GameObject" />
        public class Sprite : GameObject
        {
            Bitmap[] surfaces = { };
            String iname = null;
            int idx = 0;
            float elapsed = 0;

            /// <summary>
            /// Crea un GameObject animado con la secuencia de imagenes a utilizar a ser
            /// especificada posteriormente
            /// </summary>
            /// <param name="position">Posicion inicial(x, y) del GameObject</param>
            public Sprite(PointF position) :
                this(null, position, null)
            { }

            /// <summary>
            /// Crea un GameObject animado con la secuencia de imagenes a utilizar
            /// </summary>
            /// <param name="iname">Nombre de la secuencia de imagenes a utilizar</param>
            /// <param name="position">Posicion inicial(x, y) del GameObject</param>
            public Sprite(String iname, PointF position) :
                this(iname, position, null)
            { }

            /// <summary>
            /// Crea un GameObject animado con la secuencia de imagenes a utilizar
            /// </summary>
            /// <param name="iname">Nombre de la secuencia de imagenes a utilizar</param>
            /// <param name="position">Posicion inicial(x, y) del GameObject</param>
            /// <param name="name">Nombre a asignar a este GameObject</param>
            public Sprite(String iname, PointF position, String name) :
                base(position, new SizeF(0, 0), name)
            {
                SetImage(iname);
            }

            /// <summary>
            /// Recupera el nombre de la secuencia actual de imagenes que utiliza este Sprite
            /// </summary>
            /// <returns>El nombre de la secuencia</returns>
            public String GetImagesName()
            {
                return iname;
            }

            /// <summary>
            /// Recupera el indice de la secuencia actual de imagenes que utiliza este Sprite
            /// </summary>
            /// <returns>El numero de la imagen dentro de la secuencia actual</returns>
            public int GetImagesIndex()
            {
                return idx;
            }

            /// <summary>
            /// Avanza automaticamente a la siguiente imagen de la secuencia de este Sprite
            /// </summary>
            /// <returns>El numero de la imagen dentro de la secuencia actual</returns>
            public int NextImage()
            {
                return NextImage(0, 0);
            }

            /// <summary>
            /// Avanza automaticamente a la siguiente imagen de la secuencia de este Sprite
            /// </summary>
            /// <param name="dt">Tiempo transcurrido desde la ultima invocacion a este metodo</param>
            /// <returns>El numero de la imagen dentro de la secuencia actual</returns>
            public int NextImage(float dt)
            {
                return NextImage(dt, 0);
            }

            /// <summary>
            /// Avanza automaticamente a la siguiente imagen de la secuencia de este Sprite
            /// </summary>
            /// <param name="dt">Tiempo transcurrido desde la ultima invocacion a este metodo</param>
            /// <param name="delay">Tiempo que debe transcurrir antes de pasar a la siguiente imagen de la secuencia</param>
            /// <returns>El numero de la imagen dentro de la secuencia actual</returns>
            public int NextImage(float dt, float delay)
            {
                elapsed += dt;
                if (elapsed < delay)
                    return idx;

                elapsed = 0;
                idx++;
                if (idx >= surfaces.Length)
                    idx = 0;

                surface = surfaces[idx];
                this.rect.Width = this.surface.Width;
                this.rect.Height = this.surface.Height;
                SetCollider(new RectangleF(0, 0, this.rect.Width, this.rect.Height));
                return idx;
            }

            /// <summary>
            /// Establece la secuencia de imagenes a utilizar en este Sprite
            /// </summary>
            /// <param name="iname">El nombre de la secuencia (cargada con LoadImage y especificada al crear este Sprite)</param>
            /// <returns>El numero de la imagen dentro de la secuencia actual</returns>
            public int SetImage(String iname)
            {
                return SetImage(iname, -1);
            }

            /// <summary>
            /// Establece la secuencia de imagenes a utilizar en este Sprite
            /// </summary>
            /// <param name="iname">El nombre de la secuencia (cargada con LoadImage y especificada al crear este Sprite)</param>
            /// <param name="idx">El numero de la secuencia a utilizar</param>
            /// <returns>El numero de la imagen dentro de la secuencia actual</returns>
            public int SetImage(String iname, int idx)
            {
                LittleGameEngine lge = LittleGameEngine.GetInstance();
                if (lge == null) return 0;

                if (iname != null)
                {
                    if (!iname.Equals(this.iname))
                    {
                        surfaces = lge.imagesManager.GetImages(iname);
                        this.iname = iname;
                    }

                    if (idx == -1)
                        idx = this.idx;
                    if (idx >= surfaces.Length)
                        idx = 0;
                    this.idx = idx;

                    surface = surfaces[idx];
                    this.rect.Width = this.surface.Width;
                    this.rect.Height = this.surface.Height;
                    SetCollider(new RectangleF(0, 0, this.rect.Width, this.rect.Height));
                }

                return this.idx;
            }
        }
    }
}
