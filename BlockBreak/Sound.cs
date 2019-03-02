using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockBreak
{
    class Sound
    {
        private System.Media.SoundPlayer player = null;
        string SoundFile = "C:\\Users\\USER\\source\\repos\\BlockBreak\\BlockBreak\\Sound\\arabianjewel.WAV";

        public void PlaySound()
        {
            player = new System.Media.SoundPlayer(SoundFile);
            player.Play();
        }

        public void StopSound()
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
        }
    }
}
