namespace FlappyBird
{
    /// <summary>
    /// Oyunun tüm kurallarını yönetir: kuş, engeller, skor, hız ve oyun durumu.
    /// Form bu sınıfı sadece çağırır ve ekrana çizer.
    /// </summary>
    public class Game
    {
        private const int PointsPerLevel = 5;      // kaç puanda bir seviye atlanır
        private const int PipeCount = 3;
        private const int PipeSpacing = 220;       // iki engel arasındaki yatay mesafe
        private const int MinGapTop = 60;

        private readonly Random random = new Random();
        private HighScoreStore highScoreStore;

        public int Width { get; }
        public int Height { get; }
        public int GroundY => Height - 70;   // zeminin başladığı Y değeri

        public Bird Bird { get; }
        public List<PipePair> Pipes { get; } = new List<PipePair>();

        public GameState State { get; private set; }
        public int Score { get; private set; }
        public int HighScore { get; private set; }
        public float Speed { get; private set; }
        public int Level => Score / PointsPerLevel + 1;
        /// <summary>Seçili zorluk (varsayılan: Orta).</summary>
        public Difficulty Difficulty { get; private set; } = Difficulty.Normal;

        public Game(int width, int height)
        {
            Width = width;
            Height = height;
            Bird = new Bird(80, height / 2f);
            for (int i = 0; i < PipeCount; i++)
                Pipes.Add(new PipePair(0, 0));
            SetDifficulty(Difficulty.Normal);
        }

        /// <summary>
        /// Zorluğu değiştirir; sadece oyun başlamadan önce veya oyun bittikten sonra.
        /// Her zorluğun en yüksek skoru ayrı dosyadan okunur.
        /// </summary>
        public bool SetDifficulty(Difficulty difficulty)
        {
            if (State == GameState.Playing || State == GameState.Paused)
                return false;
            Difficulty = difficulty;
            highScoreStore = new HighScoreStore(difficulty.HighScoreFile);
            HighScore = highScoreStore.Load();
            Reset();
            return true;
        }

        /// <summary>Oyunu baştan başlatmaya hazırlar (Yeni Oyun).</summary>
        public void Reset()
        {
            Bird.Reset(80, Height / 2f - Bird.Height);
            Score = 0;
            Speed = Difficulty.StartSpeed;
            for (int i = 0; i < Pipes.Count; i++)
            {
                Pipes[i].GapHeight = Difficulty.GapHeight;
                Pipes[i].Reset(Width + 100 + i * PipeSpacing, RandomGapTop());
            }
            State = GameState.Ready;
        }

        /// <summary>Oyuncu zıplama tuşuna bastığında çağrılır.</summary>
        public void Flap()
        {
            if (State == GameState.GameOver || State == GameState.Paused)
                return;
            if (State == GameState.Ready)
                State = GameState.Playing;
            Bird.Flap();
        }

        /// <summary>
        /// Oyunu duraklatır ya da devam ettirir (P / ESC tuşu).
        /// Sadece oyun oynanırken veya duraklatılmışken çalışır.
        /// </summary>
        public void TogglePause()
        {
            if (State == GameState.Playing)
                State = GameState.Paused;
            else if (State == GameState.Paused)
                State = GameState.Playing;
        }

        /// <summary>Pencere arka plana geçince oyunu otomatik duraklatır.</summary>
        public void Pause()
        {
            if (State == GameState.Playing)
                State = GameState.Paused;
        }

        /// <summary>Oyun döngüsünün bir adımı. Timer her tetiklendiğinde çağrılır.</summary>
        public void Update()
        {
            if (State != GameState.Playing)
                return;

            Bird.Update();

            foreach (PipePair pipe in Pipes)
            {
                pipe.Move(Speed);

                // Ekrandan çıkan engeli en sondaki engelin arkasına taşı
                if (pipe.IsOffScreen)
                    pipe.Reset(RightmostPipeX() + PipeSpacing, RandomGapTop());

                // Kuş engelin arkasına geçtiyse skor artır
                if (!pipe.Passed && pipe.X + PipePair.Width < Bird.X)
                {
                    pipe.Passed = true;
                    Score++;
                    Speed = Difficulty.StartSpeed + (Level - 1) * Difficulty.SpeedStep;
                }

                if (pipe.CollidesWith(Bird.Bounds, GroundY))
                {
                    EndGame();
                    return;
                }
            }

            // Zemine veya tavana çarpma
            if (Bird.Y + Bird.Height >= GroundY || Bird.Y < 0)
                EndGame();
        }

        private void EndGame()
        {
            State = GameState.GameOver;
            if (Score > HighScore)
            {
                HighScore = Score;
                highScoreStore.Save(HighScore);
            }
        }

        private float RightmostPipeX()
        {
            float max = 0;
            foreach (PipePair p in Pipes)
                if (p.X > max) max = p.X;
            return max;
        }

        private int RandomGapTop()
        {
            int maxGapTop = GroundY - Difficulty.GapHeight - 60;
            return random.Next(MinGapTop, maxGapTop);
        }
    }
}
