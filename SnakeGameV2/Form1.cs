using System.Data.SqlTypes;
using System.Security.Policy;
using System.Text.Json;

namespace SnakeGameV2
{
    public partial class FormMain : Form
    {
        private const int DefaultCellSize = 20;                      // ขนาดช่องมาตรฐาน
        private int cellSize = DefaultCellSize;                      // ตัวแปร cellSize (ปรับได้เมื่อ resize)
        private SnakeGameController controller;                      // จัดการ logic เกม
        private SnakeRenderer renderer;                              // จัดการการวาด
        private SkinManager skinMgr = new SkinManager();             // จัดการสกิน
        private SoundManager sound = new SoundManager();             // จัดการเสียง
        private string saveFile = "savegame.json";                   // ไฟล์เก็บสถานะ

        public FormMain()
        {
            InitializeComponent();                                   // สร้าง control ทั้งหมดจากไฟล์ Designer อัตโนมัติ
            this.DoubleBuffered = true;                              // เปิดโหมด double-buffer เพื่อลดอาการภาพกระพริบ (flicker)

            int cols = Math.Max(20, gameArea.Width / cellSize);      // คำนวณจำนวนคอลัมน์จากความกว้าง panel แต่ต้องไม่ต่ำกว่า 20
            int rows = Math.Max(10, gameArea.Height / cellSize);     // คำนวณจำนวนแถวจากความสูง panel แต่ต้องไม่ต่ำกว่า 10

            controller = new SnakeGameController(cols, rows);        // สร้าง controller ของเกม ระบุขนาด Grid
            renderer = new SnakeRenderer(cellSize);                  // สร้าง renderer ที่จะวาดเกมทั้งหมด

            // เมื่อเกม update (งูขยับ / กินอาหาร / spawn food / etc)
            controller.GameUpdated += () =>
            {
                gameArea.Invalidate();                               // ขอให้ Panel วาดใหม่ทั้งหมด
                UpdateUI();                                          // อัปเดตคะแนน เงิน score ต่าง ๆ ใน UI
            };

            renderer.DeathEffectFinishedEvent += (s, e) => OnGameOver(); // เมื่อเอฟเฟกต์ตายจบ → แสดงจอ Game Over

            controller.FoodEaten += pos =>
            {
                renderer.NotifyFoodEaten(pos);                       // แจ้ง Renderer ให้เล่นเอฟเฟกต์วงกระจายตอนกิน
                gameArea.Invalidate();                               // บังคับให้วาด frame แรกทันที
            };

            controller.OnEat += () =>
            {
                sound.PlayEat();                                     // เล่นเสียงกิน
                renderer.NotifyFoodEaten(controller.Food);           // ให้ renderer แสดง eat effect
            };

            controller.OnDie += () =>
            {
                sound.PlayDie();                                     // เล่นเสียงตาย
                renderer.CreateDeathExplosion(controller.Snake);      // สร้าง particle explosion ทั้งตัวงู
            };

            gameTimer.Interval = controller.Speed;                   // ตั้งความเร็วเกมเริ่มต้นตาม controller
            gameArea.Paint += gameArea_Paint;                        // ผูก event Paint ของ panel → ให้ renderer วาดเกม
            gameArea.PreviewKeyDown += gameArea_PreviewKeyDown_1;    // เปิดรับปุ่มลูกศรใน panel

            // ผูกปุ่มทั้งหมดจาก UI
            btnStart.Click += (s, e) => StartButton_Click();         // ปุ่มเริ่มเกม
            btnSave.Click += (s, e) => SaveButton_Click();           // ปุ่มเซฟ
            btnLoad.Click += (s, e) => LoadButton_Click();           // ปุ่มโหลด
            btnPause.Click += (s, e) => PauseButton_Click();         // ปุ่มพักเกม
            btnPlay.Click += (s, e) => PlayButton_Click();           // ปุ่ม resume
            btnReplay.Click += (s, e) => ReplayButton_Click();       // ปุ่มเล่นใหม่
            btnStop.Click += (s, e) => StopButton_Click();           // ปิดเกมกลับเมนู
            btnShop.Click += (s, e) => OpenShop();                   // เปิดร้านขายสกิน
            btnback.Click += (s, e) => BackButton_Click();           // ย้อนกลับจากร้าน
            btnExit.Click += (s, e) => ExitButton_Click();           // ออกจากเกม

            btnBuyCyan.Click += (s, e) => BuycyanButton_Click();     // ซื้อสกิน Cyan
            btnUseCyan.Click += (s, e) => UsecyanButton_Click();     // ใช้สกิน Cyan

            btnBuyYellow.Click += (s, e) => BuyyellowButton_Click(); // ซื้อ Yellow
            btnUseYellow.Click += (s, e) => UseyellowButton_Click(); // ใช้ Yellow

            btnUseGreen.Click += (s, e) => UsegreenButton_Click();   // ใช้ Default green

            UpdateUI();                                              // แสดงคะแนน/เงินทันทีตอนเปิดเกม
        }

        // ฟังก์ชันวาดภาพของ gameArea (เรียกโดยระบบเมื่อ Invalidate)
        private void gameArea_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);                            // เคลียร์พื้นหลังเป็นสีดำ

            var colors = skinMgr.GetSkinColors(skinMgr.ActiveSkin);   // ดึงสีของสกินปัจจุบัน
            renderer.SetSkin(colors.main, colors.accent);             // ส่งให้ renderer ใช้สีนี้

            renderer.Render(e.Graphics, controller);                  // วาดทุกอย่างในเกม
        }

        // เมื่อ controller แจ้ง GameOver
        private void OnGameOver()
        {
            btnReplay.Visible = true;                                 // แสดงปุ่ม Replay
            btnPlay.Visible = false;                                  // ซ่อนปุ่ม Play (Resume)
            lblPause.Text = "Game Over!";                             // แสดงข้อความ
            panelPause.Visible = true;                                // เปิด panel Pause
            lblPause.Location = new Point(237, 116);                  // จัดตำแหน่งให้ข้อความอยู่ตรงกลาง
        }

        // เริ่มเกมใหม่
        private void StartButton_Click()
        {
            gameArea.Visible = true;                                  // แสดงพื้นที่เกม
            panelMainMenu.Visible = false;                            // ซ่อนเมนูหลัก
            panelSaveLoad.Visible = false;                            // ซ่อนหน้าบันทึก/โหลด
            btnPause.Visible = true;                                  // แสดงปุ่ม Pause
            lblScore.Visible = true;                                  // แสดงคะแนน
            panelHighest.Visible = true;                              // แสดงคะแนนสูงสุด

            gameTimer.Interval = controller.Speed;                    // กำหนดความเร็วเริ่มต้น
            controller.ResetDeathFlag();                              // Reset ตัวแปร IsDead
            controller.StartNewGame(3);                               // เริ่มเกมใหม่ ความยาวงูเริ่มต้น = 3
            gameTimer.Start();                                        // เริ่ม Timer เกม
            UpdateUI();                                               // อัปเดต UI ทั้งหมด

            gameArea.Focus();                                         // ให้ Panel รับ input จาก keyboard
        }

        // ปุ่ม Pause
        private void PauseButton_Click()
        {
            gameTimer.Stop();                                         // หยุดการอัปเดตของเกมโดยหยุด timer
            panelPause.Visible = true;                                // แสดง panel พักเกม
            btnPlay.Visible = true;                                   // แสดงปุ่ม resume
            btnReplay.Visible = false;                                // ซ่อนปุ่ม replay เพราะยังไม่ game over
            lblPause.Text = "Pause";                                   // ข้อความแสดงบนหน้าจอ
            lblPause.Location = new Point(292, 116);                  // จัดตำแหน่งข้อความ pause
        }

        // ปุ่ม Back
        private void BackButton_Click()
        {
            btnback.Visible = false;                                  // ซ่อนปุ่มย้อนกลับ
            panelShop.Visible = false;                                // ซ่อนร้านค้า
            panelMainMenu.Visible = true;                             // แสดงเมนูหลัก
        }

        // ปุ่ม Stop
        private void StopButton_Click()
        {
            panelPause.Visible = false;                               // ซ่อน pause menu
            gameArea.Visible = false;                                 // ซ่อนพื้นที่เกม
            btnPause.Visible = false;                                 // ซ่อนปุ่ม pause
            panelHighest.Visible = false;                             // ซ่อนคะแนนสูงสุด
            lblScore.Visible = false;                                 // ซ่อนคะแนนปัจจุบัน
            panelMainMenu.Visible = true;                             // กลับเมนูหลัก
            panelSaveLoad.Visible = true;                             // แสดงเมนู save/load  
        }

        // ปุ่ม Play
        private void PlayButton_Click()
        {
            gameArea.Visible = true;                                  // แสดงพื้นที่เกมอีกครั้ง
            panelPause.Visible = false;                               // ซ่อน pause menu
            gameTimer.Start();                                        // เริ่ม timer เล่นต่อ
            gameArea.Focus();                                         // รับ key input ได้อีกครั้ง
        }

        // ปุ่ม Replay
        private void ReplayButton_Click()
        {
            gameArea.Visible = true;                                  // แสดงพื้นที่เกม
            panelPause.Visible = false;                               // ซ่อน pause menu
            btnReplay.Visible = false;                                // ซ่อนปุ่ม replay (เพราะเริ่มใหม่แล้ว)
            btnPlay.Visible = false;                                  // ซ่อนปุ่ม resume ด้วย
            gameTimer.Interval = controller.Speed;                    // ตั้งความเร็วใหม่ตาม controller
            controller.ResetDeathFlag();                              // รีเซ็ตสถานะ IsDead
            gameTimer.Start();                                        // เริ่ม timer
            controller.StartNewGame(3);                               // รีเซ็ตเกม → เริ่มเลเวลใหม่
            UpdateUI();                                               // อัปเดตคะแนน/เงิน/สกิน
            this.gameArea.Focus();                                    // ตั้ง focus ให้รับ input
        }

        // ปุ่ม Exit
        private void ExitButton_Click()
        {
            this.Close();                                              // ปิดโปรแกรม
        }

        // ปุ่ม buyyellow
        private void BuyyellowButton_Click()
        {
            int money = controller.Money;                             // ดึงจำนวนเงินจาก controller
            if (skinMgr.BuySkin("Yellow", 10, ref money))             // ถ้าซื้อได้ (เงินพอ)
            {
                btnBuyYellow.Visible = false;                         // ซ่อนปุ่มซื้อ เพราะมีแล้ว
                groupYellow.Text = "Yellow : owned";                  // เปลี่ยน label เป็น owned
            }
            else MessageBox.Show("Your money is not enough!");        // เงินไม่พอ

            controller.Money = money;                                 // อัปเดตเงินกลับเข้า controller
            lblMoney.Text = $"Your Money: {money}";                   // อัปเดต UI
            btnUseYellow.Visible = true;                              // เปิดปุ่มใช้สกิน
        }

        // ปุ่ม useyellow
        private void UseyellowButton_Click()
        {
            skinMgr.UseSkin("Yellow");                                // ใช้สกิน Yellow

            btnUseGreen.Text = "use";                                 // ปรับปุ่มอื่นให้เป็นสีปกติ
            btnUseGreen.BackColor = Color.FromArgb(128, 150, 255);
            btnUseGreen.ForeColor = Color.White;

            btnUseCyan.BackColor = Color.FromArgb(128, 150, 255);
            btnUseCyan.ForeColor = Color.White;
            btnUseCyan.Text = "use";

            btnUseYellow.BackColor = Color.FromArgb(128, 150, 180);   // ปุ่มสกินที่ถูกใช้ตอนนี้
            btnUseYellow.ForeColor = Color.DarkOliveGreen;
            btnUseYellow.Text = "used";
        }

        // ปุ่ม buycyan
        private void BuycyanButton_Click()
        {
            int money = controller.Money;                              // ดึงเงินปัจจุบัน
            if (skinMgr.BuySkin("Cyan", 10, ref money))                // ซื้อ Cyan ถ้าเงินพอ
            {
                btnBuyCyan.Visible = false;                            // ซ่อนปุ่มซื้อ
                groupCyan.Text = "Cyan : owned";                       // เปลี่ยน label เป็น owned
            }
            else MessageBox.Show("Your money is not enough!");         // เงินไม่พอ

            controller.Money = money;                                  // อัปเดตเงิน
            lblMoney.Text = $"Your Money: {money}";                    // อัปเดต UI
            btnUseCyan.Visible = true;                                 // เปิดปุ่มใช้สกิน
        }

        // ปุ่ม usecyan
        private void UsecyanButton_Click()
        {
            skinMgr.UseSkin("Cyan");                                   // ใช้สกิน cyan

            btnUseGreen.Text = "use";                                  // reset สีปุ่ม
            btnUseGreen.BackColor = Color.FromArgb(128, 150, 255);
            btnUseGreen.ForeColor = Color.White;

            btnUseCyan.BackColor = Color.FromArgb(128, 150, 180);      // ปุ่ม cyan สีเด่นที่สุด → ใช้แล้ว
            btnUseCyan.ForeColor = Color.DarkOliveGreen;
            btnUseCyan.Text = "used";

            btnUseYellow.BackColor = Color.FromArgb(128, 150, 255);
            btnUseYellow.ForeColor = Color.White;
            btnUseYellow.Text = "use";
        }

        // ปุ่ม usegreen
        private void UsegreenButton_Click()
        {
            skinMgr.UseSkin("Default");                                // ใช้สกิน Default green

            btnUseGreen.Text = "used";                                 // ปุ่ม Green → used
            btnUseGreen.BackColor = Color.FromArgb(128, 150, 180);
            btnUseGreen.ForeColor = Color.DarkOliveGreen;

            btnUseCyan.BackColor = Color.FromArgb(128, 150, 255);       // รีเซ็ตปุ่มอื่น
            btnUseCyan.ForeColor = Color.White;
            btnUseCyan.Text = "use";

            btnUseYellow.BackColor = Color.FromArgb(128, 150, 255);
            btnUseYellow.ForeColor = Color.White;
            btnUseYellow.Text = "use";
        }

        // ปุ่ม Save
        private void SaveButton_Click()
        {
            var state = controller.ToGameState(skinMgr.ActiveSkin, skinMgr.GetOwnedSkins());
            // บันทึกข้อมูลเกมทั้งหมด (งู, อาหาร, คะแนน, สกิน)
            string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            // แปลงเป็น JSON อ่านง่าย (จัด format)
            File.WriteAllText(saveFile, json);                       // เขียนลงไฟล์
            MessageBox.Show("Game saved.");                          // แจ้งผู้เล่นว่าเซฟแล้ว
        }

        // ปุ่ม Load
        private void LoadButton_Click()
        {
            if (!File.Exists(saveFile))                              // ถ้าไม่มีไฟล์เซฟ
            {
                MessageBox.Show("No save file."); return;            // แจ้งเตือนและออก
            }

            string json = File.ReadAllText(saveFile);                // อ่านไฟล์ JSON
            var state = JsonSerializer.Deserialize<GameState>(json); // แปลงกลับเป็น object

            if (state == null)                                       // เฟลในการแปลง?
            {
                MessageBox.Show("Invalid save."); return;
            }

            controller.LoadFromState(state);                         // โหลดสถานะเกมกลับเข้า controller

            skinMgr = new SkinManager();                             // reset skin manager ใหม่ (กันข้อมูลซ้อน)

            foreach (var s in state.OwnedSkins.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (s == "Cyan")
                {
                    btnBuyCyan.Visible = false;                      // ใน UI ตั้งค่า owned
                    groupCyan.Text = "Cyan : owned";
                    btnUseCyan.Visible = true;
                }
                else if (s == "Yellow")
                {
                    btnBuyYellow.Visible = false;
                    groupYellow.Text = "Yellow : owned";
                    btnUseYellow.Visible = true;
                }

                skinMgr.OwnedSkins.Add(s);                           // เพิ่มสกินเข้า owned set
            }

            skinMgr.UseSkin(state.ActiveSkin);                       // ใช้สกินที่เซฟไว้

            UpdateUI();                                              // อัปเดตคะแนนและเงินบนหน้าจอ

            MessageBox.Show("Game loaded.");                         // แจ้งผู้เล่น
        }

        // UI: update labels จาก controller
        private void UpdateUI()
        {
            lblScore.Text = $"Score: {controller.Score}";             // อัปเดตคะแนน
            lblMoney.Text = $"Your Money: {controller.Money}";        // อัปเดตเงิน
            lblHighest.Text = $"Highest Score: {controller.BestScore}"; // อัปเดต highscore
        }

        // ฟังก์ชันสำหรับรับ key (panel PreviewKeyDown)
        private void gameArea_PreviewKeyDown_1(object? sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    controller.ChangeDirection("Up");                 // เปลี่ยนทิศขึ้น
                    e.IsInputKey = true;                             // มีคีย์อินพุตจริง
                    break;
                case Keys.Down:
                    controller.ChangeDirection("Down");               // เปลี่ยนลง
                    e.IsInputKey = true;
                    break;
                case Keys.Left:
                    controller.ChangeDirection("Left");               // เปลี่ยนซ้าย
                    e.IsInputKey = true;
                    break;
                case Keys.Right:
                    controller.ChangeDirection("Right");              // เปลี่ยนขวา
                    e.IsInputKey = true;
                    break;
                case Keys.Escape:
                    PauseButton_Click();                              // กด ESC → pause
                    e.IsInputKey = true;
                    break;
            }

            if (controller.IsDead)
                return;                                               // ถ้างูตายแล้ว ห้ามบังคับต่อ

            gameArea.Focus();                                         // ให้ panel รับ input ต่อเนื่อง
        }

        // เปิดหน้าร้าน (UI logic) — จะเชื่อมกับ skinMgr เพื่อซื้อ/ใช้สกิน
        private void OpenShop()
        {
            btnback.Visible = true;                                   // แสดงปุ่ม Back
            panelShop.Visible = true;                                 // เปิดร้านค้า
            panelMainMenu.Visible = false;                            // ซ่อนเมนูหลัก
            lblMoney.Visible = true;                                  // แสดงเงิน
            lblMoney.Text = $"Your money: {controller.Money}";        // ตั้งค่าข้อความ

            btnBuyCyan.Visible = !skinMgr.GetOwnedSkins().Contains("Cyan");   // ซ่อนถ้าซื้อแล้ว
            btnUseCyan.Visible = skinMgr.GetOwnedSkins().Contains("Cyan");    // โชว์ปุ่ม use

            btnBuyYellow.Visible = !skinMgr.GetOwnedSkins().Contains("Yellow");
            btnUseYellow.Visible = skinMgr.GetOwnedSkins().Contains("Yellow");
        }

        private void gameTimer_Tick(object? sender, EventArgs e)
        {
            if (!controller.IsDead)
            {
                controller.Update();                                   // ถ้ายังไม่ตาย → อัปเดตตำแหน่งงู
            }
            else
            {
                gameArea.Invalidate();                                 // ถ้าตาย → วาด particle explosion ต่อไปเรื่อย ๆ
            }
        }
    }
}

