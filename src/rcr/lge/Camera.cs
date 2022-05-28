using System.Drawing;

namespace rcr
{
    namespace lge
    {
        /**
        * La camara de Little Game Engine
        *
        * @author Roberto carrasco (titos.carrasco@gmail.com)
        *
        */
        public class Camera : GameObject
        {
            protected internal GameObject target;
            protected internal bool targetInCenter;

            /**
            * Crea la camara en la posicion y dimensiones dadas.
            *
            * @param position coordenadas (x, y) de la posicion inicial de la camara
            * @param size     dimension (width, height) de la camara
            */
            protected internal Camera(PointF position, SizeF size) :
                base(position, size, "__LGE_CAMERA__")
            {
                target = null;
                targetInCenter = true;
            }

            /**
            * Mueve la camara segun se desplace su objetivo
            */
            protected internal void FollowTarget()
            {
                // nadie a quien seguir
                if (target == null)
                    return;

                // la posicion del que seguimos
                float x = target.rect.X;
                float y = target.rect.Y;

                // el centro de la camara en el centro del gobj
                if (targetInCenter)
                {
                    x += target.rect.Width / 2.0f;
                    y += target.rect.Height / 2.0f;
                }

                SetPosition((int)(x - rect.Width / 2.0), (int)(y - rect.Height / 2.0));
            }

        }
    }
}
