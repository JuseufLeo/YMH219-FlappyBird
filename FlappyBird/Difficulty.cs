namespace FlappyBird
{
    /// <summary>
    /// Bir zorluk seviyesinin ayarları: başlangıç hızı, seviye başına hız artışı ve borular arasındaki boşluk.
    /// Her zorluğun en yüksek skoru ayrı dosyada tutulur.
    /// </summary>
    public class Difficulty
    {
        public string Name { get; }
        public float StartSpeed { get; }
        public float SpeedStep { get; }
        public int GapHeight { get; }
        public string HighScoreFile { get; }

        private Difficulty(string name, float startSpeed, float speedStep, int gapHeight, string highScoreFile)
        {
            Name = name;
            StartSpeed = startSpeed;
            SpeedStep = speedStep;
            GapHeight = gapHeight;
            HighScoreFile = highScoreFile;
        }

        public static readonly Difficulty Easy = new Difficulty("Kolay", 2.8f, 0.3f, 190, "highscore_kolay.txt");
        public static readonly Difficulty Normal = new Difficulty("Orta", 3.5f, 0.5f, 160, "highscore.txt");   // eski rekor dosyası korunur
        public static readonly Difficulty Hard = new Difficulty("Zor", 4.5f, 0.6f, 130, "highscore_zor.txt");

        /// <summary>1, 2, 3 tuşlarının sırasıyla seçtiği zorluklar.</summary>
        public static readonly Difficulty[] All = { Easy, Normal, Hard };

        public override string ToString() => Name;
    }
}
