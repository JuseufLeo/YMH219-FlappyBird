namespace FlappyBird
{
    /// <summary>
    /// Ortasında boşluk olan bir çift boru (üst ve alt engel). Sağdan sola hareket eder.
    /// </summary>
    public class PipePair
    {
        public const int Width = 64;
        public const int GapHeight = 160;   // kuşun geçeceği boşluğun yüksekliği

        public float X { get; private set; }
        public int GapTop { get; private set; }   // boşluğun üst kenarının Y değeri
        public bool Passed { get; set; }           // kuş bu engeli geçti mi (skor bir kez artsın diye)

        public PipePair(float x, int gapTop)
        {
            Reset(x, gapTop);
        }

        /// <summary>Engeli yeni bir konuma ve yeni bir boşluk yüksekliğine taşır.</summary>
        public void Reset(float x, int gapTop)
        {
            X = x;
            GapTop = gapTop;
            Passed = false;
        }

        public void Move(float speed)
        {
            X -= speed;
        }

        public bool IsOffScreen => X + Width < 0;

        public Rectangle TopRect => new Rectangle((int)X, 0, Width, GapTop);

        public Rectangle BottomRect(int groundY) =>
            new Rectangle((int)X, GapTop + GapHeight, Width, groundY - (GapTop + GapHeight));

        public bool CollidesWith(Rectangle bird, int groundY) =>
            bird.IntersectsWith(TopRect) || bird.IntersectsWith(BottomRect(groundY));
    }
}
