namespace FlappyBird
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            gameTimer = new System.Windows.Forms.Timer(components);
            btnNewGame = new Button();
            SuspendLayout();
            //
            // gameTimer
            //
            gameTimer.Interval = 16;
            gameTimer.Tick += GameTimer_Tick;
            //
            // btnNewGame
            //
            btnNewGame.BackColor = Color.FromArgb(255, 193, 7);
            btnNewGame.FlatStyle = FlatStyle.Flat;
            btnNewGame.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnNewGame.Location = new Point(150, 588);
            btnNewGame.Name = "btnNewGame";
            btnNewGame.Size = new Size(120, 34);
            btnNewGame.TabStop = false;
            btnNewGame.Text = "Yeni Oyun";
            btnNewGame.UseVisualStyleBackColor = false;
            btnNewGame.Click += BtnNewGame_Click;
            //
            // MainForm
            //
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(420, 640);
            Controls.Add(btnNewGame);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Flappy Bird - YMH219";
            KeyDown += MainForm_KeyDown;
            ResumeLayout(false);
        }

        private System.Windows.Forms.Timer gameTimer;
        private Button btnNewGame;
    }
}
