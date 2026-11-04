namespace FlappyBird
{
    /// <summary>
    /// En yüksek skoru bir metin dosyasında saklar ve okur.
    /// </summary>
    public class HighScoreStore
    {
        private readonly string filePath;

        public HighScoreStore(string fileName)
        {
            filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        }

        public int Load()
        {
            try
            {
                if (File.Exists(filePath) && int.TryParse(File.ReadAllText(filePath), out int score))
                    return score;
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            return 0;   // dosya yoksa ya da okunamıyorsa 0 kabul edilir
        }

        public void Save(int score)
        {
            try
            {
                File.WriteAllText(filePath, score.ToString());
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            // Kaydedilemezse oyun yine de devam eder; sadece rekor saklanmaz.
        }
    }
}
