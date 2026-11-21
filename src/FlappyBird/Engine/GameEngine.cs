using System.Drawing;
using System.Drawing.Drawing2D;
using FlappyBird.Managers;
using FlappyBird.Models;

namespace FlappyBird.Engine;

/// <summary>
/// Oyun motoru: Fizik simülasyonunu, çarpışma algoritmalarını,
/// boruların döngüsel yönetimini ve durum geçişlerini koordine eder.
/// </summary>
public class GameEngine : IDisposable
{
    public Bird Bird { get; }
    public List<PipePair> Pipes { get; } = new();
    public GameState State { get; private set; } = GameState.Ready;
    public DifficultyConfig Difficulty { get; private set; }
    public ScoreManager ScoreManager { get; }
    public SoundManager SoundManager { get; }

    public float CanvasWidth { get; private set; }
    public float CanvasHeight { get; private set; }
    public float GroundHeight { get; } = 80f;
    public float GroundY => CanvasHeight - GroundHeight;

    private float _groundScrollX;
    private const int PipeCount = 3; // Föy gereksinimi: En az 2 engel (3 boru ile ideal akış sağlanır)

    // Olay bildirimleri (Events)
    public event Action<int>? ScoreUpdated;
    public event Action? GameOverTriggered;
    public event Action<GameState>? StateChanged;

    public GameEngine(float width, float height)
    {
        CanvasWidth = width;
        CanvasHeight = height;
        Difficulty = new DifficultyConfig(DifficultyLevel.Normal);
        ScoreManager = new ScoreManager();
        SoundManager = new SoundManager();

        Bird = new Bird(80, GroundY / 2f, Difficulty.Gravity, Difficulty.JumpStrength);
        InitializePipes();
    }

    public void Resize(float newWidth, float newHeight)
    {
        CanvasWidth = newWidth;
        CanvasHeight = newHeight;
        if (State == GameState.Ready)
        {
            Reset();
        }
    }

    public void SetDifficulty(DifficultyLevel level)
    {
        Difficulty = new DifficultyConfig(level);
        Bird.Gravity = Difficulty.Gravity;
        Bird.JumpStrength = Difficulty.JumpStrength;
        Reset();
    }

    private void InitializePipes()
    {
        Pipes.Clear();
        float startX = CanvasWidth + 80f;

        for (int i = 0; i < PipeCount; i++)
        {
            var pipe = new PipePair(startX + (i * Difficulty.PipeSpawnDistance), GroundY, Difficulty.PipeGap);
            Pipes.Add(pipe);
        }
    }

    public void Flap()
    {
        if (State == GameState.Ready)
        {
            State = GameState.Playing;
            StateChanged?.Invoke(State);
            Bird.Jump();
            SoundManager.PlayJump();
        }
        else if (State == GameState.Playing)
        {
            Bird.Jump();
            SoundManager.PlayJump();
        }
        else if (State == GameState.GameOver)
        {
            Reset();
        }
    }

    public void TogglePause()
    {
        if (State == GameState.Playing)
        {
            State = GameState.Paused;
            StateChanged?.Invoke(State);
        }
        else if (State == GameState.Paused)
        {
            State = GameState.Playing;
            StateChanged?.Invoke(State);
        }
    }

    public void Reset()
    {
        State = GameState.Ready;
        Bird.Reset(new PointF(80, GroundY / 2f));
        ScoreManager.Reset();
        InitializePipes();
        _groundScrollX = 0;
        StateChanged?.Invoke(State);
        ScoreUpdated?.Invoke(ScoreManager.CurrentScore);
    }

    /// <summary>
    /// Her timer döngüsünde (frame) çağrılan ana güncelleme metodu.
    /// </summary>
    public void Update()
    {
        if (State != GameState.Playing)
            return;

        // 1. Kuş fiziğini güncelle
        Bird.Update(GroundY);

        // 2. Kuşun zemine çarpma kontrolü
        if (Bird.Position.Y + Bird.Height >= GroundY)
        {
            TriggerGameOver();
            return;
        }

        // 3. Zemin akış animasyonu
        float currentSpeed = Difficulty.PipeSpeed + (ScoreManager.CurrentScore * 0.05f); // İsteğe bağlı: Seviye ilerledikçe hafif hız artışı
        _groundScrollX = (_groundScrollX + currentSpeed) % 24f;

        // 4. Boruların hareketi ve çarpışma kontrolü
        float maxPipeX = Pipes.Max(p => p.X);

        foreach (var pipe in Pipes)
        {
            pipe.Update(currentSpeed);

            // Çarpışma kontrolü
            if (pipe.CheckCollision(Bird.Bounds, GroundY))
            {
                TriggerGameOver();
                return;
            }

            // Skor kontrolü: Kuş borunun sağını geçtiğinde
            if (!pipe.Passed && pipe.X + pipe.Width < Bird.Position.X)
            {
                pipe.Passed = true;
                ScoreManager.IncrementScore(Difficulty.Level.ToString());
                SoundManager.PlayScore();
                ScoreUpdated?.Invoke(ScoreManager.CurrentScore);
            }

            // Ekrandan çıkan boruyu sıranın sonuna taşı
            if (pipe.X + pipe.Width < -10f)
            {
                float newSpawnX = Math.Max(CanvasWidth, maxPipeX + Difficulty.PipeSpawnDistance);
                pipe.Reset(newSpawnX, GroundY, Difficulty.PipeGap);
                maxPipeX = newSpawnX;
            }
        }
    }

    private void TriggerGameOver()
    {
        State = GameState.GameOver;
        SoundManager.PlayHit();
        StateChanged?.Invoke(State);
        GameOverTriggered?.Invoke();
    }

    /// <summary>
    /// Tüm oyun sahnesini arka plan, borular, zemin, kuş ve kullanıcı arayüzü ile çizer.
    /// </summary>
    public void Draw(Graphics g)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // 1. Gökyüzü arka planı (Gradient)
        using (var skyBrush = new LinearGradientBrush(
            new RectangleF(0, 0, CanvasWidth, GroundY),
            Color.FromArgb(112, 197, 206), // Flappy Blue
            Color.FromArgb(198, 238, 243),
            LinearGradientMode.Vertical))
        {
            g.FillRectangle(skyBrush, 0, 0, CanvasWidth, GroundY);
        }

        // Arka plan dekoratif bulutlar
        DrawBackgroundClouds(g);

        // 2. Borular
        foreach (var pipe in Pipes)
        {
            pipe.Draw(g, GroundY);
        }

        // 3. Zemin (Kaldırım ve toprak)
        DrawGround(g);

        // 4. Kuş
        Bird.Draw(g);

        // 5. Durum Bildirimleri ve HUD
        DrawHud(g);
    }

    private void DrawBackgroundClouds(Graphics g)
    {
        using var cloudBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 255));
        // Statik estetik bulutlar
        g.FillEllipse(cloudBrush, 40, GroundY - 140, 70, 35);
        g.FillEllipse(cloudBrush, 80, GroundY - 155, 90, 45);
        g.FillEllipse(cloudBrush, 140, GroundY - 140, 60, 30);

        g.FillEllipse(cloudBrush, CanvasWidth - 180, GroundY - 160, 80, 40);
        g.FillEllipse(cloudBrush, CanvasWidth - 130, GroundY - 175, 100, 50);
        g.FillEllipse(cloudBrush, CanvasWidth - 60, GroundY - 155, 70, 35);

        // Şehir silüeti (Zeminin hemen üstü)
        using var cityBrush = new SolidBrush(Color.FromArgb(120, 215, 240, 215));
        for (int i = 0; i < CanvasWidth; i += 40)
        {
            float buildingH = 25 + (i % 3) * 15;
            g.FillRectangle(cityBrush, i, GroundY - buildingH, 35, buildingH);
        }
    }

    private void DrawGround(Graphics g)
    {
        // Çimen şeridi (Yeşil)
        using (var grassBrush = new SolidBrush(Color.FromArgb(115, 190, 45)))
        using (var grassBorderPen = new Pen(Color.FromArgb(70, 130, 25), 3f))
        {
            g.FillRectangle(grassBrush, 0, GroundY, CanvasWidth, 14);
            g.DrawLine(grassBorderPen, 0, GroundY, CanvasWidth, GroundY);
        }

        // Toprak katmanı (Sarı-kahve)
        using (var groundBrush = new SolidBrush(Color.FromArgb(222, 216, 149)))
        using (var stripeBrush = new SolidBrush(Color.FromArgb(206, 198, 126)))
        {
            g.FillRectangle(groundBrush, 0, GroundY + 14, CanvasWidth, GroundHeight - 14);

            // Hareketli şerit çizgileri
            for (float x = -_groundScrollX - 24; x < CanvasWidth + 24; x += 18)
            {
                PointF[] stripePoints =
                {
                    new(x, GroundY + 14),
                    new(x + 10, GroundY + 14),
                    new(x + 2, GroundY + GroundHeight),
                    new(x - 8, GroundY + GroundHeight)
                };
                g.FillPolygon(stripeBrush, stripePoints);
            }
        }
    }

    private void DrawHud(Graphics g)
    {
        // Canlı skor gösterimi (Oyun esnasında)
        if (State == GameState.Playing)
        {
            string scoreText = ScoreManager.CurrentScore.ToString();
            using var font = new Font("Impact", 36, FontStyle.Bold);
            var size = g.MeasureString(scoreText, font);
            float scoreX = (CanvasWidth - size.Width) / 2f;
            float scoreY = 30f;

            // Yazı gölgesi / Dış hat
            using var outlineBrush = new SolidBrush(Color.FromArgb(50, 40, 20));
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    g.DrawString(scoreText, font, outlineBrush, scoreX + dx, scoreY + dy);
                }
            }
            g.DrawString(scoreText, font, Brushes.White, scoreX, scoreY);
        }
        else if (State == GameState.Ready)
        {
            DrawOverlayMessage(g, "BAŞLAMAK İÇİN BOŞLUK TUŞUNA BASIN", "Zıplamak için: Boşluk / Tıklama\nDuraklatmak için: P tuşu");
        }
        else if (State == GameState.Paused)
        {
            DrawOverlayMessage(g, "OYUN DURAKLATILDI", "Devam etmek için 'P' tuşuna veya Boşluk'a basın");
        }
        else if (State == GameState.GameOver)
        {
            DrawGameOverCard(g);
        }
    }

    private void DrawOverlayMessage(Graphics g, string title, string subtitle)
    {
        using var boxBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
        using var titleFont = new Font("Arial", 16, FontStyle.Bold);
        using var subFont = new Font("Arial", 11, FontStyle.Regular);

        var titleSize = g.MeasureString(title, titleFont);
        var subSize = g.MeasureString(subtitle, subFont);

        float boxW = Math.Max(titleSize.Width, subSize.Width) + 40f;
        float boxH = titleSize.Height + subSize.Height + 35f;
        float boxX = (CanvasWidth - boxW) / 2f;
        float boxY = (GroundY - boxH) / 2f;

        g.FillRectangle(boxBrush, boxX, boxY, boxW, boxH);
        using var borderPen = new Pen(Color.FromArgb(245, 200, 50), 2f);
        g.DrawRectangle(borderPen, boxX, boxY, boxW, boxH);

        g.DrawString(title, titleFont, Brushes.Gold, (CanvasWidth - titleSize.Width) / 2f, boxY + 12);
        g.DrawString(subtitle, subFont, Brushes.White, (CanvasWidth - subSize.Width) / 2f, boxY + 18 + titleSize.Height);
    }

    private void DrawGameOverCard(Graphics g)
    {
        float cardW = 280f;
        float cardH = 175f;
        float cardX = (CanvasWidth - cardW) / 2f;
        float cardY = (GroundY - cardH) / 2f - 10f;

        // Kart Arka Planı (Panel)
        using (var cardBrush = new SolidBrush(Color.FromArgb(222, 216, 149)))
        using (var borderPen = new Pen(Color.FromArgb(85, 60, 20), 4f))
        {
            g.FillRectangle(cardBrush, cardX, cardY, cardW, cardH);
            g.DrawRectangle(borderPen, cardX, cardY, cardW, cardH);
        }

        // GAME OVER Başlığı
        using (var goFont = new Font("Impact", 24, FontStyle.Bold))
        {
            string goText = "GAME OVER";
            var goSize = g.MeasureString(goText, goFont);
            float goX = (CanvasWidth - goSize.Width) / 2f;
            float goY = cardY - 45f;

            // Dış çizgi
            using var outBrush = new SolidBrush(Color.FromArgb(80, 30, 20));
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    g.DrawString(goText, goFont, outBrush, goX + dx, goY + dy);
                }
            }
            g.DrawString(goText, goFont, new SolidBrush(Color.FromArgb(235, 75, 45)), goX, goY);
        }

        // Skor Tablosu
        using (var labelFont = new Font("Arial", 11, FontStyle.Bold))
        using (var valFont = new Font("Impact", 18, FontStyle.Regular))
        using (var textBrush = new SolidBrush(Color.FromArgb(85, 60, 20)))
        {
            // Skor
            g.DrawString("SKOR", labelFont, textBrush, cardX + 30, cardY + 25);
            g.DrawString(ScoreManager.CurrentScore.ToString(), valFont, textBrush, cardX + 30, cardY + 45);

            // En Yüksek Skor
            g.DrawString("EN YÜKSEK", labelFont, textBrush, cardX + 160, cardY + 25);
            g.DrawString(ScoreManager.HighScore.ToString(), valFont, textBrush, cardX + 160, cardY + 45);

            if (ScoreManager.IsNewHighScore)
            {
                using var badgeBrush = new SolidBrush(Color.FromArgb(220, 50, 40));
                g.FillRectangle(badgeBrush, cardX + 155, cardY + 10, 80, 18);
                using var badgeFont = new Font("Arial", 8, FontStyle.Bold);
                g.DrawString("YENİ REKOR!", badgeFont, Brushes.White, cardX + 160, cardY + 12);
            }

            // Yeniden Başlat Yönergesi
            using var hintFont = new Font("Arial", 10, FontStyle.Italic);
            string hint = "Yeniden başlamak için BOŞLUK veya R'ye basın";
            var hintSize = g.MeasureString(hint, hintFont);
            g.DrawString(hint, hintFont, Brushes.Black, (CanvasWidth - hintSize.Width) / 2f, cardY + 135);
        }
    }

    public void Dispose()
    {
        SoundManager.Dispose();
    }
}
