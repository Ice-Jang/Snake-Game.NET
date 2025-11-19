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
        private WindowsMediaPlayer eatPlayer = new WindowsMediaPlayer();     // ตัวเล่นเสียง "กิน"
        private WindowsMediaPlayer deathPlayer = new WindowsMediaPlayer();   // ตัวเล่นเสียง "ตาย"
        private Random rand = new Random();                                   // สุ่มไฟล์เสียงกิน
        private System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer(); // timer สำหรับ fade-out
        private int volume = 100;                                             // volume ปัจจุบันของเสียงตาย

        private readonly string soundDir;                                     // path โฟลเดอร์เสียงแบบ auto-detect
        private readonly string debugLog = "sound_debug_log.txt";             // ไฟล์ log สำหรับ debug

        public SoundManager()
        {
            fadeTimer.Interval = 100;                                         // ให้ fade-out ทำงานทุก 0.1 วินาที

            Debug("=== NEW SoundManager Init ===");                          // เขียน log ว่ามีการสร้าง object แล้ว

            string start = Directory.GetCurrentDirectory();                   // โฟลเดอร์ที่รันโปรแกรมจริง (bin/Debug/…)
            Debug("Start Directory = " + start);                              // เขียน log

            string? found = null;                                             // เตรียมตัวแปรสำหรับเก็บ path ที่เจอจริง
            string dir = start;                                               // เริ่มไล่จากโฟลเดอร์ปัจจุบัน

            for (int i = 0; i < 10; i++)                                      // ไล่ขึ้นสูงสุด 10 ชั้น
            {
                string test = Path.Combine(dir, "Assets", "Sounds");          // สร้าง path ทดลองดูว่ามี Assets/Sounds ไหม

                if (Directory.Exists(test))                                   // ถ้าเจอจริง
                {
                    found = test;                                             // เก็บ path
                    break;                                                    // หยุด loop เพราะหาเจอแล้ว
                }

                dir = Directory.GetParent(dir)?.FullName ?? "";               // ขึ้นไปโฟลเดอร์บนสุด
            }

            if (found != null)                                                // ถ้าเจอ path จริง
            {
                soundDir = found;                                             // ใช้ path นั้นเป็นโฟลเดอร์เสียง
                Debug("Found soundDir = " + soundDir);                        // เขียน log
            }
            else
            {
                soundDir = Path.Combine(start, "Assets", "Sounds");           // fallback (กรณีไม่เจอจริง)
                Debug("WARNING: Sound folder not found. Using fallback = " + soundDir);
            }
        }

        // 🔥 เพิ่มฟังก์ชัน WarmUp — แก้กระตุกตอนกินครั้งแรก
        public void WarmUp()
        {
            try
            {
                string warmFile = Path.Combine(soundDir, "eat1.mp3");         // ใช้ไฟล์ eat1 สำหรับ warm-up
                Debug("WarmUp using → " + warmFile);                          // เขียน log

                if (!File.Exists(warmFile))                                   // ถ้าไฟล์หาย → ไม่ warm-up
                {
                    Debug("WarmUp ERROR: file not found");
                    return;
                }

                eatPlayer.settings.volume = 0;                                // ปิดเสียงให้เงียบ
                eatPlayer.URL = warmFile;                                     // โหลดไฟล์เข้าสู่ memory
                eatPlayer.controls.play();                                    // เล่นครั้งแรกแบบเงียบ → ทำให้ WMP warm-up

                var t = new System.Windows.Forms.Timer();                     // timer สำหรับหยุดเสียง warm-up
                t.Interval = 60;                                              // ให้เล่นเบา ๆ แค่ 60ms
                t.Tick += (s, e) =>
                {
                    t.Stop();                                                 // หยุด timer
                    eatPlayer.controls.stop();                                // หยุดเล่นไฟล์ warm-up
                    eatPlayer.settings.volume = 100;                          // คืน volume ปกติ
                    Debug("WarmUp complete");                                 // บันทึก log ว่า warm-up เสร็จแล้ว
                };
                t.Start();                                                    // เริ่ม warm-up
            }
            catch (Exception ex)
            {
                Debug("WarmUp EXCEPTION: " + ex.Message);                     // ถ้ามี exception → log ไว้
            }
        }

        private void Debug(string msg)
        {
            File.AppendAllText(debugLog, $"[{DateTime.Now:HH:mm:ss}] {msg}\n"); // เขียน log ใส่ไฟล์
        }

        public void PlayEat()
        {
            string[] sounds =
            {
            Path.Combine(soundDir, "eat1.mp3"),
            Path.Combine(soundDir, "eat2.mp3"),
            Path.Combine(soundDir, "eat3.mp3"),
            Path.Combine(soundDir, "eat4.mp3"),
            Path.Combine(soundDir, "eat5.mp3"),
        };

            string chosen = sounds[rand.Next(sounds.Length)];                 // สุ่มเลือกไฟล์กิน
            Debug("PlayEat → " + chosen);                                    // log

            if (!File.Exists(chosen))                                         // ถ้าไฟล์หาย → ไม่เล่น
            {
                Debug("ERROR: Eat sound NOT FOUND!");
                return;
            }

            eatPlayer.settings.volume = 100;                                  // volume ปกติ
            eatPlayer.URL = chosen;                                           // โหลดไฟล์
            eatPlayer.controls.play();                                        // เล่นทันที
        }

        public void PlayDie()
        {
            string file = Path.Combine(soundDir, "death.mp3");                // ไฟล์เสียงตาย
            Debug("PlayDie → " + file);                                       // log

            if (!File.Exists(file))                                           // ถ้าไฟล์หาย → ไม่เล่น
            {
                Debug("ERROR: death.mp3 NOT FOUND!");
                return;
            }

            volume = 200;                                                     // เริ่มต้น volume สูงเพื่อ fade ลง
            deathPlayer.settings.volume = volume;                             // ตั้งค่า volume
            deathPlayer.URL = file;                                           // ใส่ไฟล์เสียง
            deathPlayer.controls.play();                                      // เล่นเสียงตาย

            fadeTimer.Tick -= FadeOutTick;                                    // กัน event handler ซ้ำ
            fadeTimer.Tick += FadeOutTick;                                    // ผูก event handler
            fadeTimer.Start();                                                // เริ่ม fade-out
        }

        private void FadeOutTick(object? sender, EventArgs e)
        {
            volume -= 5;                                                      // ลด volume ทีละ 5

            if (volume <= 0)                                                  // ถ้า volume หมด
            {
                fadeTimer.Stop();                                             // หยุด timer
                deathPlayer.controls.stop();                                  // หยุดเสียงจริง ๆ
                return;
            }

            deathPlayer.settings.volume = volume;                             // อัปเดต volume ใหม่
        }
    }
}
