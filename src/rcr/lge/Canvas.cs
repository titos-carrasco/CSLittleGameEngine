using System;
using System.Drawing;
using System.Drawing.Text;

namespace rcr
{
    namespace lge
    {
        /**
        * GameObject para trazar formas en Little Game Engine
        *
        * @author Roberto carrasco (titos.carrasco@gmail.com)
        *
        */
        public class Canvas : GameObject
        {

            /**
            * Crea un canvas, para dibujar, en la posicion y dimensiones dadas
            *
            * @param origin posicion (x, y) del canvas
            * @param size   dimension (width, height) del canvas
            */
            public Canvas(PointF origin, SizeF size) :
                this(origin, size, null)
            { }

            /**
            * Crea un canvas, para dibujar, en la posicion y dimensiones dadas
            *
            * @param origin posicion (x, y) del canvas
            * @param size   dimension (width, height) del canvas
            * @param name   nombre para esta GameObject
            */
            public Canvas(PointF origin, SizeF size, String? name) :
                base(origin, size, name)
            {
                surface = LittleGameEngine.CreateTranslucentImage((int)size.Width, (int)size.Height);
            }

            /**
            * Colorea el canvas con el color especificado
            *
            * @param color el color de relleno
            */
            public void Fill(Color color)
            {
                Graphics g = Graphics.FromImage(surface!);
                g.Clear(color);
                g.Dispose();
            }

            /**
            *
            * Traza un texto en este canvas en la posicion, tipo de letra y color
            * especificados
            *
            * @param text     el texto a trazar
            * @param position coordenada (x, y) en donde se trazara el texto dentro del
            *                 canvas (linea base del texto)
            * @param fname    nombre del font (cargado con LoadFont) a utilizar para trazar
            *                 el texto
            * @param color    color a utilizar (r,g,b) para trazar el texto
            */
            public void DrawText(String text, PointF position, String fname, Color color)
            {
                LittleGameEngine lge = LittleGameEngine.GetInstance();

                int x = (int)position.X;
                int y = (int)position.Y;
                Graphics g = Graphics.FromImage(surface!);
                g.TextRenderingHint = TextRenderingHint.AntiAlias;
                Font f = lge.GetFont(fname);
                SolidBrush brush = new SolidBrush(color);
                g.DrawString(text, f, brush, new Point(x, y));
                brush.Dispose();
                g.Dispose();
            }

            /**
            * Traza un punto en este canvas en la posicion y color especificados
            *
            * @param position coordenada (x, y) en donde se trazara el punto dentro del
            *                 canvas
            * @param color    color a utilizar (r,g,b) para trazar el punto
            */
            public void DrawPoint(PointF position, Color color)
            {
                int x = (int)position.X;
                int y = (int)position.Y;

                Graphics g = Graphics.FromImage(surface!);
                SolidBrush brush = new SolidBrush(color);
                Pen pen = new Pen(brush);
                g.DrawLine(pen, new Point(x, y), new Point(x, y));
                pen.Dispose();
                brush.Dispose();
                g.Dispose();
            }

            /**
            * Traza un circulo en este canvas en la posicion, de radio y color especificado
            *
            * @param position  coordenada (x, y) en donde se trazara el circulo dentro del
            *                  canvas
            * @param radius    radio del circulo a trazar
            * @param color     color a utilizar (r,g,b) para trazar el circulo
            * @param thickness si es verdadero se mostrara el borde del circulo
            */
            public void DrawCircle(PointF position, float radius, Color color, bool thickness)
            {
                int x = (int)position.X;
                int y = (int)position.Y;
                int w = (int)radius;
                int h = w;

                Graphics g = Graphics.FromImage(surface!);
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

            /**
            * Traza un rectangulo en este canvas en la posicion, dimensiones y color
            * especificado
            *
            * @param position  coordenada (x, y) en donde se trazara el circulo dentro del
            *                  canvas
            * @param size      dimension (width, height) del rectangulo
            * @param color     color a utilizar (r,g,b) para trazar el rectangulo
            * @param thickness si es True se mostrara el borde del rectangulo
            */
            public void DrawRectangle(PointF position, SizeF size, Color color, bool thickness)
            {
                int x = (int)position.X;
                int y = (int)position.Y;
                int w = (int)size.Width;
                int h = (int)size.Height;

                Graphics g = Graphics.FromImage(surface!);
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

            /**
            * Traza una superficie en este canvas en la posicion dada
            *
            * @param position coordenada (x, y) en donde se trazara la superfice dentro del
            *                 canvas
            * @param surface  superficie (imagen) a trazar
            */
            public void DrawSurface(PointF position, Bitmap surface)
            {
                int x = (int)position.X;
                int y = (int)position.Y;

                Graphics g = Graphics.FromImage(surface);
                g.DrawImage(this.surface!, new Point(x, y));
                g.Dispose();
            }

        }
    }
}
