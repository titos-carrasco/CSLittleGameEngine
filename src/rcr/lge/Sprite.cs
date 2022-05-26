using System;
using System.Drawing;

namespace rcr
{
    namespace lge
    {
        /**
        * Clase para manejar GameObjects animados
        *
        * @author Roberto carrasco (titos.carrasco@gmail.com)
        */
        public class Sprite : GameObject
        {
            Bitmap[] surfaces = { };
            String? iname = null;
            int idx = 0;
            float elapsed = 0;

            /**
            * Crea un GameObject animado con la secuencia de imagenes a utilizar a ser
            * especificada posteriormente
            *
            * @param position posicion inicial (x, y) del GameObject
            */
            public Sprite(PointF position) :
                this(null, position, null)
            { }

            /**
            * Crea un GameObject animado con la secuencia de imagenes a utilizar
            *
            * @param iname    nombre de la secuencia de imagenes a utilizar
            * @param position posicion inicial (x, y) del GameObject
            */
            public Sprite(String iname, PointF position) :
                this(iname, position, null)
            { }

            /**
            * Crea un GameObject animado con la secuencia de imagenes a utilizar
            *
            * @param iname    nombre de la secuencia de imagenes a utilizar
            * @param position posicion inicial (x, y) del GameObject
            * @param name     nombre a asignar a este GameObject
            */
            public Sprite(String? iname, PointF position, String? name) :
                base(position, new SizeF(0, 0), name)
            {
                SetImage(iname);
            }

            /**
            * Retorna el nombre de la secuencia actual de imagenes que utiliza este Sprite
            *
            * @return el nombre de la secuencia
            */
            public String? GetImagesName()
            {
                return iname;
            }

            /**
            * Retorna el indice de la secuencia actual de imagenes que utiliza este Sprite
            *
            * @return el numero de la imagen dentro de la secuencia actual
            */
            public int GetImagesIndex()
            {
                return idx;
            }

            /**
            * Avanza automaticamente a la siguiente imagen de la secuencia de este Sprite
            *
            * @return el indice de la imagen actual
            */
            public int NextImage()
            {
                return NextImage(0, 0);
            }

            /**
            * Avanza automaticamente a la siguiente imagen de la secuencia de este Sprite
            *
            * @param dt tiempo transcurrido desde la ultima invocacion a este metodo
            * @return el indice de la imagen actual
            */
            public int NextImage(float dt)
            {
                return NextImage(dt, 0);
            }

            /**
            * Avanza automaticamente a la siguiente imagen de la secuencia de este Sprite
            *
            * @param dt    tiempo transcurrido desde la ultima invocacion a este metodo
            * @param delay tiempo que debe transcurrir antes de pasar a la siguiente imagen
            *              de la secuencia
            * @return el indice de la imagen actual
            */
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

            /**
            * Establece la secuencia de imagenes a utilizar en este Sprite
            *
            * @param iname el nombre de la secuencia (cargada con LoadImage y especificada
            *              al crear este Sprite)
            * @return el indice de la imagen actual
            */
            public int SetImage(String? iname)
            {
                return SetImage(iname, -1);
            }

            /**
            * Establece la secuencia de imagenes a utilizar en este Sprite
            *
            * @param iname el nombre de la secuencia (cargada con LoadImage y especificada
            *              al crear este Sprite)
            * @param idx   el numero de la secuencia a utilizar
            * @return el indice de la imagen actual
            */
            public int SetImage(String? iname, int idx)
            {
                LittleGameEngine? lge = LittleGameEngine.GetInstance();
                if (lge == null) return 0;

                if (iname != null)
                {
                    if (!iname.Equals(this.iname))
                    {
                        surfaces = lge.GetImages(iname);
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
