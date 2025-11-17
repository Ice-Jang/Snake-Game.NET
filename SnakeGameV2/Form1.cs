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
            InitializeComponent();                                   // สร้าง controls (จาก Designer)
            // กำหนดค่าเริ่มต้นและเชื่อม object ต่าง ๆ
            this.DoubleBuffered = true;                              // ลด flicker
            // คำนวณ grid ขนาดโดยอิงจากพื้นที่เริ่มต้น (ใช้ cols/rows พอสมควร)
            int cols = Math.Max(20, gameArea.Width / cellSize);
            int rows = Math.Max(10, gameArea.Height / cellSize);
            controller = new SnakeGameController(cols, rows);        // สร้าง controller
            renderer = new SnakeRenderer(cellSize);                  // สร้าง renderer
            // subscribe events
            controller.GameUpdated += () => { gameArea.Invalidate(); UpdateUI(); }; // เมื่อ game updated ให้ redraw + update UI

            controller.FoodEaten += pos =>
            {
                renderer.NotifyFoodEaten(pos);
                gameArea.Invalidate();       // กระตุ้นให้วาดเฟรมแรกของเอฟเฟกต์
            };

            controller.OnEat += () =>
            {
                sound.PlayEat();
                renderer.NotifyFoodEaten(controller.Food); // effect
            };

            controller.OnDie += () =>
            {
                sound.PlayDie();
                renderer.CreateDeathExplosion(controller.Snake[0]);
            };

            // timer config
            gameTimer.Interval = controller.Speed;                                // default interval (ms)
            // connect paint event for drawing
            gameArea.Paint += gameArea_Paint;                        // ผูก event paint เข้ากับ method ของเรา
            // preview key down if using Panel's PreviewKeyDown
            gameArea.PreviewKeyDown += gameArea_PreviewKeyDown_1;      // ให้ panel ส่ง key ที่เป็นลูกศรไปยัง form
            // UI buttons handlers (ตัวอย่าง)
            btnStart.Click += (s, e) => StartButton_Click();        // start button
            btnSave.Click += (s, e) => SaveButton_Click();          // save button
            btnLoad.Click += (s, e) => LoadButton_Click();          // load button
            btnPause.Click += (s, e) => PauseButton_Click();        // pause button
            btnPlay.Click += (s, e) => PlayButton_Click();          // play button
            btnReplay.Click += (s, e) => ReplayButton_Click();      // replay button
            btnStop.Click += (s, e) => StopButton_Click();          // stop button
            btnShop.Click += (s, e) => OpenShop();                  // open shop panel
            btnback.Click += (s, e) => BackButton_Click();          // back button
            btnExit.Click += (s, e) => ExitButton_Click();          // exit button
            btnBuyCyan.Click += (s, e) => BuycyanButton_Click();    // buy button
            btnUseCyan.Click += (s, e) => UsecyanButton_Click();    // use button
            btnBuyYellow.Click += (s, e) => BuyyellowButton_Click();    // buy button
            btnUseYellow.Click += (s, e) => UseyellowButton_Click();    // use button
            btnUseGreen.Click += (s, e) => UsegreenButton_Click();    // use button
            // initialize UI
            UpdateUI();
        }

        // ฟังก์ชันวาดภาพของ gameArea (เรียกโดยระบบเมื่อ Invalidate)
        private void gameArea_Paint(object? sender, PaintEventArgs e)
        {
            // เคลียร์พื้นหลัง
            e.Graphics.Clear(Color.Black);
            // ปรับสีสกินจาก skin manager
            var colors = skinMgr.GetSkinColors(skinMgr.ActiveSkin);
            renderer.SetSkin(colors.main, colors.accent);
            // เรียก renderer วาดทั้งหมด
            renderer.Render(e.Graphics, controller);
        }

        // เมื่อ controller แจ้ง GameOver
        private void OnGameOver()
        {
            btnReplay.Visible = true;           // แสดงปุ่ม replay
            btnPlay.Visible = false;            // ซ่อนปุ่ม play
            lblPause.Text = "Game Over!";
            panelPause.Visible = true;          // แสดง panel pause (UI)
            lblPause.Location = new Point(237, 116);
            gameTimer.Stop();                   // หยุด loop
        }

        // เริ่มเกมใหม่
        private void StartButton_Click()
        {
            gameArea.Visible = true;
            panelMainMenu.Visible = false;
            panelSaveLoad.Visible = false;
            btnPause.Visible = true;
            lblScore.Visible = true;
            panelHighest.Visible = true;
            gameTimer.Interval = controller.Speed;
            controller.ResetDeathFlag();
            controller.StartNewGame(3);         // เริ่มเกมความยาวเริ่มต้น 3
            gameTimer.Start();                  // เริ่ม timer loop
            UpdateUI();                         // update labels/buttons
            gameArea.Focus();                   // ให้ panel รับ focus เพื่อกดลูกศร
            
        }

        // ปุ่ม Pause
        private void PauseButton_Click()
        {
            gameTimer.Stop();
            panelPause.Visible = true;
            btnPlay.Visible = true;
            btnReplay.Visible = false;
            lblPause.Text = "Pause";
            lblPause.Location = new Point(292, 116);
        }

        // ปุ่ม Back
        private void BackButton_Click()
        {
            btnback.Visible = false;
            panelShop.Visible = false;
            panelMainMenu.Visible = true;
        }

        // ปุ่ม Stop
        private void StopButton_Click()
        {
            panelPause.Visible = false;
            gameArea.Visible = false;
            btnPause.Visible = false;
            panelHighest.Visible = false;
            lblScore.Visible = false;
            panelMainMenu.Visible = true;
            panelSaveLoad.Visible = true;
        }

        // ปุ่ม Play
        private void PlayButton_Click()
        {
            gameArea.Visible = true;
            panelPause.Visible = false;
            gameTimer.Start();
            gameArea.Focus();
        }

        // ปุ่ม Replay
        private void ReplayButton_Click()
        {
            gameArea.Visible = true;
            panelPause.Visible = false;
            btnReplay.Visible = false;
            btnPlay.Visible = false;
            gameTimer.Interval = controller.Speed;
            controller.ResetDeathFlag();
            gameTimer.Start();
            controller.StartNewGame(3);
            UpdateUI();                         // update labels/buttons
            this.gameArea.Focus();
        }

        // ปุ่ม Exit
        private void ExitButton_Click()
        {
            this.Close();
        }

        // ปุ่ม buyyellow
        private void BuyyellowButton_Click()
        {
            int money = controller.Money;
            if (skinMgr.BuySkin("Yellow", 10, ref money))
            {
                btnBuyYellow.Visible = false;
                groupYellow.Text = "Yellow : owned";
            }
            else MessageBox.Show("Your money is not enough!");
            controller.Money = money;
            lblMoney.Text = $"Your Money: {money}";
            btnUseYellow.Visible = true;
        }

        // ปุ่ม useyellow
        private void UseyellowButton_Click()
        {
            skinMgr.UseSkin("Yellow");
            btnUseGreen.Text = "use";
            btnUseGreen.BackColor = Color.FromArgb(128, 150, 255);
            btnUseGreen.ForeColor = Color.White;
            btnUseCyan.BackColor = Color.FromArgb(128, 150, 255);
            btnUseCyan.ForeColor = Color.White;
            btnUseCyan.Text = "use";
            btnUseYellow.BackColor = Color.FromArgb(128, 150, 180);
            btnUseYellow.ForeColor = Color.DarkOliveGreen;
            btnUseYellow.Text = "used";
        }

        // ปุ่ม buycyan
        private void BuycyanButton_Click()
        {
            int money = controller.Money;
            if (skinMgr.BuySkin("Cyan", 10, ref money))
            {
                btnBuyCyan.Visible = false;
                groupCyan.Text = "Cyan : owned";
            }
            else MessageBox.Show("Your money is not enough!");
            controller.Money = money;
            lblMoney.Text = $"Your Money: {money}";
            btnUseCyan.Visible = true;
        }

        // ปุ่ม usecyan
        private void UsecyanButton_Click()
        {
            skinMgr.UseSkin("Cyan");
            btnUseGreen.Text = "use";
            btnUseGreen.BackColor = Color.FromArgb(128, 150, 255);
            btnUseGreen.ForeColor = Color.White;
            btnUseCyan.BackColor = Color.FromArgb(128, 150, 180);
            btnUseCyan.ForeColor = Color.DarkOliveGreen;
            btnUseCyan.Text = "used";
            btnUseYellow.BackColor = Color.FromArgb(128, 150, 255);
            btnUseYellow.ForeColor = Color.White;
            btnUseYellow.Text = "use";
        }

        // ปุ่ม usegreen
        private void UsegreenButton_Click()
        {
            skinMgr.UseSkin("Default");
            btnUseGreen.Text = "used";
            btnUseGreen.BackColor = Color.FromArgb(128, 150, 180);
            btnUseGreen.ForeColor = Color.DarkOliveGreen;
            btnUseCyan.BackColor = Color.FromArgb(128, 150, 255);
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
            string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(saveFile, json);
            MessageBox.Show("Game saved.");
        }

        // ปุ่ม Load
        private void LoadButton_Click()
        {
            if (!File.Exists(saveFile)) { MessageBox.Show("No save file."); return; }
            string json = File.ReadAllText(saveFile);
            var state = JsonSerializer.Deserialize<GameState>(json);
            if (state == null) { MessageBox.Show("Invalid save."); return; }
            controller.LoadFromState(state);
            skinMgr = new SkinManager(); // reset skin manager then apply owned from save
            foreach (var s in state.OwnedSkins.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (s == "Cyan")
                {
                    btnBuyCyan.Visible = false;
                    groupCyan.Text = "Cyan : owned";
                    btnUseCyan.Visible = true;
                }
                else if (s == "Yellow")
                {
                    btnBuyYellow.Visible = false;
                    groupYellow.Text = "Yellow : owned";
                    btnUseYellow.Visible = true;
                }
                skinMgr.OwnedSkins.Add(s);
            }
            skinMgr.UseSkin(state.ActiveSkin);
            UpdateUI();
            MessageBox.Show("Game loaded.");
        }

        // UI: update labels จาก controller
        private void UpdateUI()
        {
            lblScore.Text = $"Score: {controller.Score}";
            lblMoney.Text = $"Your Money: {controller.Money}";
            lblHighest.Text = $"Highest Score: {controller.BestScore}";
        }

        // ฟังก์ชันสำหรับรับ key (panel PreviewKeyDown)
        private void gameArea_PreviewKeyDown_1(object? sender, PreviewKeyDownEventArgs e)
        {
            // ทำให้ปุ่มลูกศรถูกส่งมาเป็น input key แทน action ของ control
            switch (e.KeyCode)
            {
                case Keys.Up:
                    controller.ChangeDirection("Up");
                    e.IsInputKey = true;
                    break;
                case Keys.Down:
                    controller.ChangeDirection("Down");
                    e.IsInputKey = true;
                    break;
                case Keys.Left:
                    controller.ChangeDirection("Left");
                    e.IsInputKey = true;
                    break;
                case Keys.Right:
                    controller.ChangeDirection("Right");
                    e.IsInputKey = true;
                    break;
                case Keys.Escape:
                    PauseButton_Click();
                    e.IsInputKey = true;
                    break;
            }

            if (controller.IsDead)
                return; // ❗ งูตายแล้ว ไม่รับ input
            // ให้ focus กลับไปที่ gameArea เพื่อให้ยังรับ key ต่อเนื่อง
            gameArea.Focus();
        }

        // เปิดหน้าร้าน (UI logic) — จะเชื่อมกับ skinMgr เพื่อซื้อ/ใช้สกิน
        private void OpenShop()
        {
            btnback.Visible = true; // แสดงปุ่ม back
            panelShop.Visible = true; // แสดง panel shop
            panelMainMenu.Visible = false; // ซ่อน panel Main
            lblMoney.Visible = true;
            lblMoney.Text = $"Your money: {controller.Money}";
            // แสดงสถานะปุ่มตาม owned skins
            btnBuyCyan.Visible = !skinMgr.GetOwnedSkins().Contains("Cyan");
            btnUseCyan.Visible = skinMgr.GetOwnedSkins().Contains("Cyan");
            btnBuyYellow.Visible = !skinMgr.GetOwnedSkins().Contains("Yellow");
            btnUseYellow.Visible = skinMgr.GetOwnedSkins().Contains("Yellow");
        }

        private void gameTimer_Tick(object? sender, EventArgs e)
        {
            if (!controller.IsDead)
            {
                controller.Update();  // งูยังไม่ตาย → อัปเดตตำแหน่งต่อ
            }
            else
            {
                renderer.UpdateOnlyAnimation();
                // งูตายแล้ว → รอเอฟเฟกต์ explosion หมดก่อน
                if (renderer.DeathEffectFinished)
                {
                    OnGameOver();  // method game over ของคุณ
                }
            }


        }
    }
}

