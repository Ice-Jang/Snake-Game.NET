namespace SnakeGameV2
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            lblLoad = new Label();
            lblSave = new Label();
            btnLoad = new Button();
            panelShop = new Panel();
            lblMoney = new Label();
            groupYellow = new GroupBox();
            btnUseYellow = new Button();
            pictureBox3 = new PictureBox();
            btnBuyYellow = new Button();
            groupCyan = new GroupBox();
            btnUseCyan = new Button();
            pictureBox2 = new PictureBox();
            btnBuyCyan = new Button();
            groupGreen = new GroupBox();
            btnUseGreen = new Button();
            picGreen = new PictureBox();
            btnSave = new Button();
            btnback = new Button();
            panelPause = new Panel();
            lblPause = new Label();
            btnReplay = new Button();
            btnPlay = new Button();
            btnStop = new Button();
            panelHighest = new Panel();
            pictureBox1 = new PictureBox();
            lblHighest = new Label();
            btnPause = new Button();
            lblScore = new Label();
            lblTitle = new Label();
            btnStart = new Button();
            btnExit = new Button();
            btnShop = new Button();
            gameArea = new PictureBox();
            gameTimer = new System.Windows.Forms.Timer(components);
            panelMainMenu = new Panel();
            panelSaveLoad = new Panel();
            panelShop.SuspendLayout();
            groupYellow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            groupCyan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            groupGreen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picGreen).BeginInit();
            panelPause.SuspendLayout();
            panelHighest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gameArea).BeginInit();
            panelMainMenu.SuspendLayout();
            panelSaveLoad.SuspendLayout();
            SuspendLayout();
            // 
            // lblLoad
            // 
            lblLoad.AutoSize = true;
            lblLoad.Font = new Font("Segoe UI", 20F);
            lblLoad.ForeColor = Color.White;
            lblLoad.Location = new Point(42, 54);
            lblLoad.Name = "lblLoad";
            lblLoad.Size = new Size(76, 37);
            lblLoad.TabIndex = 36;
            lblLoad.Text = "Load";
            // 
            // lblSave
            // 
            lblSave.AutoSize = true;
            lblSave.Font = new Font("Segoe UI", 20F);
            lblSave.ForeColor = Color.White;
            lblSave.Location = new Point(42, 2);
            lblSave.Name = "lblSave";
            lblSave.Size = new Size(72, 37);
            lblSave.TabIndex = 35;
            lblSave.Text = "Save";
            // 
            // btnLoad
            // 
            btnLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnLoad.BackColor = Color.FromArgb(60, 50, 255);
            btnLoad.BackgroundImage = (Image)resources.GetObject("btnLoad.BackgroundImage");
            btnLoad.BackgroundImageLayout = ImageLayout.Center;
            btnLoad.Cursor = Cursors.Hand;
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.Location = new Point(0, 54);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(36, 39);
            btnLoad.TabIndex = 34;
            btnLoad.UseVisualStyleBackColor = false;
            // 
            // panelShop
            // 
            panelShop.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelShop.AutoScroll = true;
            panelShop.AutoSize = true;
            panelShop.BorderStyle = BorderStyle.Fixed3D;
            panelShop.Controls.Add(lblMoney);
            panelShop.Controls.Add(groupYellow);
            panelShop.Controls.Add(groupCyan);
            panelShop.Controls.Add(groupGreen);
            panelShop.Location = new Point(159, 56);
            panelShop.Margin = new Padding(100, 100, 100, 300);
            panelShop.Name = "panelShop";
            panelShop.Padding = new Padding(1);
            panelShop.Size = new Size(769, 460);
            panelShop.TabIndex = 32;
            panelShop.Visible = false;
            // 
            // lblMoney
            // 
            lblMoney.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblMoney.BackColor = Color.FromArgb(128, 128, 255);
            lblMoney.FlatStyle = FlatStyle.Flat;
            lblMoney.Font = new Font("Segoe UI", 12F);
            lblMoney.ForeColor = Color.White;
            lblMoney.Location = new Point(640, 432);
            lblMoney.Name = "lblMoney";
            lblMoney.Size = new Size(127, 26);
            lblMoney.TabIndex = 18;
            lblMoney.Text = "Your money: 0";
            // 
            // groupYellow
            // 
            groupYellow.Controls.Add(btnUseYellow);
            groupYellow.Controls.Add(pictureBox3);
            groupYellow.Controls.Add(btnBuyYellow);
            groupYellow.ForeColor = Color.White;
            groupYellow.Location = new Point(329, 44);
            groupYellow.Margin = new Padding(10);
            groupYellow.Name = "groupYellow";
            groupYellow.Size = new Size(121, 124);
            groupYellow.TabIndex = 4;
            groupYellow.TabStop = false;
            groupYellow.Text = "Yellow : 10$";
            // 
            // btnUseYellow
            // 
            btnUseYellow.BackColor = Color.FromArgb(128, 150, 255);
            btnUseYellow.Cursor = Cursors.Hand;
            btnUseYellow.FlatAppearance.BorderSize = 0;
            btnUseYellow.FlatStyle = FlatStyle.Flat;
            btnUseYellow.ForeColor = Color.White;
            btnUseYellow.Location = new Point(21, 87);
            btnUseYellow.Name = "btnUseYellow";
            btnUseYellow.Size = new Size(75, 23);
            btnUseYellow.TabIndex = 2;
            btnUseYellow.Text = "use";
            btnUseYellow.UseVisualStyleBackColor = false;
            btnUseYellow.Visible = false;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.Yellow;
            pictureBox3.BackgroundImageLayout = ImageLayout.None;
            pictureBox3.Location = new Point(6, 22);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(109, 50);
            pictureBox3.TabIndex = 1;
            pictureBox3.TabStop = false;
            // 
            // btnBuyYellow
            // 
            btnBuyYellow.BackColor = Color.FromArgb(128, 150, 255);
            btnBuyYellow.Cursor = Cursors.Hand;
            btnBuyYellow.FlatAppearance.BorderSize = 0;
            btnBuyYellow.FlatStyle = FlatStyle.Flat;
            btnBuyYellow.ForeColor = Color.White;
            btnBuyYellow.Location = new Point(21, 87);
            btnBuyYellow.Name = "btnBuyYellow";
            btnBuyYellow.Size = new Size(75, 23);
            btnBuyYellow.TabIndex = 0;
            btnBuyYellow.Text = "buy";
            btnBuyYellow.UseVisualStyleBackColor = false;
            // 
            // groupCyan
            // 
            groupCyan.Controls.Add(btnUseCyan);
            groupCyan.Controls.Add(pictureBox2);
            groupCyan.Controls.Add(btnBuyCyan);
            groupCyan.ForeColor = Color.White;
            groupCyan.Location = new Point(188, 43);
            groupCyan.Margin = new Padding(10);
            groupCyan.Name = "groupCyan";
            groupCyan.Size = new Size(121, 124);
            groupCyan.TabIndex = 3;
            groupCyan.TabStop = false;
            groupCyan.Text = "Cyan : 10$";
            // 
            // btnUseCyan
            // 
            btnUseCyan.BackColor = Color.FromArgb(128, 150, 255);
            btnUseCyan.Cursor = Cursors.Hand;
            btnUseCyan.FlatAppearance.BorderSize = 0;
            btnUseCyan.FlatStyle = FlatStyle.Flat;
            btnUseCyan.ForeColor = Color.White;
            btnUseCyan.Location = new Point(21, 87);
            btnUseCyan.Name = "btnUseCyan";
            btnUseCyan.Size = new Size(75, 23);
            btnUseCyan.TabIndex = 2;
            btnUseCyan.Text = "use";
            btnUseCyan.UseVisualStyleBackColor = false;
            btnUseCyan.Visible = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackgroundImage = (Image)resources.GetObject("pictureBox2.BackgroundImage");
            pictureBox2.BackgroundImageLayout = ImageLayout.None;
            pictureBox2.Location = new Point(6, 22);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(109, 50);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // btnBuyCyan
            // 
            btnBuyCyan.BackColor = Color.FromArgb(128, 150, 255);
            btnBuyCyan.Cursor = Cursors.Hand;
            btnBuyCyan.FlatAppearance.BorderSize = 0;
            btnBuyCyan.FlatStyle = FlatStyle.Flat;
            btnBuyCyan.ForeColor = Color.White;
            btnBuyCyan.Location = new Point(21, 87);
            btnBuyCyan.Name = "btnBuyCyan";
            btnBuyCyan.Size = new Size(75, 23);
            btnBuyCyan.TabIndex = 0;
            btnBuyCyan.Text = "buy";
            btnBuyCyan.UseVisualStyleBackColor = false;
            // 
            // groupGreen
            // 
            groupGreen.Controls.Add(btnUseGreen);
            groupGreen.Controls.Add(picGreen);
            groupGreen.ForeColor = Color.White;
            groupGreen.Location = new Point(44, 43);
            groupGreen.Margin = new Padding(10);
            groupGreen.Name = "groupGreen";
            groupGreen.Size = new Size(121, 124);
            groupGreen.TabIndex = 0;
            groupGreen.TabStop = false;
            groupGreen.Text = "Green : owned";
            // 
            // btnUseGreen
            // 
            btnUseGreen.BackColor = Color.FromArgb(128, 150, 180);
            btnUseGreen.Cursor = Cursors.Hand;
            btnUseGreen.FlatAppearance.BorderSize = 0;
            btnUseGreen.FlatStyle = FlatStyle.Flat;
            btnUseGreen.ForeColor = Color.DarkOliveGreen;
            btnUseGreen.Location = new Point(21, 87);
            btnUseGreen.Name = "btnUseGreen";
            btnUseGreen.Size = new Size(75, 23);
            btnUseGreen.TabIndex = 2;
            btnUseGreen.Text = "used";
            btnUseGreen.UseVisualStyleBackColor = false;
            // 
            // picGreen
            // 
            picGreen.BackgroundImage = (Image)resources.GetObject("picGreen.BackgroundImage");
            picGreen.BackgroundImageLayout = ImageLayout.None;
            picGreen.Location = new Point(6, 22);
            picGreen.Name = "picGreen";
            picGreen.Size = new Size(109, 50);
            picGreen.TabIndex = 1;
            picGreen.TabStop = false;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSave.BackColor = Color.FromArgb(60, 50, 255);
            btnSave.BackgroundImage = (Image)resources.GetObject("btnSave.BackgroundImage");
            btnSave.BackgroundImageLayout = ImageLayout.Center;
            btnSave.Cursor = Cursors.Hand;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Location = new Point(0, 0);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(36, 39);
            btnSave.TabIndex = 33;
            btnSave.UseVisualStyleBackColor = false;
            // 
            // btnback
            // 
            btnback.BackColor = Color.FromArgb(60, 50, 255);
            btnback.BackgroundImage = (Image)resources.GetObject("btnback.BackgroundImage");
            btnback.BackgroundImageLayout = ImageLayout.Center;
            btnback.Cursor = Cursors.Hand;
            btnback.FlatAppearance.BorderSize = 0;
            btnback.FlatStyle = FlatStyle.Flat;
            btnback.Location = new Point(12, 6);
            btnback.Name = "btnback";
            btnback.Size = new Size(36, 39);
            btnback.TabIndex = 31;
            btnback.UseVisualStyleBackColor = false;
            btnback.Visible = false;
            // 
            // panelPause
            // 
            panelPause.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelPause.BorderStyle = BorderStyle.FixedSingle;
            panelPause.Controls.Add(lblPause);
            panelPause.Controls.Add(btnReplay);
            panelPause.Controls.Add(btnPlay);
            panelPause.Controls.Add(btnStop);
            panelPause.Location = new Point(159, 56);
            panelPause.Margin = new Padding(0);
            panelPause.Name = "panelPause";
            panelPause.Size = new Size(769, 460);
            panelPause.TabIndex = 29;
            panelPause.Visible = false;
            // 
            // lblPause
            // 
            lblPause.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            lblPause.AutoSize = true;
            lblPause.Font = new Font("Segoe UI", 30F);
            lblPause.ForeColor = Color.White;
            lblPause.Location = new Point(237, 133);
            lblPause.Name = "lblPause";
            lblPause.Size = new Size(231, 54);
            lblPause.TabIndex = 12;
            lblPause.Text = "Game Over!";
            // 
            // btnReplay
            // 
            btnReplay.BackColor = Color.Black;
            btnReplay.BackgroundImage = (Image)resources.GetObject("btnReplay.BackgroundImage");
            btnReplay.BackgroundImageLayout = ImageLayout.Zoom;
            btnReplay.Cursor = Cursors.Hand;
            btnReplay.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnReplay.FlatStyle = FlatStyle.Flat;
            btnReplay.ForeColor = Color.White;
            btnReplay.Location = new Point(237, 204);
            btnReplay.Name = "btnReplay";
            btnReplay.Size = new Size(101, 87);
            btnReplay.TabIndex = 11;
            btnReplay.UseVisualStyleBackColor = false;
            btnReplay.Visible = false;
            // 
            // btnPlay
            // 
            btnPlay.BackColor = Color.Black;
            btnPlay.BackgroundImage = (Image)resources.GetObject("btnPlay.BackgroundImage");
            btnPlay.BackgroundImageLayout = ImageLayout.Zoom;
            btnPlay.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.ForeColor = Color.White;
            btnPlay.Location = new Point(237, 204);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(101, 87);
            btnPlay.TabIndex = 9;
            btnPlay.UseVisualStyleBackColor = false;
            btnPlay.Visible = false;
            // 
            // btnStop
            // 
            btnStop.BackColor = Color.Black;
            btnStop.BackgroundImage = (Image)resources.GetObject("btnStop.BackgroundImage");
            btnStop.BackgroundImageLayout = ImageLayout.Zoom;
            btnStop.Cursor = Cursors.Hand;
            btnStop.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.ForeColor = Color.White;
            btnStop.Location = new Point(379, 204);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(101, 87);
            btnStop.TabIndex = 10;
            btnStop.UseVisualStyleBackColor = false;
            // 
            // panelHighest
            // 
            panelHighest.Anchor = AnchorStyles.Top;
            panelHighest.Controls.Add(pictureBox1);
            panelHighest.Controls.Add(lblHighest);
            panelHighest.Location = new Point(393, 7);
            panelHighest.Name = "panelHighest";
            panelHighest.Size = new Size(282, 31);
            panelHighest.TabIndex = 30;
            panelHighest.Visible = false;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Center;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(33, 30);
            pictureBox1.TabIndex = 13;
            pictureBox1.TabStop = false;
            // 
            // lblHighest
            // 
            lblHighest.AutoSize = true;
            lblHighest.FlatStyle = FlatStyle.Flat;
            lblHighest.Font = new Font("Segoe UI", 17F);
            lblHighest.ForeColor = Color.White;
            lblHighest.Location = new Point(39, 0);
            lblHighest.Name = "lblHighest";
            lblHighest.Size = new Size(178, 31);
            lblHighest.TabIndex = 12;
            lblHighest.Text = "Highest Score: 0";
            // 
            // btnPause
            // 
            btnPause.BackColor = Color.FromArgb(60, 50, 255);
            btnPause.BackgroundImage = (Image)resources.GetObject("btnPause.BackgroundImage");
            btnPause.BackgroundImageLayout = ImageLayout.Center;
            btnPause.FlatAppearance.BorderSize = 0;
            btnPause.FlatStyle = FlatStyle.Flat;
            btnPause.Location = new Point(54, 6);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(36, 39);
            btnPause.TabIndex = 28;
            btnPause.UseVisualStyleBackColor = false;
            btnPause.Visible = false;
            // 
            // lblScore
            // 
            lblScore.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblScore.AutoSize = true;
            lblScore.BackColor = Color.FromArgb(128, 128, 255);
            lblScore.FlatStyle = FlatStyle.Flat;
            lblScore.Font = new Font("Segoe UI", 17F);
            lblScore.ForeColor = Color.White;
            lblScore.Location = new Point(881, 6);
            lblScore.Margin = new Padding(3, 0, 50, 0);
            lblScore.Name = "lblScore";
            lblScore.Size = new Size(93, 31);
            lblScore.TabIndex = 23;
            lblScore.Text = "Score: 0";
            lblScore.Visible = false;
            // 
            // lblTitle
            // 
            lblTitle.Anchor = AnchorStyles.Top;
            lblTitle.BackColor = Color.White;
            lblTitle.Font = new Font("Segoe UI", 40F);
            lblTitle.Location = new Point(59, 27);
            lblTitle.Margin = new Padding(300);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(657, 72);
            lblTitle.TabIndex = 27;
            lblTitle.Text = "Welcome to Snake Game!!";
            // 
            // btnStart
            // 
            btnStart.Anchor = AnchorStyles.Top;
            btnStart.Cursor = Cursors.Hand;
            btnStart.Font = new Font("Segoe UI", 20F);
            btnStart.Location = new Point(289, 146);
            btnStart.Margin = new Padding(0, 40, 0, 0);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(213, 50);
            btnStart.TabIndex = 26;
            btnStart.Text = "Play";
            btnStart.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Top;
            btnExit.Cursor = Cursors.Hand;
            btnExit.Font = new Font("Segoe UI", 20F);
            btnExit.Location = new Point(289, 286);
            btnExit.Margin = new Padding(0, 40, 0, 0);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(213, 50);
            btnExit.TabIndex = 25;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            // 
            // btnShop
            // 
            btnShop.Anchor = AnchorStyles.Top;
            btnShop.Cursor = Cursors.Hand;
            btnShop.Font = new Font("Segoe UI", 20F);
            btnShop.Location = new Point(289, 216);
            btnShop.Margin = new Padding(0, 40, 0, 0);
            btnShop.Name = "btnShop";
            btnShop.Size = new Size(213, 50);
            btnShop.TabIndex = 24;
            btnShop.Text = "Shop";
            btnShop.UseVisualStyleBackColor = true;
            // 
            // gameArea
            // 
            gameArea.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gameArea.BackColor = Color.Black;
            gameArea.BorderStyle = BorderStyle.FixedSingle;
            gameArea.Location = new Point(159, 56);
            gameArea.Margin = new Padding(100);
            gameArea.Name = "gameArea";
            gameArea.Size = new Size(769, 460);
            gameArea.TabIndex = 22;
            gameArea.TabStop = false;
            gameArea.PreviewKeyDown += gameArea_PreviewKeyDown_1;
            // 
            // gameTimer
            // 
            gameTimer.Tick += gameTimer_Tick;
            // 
            // panelMainMenu
            // 
            panelMainMenu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelMainMenu.Controls.Add(lblTitle);
            panelMainMenu.Controls.Add(btnShop);
            panelMainMenu.Controls.Add(btnExit);
            panelMainMenu.Controls.Add(btnStart);
            panelMainMenu.Location = new Point(159, 56);
            panelMainMenu.Name = "panelMainMenu";
            panelMainMenu.Size = new Size(769, 460);
            panelMainMenu.TabIndex = 37;
            // 
            // panelSaveLoad
            // 
            panelSaveLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            panelSaveLoad.Controls.Add(lblSave);
            panelSaveLoad.Controls.Add(btnSave);
            panelSaveLoad.Controls.Add(lblLoad);
            panelSaveLoad.Controls.Add(btnLoad);
            panelSaveLoad.Location = new Point(12, 448);
            panelSaveLoad.Name = "panelSaveLoad";
            panelSaveLoad.Size = new Size(113, 100);
            panelSaveLoad.TabIndex = 38;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Black;
            ClientSize = new Size(1004, 587);
            Controls.Add(panelSaveLoad);
            Controls.Add(panelMainMenu);
            Controls.Add(panelShop);
            Controls.Add(btnback);
            Controls.Add(panelPause);
            Controls.Add(panelHighest);
            Controls.Add(btnPause);
            Controls.Add(lblScore);
            Controls.Add(gameArea);
            Name = "FormMain";
            Text = "FormMain";
            panelShop.ResumeLayout(false);
            groupYellow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            groupCyan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            groupGreen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picGreen).EndInit();
            panelPause.ResumeLayout(false);
            panelPause.PerformLayout();
            panelHighest.ResumeLayout(false);
            panelHighest.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)gameArea).EndInit();
            panelMainMenu.ResumeLayout(false);
            panelSaveLoad.ResumeLayout(false);
            panelSaveLoad.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblLoad;
        private Label lblSave;
        private Button btnLoad;
        private Panel panelShop;
        private GroupBox groupYellow;
        private Button btnUseYellow;
        private PictureBox pictureBox3;
        private Button btnBuyYellow;
        private Label lblMoney;
        private GroupBox groupCyan;
        private Button btnUseCyan;
        private PictureBox pictureBox2;
        private Button btnBuyCyan;
        private GroupBox groupGreen;
        private Button btnUseGreen;
        private PictureBox picGreen;
        private Button btnSave;
        private Button btnback;
        private Panel panelPause;
        private Button btnReplay;
        private Button btnPlay;
        private Button btnStop;
        private Panel panelHighest;
        private PictureBox pictureBox1;
        private Label lblHighest;
        private Button btnPause;
        private Label lblScore;
        private Label lblTitle;
        private Button btnStart;
        private Button btnExit;
        private Button btnShop;
        private PictureBox gameArea;
        private System.Windows.Forms.Timer gameTimer;
        private Panel panelMainMenu;
        private Panel panelSaveLoad;
        private Label lblPause;
    }
}
