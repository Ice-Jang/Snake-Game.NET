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
        public List<Point> Snake { get; private set; } = new(); // ตำแหน่ง segment ของงู
        public Point Food { get; private set; }                 // ตำแหน่งอาหาร
        public string Direction { get; private set; } = "Right";// ทิศทางปัจจุบัน
        public int Score { get; private set; } = 0;             // คะแนนปัจจุบัน
        public int BestScore { get; private set; } = 0;         // คะแนนสูงสุด
        public int Money { get; set; } = 0;             // เงินผู้เล่น
        public int Speed { get; set; } = 110;
        public bool IsDead { get; private set; } = false;

        private readonly Random rand = new Random();            // ตัวสุ่มใช้ในหลายจุด
        private readonly int gridCols;                          // จำนวนคอลัมน์ของ grid
        private readonly int gridRows;                          // จำนวนแถวของ grid

        // Events ให้ UI subscribe เพื่ออัปเดตหรือแสดงผล
        public event Action? GameUpdated;                       // เรียกเมื่อสถานะเปลี่ยน (เช่น move / spawn)
        public event Action? GameOver;                          // เรียกเมื่อเกมจบ
        public event Action<Point>? FoodEaten;
        public event Action? OnEat;
        public event Action? OnDie;

        // สร้าง controller ระบุขนาด grid (cols, rows)
        public SnakeGameController(int cols, int rows)
        {
            gridCols = cols; gridRows = rows;
        }

        // เริ่มเกมใหม่ (รีเซ็ตสถานะ)
        public void StartNewGame(int initialLen = 3)
        {
            Score = 0; Direction = "Right"; Snake.Clear(); // รีเซ็ตสถานะ
            // สร้างงูเริ่มต้นตรงกลาง
            int cx = Math.Max(2, gridCols / 4);
            int cy = Math.Max(2, gridRows / 2);
            for (int i = 0; i < initialLen; i++)
                Snake.Add(new Point(cx - i, cy)); // เพิ่ม segment (หางที่ index end)

            SpawnFood(); // สร้างอาหาร
            GameUpdated?.Invoke(); // แจ้ง UI ให้รีเฟรช
        }

        // เปลี่ยนทิศทาง (รับ string เพื่อความเข้ากับโค้ดเดิม)
        public void ChangeDirection(string newDir)
        {
            // ป้องกันย้อนศรตรงกัน (เช่น Left <-> Right)
            if ((newDir == "Left" && Direction == "Right") ||
                (newDir == "Right" && Direction == "Left") ||
                (newDir == "Up" && Direction == "Down") ||
                (newDir == "Down" && Direction == "Up"))
                return;
            Direction = newDir;
        }

        // อัปเดตสถานะเกมหนึ่ง step (เรียกโดย timer)
        public void Update()
        {
            if (Snake.Count == 0) return; // ถ้าไม่มีงู ไม่ทำอะไร

            Point head = Snake[0];                // หาตำแหน่งหัวปัจจุบัน
            Point newHead = new Point(head.X, head.Y); // คัดลอกเพื่อคำนวณตำแหน่งใหม่

            // คำนวณ newHead ตามทิศทาง
            switch (Direction)
            {
                case "Up": newHead.Y -= 1; break;
                case "Down": newHead.Y += 1; break;
                case "Left": newHead.X -= 1; break;
                case "Right": newHead.X += 1; break;
            }

            // ตรวจชนขอบกริด -> ถ้าชน ให้จบเกม
            if (newHead.X < 0 || newHead.Y < 0 || newHead.X >= gridCols || newHead.Y >= gridRows)
            {
                OnDie?.Invoke(); // 🔥 แจ้งว่า "ตายแล้ว"
                IsDead = true;
                return;
            }

            // ตรวจชนตัวเอง -> ถ้าชน ให้จบเกม
            if (Snake.Contains(newHead))
            {
                OnDie?.Invoke(); // 🔥 แจ้งว่า "ตายแล้ว"
                IsDead = true;
                return;
            }

            // เพิ่มหัวใหม่เข้า list
            Snake.Insert(0, newHead);

            // ถ้า newHead == Food -> กินอาหาร (ไม่ลบหาง)
            if (newHead == Food)
            {
                OnEat?.Invoke(); // 🔥 แจ้งว่า "กินแล้ว"
                FoodEaten?.Invoke(Food);   // <<< แจ้ง UI ว่าอาหารถูกกินแล้ว
                Score += 10;                     // เพิ่มคะแนน
                Money += 1;                      // ได้เงิน
                if (Score > BestScore) { BestScore = Score; } // อัปเดต highscore
                SpawnFood();                     // สร้างอาหารใหม่
            }
            else
            {
                Snake.RemoveAt(Snake.Count - 1); // ถ้าไม่กิน -> ลบหาง (เคลื่อน)
            }

            GameUpdated?.Invoke(); // แจ้ง UI ให้ redraw
        }

        // สร้างอาหารในตำแหน่งสุ่มที่ไม่ชนงู
        private void SpawnFood()
        {
            Point p;
            do
            {
                p = new Point(rand.Next(0, gridCols), rand.Next(0, gridRows));
            } while (Snake.Contains(p));
            Food = p;
        }

        // เติมฟังก์ชัน Save -> คืนค่า GameState object สำหรับ serialization
        public GameState ToGameState(string activeSkin, List<string> ownedSkins)
        {
            return new GameState
            {
                Snake = new List<Point>(Snake),          // copy list
                Food = Food,
                Direction = Direction,
                Score = Score,
                BestScore = BestScore,
                Money = Money,
                ActiveSkin = activeSkin,
                OwnedSkins = string.Join(",", ownedSkins) // เก็บเป็น CSV ง่ายๆ
            };
        }

        // โหลดจาก GameState
        public void LoadFromState(GameState st)
        {
            Snake = new List<Point>(st.Snake);
            Food = st.Food;
            Direction = st.Direction;
            Score = st.Score;
            BestScore = st.BestScore;
            Money = st.Money;
            GameUpdated?.Invoke();
        }

        public void ResetDeathFlag()
        {
            IsDead = false;
        }
    }
}
