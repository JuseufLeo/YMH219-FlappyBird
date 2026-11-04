namespace FlappyBird
{
    /// <summary>
    /// Oyuncunun kontrol ettiği kuş. Konumu, hızı ve yer çekimi etkisini kendisi yönetir.
    /// </summary>
    public class Bird
    {
        public const int Width = 34;
        public const int Height = 26;

        private const float Gravity = 0.55f;     // her karede hıza eklenen aşağı yönlü ivme
        private const float FlapPower = -8.5f;   // zıplayınca verilen yukarı yönlü hız
        private const float MaxFallSpeed = 12f;  // kuşun ulaşabileceği en yüksek düşme hızı

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Velocity { get; private set; }

        public Bird(float startX, float startY)
        {
            Reset(startX, startY);
        }

        public void Reset(float startX, float startY)
        {
            X = startX;
            Y = startY;
            Velocity = 0;
        }

        /// <summary>Kuşu yukarı zıplatır.</summary>
        public void Flap()
        {
            Velocity = FlapPower;
        }

        /// <summary>Her karede çağrılır: yer çekimini uygular ve kuşu hareket ettirir.</summary>
        public void Update()
        {
            Velocity = Math.Min(Velocity + Gravity, MaxFallSpeed);
            Y += Velocity;
        }

        /// <summary>Çarpışma kontrolü için kuşun kapladığı alan (kenarlardan biraz küçültülmüş).</summary>
        public Rectangle Bounds => new Rectangle((int)X + 3, (int)Y + 3, Width - 6, Height - 6);

        /// <summary>Çizim için kuşun tam alanı.</summary>
        public RectangleF DrawArea => new RectangleF(X, Y, Width, Height);
    }
}
