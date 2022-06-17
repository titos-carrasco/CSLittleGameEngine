using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace rcr
{
    namespace lge
    {
        /// <summary>
        /// Manejador de sonidos en memoria
        /// </summary>
        public class SoundManager
        {
            private readonly Dictionary<String, Byte[]> sounds;
            private readonly List<WaveOut> players;

            /// <summary>
            /// Construye un objeto manejador de sonidos en memoria
            /// </summary>
            public SoundManager()
            {
                sounds = new Dictionary<String, Byte[]>();
                players = new List<WaveOut>();
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
                IWaveProvider provider = new RawSourceWaveStream(ms, new WaveFormat());
                var waveOut = new WaveOut();
                waveOut.Init(provider);
                waveOut.Play();
                if(loop)
                    waveOut.PlaybackStopped += (object sender, StoppedEventArgs e) => { ms.Position = 0; waveOut.Play(); };
                lock (players)
                {
                    players.Add(waveOut);
                }
                return waveOut;
            }

            /// <summary>
            /// Detiene la reproduccion del sonido especificado
            /// </summary>
            /// <param name="player">El ID del player del sonido a detener</param>
            public void StopSound(Object player)
            {
                WaveOut waveOut = (WaveOut)player;
                lock (players)
                {
                    waveOut.Stop();
                    players.Remove(waveOut);
                }
            }

            /// <summary>
            /// Detiene la reproduccion de todos los sonidos
            /// </summary>
            public void StopAll()
            {
                WaveOut[] _players;
                lock (players)
                {
                    _players = players.ToArray();
                }

                foreach (WaveOut p in _players)
                {
                    p.Stop();
                    players.Remove(p);
                }
            }
        }
    }
}
