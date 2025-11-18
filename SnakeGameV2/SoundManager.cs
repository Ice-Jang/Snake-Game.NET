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
        private WindowsMediaPlayer eatPlayer = new WindowsMediaPlayer();     // ตัวเล่นเสียงสำหรับ "กิน"
        private WindowsMediaPlayer deathPlayer = new WindowsMediaPlayer();   // ตัวเล่นเสียงสำหรับ "ตาย"
        private Random rand = new Random();                                   // ใช้สุ่มไฟล์เสียงกิน
        private System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer(); // timer สำหรับ fade-out
        private int volume = 100;                                             // ค่า volume ปัจจุบันของเสียงตาย

        private readonly string soundDir;                                     // path โฟลเดอร์เสียงแบบ auto-detect

        public SoundManager()
        {
            fadeTimer.Interval = 100;                                         // fade-out ขยับทุก 100ms (0.1 วินาที)

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;           // baseDir = bin\Debug\net6.0-windows\
            string projectDir = Directory.GetParent(                          // ขึ้นไป 2 ชั้น
                                    Directory.GetParent(baseDir).FullName
                                 ).FullName;                                  // projectDir = โฟลเดอร์โปรเจกต์จริง

            soundDir = Path.Combine(projectDir, "Assets", "Sounds");          // ต่อ path → Project/Assets/Sounds

            if (!Directory.Exists(soundDir))                                  // ถ้าโฟลเดอร์นี้ไม่เจอ (กรณี publish)
                soundDir = Path.Combine(baseDir, "Assets", "Sounds");         // fallback → bin/.../Assets/Sounds
        }

        public void PlayEat()
        {
            string[] sounds =                                                  // เตรียมไฟล์เสียงทั้งหมด
            {
            Path.Combine(soundDir, "eat1.mp3"),                           // eat1
            Path.Combine(soundDir, "eat2.mp3"),                           // eat2
            Path.Combine(soundDir, "eat3.mp3"),                           // eat3
            Path.Combine(soundDir, "eat4.mp3"),                           // eat4
            Path.Combine(soundDir, "eat5.mp3")                            // eat5
        };

            eatPlayer.settings.volume = 100;                                  // reset volume (กันเสียงเบา)
            eatPlayer.URL = sounds[rand.Next(sounds.Length)];                 // เลือกเสียงแบบสุ่ม 1 อัน
            eatPlayer.controls.play();                                        // เล่นทันที
        }

        public void PlayDie()
        {
            string file = Path.Combine(soundDir, "death.mp3");                // ระบุไฟล์ death.mp3

            volume = 200;                                                     // start volume สูงเพื่อ fade ลงสวย ๆ
            deathPlayer.settings.volume = volume;                             // ตั้งค่า volume ให้ player
            deathPlayer.URL = file;                                           // ใส่ไฟล์เสียง
            deathPlayer.controls.play();                                      // เล่นเสียงตายทันที

            fadeTimer.Tick -= FadeOutTick;                                    // กัน event handler ซ้ำซ้อน
            fadeTimer.Tick += FadeOutTick;                                    // ใส่ event ใหม่สำหรับ fade-out
            fadeTimer.Start();                                                // เริ่มการ fade-out
        }

        private void FadeOutTick(object? sender, EventArgs e)
        {
            volume -= 5;                                                      // ลดเสียงลงครั้งละ 5 ทุก 100ms

            if (volume <= 0)                                                  // ถ้าลดจนหมด
            {
                fadeTimer.Stop();                                             // หยุด timer
                deathPlayer.controls.stop();                                  // หยุดเล่นเสียงจริง ๆ
                return;                                                       // ออกจากฟังก์ชัน
            }

            deathPlayer.settings.volume = volume;                             // อัปเดต volume ใหม่ (เสียงค่อยลดลงเรื่อย ๆ)
        }
    }
}
