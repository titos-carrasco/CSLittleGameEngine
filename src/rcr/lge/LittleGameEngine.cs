using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
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
            private const int GUI_LAYER = 0xFFFF;

            private static LittleGameEngine lge = null;

            private readonly SortedDictionary<int, List<GameObject>> gLayers;
            private readonly Dictionary<String, GameObject> gObjects;
            private readonly List<GameObject> gObjectsToAdd;
            private readonly List<GameObject> gObjectsToDel;
            private readonly Camera camera;

            private readonly Size winSize;
            private readonly float[] fpsData;
            private int fpsIdx;
            private readonly float[] lpsData;
            private int lpsIdx;
            private volatile bool running = false;

            /// <value>Metodo que procesara el evento OnMainUpdate</value>
            public Action<float> onMainUpdate = null;
            private readonly Dictionary<Keys, bool> keysPressed;
            private readonly bool[] mouseButtons = { false, false, false };
            private readonly Nullable<Point>[] mouseClicks = { null, null, null };

            public readonly ImageManager imageManager;
            public readonly FontManager fontManager;
            public readonly SoundManager soundManager;

            private readonly Color bgColor;
            private Nullable<Color> collidersColor = null;

            private readonly Bitmap screen;
            private readonly Stopwatch screenSpeed;

            [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
            private static extern uint TimeBeginPeriod(uint uMilliseconds);
            [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
            private static extern uint TimeEndPeriod(uint uMilliseconds);


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

                fpsData = new float[10];
                fpsIdx = 0;
                lpsData = new float[10];
                lpsIdx = 0;

                this.bgColor = bgColor;

                imageManager = new ImageManager();
                fontManager = new FontManager();
                soundManager = new SoundManager();

                keysPressed = new Dictionary<Keys, bool>();

                gObjects = new Dictionary<String, GameObject>();
                gLayers = new SortedDictionary<int, List<GameObject>>();
                gObjectsToAdd = new List<GameObject>();
                gObjectsToDel = new List<GameObject>();

                camera = new Camera(new PointF(0, 0), winSize);

                screenSpeed = new Stopwatch();
                screen = ImageManager.CreateOpaqueImage(winSize.Width, winSize.Height);
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
                this.ClientSize = winSize;
                this.DoubleBuffered = true;

                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.Opaque, true);

                this.Show();

                Application.DoEvents();
            }

            /// <summary>
            /// Obtiene una instancia del juego en ejecucion. Util para las diferentes clases utilizadas en un juego tal de acceder a metodos estaticos
            /// </summary>
            /// <returns>La instancia de LGE en ejecucion</returns>
            /// <exception cref="System.ApplicationException">LGE no se encuentra activo</exception>
            public static LittleGameEngine GetInstance()
            {
                if (lge == null)
                    throw new ApplicationException("LGE no se encuentra activo");
                return lge;
            }

            /// <summary>
            /// Obtiene los FPS calculados como el promedio de los ultimos valores
            /// </summary>
            /// <returns>Los frame por segundo calculados</returns>
            public float GetFPS()
            {
                float dt = 0;
                lock (fpsData)
                {
                    foreach (float val in fpsData)
                        dt += val;
                    dt /= fpsData.Length;
                }
                return dt == 0 ? 0 : 1.0f / dt;
            }

            /// <summary>
            /// Obtiene los LPS (Loops per Seconds) del GameLoop calculados como el promedio de los ultimos valores
            /// </summary>
            /// <returns>Los ciclos por segundo del GameLoop</returns>
            public float GetLPS()
            {
                float dt = 0;
                lock (lpsData)
                {
                    foreach (float val in lpsData)
                        dt += val;
                    dt /= lpsData.Length;
                }
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
                running = false;
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
                bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;

                screenSpeed.Start();
                Bitmap screenImage = ImageManager.CreateOpaqueImage(winSize.Width, winSize.Height);

                long tExpected = 1000 / fps;

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                AutoResetEvent autoResetEvent = new AutoResetEvent(false);

                running = true;
                while (running)
                {
                    // los eventos son atrapados por los listener

                    // --- nos ajustamos a 1/fps
                    long t = tExpected - stopwatch.ElapsedMilliseconds;
                    if (t > 0)
                    {
                        if (isWindows)
                        {
                            TimeBeginPeriod(1);
                            autoResetEvent.WaitOne((int)t);
                            TimeEndPeriod(1);
                        }
                        else
                            autoResetEvent.WaitOne((int)t);
                    }

                    // --- tiempo desde el ciclo anterior
                    float dt = stopwatch.ElapsedMilliseconds / 1000.0f;
                    stopwatch.Restart();

                    // los LPS
                    lock (lpsData)
                    {
                        lpsData[lpsIdx++] = dt;
                        lpsIdx %= lpsData.Length;
                    }

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
                                Bitmap surface = gobj.surface;
                                if (surface != null)
                                    g.DrawImageUnscaled(surface, (int)p.X, (int)p.Y);

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
                                    g.DrawImageUnscaled(surface, (int)x, (int)y);
                                }
                            }
                        }
                    }
                    g.Dispose();

                    lock (screen)
                    {
                        g = Graphics.FromImage(screen);
                        g.DrawImageUnscaled(screenImage, 0, 0);
                        g.Dispose();
                        this.Invalidate();
                    }

                    this.Invoke(
                        new Action(
                            () => this.Refresh()
                        )
                    );
                }

                // --- gobj.OnQuit
                foreach (KeyValuePair<int, List<GameObject>> elem in gLayers)
                    foreach (GameObject gobj in elem.Value)
                        gobj.OnQuit();

                // cerramos la ventana en caso de que siga abierta
                this.Invoke(
                    new Action(
                        () =>
                        {
                            this.Close();
                            this.Dispose();
                        }
                    )
                );

                // finalizamos la aplicacion
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
                    return keysPressed.ContainsKey(key) && keysPressed[key];
                }
            }

            /// <summary>
            /// Reacciona cuando una tecla es presionada
            /// </summary>
            /// <param name="e">Contiene la data del evento</param>
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

            /// <summary>
            /// Reacciona cuando una tecla es soltada
            /// </summary>
            /// <param name="e">Contiene la data del evento</param>
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
                Point p = new Point(-1, -1);

                this.Invoke(new Action(() => p = PointToClient(Cursor.Position)));
                if (p.X < 0 || p.X >= this.winSize.Width || p.Y < 0 || p.Y >= this.winSize.Height)
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

            /// <summary>
            /// Reacciona cuando un boton del mouse realiza un clic
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">La data del evento</param>
            public void MouseClicked(Object sender, MouseEventArgs e)
            {
                lock (mouseClicks)
                {
                    int idx = e.Button.Equals(MouseButtons.Left) ? 0 : e.Button.Equals(MouseButtons.Middle) ? 1 : 2;
                    mouseClicks[idx] = new Point(e.X, e.Y);
                }
            }

            /// <summary>
            /// Reacciona cuando un boton del mouse es presionado
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">La data del evento</param>
            public void MousePressed(Object sender, MouseEventArgs e)
            {
                lock (mouseButtons)
                {
                    int idx = e.Button.Equals(MouseButtons.Left) ? 0 : e.Button.Equals(MouseButtons.Middle) ? 1 : 2;
                    mouseButtons[idx] = true;
                }
            }

            /// <summary>
            /// Reacciona cuando un boton del mouse es soltado
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">La data del evento</param>
            public void MouseReleased(Object sender, MouseEventArgs e)
            {
                lock (mouseButtons)
                {
                    int idx = e.Button.Equals(MouseButtons.Left) ? 0 : e.Button.Equals(MouseButtons.Middle) ? 1 : 2;
                    mouseButtons[idx] = false;
                }
            }

            // ------ form ------

            /// <summary>
            /// Reacciona al evento OnPaint
            ///
            /// <para>Todo el despliegue ocurre aqui junto con calcular los FPS</para>
            /// </summary>
            /// <param name="e">La data del evento</param>
            protected override void OnPaint(PaintEventArgs e)
            {
                // los FPS
                float dt = screenSpeed.ElapsedMilliseconds;
                screenSpeed.Restart();

                lock (fpsData)
                {
                    fpsData[fpsIdx++] = dt / 1000.0f;
                    fpsIdx %= fpsData.Length;
                }

                // vaciamos la imagen del juego
                lock (screen)
                {
                    Graphics g = e.Graphics;
                    //g.CompositingMode = CompositingMode.SourceCopy;
                    g.DrawImageUnscaled(screen, 0, 0);
                }
            }

            /// <summary>
            /// Reacciona al evento OnPaintBackground
            /// </summary>
            /// <param name="e">La data del evento.</param>
            protected override void OnPaintBackground(PaintEventArgs e)
            {
                //Console.WriteLine("OnPaintBackground");
            }

            /// <summary>
            /// Reacciona al evento OnFormCLosing
            /// </summary>
            /// <param name="e">A <see cref="T:System.Windows.Forms.FormClosingEventArgs" /> that contains the event data.</param>
            protected override void OnFormClosing(FormClosingEventArgs e)
            {
                running = false;

                e.Cancel = true;
                base.OnFormClosing(e);
            }

            // ------ utils ------

            /// <summary>
            /// Reemplaza, en una ruta de archivo, el caracter '/' por el adecuado al S.O.
            /// </summary>
            /// <param name="path">La ruta a corregir</param>
            /// <returns></returns>
            static protected internal String FixDirectorySeparatorChar(String path)
            {
                return path.Replace('/', Path.DirectorySeparatorChar);
            }


            /// <summary>
            /// Expande un patron de nombres de archivos tipo glob
            /// </summary>
            /// <param name="pattern">El patron a expandir</param>
            /// <returns>Los nombres de archivos cubiertos por el patron</returns>
            static protected internal String[] ExpandFilenames(String pattern)
            {
                pattern = FixDirectorySeparatorChar(pattern);

                String dir = Path.GetDirectoryName(pattern);
                String patt = Path.GetFileName(pattern);

                String[] fnames = new List<String>(Directory.EnumerateFiles(dir, patt)).ToArray();
                Array.Sort(fnames);
                return fnames;
            }
        }
    }
}
