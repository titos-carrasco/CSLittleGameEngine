using System;
using System.Drawing;
using System.Drawing.Text;

namespace rcr
{
    namespace lge
    {
        /// <summary>
        /// GameObject para trazar formas en Little Game Engine
        /// <para>@author Roberto carrasco (titos.carrasco@gmail.com)</para>
        /// </summary>
        /// <seealso cref="rcr.lge.GameObject" />
        public class Canvas : GameObject
        {
            /// <summary>
            /// Crea un canvas, para dibujar, en la posicion y dimensiones dadas
            /// </summary>
            /// <param name="origin">Posicion (x, y) del canvas</param>
            /// <param name="size">Dimension (width, height) del canvas</param>
            public Canvas(PointF origin, SizeF size) :
                this(origin, size, null)
            { }

            /// <summary>
            /// Crea un canvas, para dibujar, en la posicion y dimensiones dadas
            /// </summary>
            /// <param name="origin">posicion (x, y) del canvas</param>
            /// <param name="size">dimension (width, height) del canvas</param>
            /// <param name="name">nombre unico para este GameObject</param>
            public Canvas(PointF origin, SizeF size, String name) :
                base(origin, size, name)
            {
                surface = LittleGameEngine.CreateTranslucentImage((int)size.Width, (int)size.Height);
            }

            /// <summary>
            /// Colorea el canvas con el color especificado
            /// </summary>
            /// <param name="color">color el color de relleno.</param>
            public void Fill(Color color)
            {
                Graphics g = Graphics.FromImage(surface);
                g.Clear(color);
                g.Dispose();
            }

            /// <summary>
            /// Traza un texto en este canvas en la posicion, tipo de letra y color
            /// </summary>
            /// <param name="text">El texto a trazar</param>
            /// <param name="position">Coordenada (x, y) en donde se trazara el texto dentro del canvas (linea base del texto).</param>
            /// <param name="fname">Nombre del font (cargado con LoadFont) a utilizar para trazar el texto.</param>
            /// <param name="color">Color a utilizar (r,g,b) para trazar el texto</param>
            public void DrawText(String text, PointF position, String fname, Color color)
            {
                LittleGameEngine lge = LittleGameEngine.GetInstance();

                int x = (int)position.X;
                int y = (int)position.Y;
                Graphics g = Graphics.FromImage(surface);
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                Font f = lge.GetFont(fname);
                SolidBrush brush = new SolidBrush(color);
                g.DrawString(text, f, brush, new Point(x, y));
                brush.Dispose();
                g.Dispose();
            }

            /// <summary>
            /// Traza un punto en este canvas en la posicion y color especificados
            /// </summary>
            /// <param name="position">Coordenada (x, y) en donde se trazara el punto dentro del canvas</param>
            /// <param name="color">Color a utilizar (r,g,b) para trazar el punto.</param>
            public void DrawPoint(PointF position, Color color)
            {
                int x = (int)position.X;
                int y = (int)position.Y;

                Graphics g = Graphics.FromImage(surface);
                SolidBrush brush = new SolidBrush(color);
                Pen pen = new Pen(brush);
                g.DrawLine(pen, new Point(x, y), new Point(x, y));
                pen.Dispose();
                brush.Dispose();
                g.Dispose();
            }

            /// <summary>
            /// Traza un circulo en este canvas en la posicion, de radio y color especificado
            /// </summary>
            /// <param name="position">Coordenada (x, y) en donde se trazara el circulo dentro del canvas</param>
            /// <param name="radius">Radio del circulo a trazar.</param>
            /// <param name="color">Color a utilizar (r,g,b) para trazar el circulo.</param>
            /// <param name="thickness">Si es verdadero se mostrara el borde del circulo.</param>
            public void DrawCircle(PointF position, float radius, Color color, bool thickness)
            {
                int x = (int)position.X;
                int y = (int)position.Y;
                int w = (int)radius;
                int h = w;

                Graphics g = Graphics.FromImage(surface);
                SolidBrush brush = new SolidBrush(color);
                Pen pen = new Pen(brush);
                if (thickness)
                    g.DrawEllipse(pen, new Rectangle(x, y, w, h));
                else
                    g.FillEllipse(brush, new Rectangle(x, y, w, h));
                pen.Dispose();
                brush.Dispose();
                g.Dispose();
            }

            /// <summary>
            /// Traza un rectangulo en este canvas en la posicion, dimensiones y color especificado
            /// </summary>
            /// <param name="position">Coordenada (x, y) en donde se trazara el circulo dentro del canvas.</param>
            /// <param name="size">Dimension (width, height) del rectangulo</param>
            /// <param name="color">Color a utilizar (r,g,b) para trazar el rectangulo</param>
            /// <param name="thickness">Si es True se mostrara el borde del rectangulo</param>
            public void DrawRectangle(PointF position, SizeF size, Color color, bool thickness)
            {
                int x = (int)position.X;
                int y = (int)position.Y;
                int w = (int)size.Width;
                int h = (int)size.Height;

                Graphics g = Graphics.FromImage(surface);
                SolidBrush brush = new SolidBrush(color);
                Pen pen = new Pen(brush);
                if (thickness)
                    g.DrawRectangle(pen, new Rectangle(x, y, w, h));
                else
                    g.FillRectangle(brush, new Rectangle(x, y, w, h));
                pen.Dispose();
                brush.Dispose();
                g.Dispose();
            }

            /// <summary>
            /// Traza una superficie en este canvas en la posicion dada
            /// </summary>
            /// <param name="position">Coordenada (x, y) en donde se trazara la superfice dentro del canvas</param>
            /// <param name="surface">Superficie (imagen) a trazar</param>
            public void DrawSurface(PointF position, Bitmap surface)
            {
                int x = (int)position.X;
                int y = (int)position.Y;

                Graphics g = Graphics.FromImage(surface);
                g.DrawImageUnscaled(this.surface, x, y);
                g.Dispose();
            }

        }
    }
}
