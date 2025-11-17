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
        public string ActiveSkin { get; private set; } = "Default";     // สกินที่ใช้
        public HashSet<string> OwnedSkins { get; private set; } = new(); // สกินที่ซื้อแล้ว

        // สกินพื้นฐานเก็บเป็น mapping ชื่อ -> (mainColor, accentColor)
        private readonly Dictionary<string, (Color main, Color accent)> skins =
            new Dictionary<string, (Color, Color)>()
            {
                {"Default", (Color.FromArgb(0,200,0), Color.Cyan)},
                {"Cyan", (Color.FromArgb(0,200,200), Color.Blue)},
                {"Yellow", (Color.FromArgb(255,200,0), Color.Orange)}
            };

        public SkinManager()
        {
            OwnedSkins.Add("Default"); // ให้สกินพื้นฐานฟรี
        }

        // คืนสีสำหรับสกินชื่อ X
        public (Color main, Color accent) GetSkinColors(string skinName)
        {
            if (skins.TryGetValue(skinName, out var v)) return v;
            return skins["Default"];
        }

        // ซื้อสกิน (ถ้ามีเงินพอ) — caller ตัดเงิน
        public bool BuySkin(string skinName, int price, ref int money)
        {
            if (OwnedSkins.Contains(skinName)) return true;
            if (money < price) return false;
            money -= price;
            OwnedSkins.Add(skinName);
            return true;
        }

        // เปลี่ยนสกินที่ใช้งาน ถ้าเป็น owned
        public bool UseSkin(string skinName)
        {
            if (!OwnedSkins.Contains(skinName)) return false;
            ActiveSkin = skinName;
            return true;
        }

        // คืนรายชื่อ owned เป็น list
        public List<string> GetOwnedSkins() => new List<string>(OwnedSkins);
    }
}
