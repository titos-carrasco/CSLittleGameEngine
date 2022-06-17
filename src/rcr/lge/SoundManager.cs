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
            private readonly Dictionary<String, Byte[]> sounds;
            private readonly List<Object> players;

            /// <summary>
            /// Construye un objeto manejador de sonidos en memoria
            /// </summary>
            public SoundManager()
            {
                isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;

                sounds = new Dictionary<String, Byte[]>();
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
                sounds.Add(name, wav);
            }

            /// <summary>
            /// Inicia la reproduccion de un sonido
            /// </summary>
            /// <param name="name">Nombre del sonido (previamente cargado) a reproducir</param>
            /// <param name="loop">Verdadero para reproducirlo en loop</param>
            /// <returns>El ID del player del sonido a reproducir</returns>
            public Object PlaySound(String name, bool loop)
            {
                Byte[] wav = sounds[name];
                MemoryStream ms = new MemoryStream(wav);

                if (isWindows)
                {
                    NAudio.Wave.IWaveProvider provider = new NAudio.Wave.RawSourceWaveStream(ms, new NAudio.Wave.WaveFormat());
                    var waveOut = new NAudio.Wave.WaveOut();
                    waveOut.Init(provider);
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
