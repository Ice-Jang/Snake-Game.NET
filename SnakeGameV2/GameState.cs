using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGameV2
{
    internal class GameState
    {
        // DTO สำหรับบันทึกสถานะเกมเป็น JSON เวลา Save/Load
        public List<Point> Snake { get; set; } = new(); // เก็บตำแหน่งแต่ละ segment
        public Point Food { get; set; }                 // ตำแหน่งอาหาร
        public string Direction { get; set; } = "Right"; // ทิศทางปัจจุบัน
        public int Score { get; set; } = 0;              // คะแนนปัจจุบัน
        public int BestScore { get; set; } = 0;          // คะแนนสูงสุด
        public int Money { get; set; } = 0;              // เงินผู้เล่น
        public string OwnedSkins { get; set; } = "";     // ข้อมูลสกินที่ซื้อแล้ว (string list) 
        public string ActiveSkin { get; set; } = "";     // สกินที่กำลังใช้
    }
}
