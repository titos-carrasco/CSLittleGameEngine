using System;
using System.Collections.Generic;
using System.IO;

namespace rcr
{
    namespace lge
    {
        /// <summary>
        /// Manejador de sonidos en memoria
        /// </summary>
        public class SoundManager
        {
            private readonly bool isWindows;
            private readonly Dictionary<String, Byte[]> waves;
            private readonly List<Object> players;

            /// <summary>
            /// Construye un objeto manejador de sonidos en memoria
            /// </summary>
            public SoundManager()
            {
                isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;

                waves = new Dictionary<String, Byte[]>();
                players = new List<Object>();
            }

            /// <summary>
            /// Carga un archivo de sonido para ser utilizado durante el juego
            /// </summary>
            /// <param name="name">Nombre a asignar al sonido</param>
            /// <param name="fname">Nombre del archivo que contiene el sonido</param>
            public void LoadSound(String name, String fname)
            {
                fname = fname.Replace('\\', '/');
                byte[] wav = File.ReadAllBytes(fname);
                waves.Add(name, wav);
            }

            /// <summary>
            /// Inicia la reproduccion de un sonido
            /// </summary>
            /// <param name="name">Nombre del sonido (previamente cargado) a reproducir</param>
            /// <param name="loop">Verdadero para reproducirlo en loop</param>
            /// <param name="level">El nivel de volumen (0-100)</param>
            /// <returns>El ID del player del sonido a reproducir</returns>
            public Object PlaySound(String name, bool loop, int level=50)
            {
                if (level < 0) level = 0;
                else if (level > 100) level = 100;

                Byte[] wav = waves[name];
                MemoryStream ms = new MemoryStream(wav);

                if (isWindows)
                {
                    NAudio.Wave.IWaveProvider provider = new NAudio.Wave.RawSourceWaveStream(ms, new NAudio.Wave.WaveFormat());
                    var waveOut = new NAudio.Wave.WaveOut();
                    waveOut.Init(provider);
                    waveOut.Volume = level / 100.0f;
                    waveOut.Play();
                    if (loop)
                        waveOut.PlaybackStopped += (object sender, NAudio.Wave.StoppedEventArgs e) => { ms.Position = 0; waveOut.Play(); };
                    lock (players)
                    {
                        players.Add(waveOut);
                    }
                    return waveOut;
                }
                else
                    return null;
            }

            /// <summary>
            /// Detiene la reproduccion del sonido especificado
            /// </summary>
            /// <param name="player">El ID del player del sonido a detener</param>
            public void StopSound(Object player)
            {
                if (isWindows)
                {
                    NAudio.Wave.WaveOut waveOut = (NAudio.Wave.WaveOut)player;
                    lock (players)
                    {
                        waveOut.Stop();
                        players.Remove(waveOut);
                    }
                }
            }

            /// <summary>
            /// Establece el volumen de un sonido previamente cargado
            /// </summary>
            /// <param name="player">El ID del player del sonido</param>
            /// <param name="level">El nivel de sonido (0-100)</param>
            public void SetSoundVolume(Object player, int level)
            {
                if (level < 0) level = 0;
                else if (level > 100) level = 100;

                if (isWindows)
                {
                    NAudio.Wave.WaveOut waveOut = (NAudio.Wave.WaveOut)player;
                    waveOut.Volume = level / 100.0f;
                }
            }

            /// <summary>
            /// Obtiene el volumen de un sonido previamente cargad
            /// </summary>
            /// <param name="player">El ID del player del sonid</param>
            /// <returns>El nivel de sonido (0-100)</returns>
            public int GetSoundVolume(Object player)
            {
                if (isWindows)
                {
                    NAudio.Wave.WaveOut waveOut = (NAudio.Wave.WaveOut)player;
                    return (int) waveOut.Volume * 100;

                }
                else return 0;
            }

            /// <summary>
            /// Detiene la reproduccion de todos los sonidos
            /// </summary>
            public void StopAll()
            {
                if (isWindows)
                {
                    Object[] _players;
                    lock (players)
                    {
                        _players = players.ToArray();
                    }

                    foreach (NAudio.Wave.WaveOut p in _players)
                    {
                        p.Stop();
                        players.Remove(p);
                    }
                }
            }
        }
    }
}
