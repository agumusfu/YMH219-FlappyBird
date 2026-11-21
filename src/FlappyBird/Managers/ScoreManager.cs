using System.Text.Json;

namespace FlappyBird.Managers;

/// <summary>
/// Yüksek skor verilerini JSON formatında yerel diskte saklayan model.
/// </summary>
public class ScoreData
{
    public int HighScore { get; set; }
    public string BestDifficulty { get; set; } = "Normal";
    public DateTime LastAchievedDate { get; set; } = DateTime.Now;
}

/// <summary>
/// Oyun skorunu ve rekor skoru yöneten, dosya giriş/çıkış (I/O) işlemlerini
/// güvenli şekilde gerçekleştiren yönetici sınıfı (Single Responsibility Principle).
/// </summary>
public class ScoreManager
{
    private readonly string _filePath;
    public int CurrentScore { get; private set; }
    public int HighScore { get; private set; }
    public bool IsNewHighScore { get; private set; }

    public ScoreManager()
    {
        // Dosyayı kullanıcıya ait yerel uygulama dizininde sakla
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string appFolder = Path.Combine(appData, "YMH219_FlappyBird");
        
        try
        {
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            _filePath = Path.Combine(appFolder, "highscore.json");
        }
        catch
        {
            // İzin hatası olursa mevcut çalışma klasörüne yedekle
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "highscore.json");
        }

        LoadHighScore();
    }

    /// <summary>
    /// Skoru 1 artırır ve gerekiyorsa rekoru günceller.
    /// </summary>
    public void IncrementScore(string currentDifficulty)
    {
        CurrentScore++;
        if (CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            IsNewHighScore = true;
            SaveHighScore(currentDifficulty);
        }
    }

    /// <summary>
    /// Yeni oyun başladığında anlık skoru sıfırlar.
    /// </summary>
    public void Reset()
    {
        CurrentScore = 0;
        IsNewHighScore = false;
    }

    /// <summary>
    /// Diskten kayıtlı rekor skoru okur.
    /// </summary>
    private void LoadHighScore()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                var data = JsonSerializer.Deserialize<ScoreData>(json);
                if (data != null)
                {
                    HighScore = data.HighScore;
                    return;
                }
            }
        }
        catch
        {
            // Dosya bozuk veya okunamıyorsa sıfırdan başla
            HighScore = 0;
        }
    }

    /// <summary>
    /// Yeni rekoru diske JSON formatında yazar.
    /// </summary>
    private void SaveHighScore(string difficulty)
    {
        try
        {
            var data = new ScoreData
            {
                HighScore = HighScore,
                BestDifficulty = difficulty,
                LastAchievedDate = DateTime.Now
            };

            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch
        {
            // Disk yazma hatası oyunu durdurmamalıdır
        }
    }
}
