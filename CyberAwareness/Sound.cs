using System;
using System.Media;
using System.Runtime.Versioning;

namespace CyberAwareness
{
    internal class Sound
    {
        [SupportedOSPlatform("windows")]
        public void Sound_wav(string full_path)
        {
            try
            {
                SoundPlayer player = new SoundPlayer(full_path);
                player.Play();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }
    }
}