using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGameV2
{
    // Renderer รับผิดชอบการวาดทั้งหมด โดยไม่เปลี่ยนสถานะเกม
    internal class SnakeRenderer
    {
        private readonly int cellSize;                   // ขนาดช่องพิกเซล
        private int eatEffectFrame = 0;                  // frame ที่เหลือของเอฟเฟกต์กินอาหาร
        private Point lastFoodPos = Point.Empty;         // ตำแหน่งที่เกิดการกิน (สำหรับเอฟเฟกต์)
        private Color primaryColor = Color.FromArgb(0, 200, 0); // สีหลักเริ่มต้น
        private Color accentColor = Color.Cyan;          // สีหัว/highlight
        private List<Particle> particles = new List<Particle>();
        private bool useGlow = true;                     // เปิด/ปิด glow
        public bool DeathEffectFinished => particles.Count == 0;

        public SnakeRenderer(int cellSize)
        {
            this.cellSize = cellSize; // กำหนดขนาดช่อง
        }

        // ตั้งค่าสกิน (รับสีจาก SkinManager)
        public void SetSkin(Color mainColor, Color accent)
        {
            primaryColor = mainColor;
            accentColor = accent;
        }

        // ฟังก์ชันหลัก: วาดทั้ง map/food/snake/particle effect
        public void Render(Graphics g, SnakeGameController game)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias; // ทำให้เส้นขอบนุ่ม
            // วาด background (ฟลูซ) — caller ควบคุม background ของ control เอง
            // วาดอาหารเป็นวงกลมสีแดง
            g.FillEllipse(Brushes.Red, game.Food.X * cellSize, game.Food.Y * cellSize, cellSize, cellSize);

            // วาดเอฟเฟกต์กินอาหาร (ถ้ามี) ก่อนหรือหลังงูก็ได้ — เราวาดบนสุดท้าย
            DrawEatEffect(g);

            // วาดลำตัวงู: หัว -> ลำตัว -> หาง (เรียง index)
            for (int i = 0; i < game.Snake.Count; i++)
            {
                Point seg = game.Snake[i];
                int x = seg.X * cellSize;
                int y = seg.Y * cellSize;

                // ขนาด taper: หัวใหญ่ หางเล็ก (sizeFactor ในช่วง 1.0 -> 0.3)
                float sizeFactor = 1.0f - (i / (float)game.Snake.Count) * 0.7f; // 70% taper
                sizeFactor = Math.Max(0.3f, sizeFactor); // ไม่เล็กเกินไป

                int segSize = (int)(cellSize * sizeFactor);
                int offset = (cellSize - segSize) / 2;

                // สร้าง gradient brush ให้มีมิติ
                using (var bodyBrush = new LinearGradientBrush(new Rectangle(x, y, segSize, segSize),
                                                               ControlPaint.Dark(primaryColor),
                                                               ControlPaint.Light(primaryColor),
                                                               LinearGradientMode.ForwardDiagonal))
                {
                    // ถ้าเป็นหาง (index == last) ให้วาดเป็นวงรีแคบ ๆ (หรือเป็นสามเหลี่ยม)
                    if (i == game.Snake.Count - 1)
                    {
                        // วาดหางเป็นวงรีเรียว
                        using (var tailBrush = new SolidBrush(Color.FromArgb(200, ControlPaint.Dark(primaryColor))))
                        {
                            int tailW = Math.Max(2, (int)(segSize * 0.7));
                            int tailH = Math.Max(2, (int)(segSize * 0.4));
                            g.FillEllipse(tailBrush, x + offset + (segSize - tailW) / 2, y + offset + (segSize - tailH) / 2, tailW, tailH);
                        }
                    }

                    // วาด body (ellipse ให้กลมเรียบ)
                    g.FillEllipse(bodyBrush, x + offset, y + offset, segSize, segSize);

                    // ถ้าเป็นหัว ให้วาดตาและ highlight
                    if (i == 0)
                    {
                        // วาด highlight เล็ก ๆ ที่หัว
                        using (var pen = new Pen(Color.FromArgb(160, accentColor), 2))
                        {
                            g.DrawEllipse(pen, x + offset, y + offset, segSize, segSize);
                        }
                        // วาดตา (สองตา)
                        int eye = Math.Max(2, segSize / 6);
                        int eyeOffset = Math.Max(2, segSize / 5);
                        g.FillEllipse(Brushes.White, x + offset + eyeOffset, y + offset + eyeOffset / 2, eye, eye);
                        g.FillEllipse(Brushes.White, x + offset + segSize - eyeOffset - eye, y + offset + eyeOffset / 2, eye, eye);
                        g.FillEllipse(Brushes.Black, x + offset + eyeOffset + 1, y + offset + eyeOffset / 2 + 1, Math.Max(1, eye / 2), Math.Max(1, eye / 2));
                        g.FillEllipse(Brushes.Black, x + offset + segSize - eyeOffset - eye + 1, y + offset + eyeOffset / 2 + 1, Math.Max(1, eye / 2), Math.Max(1, eye / 2));
                    }
                }

                // ถ้าเปิด glow ให้วาด glow รอบ ๆ segment แบบนุ่ม
                if (useGlow)
                {
                    int glowSize = (int)(segSize * 0.6f);
                    using (Brush glow = new SolidBrush(Color.FromArgb(40, accentColor)))
                    {
                        g.FillEllipse(glow, x + offset - glowSize / 4, y + offset - glowSize / 4, segSize + glowSize / 2, segSize + glowSize / 2);
                    }
                }
            }

            // วาดอนุภาค Explosion
            foreach (var p in particles)
            {
                int alpha = Math.Max(0, Math.Min(255, p.Life * 5));

                using var brush = new SolidBrush(Color.FromArgb(alpha, p.Color));
                g.FillEllipse(brush, p.X, p.Y, 6, 6);
            }
            UpdateParticles();
        }

        // วาดเอฟเฟกต์ตอนกินอาหาร (วงแหวนขยาย)
        private void DrawEatEffect(Graphics g)
        {
            if (eatEffectFrame <= 0) return;                 // ถ้าไม่มีเฟรมให้วาด ให้กลับ
            int maxFrames = 12;                              // กำหนดจำนวนเฟรมทั้งหมดของเอฟเฟกต์
            float t = eatEffectFrame / (float)maxFrames;     // t ในช่วง 0..1
            int radius = (int)(cellSize * (1 + (1 - t) * 3)); // ขนาดวงกลมขึ้นอยู่กับเฟรม
            int alpha = (int)(200 * t);                      // alpha ลดเมื่อเฟรมลดลง

            using (Pen p = new Pen(Color.FromArgb(alpha, 255, 220, 60), 3))
            {
                // วาดวงกลมที่ตำแหน่ง lastFoodPos
                g.DrawEllipse(p, lastFoodPos.X * cellSize + cellSize / 2 - radius / 2,
                                lastFoodPos.Y * cellSize + cellSize / 2 - radius / 2,
                                radius, radius);
            }
            eatEffectFrame--;                                 // ลดเฟรมทีละ 1
        }

        // ฟังก์ชันที่ UI เรียกทุกครั้งที่เห็นว่าอาหารถูกกิน ให้ renderer เริ่มเอฟเฟกต์
        public void NotifyFoodEaten(Point foodPos, int frames = 12)
        {
            lastFoodPos = foodPos;
            eatEffectFrame = frames;
        }

        public void CreateDeathExplosion(Point head, int count = 40)
        {
            particles.Clear();

            float cx = head.X * cellSize + cellSize / 2;
            float cy = head.Y * cellSize + cellSize / 2;

            Random r = new Random();

            for (int i = 0; i < count; i++)
            {
                // กระจายอนุภาคออกทุกทิศ
                float angle = (float)(r.NextDouble() * Math.PI * 2);
                float speed = 1.0f + (float)r.NextDouble() * 4f;

                float vx = (float)Math.Cos(angle) * speed;
                float vy = (float)Math.Sin(angle) * speed;

                var p = new Particle(
                    cx,
                    cy,
                    vx,
                    vy,
                    life: 30 + r.Next(20),
                    color: Color.FromArgb(255,
                                          255,
                                          r.Next(150, 255),
                                          r.Next(0, 80))      // ส้ม-แดงแบบไฟ
                );

                particles.Add(p);
            }
        }

        private void UpdateParticles()
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var p = particles[i];

                p.X += p.VX;
                p.Y += p.VY;
                p.Life--;

                // ทำให้ fade-out
                if (p.Life <= 0)
                {
                    particles.RemoveAt(i);
                }
            }
        }

        public void UpdateOnlyAnimation()
        {
            UpdateParticles();  // อัปเดต explosion
                                // EatEffect ไม่ต้อง update เพราะตอนตายไม่ได้ใช้
        }
    }
}
