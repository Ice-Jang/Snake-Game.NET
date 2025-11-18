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
        private WindowsMediaPlayer eatPlayer = new WindowsMediaPlayer();     // ตัวเล่นเสียงสำหรับเสียง "กิน" แยกตัว เพื่อป้องกันชนกับเสียงอื่น
        private WindowsMediaPlayer deathPlayer = new WindowsMediaPlayer();   // ตัวเล่นเสียงสำหรับเสียง "ตาย" แยกอีกตัว เพื่อใช้ fade-out ได้

        private Random rand = new Random();                                   // ใช้สุ่มเลือกไฟล์เสียงกิน

        private System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer(); // timer สำหรับทำ animation fade-out
        private int volume = 100;                                             // เก็บค่า volume ปัจจุบันของ deathPlayer (ใช้ลดลงเรื่อย ๆ)

        public SoundManager()
        {
            fadeTimer.Interval = 100;     // กำหนดให้ fade-out ทำงานทุก 100 ms (0.1 วินาที)
        }

        // ฟังก์ชันเล่นเสียงกินแบบสุ่มจากหลายไฟล์
        public void PlayEat()
        {
            string soundDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Sounds"); // path โฟลเดอร์เสียงใน /Assets/Sounds

            string[] sounds =
            {
            Path.Combine(soundDir, "eat1.mp3"), // รวมไฟล์เสียงเข้า array
            Path.Combine(soundDir, "eat2.mp3"),
            Path.Combine(soundDir, "eat3.mp3"),
            Path.Combine(soundDir, "eat4.mp3"),
            Path.Combine(soundDir, "eat5.mp3")
        };

            eatPlayer.settings.volume = 100;                  // รีเซ็ต volume เพื่อป้องกันเสียงเบาเพราะเล่นซ้ำหลายครั้ง
            eatPlayer.URL = sounds[rand.Next(sounds.Length)]; // สุ่มเลือกไฟล์เสียง
            eatPlayer.controls.play();                        // สั่งเล่นทันที
        }

        // เล่นเสียงตาย และตามด้วยเอฟเฟกต์ fade-out
        public void PlayDie()
        {
            string soundDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Sounds"); // path ไปยังโฟลเดอร์เสียง
            string file = Path.Combine(soundDir, "death.mp3");                                         // ระบุไฟล์เสียงตาย

            volume = 200;                          // ⭐ รีเซ็ต volume ให้เริ่มสูงก่อนค่อยลด (200 ทำให้เสียงเริ่มชัด/ดังกว่า default)
            deathPlayer.settings.volume = volume;   // ตั้งค่า volume ให้ตัว player
            deathPlayer.URL = file;                 // ตั้งไฟล์เสียง
            deathPlayer.controls.play();            // เริ่มเล่นเสียงตายทันที

            fadeTimer.Tick -= FadeOutTick;          // ⭐ remove delegate เก่าเพื่อกันซ้อน (กัน fade ทำงานหลายรอบทับกัน)
            fadeTimer.Tick += FadeOutTick;          // สมัคร event handler สำหรับ fade-out
            fadeTimer.Start();                      // เริ่มเฟด
        }

        // ควบคุมการ fade-out ของเสียงตาย
        private void FadeOutTick(object? sender, EventArgs e)
        {
            volume -= 5;                            // ลดระดับเสียงลงทีละ 5 ทุก 100 ms

            if (volume <= 0)                        // ถ้า volume ลดจนถึง 0 → หยุดเฟด
            {
                fadeTimer.Stop();                   // หยุด timer ทันที (ไม่ให้ทำงานต่อ)
                deathPlayer.controls.stop();        // หยุดเล่นเสียงจริง ๆ
                return;                             // ออกจากฟังก์ชัน
            }

            deathPlayer.settings.volume = volume;   // ตั้ง volume ใหม่หลังจากลด
        }
    }
}
