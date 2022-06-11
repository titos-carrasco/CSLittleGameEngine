using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;

namespace rcr
{
    namespace lge
    {
        /// <summary>
        /// Manejador de sonidos
        /// </summary>
        public class SoundManager
        {
            /// <summary>
            /// Manejador de sonidos en memoria
            /// </summary>
            private readonly Dictionary<String, SoundPlayer> sounds;

            /// <summary>
            /// Construye un objeto manejador de sonidos en memoria
            /// </summary>
            public SoundManager()
            {
                sounds = new Dictionary<String, SoundPlayer>();
            }

            /// <summary>
            /// Carga un archivo de sonido para ser utilizado durante el juego
            /// </summary>
            /// <param name="name">Nombre a asignar al sonido</param>
            /// <param name="fname">Nombre del archivo que contiene el sonido</param>
            public void LoadSound(String name, String fname)
            {
                fname = LittleGameEngine.FixDirectorySeparatorChar(fname);
                SoundPlayer soundPlayer = new SoundPlayer(fname);
                soundPlayer.Load();
                while (!soundPlayer.IsLoadCompleted)
                    Thread.Sleep(1);
                sounds.Add(name, soundPlayer);
            }

            /// <summary>
            /// Inicia la reproduccion de un sonido
            /// </summary>
            /// <param name="name">Nombre del sonido (previamente cargado) a reproducir</param>
            /// <param name="loop">Verdadero para reproducirlo en loop</param>
            /// <param name="level">Volumen 0.0 a 1.0</param>
            public void PlaySound(String name, bool loop, int level)
            {
                if (level < 0) level = 1;
                else if (level > 100) level = 100;

                if (loop)
                    sounds[name].PlayLooping();
                else
                    sounds[name].Play();
            }

            /// <summary>
            /// Detiene la reproduccion del sonido especificado
            /// </summary>
            /// <param name="name">El nombre del sonido a detener</param>
            public void StopSound(String name)
            {
                sounds[name].Stop();
            }

            /// <summary>
            /// Establece el volumen de un sonido (no implementada)
            /// </summary>
            /// <param name="name">Nombre del sonido</param>
            /// <param name="level">Volumen 0.0 a 1.0</param>
            public void SetSoundVolume(String name, int level)
            {
                if (level < 0) level = 1;
                else if (level > 100) level = 100;
            }

            /// <summary>
            /// Obtiene el volumen de un sonido (no implementada)
            /// </summary>
            /// <param name="name">Nombre del sonido</param>
            /// <returns>Volumen del sonido 0.0 a 1.0</returns>
            public int GetSoundVolume(String name)
            {
                return 100;
            }

        }
    }
}
