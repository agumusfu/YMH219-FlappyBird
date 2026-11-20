using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlappyBird.Models;

/// <summary>
/// Yukarı ve aşağı çift boruyu (engeli) temsil eden sınıf.
/// Konum, boşluk mesafesi, çarpışma alanı hesaplama ve çizim işlevlerini yönetir.
/// </summary>
public class PipePair
{
    private static readonly Random RandomGenerator = new();

    public float X { get; set; }
    public float GapY { get; set; }
    public float GapHeight { get; set; }
    public float Width { get; } = 64f;
    public bool Passed { get; set; }

    public RectangleF TopPipeBounds => new(X, 0, Width, GapY);
    public RectangleF BottomPipeBounds(float groundY) => new(X, GapY + GapHeight, Width, Math.Max(0, groundY - (GapY + GapHeight)));

    public PipePair(float startX, float canvasHeight, float gapHeight = 145f)
    {
        GapHeight = gapHeight;
        Reset(startX, canvasHeight, gapHeight);
    }

    /// <summary>
    /// Boruyu sola doğru belirtilen hızda kaydırır.
    /// </summary>
    public void Update(float speed)
    {
        X -= speed;
    }

    /// <summary>
    /// Boru ekranın solundan tamamen çıktığında sağ tarafa yeni rastgele boşluk yüksekliğiyle taşınır.
    /// </summary>
    public void Reset(float newX, float canvasHeight, float gapHeight)
    {
        X = newX;
        GapHeight = gapHeight;
        Passed = false;

        // Üst ve alt boru için minimum 60px bırakacak şekilde boşluk konumunu belirle
        float minGapY = 60f;
        float maxGapY = Math.Max(minGapY + 20f, canvasHeight - gapHeight - 100f);

        GapY = (float)(minGapY + RandomGenerator.NextDouble() * (maxGapY - minGapY));
    }

    /// <summary>
    /// Kuşun borulardan birine çarpıp çarpmadığını kontrol eder.
    /// </summary>
    public bool CheckCollision(RectangleF birdBounds, float groundY)
    {
        if (TopPipeBounds.IntersectsWith(birdBounds))
            return true;

        if (BottomPipeBounds(groundY).IntersectsWith(birdBounds))
            return true;

        return false;
    }

    /// <summary>
    /// Klasik yeşil boruları, boru başlıklarını ve derinlik gölgelerini GDI+ ile çizer.
    /// </summary>
    public void Draw(Graphics g, float groundY)
    {
        const float capHeight = 26f;
        const float capOverhang = 5f; // Başlığın yanlardan taşma miktarı

        using var pipeBrush = new LinearGradientBrush(
            new RectangleF(X, 0, Width, 10),
            Color.FromArgb(115, 191, 46),
            Color.FromArgb(83, 142, 33),
            LinearGradientMode.Horizontal);

        using var highlightBrush = new SolidBrush(Color.FromArgb(170, 225, 100));
        using var shadowBrush = new SolidBrush(Color.FromArgb(55, 95, 22));
        using var borderPen = new Pen(Color.FromArgb(40, 65, 15), 2.5f);

        // ===== 1. ÜST BORU =====
        float topPipeHeight = GapY;
        if (topPipeHeight > 0)
        {
            // Boru gövdesi
            var topBodyRect = new RectangleF(X, 0, Width, Math.Max(0, topPipeHeight - capHeight));
            g.FillRectangle(pipeBrush, topBodyRect);
            g.DrawRectangle(borderPen, topBodyRect.X, topBodyRect.Y - 2, topBodyRect.Width, topBodyRect.Height + 2);

            // Gövde parıltı çizgisi
            g.FillRectangle(highlightBrush, X + 8, 0, 5, topBodyRect.Height);
            // Gövde gölgesi
            g.FillRectangle(shadowBrush, X + Width - 10, 0, 6, topBodyRect.Height);

            // Boru başlığı (Ağız kısmı)
            var topCapRect = new RectangleF(X - capOverhang, topPipeHeight - capHeight, Width + (capOverhang * 2), capHeight);
            g.FillRectangle(pipeBrush, topCapRect);
            g.DrawRectangle(borderPen, topCapRect.X, topCapRect.Y, topCapRect.Width, topCapRect.Height);
            g.FillRectangle(highlightBrush, topCapRect.X + 8, topCapRect.Y, 5, topCapRect.Height);
            g.FillRectangle(shadowBrush, topCapRect.Right - 10, topCapRect.Y, 6, topCapRect.Height);
        }

        // ===== 2. ALT BORU =====
        float bottomPipeY = GapY + GapHeight;
        float bottomPipeHeight = Math.Max(0, groundY - bottomPipeY);

        if (bottomPipeHeight > 0)
        {
            // Boru başlığı (Ağız kısmı)
            var bottomCapRect = new RectangleF(X - capOverhang, bottomPipeY, Width + (capOverhang * 2), capHeight);
            g.FillRectangle(pipeBrush, bottomCapRect);
            g.DrawRectangle(borderPen, bottomCapRect.X, bottomCapRect.Y, bottomCapRect.Width, bottomCapRect.Height);
            g.FillRectangle(highlightBrush, bottomCapRect.X + 8, bottomCapRect.Y, 5, bottomCapRect.Height);
            g.FillRectangle(shadowBrush, bottomCapRect.Right - 10, bottomCapRect.Y, 6, bottomCapRect.Height);

            // Boru gövdesi
            var bottomBodyRect = new RectangleF(X, bottomPipeY + capHeight, Width, Math.Max(0, bottomPipeHeight - capHeight));
            g.FillRectangle(pipeBrush, bottomBodyRect);
            g.DrawRectangle(borderPen, bottomBodyRect.X, bottomBodyRect.Y, bottomBodyRect.Width, bottomBodyRect.Height);

            // Gövde parıltı çizgisi
            g.FillRectangle(highlightBrush, X + 8, bottomBodyRect.Y, 5, bottomBodyRect.Height);
            // Gövde gölgesi
            g.FillRectangle(shadowBrush, X + Width - 10, bottomBodyRect.Y, 6, bottomBodyRect.Height);
        }
    }
}
