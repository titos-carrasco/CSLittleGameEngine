using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace rcr
{
    namespace lge
    {
        /// <summary>
        /// La Pequena Maquina de Juegos
        /// <para>@author Roberto carrasco (titos.carrasco@gmail.com)</para>
        /// </summary>
        public class LittleGameEngine : Form
        {
            public const int GUI_LAYER = 0xFFFF;

            private static LittleGameEngine lge = null;

            private readonly SortedDictionary<int, List<GameObject>> gLayers;
            private readonly Dictionary<String, GameObject> gObjects;
            private readonly List<GameObject> gObjectsToAdd;
            private readonly List<GameObject> gObjectsToDel;
            private readonly Camera camera;

            private readonly Size winSize;
            private readonly float[] fpsData;
            private int fpsIdx;
            private bool running = false;

            public Action<float> onMainUpdate = null;
            private readonly Dictionary<Keys, bool> keysPressed;
            private readonly bool[] mouseButtons = { false, false, false };
            private readonly Nullable<Point>[] mouseClicks = { null, null, null };

            private readonly Dictionary<String, Bitmap[]> images;
            private readonly Dictionary<String, Font> fonts;
            private readonly PrivateFontCollection ttfFonts;

            private readonly Bitmap screen;
            private readonly Color bgColor;
            private Nullable<Color> collidersColor = null;

            // ------ game engine ------

            /// <summary>
            /// Crea el juego
            /// </summary>
            /// <param name="winSize">Dimensiones de la ventana de despliegue.</param>
            /// <param name="title">Titulo de la ventana.</param>
            /// <param name="bgColor">Color de fondo de la ventana</param>
            /// <exception cref="System.ApplicationException">LittleGameEngine ya se encuentra activa</exception>
            public LittleGameEngine(Size winSize, String title, Color bgColor)
            {
                if (lge != null)
                    throw new ApplicationException("LittleGameEngine ya se encuentra activa");

                lge = this;
                this.winSize = winSize;

                fpsData = new float[30];
                fpsIdx = 0;

                this.bgColor = bgColor;

                images = new Dictionary<String, Bitmap[]>();
                fonts = new Dictionary<String, Font>();
                ttfFonts = new PrivateFontCollection();

                keysPressed = new Dictionary<Keys, bool>();

                gObjects = new Dictionary<String, GameObject>();
                gLayers = new SortedDictionary<int, List<GameObject>>();
                gObjectsToAdd = new List<GameObject>();
                gObjectsToDel = new List<GameObject>();

                camera = new Camera(new PointF(0, 0), winSize);

                screen = CreateOpaqueImage(winSize.Width, winSize.Height);
                Graphics g = Graphics.FromImage(screen);
                g.Clear(bgColor);
                g.Dispose();

                this.MouseDown += MousePressed;
                this.MouseUp += MouseReleased;
                this.MouseClick += MouseClicked;
                this.Text = title;
                this.FormBorderStyle = FormBorderStyle.Fixed3D;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.AutoSize = true;
                this.DoubleBuffered = true;
                this.ClientSize = winSize;
                this.Show();

                Application.DoEvents();
            }

            /// <summary>
            /// Obtiene una instancia del juego en ejecucion. Util para las diferentes clases utilizadas en un juego tal de acceder a metodos estaticos
            /// </summary>
            /// <returns><La instancia de LGE en ejecucion/returns>
            /// <exception cref="System.ApplicationException">LGE no se encuentra activo</exception>
            public static LittleGameEngine GetInstance()
            {
                if (lge == null)
                    throw new ApplicationException("LGE no se encuentra activo");
                return lge;
            }

            /// <summary>
            /// Obtiene los FPS calculados como el promedio de los ultimos 30 valores
            /// </summary>
            /// <returns>Los frame por segundo calculados</returns>
            public float GetFPS()
            {
                float dt = 0;
                foreach (float val in fpsData)
                    dt += val;
                dt /= fpsData.Length;
                return dt == 0 ? 0 : 1.0f / dt;
            }

            /// <summary>
            /// Si se especifica un color se habilita el despliegue del rectangulo que bordea a todos los objetos (util para ver colisiones).
            /// </summary>
            /// <param name="color">El color para los bordes de los rectangulos.</param>
            public void ShowColliders(Color color)
            {
                collidersColor = color;
            }

            /// <summary>
            /// Finaliza el Game Loop de LGE
            /// </summary>
            public void Quit()
            {
                lock (this)
                {
                    running = false;
                }
            }

            /// <summary>
            /// Inicia el Game Loop de LGE tratando de mantener los fps especificados
            /// </summary>
            /// <param name="fps">Los fps a mantener</param>
            public void Run(int fps)
            {
                Thread thread = new Thread(() => TRun(fps));
                thread.Start();
                Application.Run(this);
                thread.Join();
            }

            /// <summary>
            /// El GameLoop de la Pequena Maquina de Juegos
            /// </summary>
            /// <param name="fps">Los FPS a mantener</param>
            private void TRun(int fps)
            {
                Bitmap screenImage = CreateOpaqueImage(winSize.Width, winSize.Height);
                running = true;
                long tickExpected = TimeSpan.TicksPerSecond / fps;
                long tickPrev = DateTime.Now.Ticks;
                while (true)
                {
                    lock (this)
                    {
                        if (!running)
                            break;
                    }

                    // los eventos son atrapados por los listener

                    // --- tiempo en ms desde el ciclo anterior
                    long tickElapsed = DateTime.Now.Ticks - tickPrev;
                    if (tickElapsed < tickExpected)
                        Thread.Sleep((int)((tickExpected - tickElapsed) / TimeSpan.TicksPerMillisecond));

                    long now = DateTime.Now.Ticks;
                    float dt = (now - tickPrev) / (float)TimeSpan.TicksPerSecond;
                    tickPrev = now;

                    fpsData[fpsIdx++] = dt;
                    fpsIdx %= fpsData.Length;

                    // --- Del gobj and gobj.OnDelete
                    List<GameObject> ondelete = new List<GameObject>();
                    foreach (GameObject gobj in gObjectsToDel)
                    {
                        gObjects.Remove(gobj.name);
                        gLayers[gobj.layer].Remove(gobj);
                        if (camera.target == gobj)
                            camera.target = null;
                    }
                    foreach (GameObject gobj in ondelete)
                        gobj.OnDelete();
                    gObjectsToDel.Clear();

                    // --- Add Gobj and gobj.OnStart
                    List<GameObject> onstart = new List<GameObject>();
                    foreach (GameObject gobj in gObjectsToAdd)
                    {
                        int layer = gobj.layer;
                        List<GameObject> gobjs;
                        if (gLayers.ContainsKey(layer))
                            gobjs = gLayers[layer];
                        else
                        {
                            gobjs = new List<GameObject>();
                            gLayers.Add(layer, gobjs);
                        }
                        if (!gobjs.Contains(gobj))
                        {
                            gobjs.Add(gobj);
                        }
                    }
                    foreach (GameObject gobj in onstart)
                        gobj.OnStart();
                    gObjectsToAdd.Clear();

                    // --- gobj.OnPreUpdate
                    foreach (KeyValuePair<int, List<GameObject>> elem in gLayers)
                        foreach (GameObject gobj in elem.Value)
                            gobj.OnPreUpdate(dt);

                    // --- gobj.OnUpdate
                    foreach (KeyValuePair<int, List<GameObject>> elem in gLayers)
                        foreach (GameObject gobj in elem.Value)
                            gobj.OnUpdate(dt);

                    // --- gobj.OnPostUpdate
                    foreach (KeyValuePair<int, List<GameObject>> elem in gLayers)
                        foreach (GameObject gobj in elem.Value)
                            gobj.OnPostUpdate(dt);

                    // --- game.OnMainUpdate
                    if (onMainUpdate != null)
                        onMainUpdate(dt);

                    // --- gobj.OnCollision
                    Dictionary<GameObject, List<GameObject>> oncollisions = new Dictionary<GameObject, List<GameObject>>();
                    foreach (KeyValuePair<int, List<GameObject>> elem in gLayers)
                    {
                        int layer = elem.Key;
                        if (layer != GUI_LAYER)
                        {
                            foreach (GameObject gobj1 in elem.Value)
                            {
                                if (!gobj1.callOnCollision)
                                    continue;

                                if (!gobj1.useColliders)
                                    continue;

                                List<GameObject> colliders = new List<GameObject>();
                                foreach (GameObject gobj2 in elem.Value)
                                {
                                    if (gobj1 == gobj2)
                                        continue;
                                    if (!gobj2.useColliders)
                                        continue;
                                    if (!gobj1.CollidesWith(gobj2))
                                        continue;
                                    colliders.Add(gobj2);
                                }
                                if (colliders.Count > 0)
                                    oncollisions.Add(gobj1, colliders);

                            }
                        }
                    }
                    foreach (KeyValuePair<GameObject, List<GameObject>> elem in oncollisions)
                    {
                        GameObject gobj = elem.Key;
                        List<GameObject> gobjs = elem.Value;
                        gobj.OnCollision(dt, gobjs.ToArray());
                    }

                    // --- gobj.OnPreRender
                    foreach (KeyValuePair<int, List<GameObject>> elem in gLayers)
                        foreach (GameObject gobj in elem.Value)
                            gobj.OnPreRender(dt);

                    // --- Camera Tracking
                    camera.FollowTarget();

                    // --- Rendering
                    Graphics g = Graphics.FromImage(screenImage);
                    g.Clear(bgColor);

                    // --- layers
                    SolidBrush brush = new SolidBrush(bgColor);
                    Pen pen = new Pen(brush);
                    if (collidersColor != null)
                    {
                        brush = new SolidBrush((Color)collidersColor);
                        pen = new Pen(brush);
                    }
                    foreach (KeyValuePair<int, List<GameObject>> elem in gLayers)
                    {
                        int layer = elem.Key;
                        if (layer != GUI_LAYER)
                        {
                            foreach (GameObject gobj in elem.Value)
                            {
                                if (!gobj.rect.IntersectsWith(camera.rect))
                                    continue;
                                PointF p = FixXY(gobj.GetPosition());
                                Image surface = gobj.surface;
                                if (surface != null)
                                    g.DrawImage(surface, new Point((int)p.X, (int)p.Y));

                                if (collidersColor != null && gobj.useColliders)
                                {
                                    foreach (RectangleF r in gobj.GetCollider())
                                    {
                                        p = FixXY(new PointF(r.X, r.Y));
                                        g.DrawRectangle(pen, new Rectangle((int)p.X, (int)p.Y, (int)r.Width - 1, (int)r.Height - 1));
                                    }
                                }
                            }
                        }
                    }
                    pen.Dispose();
                    brush.Dispose();

                    // --- GUI

                    foreach (KeyValuePair<int, List<GameObject>> elem in gLayers)
                    {
                        int layer = elem.Key; ;
                        if (layer == GUI_LAYER)
                        {
                            foreach (GameObject gobj in elem.Value)
                            {
                                Bitmap surface = gobj.surface;
                                if (surface != null)
                                {
                                    float x = gobj.rect.X;
                                    float y = gobj.rect.Y;
                                    g.DrawImage(surface, new Point((int)x, (int)y));
                                }
                            }
                        }
                    }
                    g.Dispose();

                    lock (screen)
                    {
                        g = Graphics.FromImage(screen);
                        g.DrawImage(screenImage, new Point(0, 0));
                        g.Dispose();
                    }

                    this.Invoke(
                        new Action(() => this.Refresh())
                    );
                }

                // --- gobj.OnQuit
                foreach (KeyValuePair<int, List<GameObject>> elem in gLayers)
                    foreach (GameObject gobj in elem.Value)
                        gobj.OnQuit();

                // cerramos la ventana en caso de que siga abierta
                this.Invoke(
                    new Action(() => this.Close())
                );
                this.Invoke(
                    new Action(() => this.Dispose())
                );
                Application.Exit();
            }

            /// <summary>
            /// Traslada las coordenadas del GameObject a la zona de despliegue de la camara
            /// </summary>
            /// <param name="p">Las coordenadas a trasladar</param>
            /// <returns>Las coordenadas trasladadas</returns>
            private PointF FixXY(PointF p)
            {
                float xo = p.X;
                float vx = camera.rect.X;
                float x = xo - vx;

                float yo = p.Y;
                float vy = camera.rect.Y;
                float y = yo - vy;

                return new PointF(x, y);
            }

            // ------ gobjects ------

            /// <summary>
            /// Agrega un GameObject al juego el que quedara habilitado en el siguiente ciclo
            /// </summary>
            /// <param name="gobj">El GameObject a agregar</param>
            /// <param name="layer">La capa a la cual pertenece</param>
            public void AddGObject(GameObject gobj, int layer)
            {
                gobj.layer = layer;
                gObjects.Add(gobj.name, gobj);
                gObjectsToAdd.Add(gobj);
            }

            /// <summary>
            /// Agrega un GameObject a la interfaz grafica del juego
            /// </summary>
            /// <param name="gobj">El GameObject a agregar</param>
            public void AddGObjectGUI(GameObject gobj)
            {
                AddGObject(gobj, GUI_LAYER);
            }

            /// <summary>
            /// Busca un GameObject por su nombre
            /// </summary>
            /// <param name="name">El nombre del GameObject a buscar</param>
            /// <returns>El GameObject encontrado</returns>
            public GameObject GetGObject(String name)
            {
                return gObjects[name];
            }

            /// <summary>
            /// Obtiene todos los GameObject de una capa cuyo tag comienza con un texto dado
            /// </summary>
            /// <param name="layer">La capa en donde buscar</param>
            /// <param name="tag">El texto inicial del tag</param>
            /// <returns>Los GameObjects encontrados</returns>
            public GameObject[] FindGObjectsByTag(int layer, String tag)
            {
                List<GameObject> gobjs = new List<GameObject>();

                foreach (GameObject o in gLayers[layer])
                    if (o.name.StartsWith(tag))
                        gobjs.Add(o);

                return gobjs.ToArray(); ;
            }

            /// <summary>
            /// Determina el total de GameObjects en el juego
            /// </summary>
            /// <returns>El total de GameObjects</returns>
            public int GetCountGObjects()
            {
                return gObjects.Count;
            }

            /// <summary>
            /// Elimina un GameObject del juego en el siguiente ciclo
            /// </summary>
            /// <param name="gobj">El GameObject a eliminar</param>
            public void DelGObject(GameObject gobj)
            {
                gObjectsToDel.Add(gobj);
            }

            /// <summary>
            /// Determina todos los GameObject que colisionan con un GameObject dado en la misma capa
            /// </summary>
            /// <param name="gobj">El GameObject a inspeccionar.</param>
            /// <returns>Los GameObjects con los que colisiona</returns>
            public GameObject[] CollidesWith(GameObject gobj)
            {
                List<GameObject> gobjs = new List<GameObject>();

                if (gobj.useColliders)
                    foreach (GameObject o in gLayers[gobj.layer])
                        if (gobj != o && o.useColliders && gobj.CollidesWith(o))
                            gobjs.Add(o);

                return gobjs.ToArray();
            }

            // ------ camera ------

            /// <summary>
            /// Obtiene la posiciona de la camara
            /// </summary>
            /// <returns>La posicion de la camara</returns>
            public PointF GetCameraPosition()
            {
                return camera.GetPosition();
            }

            /// <summary>
            /// Obtiene la dimension de la camara
            /// </summary>
            /// <returns>La dimension de la camara</returns>
            public SizeF GetCameraSize()
            {
                return camera.GetSize();
            }

            /// <summary>
            /// Establece el GameObject al cual la camara seguira de manera automatica
            /// </summary>
            /// <param name="gobj">El GameObject a seguir</param>
            public void SetCameraTarget(GameObject gobj)
            {
                if (gobj == null)
                    camera.target = gobj;
                else
                    SetCameraTarget(gobj, true);
            }

            /// <summary>
            /// Establece el GameObject al cual la camara seguira de manera automatica
            /// </summary>
            /// <param name="gobj">El GameObject a seguir</param>
            /// <param name="center">Si es verdadero la camara se centrara en el centro del GameObject, en caso contrario lo hara en el extremo superior izquierdo</param>
            public void SetCameraTarget(GameObject gobj, bool center)
            {
                camera.target = gobj;
                camera.targetInCenter = center;
            }

            /// <summary>
            /// Establece los limites en los cuales se movera la camara
            /// </summary>
            /// <param name="bounds">Los limites</param>
            public void SetCameraBounds(RectangleF bounds)
            {
                camera.SetBounds(bounds);
            }

            /// <summary>
            /// Establece la posicion de la camara
            /// </summary>
            /// <param name="position">La posicion</param>
            public void SetCameraPosition(PointF position)
            {
                camera.SetPosition(position);
            }

            // ------ keys ------

            /// <summary>
            /// Determina si una tecla se encuentra presionada o no
            /// </summary>
            /// <param name="key">La tecla a inspeccionar</param>
            /// <returns>Verdadero si la tecla se encuentra presionada</returns>
            public bool KeyPressed(Keys key)
            {
                lock (keysPressed)
                {
                    return keysPressed.ContainsKey(key) ? keysPressed[key] : false;
                }
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);
                lock (keysPressed)
                {
                    if (keysPressed.ContainsKey(e.KeyCode))
                        keysPressed[e.KeyCode] = true;
                    else
                        keysPressed.Add(e.KeyCode, true);
                }
            }

            protected override void OnKeyUp(KeyEventArgs e)
            {
                base.OnKeyUp(e);
                lock (keysPressed)
                {
                    if (keysPressed.ContainsKey(e.KeyCode))
                        keysPressed[e.KeyCode] = false;
                    else
                        keysPressed.Add(e.KeyCode, false);
                }
            }

            // ------ mouse ------

            /// <summary>
            /// Determina el estado de los botones del mouse
            /// </summary>
            /// <returns>El estado de los botones</returns>
            public bool[] GetMouseButtons()
            {
                lock (mouseButtons)
                {
                    return (bool[])mouseButtons.Clone();
                }
            }

            /// <summary>
            /// Determina la posicion del mouse en la ventana.
            /// </summary>
            /// <returns>La posicion del mouse</returns>
            public Point GetMousePosition()
            {
                Point p = new Point(-1,-1);

                this.Invoke(new Action(() => p = PointToClient(Cursor.Position)));
                if (p == null){
                    p = new Point(-1, -1);
                }
                else if( p.X < 0 || p.X >= this.winSize.Width || p.Y < 0 || p.Y >= this.winSize.Height)
                {
                    p = new Point(-1, -1);
                }

                return p;
            }

            /// <summary>
            /// Determina si un boton del mouse se encuentra presionado
            /// </summary>
            /// <param name="button">El boton a inspeccionar</param>
            /// <returns>Verdadero si se encuentra presionado</returns>
            public Nullable<Point> GetMouseClicked(int button)
            {
                lock (mouseClicks)
                {
                    Nullable<Point> p = mouseClicks[button];
                    mouseClicks[button] = null;
                    return p;
                }
            }

            public void MouseClicked(Object sender, MouseEventArgs e)
            {
                lock (mouseClicks)
                {
                    int idx = e.Button.Equals(MouseButtons.Left) ? 0 : e.Button.Equals(MouseButtons.Middle) ? 1 : 2;
                    mouseClicks[idx] = new Point(e.X, e.Y);
                }
            }

            public void MousePressed(Object sender, MouseEventArgs e)
            {
                lock (mouseButtons)
                {
                    int idx = e.Button.Equals(MouseButtons.Left) ? 0 : e.Button.Equals(MouseButtons.Middle) ? 1 : 2;
                    mouseButtons[idx] = true;
                }
            }

            public void MouseReleased(Object sender, MouseEventArgs e)
            {
                lock (mouseButtons)
                {
                    int idx = e.Button.Equals(MouseButtons.Left) ? 0 : e.Button.Equals(MouseButtons.Middle) ? 1 : 2;
                    mouseButtons[idx] = false;
                }
            }

            // ------ fonts ------

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

            /// <summary>
            /// Carga un tipo de letra del sistema para ser utilizado en el juego
            /// </summary>
            /// <param name="name">El nombre interno a asignar</param>
            /// <param name="fname">El nombre del tipo de letra</param>
            /// <param name="fstyle">El estilo a aplicar</param>
            /// <param name="fsize">El tama�o a asignar</param>
            public void LoadSysFont(String name, String fname, FontStyle fstyle, int fsize)
            {
                FontFamily fontFamily = new FontFamily(fname);
                Font font = new Font(fontFamily, fsize, fstyle);
                fonts.Add(name, font);
            }

            /// <summary>
            /// Carga un tipo de letra TTF desde un archivo para ser utilizado en el juego
            /// </summary>
            /// <param name="name">El nombre interno a asignar</param>
            /// <param name="fname">El nombre del archivo a cargar</param>
            /// <param name="fstyle">El estilo a aplicar</param>
            /// <param name="fsize">El tama�o a asignar</param>
            public void LoadTTFont(String name, String fname, FontStyle fstyle, int fsize)
            {
                ttfFonts.AddFontFile(fname);
                FontFamily fontFamily = new FontFamily(ttfFonts.Families[ttfFonts.Families.Length - 1].Name, ttfFonts);
                Font font = new Font(fontFamily, fsize, fstyle);
                fonts.Add(name, font);
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

            // ------ images ------

            /// <summary>
            /// Crea una imagen sin transparencia de dimensiones dadas
            /// </summary>
            /// <param name="width">El ancho de la imagen</param>
            /// <param name="height">El alto de la imagen</param>
            /// <returns>La imagen sin transparencia</returns>
            static protected internal Bitmap CreateOpaqueImage(int width, int height)
            {
                return new Bitmap(width, height);
            }

            /// <summary>
            /// Crea una imagen con transparencia de dimensiones dadas
            /// </summary>
            /// <param name="width">El ancho de la imagen</param>
            /// <param name="height">El alto de la imagen</param>
            /// <returns>La imagen con transparencia</returns>
            static protected internal Bitmap CreateTranslucentImage(int width, int height)
            {
                return new Bitmap(width, height);
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

            private void FlipImage(Bitmap bitmap, bool flipX, bool flipY)
            {
                if (flipX && flipY)
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                else if (flipX)
                {
                    bitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                    bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
                else if (flipY)
                {
                    bitmap.RotateFlip(RotateFlipType.Rotate90FlipY);
                    bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
            }

            /// <summary>
            /// Carga una imagen o grupo de imagenes
            /// </summary>
            /// <param name="iname">el nombre interno a asignar a las imagenes cargadas.</param>
            /// <param name="pattern">El patron de archivos para las imagenes a cargar (glob)</param>
            /// <param name="flipX">Si es verdadero da vuelta la imagen en X</param>
            /// <param name="flipY">Si es verdadero da vuelta la imagen en Y.</param>
            public void LoadImage(String iname, String pattern, bool flipX, bool flipY)
            {
                List<Bitmap> bitmaps = ReadImages(pattern);
                foreach (Bitmap bmp in bitmaps)
                {
                    FlipImage(bmp, flipX, flipY);
                }
                this.images.Add(iname, bitmaps.ToArray());
            }

            /// <summary>
            /// Carga una imagen o grupo de imagenes
            /// </summary>
            /// <param name="iname">el nombre interno a asignar a las imagenes cargadas.</param>
            /// <param name="pattern">El patron de archivos para las imagenes a cargar (glob)</param>
            /// <param name="size">Tamano a aplicar a la imagen</param>
            /// <param name="flipX">Si es verdadero da vuelta la imagen en X</param>
            /// <param name="flipY">Si es verdadero da vuelta la imagen en Y.</param>
            public void LoadImage(String iname, String pattern, Size size, bool flipX, bool flipY)
            {
                List<Bitmap> bitmaps = ReadImages(pattern);
                int nimages = bitmaps.Count;
                for (int i = 0; i < nimages; i++)
                {
                    Bitmap b = bitmaps[i];
                    Bitmap bmp = new Bitmap(size.Width, size.Height);

                    Graphics g = Graphics.FromImage(bmp);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(b, 0, 0, size.Width, size.Height);
                    g.Dispose();
                    FlipImage(bmp, flipX, flipY);
                    bitmaps[i] = bmp;
                }
                this.images.Add(iname, bitmaps.ToArray());
            }

            /// <summary>
            /// Carga una imagen o grupo de imagenes
            /// </summary>
            /// <param name="iname">el nombre interno a asignar a las imagenes cargadas.</param>
            /// <param name="pattern">El patron de archivos para las imagenes a cargar (glob)</param>
            /// <param name="scale">Factor de escala de la imagen</param>
            /// <param name="flipX">Si es verdadero da vuelta la imagen en X</param>
            /// <param name="flipY">Si es verdadero da vuelta la imagen en Y.</param>
            public void LoadImage(String iname, String pattern, float scale, bool flipX, bool flipY)
            {
                List<Bitmap> bitmaps = ReadImages(pattern);
                int nimages = bitmaps.Count;
                for (int i = 0; i < nimages; i++)
                {
                    Bitmap b = bitmaps[i];
                    int width = (int)Math.Round(b.Width * scale);
                    int height = (int)Math.Round(b.Height * scale);
                    Bitmap bmp = new Bitmap(width, height);

                    Graphics g = Graphics.FromImage(bmp);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(b, 0, 0, width, height);
                    g.Dispose();
                    FlipImage(bmp, flipX, flipY);
                    bitmaps[i] = bmp;
                }
                this.images.Add(iname, bitmaps.ToArray());
            }

            private List<Bitmap> ReadImages(String pattern)
            {
                pattern = FixDirectorySeparatorChar(pattern);

                String dir = Path.GetDirectoryName(pattern);
                String patt = Path.GetFileName(pattern);
                List<Bitmap> bitmaps = new List<Bitmap>();

                if (dir == null)
                {
                    Bitmap bmp = new Bitmap(Image.FromFile(patt));
                    bitmaps.Add(bmp);
                }
                else
                {
                    String[] fnames = new List<String>(Directory.EnumerateFiles(dir, patt)).ToArray();
                    Array.Sort(fnames);
                    foreach (String fname in fnames)
                    {
                        Bitmap bmp = new Bitmap(Image.FromFile(fname));
                        bitmaps.Add(bmp);
                    }
                }
                return bitmaps;
            }

            protected String FixDirectorySeparatorChar(String path)
            {
                return path.Replace('/', Path.DirectorySeparatorChar);
            }

            // ------ FORM ------

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                lock (screen)
                {
                    Graphics g = e.Graphics;
                    g.Clear(Color.LightGray);
                    g.DrawImage(screen, new Point(0, 0));
                    //g.Dispose();
                }
            }

            protected override void OnFormClosing(FormClosingEventArgs e)
            {
                lock (this)
                {
                    running = false;
                }

                e.Cancel = true;
                base.OnFormClosing(e);
            }

        }
    }
}
