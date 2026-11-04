namespace FlappyBird
{
    /// <summary>
    /// Oyunun tüm kurallarını yönetir: kuş, engeller, skor, hız ve oyun durumu.
    /// Form bu sınıfı sadece çağırır ve ekrana çizer.
    /// </summary>
    public class Game
    {
        private const float StartSpeed = 3.5f;
        private const float SpeedStep = 0.5f;      // her seviyede eklenen hız
        private const int PointsPerLevel = 5;      // kaç puanda bir seviye atlanır
        private const int PipeCount = 3;
        private const int PipeSpacing = 220;       // iki engel arasındaki yatay mesafe
        private const int MinGapTop = 60;

        private readonly Random random = new Random();
        private readonly HighScoreStore highScoreStore = new HighScoreStore("highscore.txt");

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

        public Game(int width, int height)
        {
            Width = width;
            Height = height;
            Bird = new Bird(80, height / 2f);
            HighScore = highScoreStore.Load();
            for (int i = 0; i < PipeCount; i++)
                Pipes.Add(new PipePair(0, 0));
            Reset();
        }

        /// <summary>Oyunu baştan başlatmaya hazırlar (Yeni Oyun).</summary>
        public void Reset()
        {
            Bird.Reset(80, Height / 2f - Bird.Height);
            Score = 0;
            Speed = StartSpeed;
            for (int i = 0; i < Pipes.Count; i++)
                Pipes[i].Reset(Width + 100 + i * PipeSpacing, RandomGapTop());
            State = GameState.Ready;
        }

        /// <summary>Oyuncu zıplama tuşuna bastığında çağrılır.</summary>
        public void Flap()
        {
            if (State == GameState.GameOver)
                return;
            if (State == GameState.Ready)
                State = GameState.Playing;
            Bird.Flap();
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
                    Speed = StartSpeed + (Level - 1) * SpeedStep;
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
            int maxGapTop = GroundY - PipePair.GapHeight - 60;
            return random.Next(MinGapTop, maxGapTop);
        }
    }
}
