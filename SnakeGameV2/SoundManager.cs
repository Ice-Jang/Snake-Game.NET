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
        private WindowsMediaPlayer eatPlayer = new WindowsMediaPlayer();     // สร้างตัวเล่นเสียงสำหรับเสียง "กิน"
        private WindowsMediaPlayer deathPlayer = new WindowsMediaPlayer();   // สร้างตัวเล่นเสียงสำหรับเสียง "ตาย"
        private Random rand = new Random();                                   // ตัวสุ่ม เพื่อสุ่มเลือกไฟล์กินแบบ random
        private System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer(); // Timer สำหรับทำ fade-out
        private int volume = 100;                                             // เก็บระดับเสียงปัจจุบันของ deathPlayer (ใช้ลดลงทีละ step)

        private readonly string soundDir;                                     // path ไปยังโฟลเดอร์ Assets/Sounds ที่หาได้อัตโนมัติ
        private readonly string debugLog = "sound_debug_log.txt";             // ไฟล์ log สำหรับบันทึกการทำงานของ SoundManager

        public SoundManager()
        {
            fadeTimer.Interval = 100;                                         // ให้ fade-out ทำงานทุก 0.1 วินาที

            Debug("=== NEW SoundManager Init ===");                           // log ว่าเริ่มสร้าง SoundManager แล้ว

            string start = Directory.GetCurrentDirectory();                   // ดึงโฟลเดอร์ที่รันโปรแกรมจริง (bin/Debug/... )
            Debug("Start Directory = " + start);                              // log path เริ่มต้น

            string? found = null;                                             // ตัวแปรเก็บ path ที่เจอ Assets/Sounds จริง
            string dir = start;                                               // เริ่มเดินจากโฟลเดอร์ bin

            for (int i = 0; i < 10; i++)                                      // loop สูงสุด 10 ชั้น เพื่อป้องกันวนไม่รู้จบ
            {
                string test = Path.Combine(dir, "Assets", "Sounds");          // สร้าง path ที่ต้องการตรวจสอบ

                if (Directory.Exists(test))                                   // ถ้าเจอโฟลเดอร์ Assets/Sounds จริง
                {
                    found = test;                                             // เก็บ path นั้น
                    break;                                                    // หยุด loop
                }

                dir = Directory.GetParent(dir)?.FullName ?? "";               // ถ้าไม่เจอ → ขึ้นไปโฟลเดอร์พ่อหนึ่งชั้น
            }

            if (found != null)                                                // ถ้าเจอโฟลเดอร์จริง
            {
                soundDir = found;                                             // ใช้ path นั้นเป็นโฟลเดอร์เสียง
                Debug("Found soundDir = " + soundDir);                        // log ว่าเจอแล้ว
            }
            else                                                              // ถ้าไม่เจอเลย
            {
                soundDir = Path.Combine(start, "Assets", "Sounds");           // fallback → ใช้ path ในโฟลเดอร์ปัจจุบันแทน
                Debug("WARNING: Sound folder not found. Using fallback = " + soundDir);  // log แจ้งเตือน
            }
        }

        private void Debug(string msg)
        {
            File.AppendAllText(debugLog, $"[{DateTime.Now:HH:mm:ss}] {msg}\n"); // เขียนข้อความ log พร้อมเวลาลงไฟล์ text
        }

        public void PlayEat()
        {
            string[] sounds =                                                  // Array ของไฟล์เสียงกินทั้งหมด
            {
            Path.Combine(soundDir, "eat1.mp3"),                           // เสียงกินแบบที่ 1
            Path.Combine(soundDir, "eat2.mp3"),                           // แบบที่ 2
            Path.Combine(soundDir, "eat3.mp3"),                           // แบบที่ 3
            Path.Combine(soundDir, "eat4.mp3"),                           // แบบที่ 4
            Path.Combine(soundDir, "eat5.mp3"),                           // แบบที่ 5
        };

            string chosen = sounds[rand.Next(sounds.Length)];                 // สุ่มเลือกไฟล์เสียง 1 อันจาก array
            Debug("PlayEat → " + chosen);                                     // log ว่าเลือกไฟล์ไหน

            if (!File.Exists(chosen))                                         // ถ้าไฟล์ไม่มีจริง
            {
                Debug("ERROR: Eat sound NOT FOUND!");                         // เขียน log error
                return;                                                        // ไม่เล่นเสียง
            }

            eatPlayer.settings.volume = 100;                                   // ตั้งระดับเสียงให้เต็ม (ป้องกันเสียงเบา)
            eatPlayer.URL = chosen;                                           // ระบุ path ให้ player
            eatPlayer.controls.play();                                        // เล่นไฟล์
        }

        public void PlayDie()
        {
            string file = Path.Combine(soundDir, "death.mp3");                // path ของเสียงตาย
            Debug("PlayDie → " + file);                                       // log path ที่เลือก

            if (!File.Exists(file))                                           // ถ้าไฟล์ไม่เจอ
            {
                Debug("ERROR: death.mp3 NOT FOUND!");                         // log error
                return;                                                        // ไม่เล่นเสียง
            }

            volume = 200;                                                     // ตั้ง volume เริ่มต้นสูงเพื่อ fade-out สวย ๆ
            deathPlayer.settings.volume = volume;                             // ใส่ค่า volume เข้า player
            deathPlayer.URL = file;                                           // เซ็ตไฟล์เสียง
            deathPlayer.controls.play();                                      // เริ่มเล่นทันที

            fadeTimer.Tick -= FadeOutTick;                                    // ป้องกัน event handler ซ้ำซ้อน
            fadeTimer.Tick += FadeOutTick;                                    // ใส่ event handler ใหม่
            fadeTimer.Start();                                                // เริ่มทำ fade-out
        }

        private void FadeOutTick(object? sender, EventArgs e)
        {
            volume -= 5;                                                      // ลด volume ทีละ 5

            if (volume <= 0)                                                  // ถ้าเสียงลดจนถึง 0
            {
                fadeTimer.Stop();                                             // หยุด timer
                deathPlayer.controls.stop();                                  // หยุดเล่นเพลงจริง ๆ
                return;                                                        // ออกจากฟังก์ชัน
            }

            deathPlayer.settings.volume = volume;                             // อัปเดตระดับเสียงใหม่
        }
    }
}
