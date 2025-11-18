using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGameV2
{
    internal enum ParticleShape
    {
        /// <summary>สี่เหลี่ยมธรรมดา (ไม่หมุน)</summary>
        Square,
        /// <summary>สี่เหลี่ยมที่มีมุมหมุนได้ (ใช้ rotation)</summary>
        RotatedSquare,
        /// <summary>เศษเส้นยาวเหมือน shard (long rectangle)</summary>
        Shard,
        /// <summary>เส้นยาว (line) — วาดเป็นเส้นที่มีความหนา</summary>
        Line,
        /// <summary>สามเหลี่ยมชิ้นเล็กๆ (debris)</summary>
        Triangle
    }

    internal class Particle
    {
        // --- ฟิลด์ฟิสิกส์พื้นฐาน ---
        public float X;                // ตำแหน่ง x (pixel)
        public float Y;                // ตำแหน่ง y (pixel)
        public float VX;               // ความเร็วแกน x (pixel per update)
        public float VY;               // ความเร็วแกน y (pixel per update)

        // --- อายุ / สี / ขนาด ---
        public int Life;               // อายุที่เหลือ (ตัวนับเฟรม / ticks)
        public Color Color;            // สีพื้นฐานของอนุภาค

        public float Size;             // ขนาด (หลัก) ของอนุภาค (pixel)
        public float InitialSize;      // ขนาดเริ่มต้นเพื่อคำนวณการลดขนาดตามเวลา

        // --- รูปร่างและการหมุน ---
        public ParticleShape Shape;    // ประเภทรูปร่างของอนุภาค
        public float Rotation;         // มุมการหมุน (degrees)
        public float AngularVelocity;  // ความเร็วการหมุน (degrees per update)

        // --- motion blur trail ---
        // เก็บตำแหน่งย้อนหลังสั้น ๆ เพื่อวาด motion-blur หรือเส้นทางการเคลื่อนที่
        private readonly Queue<PointF> trail; // FIFO queue ให้เก็บจุดย้อนหลัง
        private readonly int maxTrail;        // ความยาวสูงสุดของ trail (motion blur steps)

        // --- แบนด์บอกสถานะ ---
        public bool IsAlive => Life > 0; // เช็กว่าอนุภาคยังไม่หมดอายุ

        public Particle(
            float x,
            float y,
            float vx,
            float vy,
            int life,
            Color color,
            float size = 6f,
            ParticleShape shape = ParticleShape.Square,
            float rotation = 0f,
            float angularVelocity = 0f,
            int motionBlurSteps = 3
        )
        {
            // กำหนดฟิลด์พื้นฐานทั้งหมดจากพารามิเตอร์
            X = x;
            Y = y;
            VX = vx;
            VY = vy;
            Life = Math.Max(1, life);            // ป้องกัน life <= 0 ในการสร้าง
            Color = color;

            InitialSize = Math.Max(1f, size);    // ไม่ให้ไซซ์เป็นศูนย์
            Size = InitialSize;

            Shape = shape;
            Rotation = rotation;
            AngularVelocity = angularVelocity;

            // เซ็ตความยาว trail สำหรับ motion blur (เก็บตำแหน่งย้อนหลัง)
            maxTrail = Math.Max(0, motionBlurSteps);
            trail = new Queue<PointF>(maxTrail + 1);
            if (maxTrail > 0)
            {
                // เริ่มด้วยตำแหน่งเริ่มต้นใน trail
                trail.Enqueue(new PointF(X, Y));
            }
        }

        public void Update(float friction, float gravity, float boundsW, float boundsH, float bounceFactor)
        {
            // ลดความเร็วตาม friction (คูณแต่ละอัปเดต)
            VX *= friction;
            VY *= friction;

            // เพิ่ม gravity ลงในความเร็วแกน Y (ถ้ามี)
            VY += gravity;

            // เปลี่ยนตำแหน่งตามความเร็ว
            X += VX;
            Y += VY;

            // อัปเดตมุมการหมุนด้วย angular velocity
            Rotation += AngularVelocity;
            // ทำให้มุมอยู่ในช่วง 0..360 เพื่อป้องกัน overflow มุม
            if (Rotation >= 360f) Rotation -= 360f;
            if (Rotation < 0f) Rotation += 360f;

            // เก็บตำแหน่งปัจจุบันลงใน trail queue เพื่อใช้วาด motion blur
            if (maxTrail > 0)
            {
                trail.Enqueue(new PointF(X, Y));
                // ถ้าเกินความยาวที่กำหนด ให้ลบตำแหน่งเก่าสุดออก
                while (trail.Count > maxTrail + 1) // +1 เก็บจุดปัจจุบันด้วย
                    trail.Dequeue();
            }

            // ตรวจชนขอบซ้าย-ขวา แล้วเด้ง (bounce)
            // แปลงเป็นขอบจริงสำหรับอนุภาคที่มีขนาด (ใช้ Size เป็นประมาณ)
            float half = Size * 0.5f;
            if (X - half < 0f)
            {
                // วางติดขอบและเด้งกลับ
                X = half;
                VX = -VX * bounceFactor;
                // ลดความเร็วหมุนเล็กน้อยเมื่อกระแทก (ให้ดูสมจริง)
                AngularVelocity *= 0.85f;
            }
            else if (X + half > boundsW)
            {
                X = boundsW - half;
                VX = -VX * bounceFactor;
                AngularVelocity *= 0.85f;
            }

            // ตรวจชนขอบบน-ล่าง
            if (Y - half < 0f)
            {
                Y = half;
                VY = -VY * bounceFactor;
                AngularVelocity *= 0.85f;
            }
            else if (Y + half > boundsH)
            {
                Y = boundsH - half;
                VY = -VY * bounceFactor;
                AngularVelocity *= 0.85f;
            }

            // ค่อย ๆ ลดขนาดเมื่ออายุลดลง (optional visual)
            float lifeRatio = Math.Max(0f, (float)Life / (float)Math.Max(1, Life + 1)); // before decrement
                                                                                        // ให้ขนาดลดตามอายุ (ที่เหลือ) — ปรับสูตรได้
            Size = InitialSize * (0.5f + 0.5f * lifeRatio);

            // ลดอายุท้ายสุด (เรียกก่อนเช็ก IsAlive ใน frame ถัดไป)
            Life--;
        }

        public void Draw(Graphics g, int motionBlurSteps)
        {
            // ถ้า motionBlurSteps > trail.Count-1 ให้ clamp
            int available = Math.Max(0, trail.Count - 1);
            int mb = Math.Min(motionBlurSteps, available);

            // ถ้ามี trail ให้วาด motion blur จากจุดเก่าสุด -> ปัจจุบัน
            if (mb > 0 && maxTrail > 0)
            {
                // คำนวณ alpha step สำหรับ blur (ชั้นด้านหลังจะจางกว่า)
                float alphaStep = 1f / (mb + 1f);

                // นำ trail มาเป็น array เพื่อเข้าถึง index ได้สะดวก
                PointF[] arr = trail.ToArray();

                // วาดจากหลังสุดไปหน้าสุด (ให้ layer ใหม่ทับท้าย)
                int start = Math.Max(0, arr.Length - 1 - mb);
                for (int t = start; t < arr.Length - 1; t++)
                {
                    // ค่าสัมพัทธ์ 0..1 (0 = far, 1 = near)
                    float rel = (t - start + 1f) / (arr.Length - start);
                    // alpha ตามตำแหน่งใน trail (farther => smaller alpha)
                    int a = (int)(ClampToByte((float)Color.A * rel * 0.6f)); // ค่อย ๆ จาง
                    Color c = Color.FromArgb(a, Color.R, Color.G, Color.B);

                    // ขนาด blur เล็กกว่า size จริง (เพื่อให้ดูเหมือนลาก)
                    float s = Math.Max(1f, Size * (0.4f + 0.6f * rel));

                    // วาด shape แบบเบลอ (ไม่มี rotation ใน blur เพื่อ performance)
                    DrawShapeAt(g, arr[t].X, arr[t].Y, s, c, Rotation * 0.5f, Shape, true);
                }
            }

            // วาดตัวหลัก (ปัจจุบัน) ด้วย alpha ขึ้นกับ Life
            int alphaMain = ClampToByte((float)Color.A * 1.0f);
            Color mainColor = Color.FromArgb(alphaMain, Color.R, Color.G, Color.B);

            DrawShapeAt(g, X, Y, Size, mainColor, Rotation, Shape, false);

            // ถ้าช่องทางต้องมี spark/glow จุดเล็ก ๆ รอบอนุภาค (optional) —
            // สามารถเพิ่ม glow ตามค่า Color หรือตามความเร็ว
            // (ไม่ใส่โดย default เพื่อไม่ให้ช้า แต่ renderer สามารถเรียกเพิ่มเติมได้)
        }

        // วาดรูปร่างจริง ๆ โดยรวมการหมุนและรูปทรงต่าง ๆ
        private void DrawShapeAt(Graphics g, float cx, float cy, float size, Color col, float rotationDeg, ParticleShape shape, bool isBlur)
        {
            // แปลงมุมเป็น radiant สำหรับ Math trig
            float rad = rotationDeg * (float)Math.PI / 180f;

            // ใช้ matrix สำหรับ translation + rotation + translate-back เพื่อวาดได้สะดวก
            var oldTransform = g.Transform;
            var m = g.Transform; // copy
            m.Translate(cx, cy);               // ย้าย origin มาที่ศูนย์กลางชิ้นส่วน
            m.Rotate(rotationDeg);             // หมุน
            g.Transform = m;

            // สร้าง brush จาก color
            using (var brush = new SolidBrush(col))
            using (var pen = new Pen(Color.FromArgb(ClampToByte(col.A), col.R, col.G, col.B)))
            {
                // ขนาดครึ่งหนึ่ง ใช้สำหรับวาดจาก center
                float half = size * 0.5f;

                switch (shape)
                {
                    case ParticleShape.Square:
                        // สี่เหลี่ยมไม่หมุน (แต่เราหมุนด้วย transform แล้วก็ได้ผลเหมือนกัน)
                        g.FillRectangle(brush, -half, -half, size, size);
                        break;

                    case ParticleShape.RotatedSquare:
                        // สี่เหลี่ยมหมุน — transform ทำงานให้แล้ว
                        g.FillRectangle(brush, -half, -half, size, size);
                        break;

                    case ParticleShape.Shard:
                        // Shard: rectangle ยาว/บาง — ให้ความรู้สึกเหมือนเศษแหลม
                        float longLen = Math.Max(size * 1.5f, size * (isBlur ? 1.0f : 2.5f));
                        float thin = Math.Max(1f, size * 0.35f);
                        g.FillRectangle(brush, -longLen / 2f, -thin / 2f, longLen, thin);
                        break;

                    case ParticleShape.Line:
                        // วาดเป็นเส้น (ใช้ pen) ที่มีความหนาเล็กน้อย
                        using (var linePen = new Pen(brush, Math.Max(1f, size * 0.25f)))
                        {
                            // เส้นจาก -half -> +half ตามแกน X (transform หมุนได้)
                            g.DrawLine(linePen, -half, 0f, half, 0f);
                        }
                        break;

                    case ParticleShape.Triangle:
                        // วาดสามเหลี่ยมชี้ขึ้น (transform หมุนได้)
                        var pts = new PointF[]
                        {
                        new PointF(0f, -half),           // top
                        new PointF(half, half),          // bottom-right
                        new PointF(-half, half)          // bottom-left
                        };
                        g.FillPolygon(brush, pts);
                        break;
                }
            }

            // คืน transform เดิมกลับไป
            g.Transform = oldTransform;
        }

        // ช่วย clamp float -> byte (0..255)
        private int ClampToByte(float v)
        {
            if (v < 0f) return 0;
            if (v > 255f) return 255;
            return (int)v;
        }
    }
}
