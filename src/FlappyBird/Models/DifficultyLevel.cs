namespace FlappyBird.Models;

/// <summary>
/// İsteğe bağlı geliştirme: Farklı zorluk seviyeleri.
/// </summary>
public enum DifficultyLevel
{
    Easy,
    Normal,
    Hard
}

/// <summary>
/// Seçilen zorluk seviyesine göre fizik ve engel parametrelerini belirleyen sınıf.
/// </summary>
public class DifficultyConfig
{
    public DifficultyLevel Level { get; }
    public float PipeSpeed { get; }
    public float PipeGap { get; }
    public float Gravity { get; }
    public float JumpStrength { get; }
    public int PipeSpawnDistance { get; }

    public DifficultyConfig(DifficultyLevel level)
    {
        Level = level;
        switch (level)
        {
            case DifficultyLevel.Easy:
                PipeSpeed = 3.2f;
                PipeGap = 170f;
                Gravity = 0.46f;
                JumpStrength = 8.0f;
                PipeSpawnDistance = 260;
                break;
            case DifficultyLevel.Hard:
                PipeSpeed = 5.6f;
                PipeGap = 125f;
                Gravity = 0.58f;
                JumpStrength = 9.2f;
                PipeSpawnDistance = 210;
                break;
            case DifficultyLevel.Normal:
            default:
                PipeSpeed = 4.2f;
                PipeGap = 145f;
                Gravity = 0.52f;
                JumpStrength = 8.6f;
                PipeSpawnDistance = 230;
                break;
        }
    }
}
