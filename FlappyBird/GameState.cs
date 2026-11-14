namespace FlappyBird
{
    /// <summary>Oyunun içinde bulunabileceği durumlar.</summary>
    public enum GameState
    {
        Ready,      // oyun başlamayı bekliyor
        Playing,    // oyun oynanıyor
        Paused,     // oyun duraklatıldı (P / ESC)
        GameOver    // kuş çarptı, oyun bitti
    }
}
