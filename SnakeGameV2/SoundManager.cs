using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace SnakeGameV2
{
    internal class SoundManager
    {
        private WindowsMediaPlayer eatPlayer = new WindowsMediaPlayer();
        private WindowsMediaPlayer deathPlayer = new WindowsMediaPlayer();

        private Random rand = new Random();

        private System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer();
        private int volume = 100;

        public SoundManager()
        {
            fadeTimer.Interval = 100;
        }

        // เล่นเสียงกินแบบสุ่ม 3 ไฟล์
        public void PlayEat()
        {
            string soundDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Sounds");

            string[] sounds =
            {
                Path.Combine(soundDir, "eat.mp3"),
                Path.Combine(soundDir, "eat2.mp3"),
                Path.Combine(soundDir, "eat3.mp3")
            };

            eatPlayer.settings.volume = 100; // กันเสียงค้าง
            eatPlayer.URL = sounds[rand.Next(sounds.Length)];
            eatPlayer.controls.play();
        }

        // เล่นเสียงตายพร้อม Fade-out
        public void PlayDie()
        {
            string soundDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Sounds");
            string file = Path.Combine(soundDir, "death.mp3");

            volume = 100;                          // ⭐ สำคัญ: reset volume
            deathPlayer.settings.volume = volume;
            deathPlayer.URL = file;
            deathPlayer.controls.play();

            fadeTimer.Tick -= FadeOutTick;         // ⭐ กัน event ซ้ำ
            fadeTimer.Tick += FadeOutTick;
            fadeTimer.Start();
        }

        // Fade-out effect
        private void FadeOutTick(object? sender, EventArgs e)
        {
            volume -= 5;

            if (volume <= 0)
            {
                fadeTimer.Stop();
                deathPlayer.controls.stop();
                return;
            }

            deathPlayer.settings.volume = volume;
        }
    }
}
