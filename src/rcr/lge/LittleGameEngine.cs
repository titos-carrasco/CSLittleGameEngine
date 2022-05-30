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
        /**
        * La Pequena Maquina de Juegos
        *
        * @author Roberto carrasco (titos.carrasco@gmail.com)
        */
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

            private IEvents onMainUpdate = null;
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
            /**
            * Crea el juego
            *
            * @param winSize dimensiones de la ventana de despliegue
            * @param title   titulo de la ventana
            * @param bgColor color de fondo de la ventana
            */
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

            /**
            * Obtiene una instancia del juego en ejecucion. Util para las diferentes clases
            * utilizadas en un juego tal de acceder a metodos estaticos
            *
            * @return la instancia de LGE en ejecucion
            */
            public static LittleGameEngine GetInstance()
            {
                if (lge == null)
                    throw new ApplicationException("LGE no se encuentra activo");
                return lge;
            }

            /**
            * Obtiene los FPS calculados como el promedio de los ultimos 30 valores
            *
            * @return los frame por segundo calculados
            */
            public float GetFPS()
            {
                float dt = 0;
                foreach (float val in fpsData)
                    dt += val;
                dt /= fpsData.Length;
                return dt == 0 ? 0 : 1.0f / dt;
            }

            /**
            * Si se especifica un color se habilita el despliegue del rectangulo que bordea
            * a todos los objetos (util para ver colisiones).
            *
            * Si se especifica null se desactiva
            *
            * @param color el color para los bordes de los rectangulos
            */
            public void ShowColliders(Color color)
            {
                collidersColor = color;
            }

            /**
            * Establece la clase que recibira el evento onMainUpdate que es invocado justo
            * despues de invocar a los metodos onUpdate() de los GameObjects.
            *
            * Esta clase debe implementar IEvents
            *
            * @param iface la clase que recibira el evento
            */
            public void SetOnMainUpdate(IEvents iface)
            {
                this.onMainUpdate = iface;
            }

            /**
            * Finaliza el Game Loop de LGE
            */
            public void Quit()
            {
                lock (this)
                {
                    running = false;
                }
            }

            /**
            * Inicia el Game Loop de LGE tratando de mantener los fps especificados
            *
            * @param fps los fps a mantener
            */
            public void Run(int fps)
            {
                Thread thread = new Thread(() => TRun(fps));
                thread.Start();
                Application.Run(this);
                thread.Join();
            }

            public void TRun(int fps)
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
                        onMainUpdate.OnMainUpdate(dt);

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

            // sistema cartesiano y zona visible dada por la camara
            /**
            * Traslada las coordenadas del GameObject a la zona de despliegue de la camara
            *
            * @param p las coordenadas a trasladar
            *
            * @return las coordenadas trasladadas
            */
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
            /**
            * Agrega un GameObject al juego el que quedara habilitado en el siguiente ciclo
            *
            * @param gobj  el GameObject a agregar
            * @param layer la capa a la cual pertenece
            */
            public void AddGObject(GameObject gobj, int layer)
            {
                gobj.layer = layer;
                gObjects.Add(gobj.name, gobj);
                gObjectsToAdd.Add(gobj);
            }

            /**
            * Agrega un GameObject a la interfaz grafica del juego
            *
            * @param gobj el GameObject a agregar
            */
            public void AddGObjectGUI(GameObject gobj)
            {
                AddGObject(gobj, GUI_LAYER);
            }

            /**
            * Retorna el GameObject identificado con el nombre especificado
            *
            * @param name el nombre del GameObject a buscar
            *
            * @return el GameObject buscado (nulo si no lo encuentra)
            */
            public GameObject GetGObject(String name)
            {
                return gObjects[name];
            }

            /**
            * Obtiene todos los GameObject de una capa cuyo tag comienza con un texto dado
            *
            * @param layer la capa den donde buscar
            * @param tag   el texto inicial del tag
            *
            * @return los GameObjects encontrados
            */
            public GameObject[] FindGObjectsByTag(int layer, String tag)
            {
                List<GameObject> gobjs = new List<GameObject>();

                foreach (GameObject o in gLayers[layer])
                    if (o.name.StartsWith(tag))
                        gobjs.Add(o);

                return gobjs.ToArray(); ;
            }

            /**
            * Retorna el total de GameObjects en el juego
            *
            * @return el total de GameObjects
            */
            public int GetCountGObjects()
            {
                return gObjects.Count;
            }

            /**
            * Elimina un GameObject del juego en el siguiente ciclo
            *
            * @param gobj el GameObject a eliminar
            */
            public void DelGObject(GameObject gobj)
            {
                gObjectsToDel.Add(gobj);
            }

            /**
            * Obtiene todos los GameObject que colisionan con un GameObject dado en la
            * misma capa
            *
            * @param gobj el GameObject a inspeccionar
            *
            * @return los GameObjects con los que colisiona
            */
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
            /**
            * Retorna la posiciona de la camara
            *
            * @return la posicion
            */
            public PointF GetCameraPosition()
            {
                return camera.GetPosition();
            }

            /**
            * retorna la dimension de la camara
            *
            * @return la dimension
            */
            public SizeF GetCameraSize()
            {
                return camera.GetSize();
            }

            /**
            * Establece el GameObject al cual la camara seguira de manera automatica
            *
            * @param gobj el GameObject a seguir
            */
            public void SetCameraTarget(GameObject gobj)
            {
                if (gobj == null)
                    camera.target = gobj;
                else
                    SetCameraTarget(gobj, true);
            }

            /**
            * Establece el GameObject al cual la camara seguira de manera automatica
            *
            * @param gobj   el GameObject a seguir
            * @param center si es verdadero la camara se centrara en el centro del
            *               GameObject, en caso contrario lo hara en el extremo superior
            *               izquierdo
            */
            public void SetCameraTarget(GameObject gobj, bool center)
            {
                camera.target = gobj;
                camera.targetInCenter = center;
            }

            /**
            * establece los limites en los cuales se movera la camara
            *
            * @param bounds los limites
            */
            public void SetCameraBounds(RectangleF bounds)
            {
                camera.SetBounds(bounds);
            }

            /**
            * Establece la posicion de la camara
            *
            * @param position la posicion
            */
            public void SetCameraPosition(PointF position)
            {
                camera.SetPosition(position);
            }

            // ------ keys ------
            /**
            * Determina si una tecla se encuentra presionada o no
            *
            * @param key la tecla a inspeccionar
            * @return verdadero si la tecla se encuentra presionada
            */
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
            /**
            * Retorna el estado de los botones del mouse
            *
            * @return el estado de los botones
            */
            public bool[] GetMouseButtons()
            {
                lock (mouseButtons)
                {
                    return (bool[])mouseButtons.Clone();
                }
            }

            /**
            * Determina la posicion del mouse en la ventana
            *
            * @return la posicion del mouse
            */
            public Point GetMousePosition()
            {
                Point p = new Point(-1,-1);
                this.Invoke(
                        new Action(() => this.PointToClient(Cursor.Position))
                    );

                if (p.X < 0 || p.X >= this.winSize.Width || p.Y < 0 || p.Y >= this.winSize.Height)
                {
                    p.X = -1;
                    p.Y = -1;
                }

                return p;
            }

            /**
            * Determina si un boton del mouse se encuentra presionado
            *
            * @param button el boton a inspeccionar
            *
            * @return verdadero si se encuentra presionado
            */
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
            /**
            * Obtiene los tipos de letra del sistema
            *
            * @return los tipos de letra
            */
            static public String[] GetSysFonts()
            {
                List<String> sysfonts = new List<String>();
                InstalledFontCollection ifc = new InstalledFontCollection();
                foreach (FontFamily fa in ifc.Families)
                    sysfonts.Add(fa.Name);
                ifc.Dispose();
                return sysfonts.ToArray();
            }

            /**
            * Carga un tipo de letra para ser utilizado en el juego
            *
            * @param name   nombre interno a asignar
            * @param fname  nombre del tipo de letra
            * @param fstyle estilo del tipo de letra
            * @param fsize  tamano del tipo de letra
            */
            public void LoadSysFont(String name, String fname, FontStyle fstyle, int fsize)
            {
                FontFamily fontFamily = new FontFamily(fname);
                Font font = new Font(fontFamily, fsize, fstyle);
                fonts.Add(name, font);
            }

            /**
            * Carga un tipo de letra True Type para ser utilizado en el juego
            *
            * @param name   nombre interno a asignar
            * @param fname  nombre del archivo que contiene la fuente TTF
            * @param fstyle estilo del tipo de letra
            * @param fsize  tamano del tipo de letra
            */
            public void LoadTTFont(String name, String fname, FontStyle fstyle, int fsize)
            {
                ttfFonts.AddFontFile(fname);
                FontFamily fontFamily = new FontFamily(ttfFonts.Families[ttfFonts.Families.Length - 1].Name, ttfFonts);
                Font font = new Font(fontFamily, fsize, fstyle);
                fonts.Add(name, font);
            }

            /**
            * Recupera un tipo de letra previamente cargado
            *
            * @param fname el nombre del tipo de letra a recuperar
            *
            * @return el tipo de letra
            */
            public Font GetFont(String name)
            {
                return fonts[name];
            }

            // ------ images ------
            /**
            * Crea una imagen sin transparencia de dimensiones dadas
            *
            * @param width  ancho deseado
            * @param height alto deseado
            *
            * @return la imagen creada
            */
            static protected internal Bitmap CreateOpaqueImage(int width, int height)
            {
                return new Bitmap(width, height);
            }

            /**
            * Crea una imagen con transparencia de dimensiones dadas
            *
            * @param width  ancho deseado
            * @param height alto deseado
            *
            * @return la imagen creada
            */
            static protected internal Bitmap CreateTranslucentImage(int width, int height)
            {
                return new Bitmap(width, height);
            }

            /**
            * Recupera un grupo de imagenes previamente cargadas
            *
            * @param iname el nombre asignado al grupo de imagenes
            *
            * @return la imagenes
            */
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

            public void LoadImage(String iname, String pattern, bool flipX, bool flipY)
            {
                List<Bitmap> bitmaps = ReadImages(pattern);
                foreach (Bitmap bmp in bitmaps)
                {
                    FlipImage(bmp, flipX, flipY);
                }
                this.images.Add(iname, bitmaps.ToArray());
            }

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

            /**
            * Carga una imagen o grupo de imagenes acorde al nombre de archivo dado
            *
            * @param pattern patron de busqueda de el o los archivos. El caracter '*' es
            *                usado como comodin
            *
            * @return la o las imagenes cargadas
            */
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

            protected String FixDirectorySeparatorChar( String path)
            {
                return path.Replace('/', Path.DirectorySeparatorChar);
            }

            // ------ FORM ------
            protected override void  OnPaintBackground(PaintEventArgs e)
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
