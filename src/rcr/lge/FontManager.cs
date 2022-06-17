using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;

namespace rcr
{
    namespace lge
    {
        /// <summary>
        /// Clase para manejar Fonts en memoria
        /// </summary>
        public class FontManager
        {
            private readonly Dictionary<String, Font> fonts;
            private readonly PrivateFontCollection ttFonts;

            /// <summary>
            /// Construye una objeto manejador de tipos de letra en memoria
            /// </summary>
            public FontManager()
            {
                fonts = new Dictionary<String, Font>();
                ttFonts = new PrivateFontCollection();
            }

            /// <summary>
            /// Carga un tipo de letra del sistema para ser utilizado en el juego
            /// </summary>
            /// <param name="name">El nombre interno a asignar</param>
            /// <param name="fname">El nombre del tipo de letra</param>
            /// <param name="fstyle">El estilo a aplicar</param>
            /// <param name="fsize">El tamano a asignar</param>
            public void LoadSysFont(String name, String fname, FontStyle fstyle, int fsize)
            {
                FontFamily fontFamily = new FontFamily(fname);
                Font font = new Font(fontFamily, fsize, fstyle);
                fonts.Add(name, font);
                fontFamily.Dispose();
            }

            /// <summary>
            /// Carga un tipo de letra TTF desde un archivo para ser utilizado en el juego
            /// </summary>
            /// <param name="name">El nombre interno a asignar</param>
            /// <param name="fname">El nombre del archivo a cargar</param>
            /// <param name="fstyle">El estilo a aplicar</param>
            /// <param name="fsize">El tamano a asignar</param>
            public void LoadTTFont(String name, String fname, FontStyle fstyle, int fsize)
            {
                fname = fname.Replace('\\', '/');
                ttFonts.AddFontFile(fname);
                FontFamily fontFamily = new FontFamily(ttFonts.Families[ttFonts.Families.Length - 1].Name, ttFonts);
                Font font = new Font(fontFamily, fsize, fstyle);
                fonts.Add(name, font);
                fontFamily.Dispose();
            }

            /// <summary>
            /// Recupera un tipo de letra previamente cargado
            /// </summary>
            /// <param name="name">El nombre del tipo de letra a recuperar</param>
            /// <returns>El tipo de letra</returns>
            public Font GetFont(String name)
            {
                return fonts[name];
            }

            /// <summary>
            /// Obtiene los tipos de letra del sistema
            /// </summary>
            /// <returns>Los tipos de letra</returns>
            static public String[] GetSysFonts()
            {
                List<String> sysfonts = new List<String>();
                InstalledFontCollection ifc = new InstalledFontCollection();
                foreach (FontFamily fa in ifc.Families)
                    sysfonts.Add(fa.Name);
                ifc.Dispose();
                return sysfonts.ToArray();
            }

        }
    }
}
