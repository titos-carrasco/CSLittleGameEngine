using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace rcr
{
    namespace lge
    {
        /// <summary>
        /// Manejador de imagenes en memoria
        /// </summary>
        public class ImageManager
        {
            private readonly Dictionary<String, Bitmap[]> images;

            /// <summary>
            /// Construye un objeto manejador de imagenes en memoria
            /// </summary>
            public ImageManager()
            {
                images = new Dictionary<String, Bitmap[]>();
            }

            /// <summary>
            /// Recupera un grupo de imagenes previamente cargadas
            /// </summary>
            /// <param name="iname">El nombre asignado al grupo de imagenes</param>
            /// <returns>La lista de imagenes</returns>
            public Bitmap[] GetImages(String iname)
            {
                return images[iname];
            }

            /// <summary>
            /// Carga una imagen o grupo de imagenes
            /// </summary>
            /// <param name="iname">el nombre interno a asignar a las imagenes cargadas.</param>
            /// <param name="pattern">El patron de archivos para las imagenes a cargar (glob)</param>
            /// <param name="flipX">Si es verdadero da vuelta la imagen en X</param>
            /// <param name="flipY">Si es verdadero da vuelta la imagen en Y.</param>
            public void LoadImages(String iname, String pattern, bool flipX, bool flipY)
            {
                LoadImages(iname, pattern, 0, null, flipX, flipY);
            }

            /// <summary>
            /// Carga una imagen o grupo de imagenes
            /// </summary>
            /// <param name="iname">el nombre interno a asignar a las imagenes cargadas.</param>
            /// <param name="pattern">El patron de archivos para las imagenes a cargar (glob)</param>
            /// <param name="size">Tamano a aplicar a la imagen</param>
            /// <param name="flipX">Si es verdadero da vuelta la imagen en X</param>
            /// <param name="flipY">Si es verdadero da vuelta la imagen en Y.</param>
            public void LoadImages(String iname, String pattern, Size size, bool flipX, bool flipY)
            {
                LoadImages(iname, pattern, 0, size, flipX, flipY);
            }

            /// <summary>
            /// Carga una imagen o grupo de imagenes
            /// </summary>
            /// <param name="iname">el nombre interno a asignar a las imagenes cargadas.</param>
            /// <param name="pattern">El patron de archivos para las imagenes a cargar (glob)</param>
            /// <param name="scale">Factor de escala de la imagen</param>
            /// <param name="flipX">Si es verdadero da vuelta la imagen en X</param>
            /// <param name="flipY">Si es verdadero da vuelta la imagen en Y.</param>
            public void LoadImages(String iname, String pattern, float scale, bool flipX, bool flipY)
            {
                LoadImages(iname, pattern, scale, null, flipX, flipY);
            }

            /// <summary>
            /// Carga una imagen o grupo de imagenes
            /// </summary>
            /// <param name="iname">el nombre interno a asignar a las imagenes cargadas.</param>
            /// <param name="pattern">El patron de archivos para las imagenes a cargar (glob)</param>
            /// <param name="scale">Factor de escala de la imagen</param>
            /// <param name="size">Tamano a aplicar a la imagen</param>
            /// <param name="flipX">Si es verdadero da vuelta la imagen en X</param>
            /// <param name="flipY">Si es verdadero da vuelta la imagen en Y.</param>
            private void LoadImages(String iname, String pattern, float scale, Size? size, bool flipX, bool flipY)
            {
                String[] fnames = LittleGameEngine.ExpandFilenames(pattern);
                List<Bitmap> bitmaps = new List<Bitmap>();

                foreach (String fname in fnames)
                {
                    Bitmap bmp = new Bitmap(fname);
                    int width = bmp.Width;
                    int height = bmp.Height;
                    if (scale > 0)
                    {
                        width = (int)Math.Round(bmp.Width * scale);
                        height = (int)Math.Round(bmp.Height * scale);
                    }
                    else if( size!=null)
                    {
                        width = size.Value.Width;
                        height = size.Value.Height;
                    }

                    Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                    Graphics g = Graphics.FromImage(image);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(bmp, 0, 0, width, height);
                    g.Dispose();

                    bmp.Dispose();

                    FlipImage(image, flipX, flipY);
                    bitmaps.Add(image);
                }
                this.images.Add(iname, bitmaps.ToArray());
            }

            /// <summary>
            /// Invierte una imagen
            /// </summary>
            /// <param name="bitmap">La imagen a invertir</param>
            /// <param name="flipX">Si es verdadero se invierte en el eje X</param>
            /// <param name="flipY">Si es verdadero se invierte en el eje Y</param>
            ///
            static public void FlipImage(Bitmap bitmap, bool flipX, bool flipY)
            {
                if (flipX)
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                if (flipY)
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }

            /// <summary>
            /// Crea una imagen sin transparencia de dimensiones dadas
            /// </summary>
            /// <param name="width">El ancho de la imagen</param>
            /// <param name="height">El alto de la imagen</param>
            /// <returns>La imagen sin transparencia</returns>
            static public Bitmap CreateOpaqueImage(int width, int height)
            {
                return new Bitmap(width, height, PixelFormat.Format32bppRgb);
            }

            /// <summary>
            /// Crea una imagen con transparencia de dimensiones dadas
            /// </summary>
            /// <param name="width">El ancho de la imagen</param>
            /// <param name="height">El alto de la imagen</param>
            /// <returns>La imagen con transparencia</returns>
            static public Bitmap CreateTranslucentImage(int width, int height)
            {
                return new Bitmap(width, height, PixelFormat.Format32bppArgb);
            }
        }
    }
}
