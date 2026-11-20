namespace FlappyBird.Models;

/// <summary>
/// Oyunun anlık çalışma durumunu temsil eden durum listesi (State pattern / Enum).
/// </summary>
public enum GameState
{
    /// <summary>
    /// Oyun başlatılmaya hazır, kullanıcı ilk tuşa (Boşluk) basana kadar bekler.
    /// </summary>
    Ready,

    /// <summary>
    /// Kuş hareket ediyor, borular akıyor ve çarpışma kontrolleri aktif.
    /// </summary>
    Playing,

    /// <summary>
    /// Oyun geçici olarak duraklatıldı ('P' tuşu ile).
    /// </summary>
    Paused,

    /// <summary>
    /// Kuş boruya veya zemine çarptı, oyun sonlandı.
    /// </summary>
    GameOver
}
