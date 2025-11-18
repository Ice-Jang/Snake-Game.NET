using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGameV2
{
    // Manager สำหรับสกิน ง่ายต่อการขยาย เช่น load from assets หรือ external store
    internal class SkinManager
    {
        public string ActiveSkin { get; private set; } = "Default";     // ชื่อสกินที่ใช้อยู่ตอนนี้ (อ่านได้จากข้างนอก แต่ set ได้เฉพาะในคลาสนี้)
        public HashSet<string> OwnedSkins { get; private set; } = new(); // รายการสกินที่ผู้เล่นเป็นเจ้าของแล้ว (ป้องกันซ้ำ และค้นหาเร็ว)

        // รายชื่อสกินทั้งหมดในเกม mapping: ชื่อสกิน -> (สีหลัก main, สีรอง accent)
        private readonly Dictionary<string, (Color main, Color accent)> skins =
            new Dictionary<string, (Color, Color)>()
            {
            {"Default", (Color.FromArgb(0,200,0), Color.Cyan)},      // Default: เขียวหลัก + Cyan accent
            {"Cyan", (Color.FromArgb(0,200,200), Color.Blue)},       // Cyan: ฟ้าน้ำทะเล + น้ำเงิน
            {"Yellow", (Color.FromArgb(255,200,0), Color.Orange)}    // Yellow: เหลือง + ส้ม
            };

        public SkinManager()
        {
            OwnedSkins.Add("Default"); // ตอนสร้าง SkinManager ใหม่ให้ Default เป็นสกินฟรีติดตัวเสมอ
        }

        // คืนชุดสีของสกินที่ระบุ (main, accent)
        public (Color main, Color accent) GetSkinColors(string skinName)
        {
            if (skins.TryGetValue(skinName, out var v)) return v; // ถ้าชื่อสกินอยู่ใน dictionary → คืนคู่สีของมัน
            return skins["Default"];                               // ถ้าไม่เจอชื่อ → ป้องกัน error โดยคืน Default กลับไป
        }

        // ฟังก์ชันซื้อสกิน (ถ้าเงินพอ) → คืน true ถ้าซื้อสำเร็จ / false ถ้าเงินไม่พอ
        // ตัวแปร money ส่งด้วย ref เพื่อให้เมธอดนี้แก้จำนวนเงินต้นทางได้โดยตรง
        public bool BuySkin(string skinName, int price, ref int money)
        {
            if (OwnedSkins.Contains(skinName)) return true;   // ถ้ามีสกินนี้อยู่แล้ว → ถือว่าสำเร็จ (เพราะซื้อซ้ำไม่จำเป็น)
            if (money < price) return false;                  // ถ้าเงินไม่พอ → ซื้อไม่สำเร็จ
            money -= price;                                   // หักเงินผู้เล่น
            OwnedSkins.Add(skinName);                         // เพิ่มสกินเข้า owned list
            return true;                                      // ซื้อสำเร็จ
        }

        // เปลี่ยนสกินที่ใช้งาน (จะเปลี่ยนได้เฉพาะสกินที่ผู้เล่นเป็นเจ้าของ)
        public bool UseSkin(string skinName)
        {
            if (!OwnedSkins.Contains(skinName)) return false; // ถ้ายังไม่เคยซื้อ → ห้ามใช้
            ActiveSkin = skinName;                            // เปลี่ยนสกินที่ใช้งาน
            return true;                                      // สำเร็จ
        }

        // ส่งคืนรายชื่อสกินที่ผู้เล่นมี (คืนเป็น List ใหม่เพื่อไม่ให้ภายนอกแก้ HashSet ต้นฉบับได้)
        public List<string> GetOwnedSkins() => new List<string>(OwnedSkins);
    }
}
