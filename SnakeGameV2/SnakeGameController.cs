using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace SnakeGameV2
{
    // Controller: จัดการสถานะเกมทั้งหมด (ไม่ยุ่งกับการวาด)
    internal class SnakeGameController
    {
        public List<Point> Snake { get; private set; } = new(); // รายการตำแหน่ง segment ของงู (หัวอยู่ index 0)
        public Point Food { get; private set; }                 // จุดตำแหน่งอาหารปัจจุบัน
        public string Direction { get; private set; } = "Right";// ทิศทางที่งูกำลังเคลื่อนที่อยู่
        public int Score { get; private set; } = 0;             // คะแนนที่สะสมในรอบนี้
        public int BestScore { get; private set; } = 0;         // คะแนนสูงสุดที่เคยทำได้
        public int Money { get; set; } = 0;                     // เงินสะสมของผู้เล่น (ใช้ซื้อ skin ฯลฯ)
        public int Speed { get; set; } = 110;                   // ความเร็วเกม (delay timer)
        public bool IsDead { get; private set; } = false;       // flag บอกว่างูตายหรือยัง

        private readonly Random rand = new Random();            // ตัวสุ่ม ใช้กับการ spawn อาหาร
        private int gridCols;                                   // จำนวนคอลัมน์ของแผง grid
        private int gridRows;                                   // จำนวนแถวของ grid
        public int GridCols => gridCols;                        // expose จำนวนคอลัมน์ให้ renderer ใช้
        public int GridRows => gridRows;                        // expose จำนวนแถวให้ renderer ใช้

        // Event ที่ UI หรือ Renderer จะ subscribe เพื่ออัปเดตหน้าจอแบบเรียลไทม์
        public event Action? GameUpdated;                       // เรียกเมื่อสถานะเกมเปลี่ยน (งูขยับ, spawn อาหาร)
        public event Action<Point>? FoodEaten;                  // เรียกเมื่อกินอาหาร (ส่งตำแหน่งอาหาร)
        public event Action? OnEat;                             // เรียกเมื่อกินอาหาร (เสียง/แอนิเมชัน)
        public event Action? OnDie;                             // เรียกเมื่อเกิดเหตุการณ์งูตาย

        // สร้าง Controller พร้อมกำหนดขนาด grid
        public SnakeGameController(int cols, int rows)
        {
            gridCols = cols; // ตั้งค่าคอลัมน์ทั้งหมดของเกม
            gridRows = rows; // ตั้งค่าแถวทั้งหมดของเกม
        }

        // เริ่มเกมใหม่ (รีเซ็ตทุกค่า)
        public void StartNewGame(int initialLen = 3)
        {
            Score = 0;                    // รีเซ็ตคะแนน
            Direction = "Right";          // ทิศเริ่มต้นงูไปทางขวา
            Snake.Clear();                // ล้างตำแหน่งงูทั้งหมด

            // สร้างงูเริ่มต้นบริเวณกลาง map
            int cx = Math.Max(2, gridCols / 4); // จุดเริ่มต้น X (เลื่อนจากซ้ายมาหน่อยเพื่อให้เล่นง่าย)
            int cy = Math.Max(2, gridRows / 2); // จุดเริ่มต้น Y (กลาง map)

            // เพิ่ม segment ของงู (หัว → หาง)
            for (int i = 0; i < initialLen; i++)
                Snake.Add(new Point(cx - i, cy));  // เช่น (10,10), (9,10), (8,10)

            SpawnFood();        // สุ่มสร้างอาหารในตำแหน่งที่ไม่ชนงู
            GameUpdated?.Invoke(); // บอก UI ให้วาดเฟรมแรกของเกมใหม่
        }

        // เปลี่ยนทิศทางตาม input
        public void ChangeDirection(string newDir)
        {
            // ป้องกันการย้อนศร → ถ้าย้อนทิศตรงกัน ให้ไม่รับคำสั่ง
            if ((newDir == "Left" && Direction == "Right") ||
                (newDir == "Right" && Direction == "Left") ||
                (newDir == "Up" && Direction == "Down") ||
                (newDir == "Down" && Direction == "Up"))
                return;

            Direction = newDir;  // ตั้งทิศใหม่ (ปลอดภัย)
        }

        // ฟังก์ชันหลักของเกม เรียกโดย Timer ทุก tick
        public void Update()
        {
            if (Snake.Count == 0) return; // ถ้าเกิดงูว่างเปล่า ไม่น่าจะเกิด แต่ป้องกัน null state

            Point head = Snake[0];        // ตำแหน่งหัวปัจจุบันของงู
            Point newHead = new Point(head.X, head.Y); // copy ตำแหน่ง เพื่อใช้คำนวณตำแหน่งใหม่

            // คำนวณตำแหน่งใหม่ตามทิศทาง
            switch (Direction)
            {
                case "Up": newHead.Y -= 1; break;
                case "Down": newHead.Y += 1; break;
                case "Left": newHead.X -= 1; break;
                case "Right": newHead.X += 1; break;
            }

            // ตรวจชนขอบ map → งูตาย
            if (newHead.X < 0 || newHead.Y < 0 ||
                newHead.X >= gridCols || newHead.Y >= gridRows)
            {
                OnDie?.Invoke(); // แจ้ง renderer ให้แสดง death effect
                IsDead = true;   // ตั้ง flag ว่า dead แล้ว
                return;
            }

            // ตรวจชนตัวเอง → งูตาย
            if (Snake.Contains(newHead))
            {
                OnDie?.Invoke();
                IsDead = true;
                return;
            }

            // แทรกหัวใหม่เข้า list (ด้านหน้า)
            Snake.Insert(0, newHead);

            // ตรวจว่ากินอาหารหรือไม่
            if (newHead == Food)
            {
                OnEat?.Invoke();     // แจ้ง renderer/UI ให้เล่นแอนิเมชัน effect
                FoodEaten?.Invoke(Food); // ส่งตำแหน่งอาหารให้ renderer
                Score += 10;         // เพิ่มคะแนน
                Money += 1;          // เพิ่มเงิน 1
                if (Score > BestScore) BestScore = Score; // อัปเดต highscore

                SpawnFood();         // สร้างอาหารใหม่
            }
            else
            {
                Snake.RemoveAt(Snake.Count - 1); // ถ้าไม่ได้กิน → ลบหาง (ทำให้เหมือนเคลื่อนที่)
            }

            GameUpdated?.Invoke(); // แจ้ง Renderer ให้วาดเฟรมใหม่
        }

        // สุ่มสร้างอาหารในตำแหน่งที่ไม่ทับงู
        private void SpawnFood()
        {
            Point p;
            do
            {
                p = new Point(rand.Next(0, gridCols), rand.Next(0, gridRows)); // สุ่มคู่อันดับ
            } while (Snake.Contains(p)); // ห้ามอาหารซ้อนบนตัวงู

            Food = p; // ตั้งค่าสำเร็จ
        }

        // แปลงสถานะเกมเป็น GameState object สำหรับบันทึกลงไฟล์
        public GameState ToGameState(string activeSkin, List<string> ownedSkins)
        {
            return new GameState
            {
                Snake = new List<Point>(Snake),           // copy รายการงู
                Food = Food,                              // ตำแหน่งอาหาร
                Direction = Direction,                    // ทิศทาง
                Score = Score,                            // คะแนน
                BestScore = BestScore,                    // highscore
                Money = Money,                            // เงิน
                ActiveSkin = activeSkin,                  // skin ปัจจุบัน
                OwnedSkins = string.Join(",", ownedSkins) // เก็บเป็น string CSV
            };
        }

        // โหลดค่าจาก GameState กลับเข้าเกม
        public void LoadFromState(GameState st)
        {
            Snake = new List<Point>(st.Snake); // เรียกคืนตำแหน่งงู
            Food = st.Food;                    // ตำแหน่งอาหาร
            Direction = st.Direction;          // ทิศทางเกม
            Score = st.Score;                  // คะแนน
            BestScore = st.BestScore;          // best score
            Money = st.Money;                  // เงิน
            GameUpdated?.Invoke();             // แจ้ง UI ให้ redraw สถานะ
        }

        // ใช้เมื่อเล่นใหม่อีกครั้งหลังจากตาย
        public void ResetDeathFlag()
        {
            IsDead = false; // ตั้งค่าให้ "ยังไม่ตาย"
        }

        public void ResizeGrid(int newCols, int newRows)
        {
            // 1) ถ้าขนาดเท่าเดิมไม่ต้องทำงานซ้ำ
            if (newCols == gridCols && newRows == gridRows)
                return;

            // 2) อัปเดตขนาด grid โดยตรง (ไม่ต้อง reflection)
            gridCols = newCols;
            gridRows = newRows;

            // 3) Clamp ตำแหน่งงูให้ไม่ออกนอกแผนที่ใหม่
            for (int i = 0; i < Snake.Count; i++)
            {
                var p = Snake[i];
                Snake[i] = new Point(
                    Math.Min(p.X, gridCols - 1),
                    Math.Min(p.Y, gridRows - 1)
                );
            }

            // 4) ป้องกันงูซ้อนกันเอง: ลบหางจนกว่าจะไม่ซ้ำกัน
            HashSet<Point> seen = new();
            for (int i = 0; i < Snake.Count; i++)
            {
                if (seen.Contains(Snake[i]))
                {
                    // ลบงูตั้งแต่ i เป็นต้นไป
                    Snake.RemoveRange(i, Snake.Count - i);
                    break;
                }
                seen.Add(Snake[i]);
            }

            // 5) ถ้าอาหารออกนอก map → หาใหม่ให้ปลอดภัย
            if (Food.X >= gridCols || Food.Y >= gridRows || Snake.Contains(Food))
                SpawnFood();

            // 6) แจ้ง UI วาดใหม่
            GameUpdated?.Invoke();
        }
    }
}
