using System.Drawing;

namespace rcr
{
    namespace lge
    {
        /// <summary>
        /// La camara de Little Game Engine
        /// <para>@author Roberto carrasco (titos.carrasco@gmail.com)</para>
        /// </summary>
        /// <seealso cref="rcr.lge.GameObject" />
        public class Camera : GameObject
        {
            /// <value>El GameObject al cual seguira la camara</value>
            protected internal GameObject target;

            /// <value>true para que la camara siga el centro del GameObject</value>
            protected internal bool targetInCenter;

            /// <summary>
            /// Crea la camara en la posicion y dimensiones dadas
            /// </summary>
            /// <param name="position">Coordenadas (x, y) de la posicion inicial de la camara</param>
            /// <param name="size">Dimension (width, height) de la camara</param>
            protected internal Camera(PointF position, SizeF size) :
                base(position, size, "__LGE_CAMERA__")
            {
                target = null;
                targetInCenter = true;
            }

            ///<summary>
            /// Mueve la camara segun se desplace su objetivo
            ///</summary>
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
