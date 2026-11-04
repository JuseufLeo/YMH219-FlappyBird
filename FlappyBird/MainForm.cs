using System.Drawing.Drawing2D;

namespace FlappyBird
{
    /// <summary>
    /// Oyun penceresi. Klavye olaylarını Game sınıfına iletir, Timer ile oyun döngüsünü çalıştırır
    /// ve oyunun o anki durumunu ekrana çizer.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly Game game;

        // Çizimde kullanılan renkler ve fırçalar
        private readonly Brush pipeBrush = new SolidBrush(Color.FromArgb(76, 175, 80));
        private readonly Brush pipeCapBrush = new SolidBrush(Color.FromArgb(56, 142, 60));
        private readonly Brush groundBrush = new SolidBrush(Color.FromArgb(222, 184, 135));
        private readonly Brush grassBrush = new SolidBrush(Color.FromArgb(124, 179, 66));
        private readonly Brush birdBrush = new SolidBrush(Color.FromArgb(255, 213, 79));
        private readonly Brush overlayBrush = new SolidBrush(Color.FromArgb(170, 0, 0, 0));
        private readonly Font scoreFont = new Font("Segoe UI", 36, FontStyle.Bold);
        private readonly Font infoFont = new Font("Segoe UI", 10, FontStyle.Bold);
        private readonly Font titleFont = new Font("Segoe UI", 26, FontStyle.Bold);
        private readonly Font textFont = new Font("Segoe UI", 12);

        public MainForm()
        {
            InitializeComponent();
            game = new Game(ClientSize.Width, ClientSize.Height);
            gameTimer.Start();
        }

        // ---------- Olaylar ----------

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            game.Update();
            Invalidate();   // formu yeniden çizdir (OnPaint çağrılır)
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                case Keys.Up:
                case Keys.W:
                    game.Flap();
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Enter:
                case Keys.R:
                    if (game.State == GameState.GameOver)
                        game.Reset();
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void BtnNewGame_Click(object sender, EventArgs e)
        {
            game.Reset();
            ActiveControl = null;   // klavye odağı butonda kalmasın, oyun tuşları çalışsın
            Invalidate();
        }

        // ---------- Çizim ----------

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DrawBackground(g);
            DrawPipes(g);
            DrawGround(g);
            DrawBird(g);
            DrawHud(g);

            if (game.State == GameState.Ready)
                DrawMessage(g, 370, "Hazır mısın?", "Zıplamak için SPACE / ↑ tuşuna bas");
            else if (game.State == GameState.GameOver)
                DrawMessage(g, 180, "GAME OVER",
                    $"Skor: {game.Score}    En yüksek: {game.HighScore}\nYeniden başlamak için ENTER ya da Yeni Oyun");

            base.OnPaint(e);
        }

        private void DrawBackground(Graphics g)
        {
            using var sky = new LinearGradientBrush(new Rectangle(0, 0, game.Width, game.GroundY),
                Color.FromArgb(79, 195, 247), Color.FromArgb(225, 245, 254), 90f);
            g.FillRectangle(sky, 0, 0, game.Width, game.GroundY);
        }

        private void DrawPipes(Graphics g)
        {
            foreach (PipePair pipe in game.Pipes)
            {
                Rectangle top = pipe.TopRect;
                Rectangle bottom = pipe.BottomRect(game.GroundY);
                g.FillRectangle(pipeBrush, top);
                g.FillRectangle(pipeBrush, bottom);
                // Boruların ağız kısımları
                g.FillRectangle(pipeCapBrush, top.X - 4, top.Bottom - 22, top.Width + 8, 22);
                g.FillRectangle(pipeCapBrush, bottom.X - 4, bottom.Y, bottom.Width + 8, 22);
            }
        }

        private void DrawGround(Graphics g)
        {
            g.FillRectangle(groundBrush, 0, game.GroundY, game.Width, game.Height - game.GroundY);
            g.FillRectangle(grassBrush, 0, game.GroundY, game.Width, 12);
        }

        private void DrawBird(Graphics g)
        {
            RectangleF area = game.Bird.DrawArea;
            float angle = Math.Clamp(game.Bird.Velocity * 4, -30, 70);   // düşerken burnu aşağı döner

            GraphicsState saved = g.Save();
            g.TranslateTransform(area.X + area.Width / 2, area.Y + area.Height / 2);
            g.RotateTransform(angle);
            float w = area.Width, h = area.Height;

            g.FillEllipse(birdBrush, -w / 2, -h / 2, w, h);                       // gövde
            g.DrawEllipse(Pens.DarkOrange, -w / 2, -h / 2, w, h);
            g.FillEllipse(Brushes.White, w / 2 - 14, -h / 2 + 3, 10, 10);          // göz
            g.FillEllipse(Brushes.Black, w / 2 - 9, -h / 2 + 6, 4, 4);
            g.FillPolygon(Brushes.OrangeRed, new[]                                  // gaga
            {
                new PointF(w / 2 - 2, -1), new PointF(w / 2 + 9, 3), new PointF(w / 2 - 2, 7)
            });
            g.FillEllipse(Brushes.Khaki, -w / 2 + 3, 0, 14, 8);                    // kanat

            g.Restore(saved);
        }

        private void DrawHud(Graphics g)
        {
            // Skor ortada büyük
            string score = game.Score.ToString();
            SizeF size = g.MeasureString(score, scoreFont);
            float x = (game.Width - size.Width) / 2;
            g.DrawString(score, scoreFont, Brushes.Black, x + 2, 22);
            g.DrawString(score, scoreFont, Brushes.White, x, 20);

            // Durum bilgileri sol üstte
            g.DrawString($"Seviye: {game.Level}   Hız: {game.Speed:0.0}", infoFont, Brushes.Black, 10, 8);
            g.DrawString($"En yüksek: {game.HighScore}", infoFont, Brushes.Black, 10, 28);
        }

        private void DrawMessage(Graphics g, int top, string title, string text)
        {
            var box = new Rectangle(30, top, game.Width - 60, 160);
            g.FillRectangle(overlayBrush, box);

            var center = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(title, titleFont, Brushes.White, new RectangleF(box.X, box.Y + 20, box.Width, 50), center);
            g.DrawString(text, textFont, Brushes.White, new RectangleF(box.X + 10, box.Y + 80, box.Width - 20, 70), center);
        }
    }
}
