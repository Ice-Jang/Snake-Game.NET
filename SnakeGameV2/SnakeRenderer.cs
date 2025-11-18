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
        private readonly int cellSize;                      // ขนาดหนึ่งช่องของ Grid ใช้กำหนดตำแหน่งและขนาดของ snake/food/particles (readonly → แก้ไม่ได้หลังสร้าง)
        private int eatEffectFrame = 0;                     // จำนวนเฟรมที่เหลือของเอฟเฟกต์เมื่อกินอาหาร (0 = ไม่เล่น)
        private Point lastFoodPos = Point.Empty;            // ตำแหน่งล่าสุดที่งูกินอาหาร ใช้เป็น origin เวลาเล่น EatEffect
        private Color primaryColor = Color.FromArgb(0, 200, 0); // สีหลักของลำตัวงู (ค่า default ก่อนเปลี่ยนสกิน)
        private Color accentColor = Color.Cyan;             // สีเน้น เช่น เส้นขอบหัว งาตา หรือเอฟเฟกต์ glow

        private List<Particle> particles = new List<Particle>(); // รายการเก็บ particle ทั้งหมดที่ถูกสร้าง (ตอนระเบิด)
        private bool useGlow = true;                        // เปิด/ปิดเอฟเฟกต์แสงเรืองจากงู
        private bool deathEffectActive = false;             // true ระหว่างเอฟเฟกต์ระเบิดงูยังค้างอยู่ (particle ยังไม่หมด)
        private bool hideSnake = false;                     // ซ่อนงูหลังเริ่ม death explosion เพื่อไม่ให้งูค้างอยู่ในฉาก

        public event EventHandler? DeathEffectFinishedEvent; // event เมื่อ particle หมด → แจ้ง Form/GameController ว่าแสดงเมนู GameOver ได้

        public SnakeRenderer(int cellSize)                  // constructor รับขนาดเซลล์ของเกม
        {
            this.cellSize = cellSize;                       // กำหนดค่า cellSize ให้ renderer ใช้งานตลอดอายุ object
        }

        // ตั้งค่าสกิน (รับสีจาก SkinManager)
        public void SetSkin(Color mainColor, Color accent)   // ฟังก์ชันเปลี่ยนสกิน รับสีตัวหลัก + สีขอบ
        {
            primaryColor = mainColor;                       // เซ็ตสีหลักของงู เช่น ร่างกายทั้งหมด
            accentColor = accent;                           // เซ็ตสีเน้น เช่น หัว, glow, highlight
        }

        // ฟังก์ชันหลัก: วาดทั้ง map/food/snake/particle effect
        public void Render(Graphics g, SnakeGameController game)   // ฟังก์ชันเรนเดอร์ทั้งหมด (อาหาร, งู, เอฟเฟกต์)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;             // เปิดโหมดทำให้ขอบวัตถุนุ่ม ไม่เป็นขั้นบันได

            // วาดอาหาร
            g.FillEllipse(                                        // ใช้วาดวงกลมอาหาร
                Brushes.Red,                                      // สีแดง
                game.Food.X * cellSize,                           // แปลงตำแหน่ง X ใน grid → pixel
                game.Food.Y * cellSize,                           // แปลงตำแหน่ง Y ใน grid → pixel
                cellSize,                                         // ความกว้างของภาพอาหาร
                cellSize                                          // ความสูงของภาพอาหาร (เท่ากัน → กลม)
            );

            DrawEatEffect(g);                                     // วาดเอฟเฟกต์กิน (ถ้ามี)

            if (!hideSnake)                                       // วาดงูเมื่อไม่ซ่อน (ตอนระเบิดจะ hide)
            {
                for (int i = 0; i < game.Snake.Count; i++)        // loop ทุก segment ของงู (หัว→หาง)
                {
                    Point seg = game.Snake[i];                    // ดึงตำแหน่ง segment ในรูปแบบ grid coordinate

                    int x = seg.X * cellSize;                     // แปลง X grid → pixel
                    int y = seg.Y * cellSize;                     // แปลง Y grid → pixel

                    float sizeFactor = 1.0f - (i / (float)game.Snake.Count) * 0.7f; // ทำให้ส่วนหน้าใหญ่ ส่วนท้ายเล็กลงอย่างลื่น
                    sizeFactor = Math.Max(0.3f, sizeFactor);      // ไม่ให้ segment เล็กเกินไป (ต่ำสุด 30%)

                    int segSize = (int)(cellSize * sizeFactor);   // คำนวณขนาดจริงของ segment เป็น pixel
                    int offset = (cellSize - segSize) / 2;        // เว้นกรอบเพื่อให้เล็กลงอย่างสมมาตรกลาง cell

                    Rectangle rect = new Rectangle(               // กรอบสี่เหลี่ยมของ segment
                        x + offset,
                        y + offset,
                        segSize,
                        segSize
                    );

                    using (var bodyBrush = new LinearGradientBrush( // ทำสีไล่เฉดสวยงามให้ลำตัวงู
                        rect,
                        ControlPaint.Dark(primaryColor),            // สีเริ่มต้น = เข้ม
                        ControlPaint.Light(primaryColor),           // สีปลาย = อ่อน
                        LinearGradientMode.ForwardDiagonal))        // ไล่เฉดเฉียงเพื่อให้ดูมีมิติ
                    {
                        g.FillRectangle(bodyBrush, rect);           // วาดลำตัวงู
                    }

                    if (i == game.Snake.Count - 1)                 // ถ้าเป็น segment ท้ายสุด → วาดหาง
                    {
                        int tailW = Math.Max(2, (int)(segSize * 0.5)); // ความกว้างหาง (ครึ่งของลำตัว)
                        int tailH = Math.Max(2, (int)(segSize * 0.3)); // ความสูงหาง (แคบลงอีก)

                        Rectangle tailRect = new Rectangle(         // กรอบหาง
                            x + offset + (segSize - tailW) / 2,
                            y + offset + (segSize - tailH) / 2,
                            tailW,
                            tailH
                        );

                        using (var tailBrush = new SolidBrush(      // แปรงสำหรับหาง (สีเข้มกว่า)
                            Color.FromArgb(200, ControlPaint.Dark(primaryColor))))
                        {
                            g.FillRectangle(tailBrush, tailRect);    // วาดหาง
                        }
                    }

                    if (i == 0)                                      // วาดหัวงู (เฉพาะ segment แรก)
                    {
                        using (var pen = new Pen(Color.FromArgb(160, accentColor), 2)) // สร้างขอบหัวงู
                        {
                            g.DrawRectangle(pen, rect);             // วาดกรอบ highlight หัว
                        }

                        int eye = Math.Max(2, segSize / 6);         // ขนาดตา
                        int eyeOffset = Math.Max(2, segSize / 5);   // การเลื่อนตำแหน่งตา

                        g.FillRectangle(                             // ตาซ้าย
                            Brushes.White,
                            x + offset + eyeOffset,
                            y + offset + eyeOffset / 2,
                            eye,
                            eye);

                        g.FillRectangle(                             // ตาขวา
                            Brushes.White,
                            x + offset + segSize - eyeOffset - eye,
                            y + offset + eyeOffset / 2,
                            eye,
                            eye);

                        g.FillRectangle(                             // ม่านตาซ้าย
                            Brushes.Black,
                            x + offset + eyeOffset + 1,
                            y + offset + eyeOffset / 2 + 1,
                            Math.Max(1, eye / 2),
                            Math.Max(1, eye / 2));

                        g.FillRectangle(                             // ม่านตาขวา
                            Brushes.Black,
                            x + offset + segSize - eyeOffset - eye + 1,
                            y + offset + eyeOffset / 2 + 1,
                            Math.Max(1, eye / 2),
                            Math.Max(1, eye / 2));
                    }

                    if (useGlow)                                     // ถ้าเปิด glow effect
                    {
                        int glowSize = (int)(segSize * 0.6f);        // ขนาดวง glow รอบ segment

                        using Brush glow = new SolidBrush(           // แปรงแสงเรือง
                            Color.FromArgb(40, accentColor));

                        g.FillRectangle(                             // วาด glow รอบ segment เป็นกล่องโปร่งแสง
                            glow,
                            x + offset - glowSize / 4,
                            y + offset - glowSize / 4,
                            segSize + glowSize / 2,
                            segSize + glowSize / 2);
                    }
                }
            }

            RenderParticles(g, game);                                // วาด + อัปเดต particle explosion
        }

        //-------------------------------------------------
        // Eat Effect
        //-------------------------------------------------

        private void DrawEatEffect(Graphics g)                 // ฟังก์ชันวาดเอฟเฟกต์วงแหวนตอนกินอาหาร
        {
            if (eatEffectFrame <= 0) return;                   // ถ้าเฟรมหมดแล้ว ไม่ต้องวาดอะไร

            int maxFrames = 12;                                // จำนวนเฟรมสูงสุดของแอนิเมชันวงแหวน
            float t = eatEffectFrame / (float)maxFrames;       // คำนวณสัดส่วน 1 → 0 เพื่อทำ fade-out + shrink

            int radius = (int)(cellSize * (1 + (1 - t) * 3));  // รัศมีวงแหวน = ใหญ่ขึ้นเมื่อ t ลดลง
            int alpha = (int)(200 * t);                        // ความโปร่งใสลดลงเมื่อใกล้จบ

            using (Pen p = new Pen(Color.FromArgb(alpha, 255, 220, 60), 3)) // ปากกาแสงสีทองโปร่งแสง
            {
                g.DrawEllipse(                                 // วาดวงแหวนออกจากจุดที่กินอาหาร
                    p,
                    lastFoodPos.X * cellSize + cellSize / 2 - radius / 2, // X center - half radius
                    lastFoodPos.Y * cellSize + cellSize / 2 - radius / 2, // Y center - half radius
                    radius,                                     // width ของวง
                    radius                                      // height ของวง
                );
            }

            eatEffectFrame--;                                   // ลดเฟรมทุกครั้งเพื่อทำให้เอฟเฟกต์จางลงเรื่อย ๆ
        }

        public void NotifyFoodEaten(Point foodPos, int frames = 12) // ฟังก์ชันเรียกเมื่อ game แจ้งว่ามีการกินอาหาร
        {
            lastFoodPos = foodPos;                                 // บันทึกตำแหน่งอาหารล่าสุดเพื่อใช้เป็นจุดวงแหวน
            eatEffectFrame = frames;                               // รีเซ็ตจำนวนเฟรมแอนิเมชันกินใหม่
        }

        //-------------------------------------------------
        // Death Explosion
        //-------------------------------------------------

        public void CreateDeathExplosion(List<Point> snake, int particlePerSegment = 40)  // ฟังก์ชันสร้างเอฟเฟกต์ระเบิดงูตอนตาย
        {
            particles.Clear();                                       // ล้าง particle ทั้งหมดก่อนสร้างใหม่
            hideSnake = true;                                        // ซ่อนงู เพื่อให้เห็นแค่เอฟเฟกต์ระเบิด
            deathEffectActive = true;                                // ทำเครื่องหมายว่ากำลังเล่น death effect

            Random r = new Random();                                 // Random สำหรับสุ่มค่า particle
            Color baseColor = primaryColor;                          // สีหลักของงู เพื่อผสมใน particle
            Color accent = accentColor;                              // สีเน้น (ใช้เพิ่มประกาย)

            ParticleShape[] shapePool = new ParticleShape[]          // ชนิดของชิ้นส่วนที่แตกออก
            {
                ParticleShape.Square,                                // สี่เหลี่ยมปกติ
                ParticleShape.RotatedSquare,                         // สี่เหลี่ยมหมุน
                ParticleShape.Shard,                                 // ชิ้นยาว (เศษแหลม)
                ParticleShape.Line,                                  // เส้นยาว
                ParticleShape.Triangle                               // สามเหลี่ยม
            };

            foreach (var seg in snake)                               // loop ทุก segment ของงู แล้วสร้าง particle รอบจุดนั้น
            {
                float cx = seg.X * cellSize + cellSize / 2f;         // X center ของ segment (เป็นจุดกำเนิด particle)
                float cy = seg.Y * cellSize + cellSize / 2f;         // Y center ของ segment

                for (int i = 0; i < particlePerSegment; i++)         // สร้าง particle ต่อ segment ตามจำนวนที่กำหนด
                {
                    ParticleShape shape = shapePool[r.Next(shapePool.Length)]; // เลือกรูปร่าง particle แบบสุ่ม

                    float rot = (float)(r.NextDouble() * 360.0);      // มุมเริ่มต้น (องศา)
                    float angVel = (float)(r.NextDouble() * 20.0 - 10.0); // ความเร็วการหมุน (แกว่งได้ -10 ถึง +10)

                    float speed = 20f + (float)r.NextDouble() * 40f;  // ความเร็วสุ่มของ particle (ระเบิดออกแรง)
                    float angle = (float)(r.NextDouble() * Math.PI * 2.0); // มุมการเคลื่อนที่ทุกทิศทาง 0–360°
                    float vx = (float)Math.Cos(angle) * speed;        // ความเร็วแกน X
                    float vy = (float)Math.Sin(angle) * speed;        // ความเร็วแกน Y

                    float size = 2f + (float)r.NextDouble()           // ขนาดเริ่มต้นของ particle
                                 * (seg == snake[0]                    // หัวงู → ขนาดใหญ่กว่า
                                    ? cellSize * 1.2f
                                    : cellSize * 0.9f);

                    if (shape == ParticleShape.Shard)                 // ถ้าเป็น shard → เพิ่มความยาว
                        size *= 1.6f;

                    int life = 10 + r.Next(40);                       // อายุ particle แบบสุ่ม (ยิ่งมากอยู่ยิ่งนาน)

                    Color mixed = Color.FromArgb(                     // สร้างสีใหม่โดยผสมสีร่างกาย + accent
                        255,
                        Clamp(baseColor.R + r.Next(-30, 50) + accent.R / 3),
                        Clamp(baseColor.G + r.Next(-30, 50) + accent.G / 3),
                        Clamp(baseColor.B + r.Next(-30, 50) + accent.B / 3)
                    );

                    particles.Add(new Particle(                       // เพิ่ม particle ลงลิสต์
                        cx, cy,                                       // ตำแหน่งเริ่มต้นกลาง segment
                        vx, vy,                                       // ความเร็ว
                        life,                                         // อายุการอยู่บนจอ
                        mixed,                                        // สีผสมที่ได้
                        size,                                         // ขนาด particle
                        shape,                                        // รูปร่าง (Square / Shard / Triangle ฯลฯ)
                        rot,                                          // มุมเริ่มต้น
                        angVel,                                       // ความเร็วหมุน
                        motionBlurSteps: 3                            // เปิด motion blur 3 ชั้น
                    ));
                }
            }
        }

        private int Clamp(int v)
        {
            if (v < 0) return 0;
            if (v > 255) return 255;
            return v;
        }

        //-------------------------------------------------
        // Render + Physics update for particles
        //-------------------------------------------------

        private void RenderParticles(Graphics g, SnakeGameController game)      // ฟังก์ชันวาด particle ทั้งหมด + อัปเดตฟิสิกส์
        {
            int W = game.GridCols * cellSize;                                   // ความกว้างพื้นที่เกมเป็นพิกเซลทั้งหมด
            int H = game.GridRows * cellSize;                                   // ความสูงพื้นที่เกมเป็นพิกเซลทั้งหมด

            for (int i = particles.Count - 1; i >= 0; i--)                      // loop ทุก particle (ย้อนหลังเพื่อ Remove ได้)
            {
                Particle p = particles[i];                                      // ดึง particle ปัจจุบัน

                p.Update(
                    friction: 0.96f,                                            // ลดความเร็วทุกเฟรมเพื่อให้ค่อย ๆ ช้าลง (แรงเสียดทาน)
                    gravity: 0f,                                                // ไม่มีแรงโน้มถ่วง (snake explosion ไม่ตกลงล่าง)
                    boundsW: W,                                                 // พื้นที่ขอบสำหรับกระแทก X
                    boundsH: H,                                                 // พื้นที่ขอบสำหรับกระแทก Y
                    bounceFactor: 0.65f                                          // อัตราแรงเด้งหลังชนกำแพง (65%)
                );                                                              // อัปเดตฟิสิกส์ particle ทั้งหมด

                p.Draw(g, motionBlurSteps: 3);                                  // วาด particle พร้อม motion blur 3 ชั้น

                if (!p.IsAlive)                                                 // ถ้า particle ตาย (Life หมด หรือขนาดเล็กเกิน)
                    particles.RemoveAt(i);                                      // ลบออกจากลิสต์
            }

            if (deathEffectActive && particles.Count == 0)                       // ถ้า death effect ยัง active แต่ไม่มี particle เหลือแล้ว
            {
                deathEffectActive = false;                                      // ปิด death effect
                hideSnake = false;                                              // ปล่อยให้วาดงูใหม่ (ตอนเริ่มเกมใหม่)
                DeathEffectFinishedEvent?.Invoke(this, EventArgs.Empty);        // แจ้ง FormMain ว่าแอนิเมชันตายจบแล้ว
            }
        }
    }
}
