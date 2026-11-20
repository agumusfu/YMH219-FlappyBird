using System.Drawing;
using System.Drawing.Drawing2D;

namespace FlappyBird.Models;

/// <summary>
/// Oyuncunun kontrol ettiği kuşu (Flappy Bird) temsil eden sınıf.
/// Nesne yönelimli programlama prensiplerine göre hareket fiziği,
/// durum bilgileri ve çizim işlevlerini kendi içinde kapsüller (Encapsulation).
/// </summary>
public class Bird
{
    public PointF Position { get; set; }
    public float VelocityY { get; set; }
    public float Gravity { get; set; }
    public float JumpStrength { get; set; }
    public int Width { get; } = 34;
    public int Height { get; } = 26;
    public float RotationAngle { get; private set; }

    // Çarpışma kutusu (Hitbox) - görsel kenarlıklardan hafif toleranslı
    public RectangleF Bounds => new(Position.X + 2, Position.Y + 2, Width - 4, Height - 4);

    public Bird(float startX, float startY, float gravity = 0.52f, float jumpStrength = 8.6f)
    {
        Gravity = gravity;
        JumpStrength = jumpStrength;
        Reset(new PointF(startX, startY));
    }

    /// <summary>
    /// Kuşun zıplama hareketi: Düşey hızını yukarı yönde günceller.
    /// </summary>
    public void Jump()
    {
        VelocityY = -JumpStrength;
    }

    /// <summary>
    /// Her karede (frame) yer çekimini uygulayarak kuşun konumunu ve eğim açısını günceller.
    /// </summary>
    public void Update(float groundY)
    {
        VelocityY += Gravity;

        // Terminal hızı sınırla (aşırı hızlanmayı önler)
        if (VelocityY > 12f) VelocityY = 12f;
        if (VelocityY < -JumpStrength) VelocityY = -JumpStrength;

        Position = new PointF(Position.X, Position.Y + VelocityY);

        // Zemine batmayı engelle
        if (Position.Y + Height > groundY)
        {
            Position = new PointF(Position.X, groundY - Height);
            VelocityY = 0;
        }

        // Tavan sınırını engelle
        if (Position.Y < 0)
        {
            Position = new PointF(Position.X, 0);
            VelocityY = 0;
        }

        // Düşüş/Zıplama açısı hesaplama (Görsel akıcılık için)
        // Yukarı zıplarken -25 derece, düşerken +70 dereceye kadar eğilir
        if (VelocityY < 0)
        {
            RotationAngle = -22f;
        }
        else
        {
            RotationAngle = Math.Min(70f, RotationAngle + 3.5f);
        }
    }

    /// <summary>
    /// Kuşu başlangıç pozisyonuna ve sıfır hıza geri döndürür.
    /// </summary>
    public void Reset(PointF startPosition)
    {
        Position = startPosition;
        VelocityY = 0;
        RotationAngle = 0;
    }

    /// <summary>
    /// Kuşu vektörel GDI+ grafikleriyle pürüzsüz ve estetik şekilde çizer.
    /// Harici resim dosyalarına bağımlılık olmadan her ortamda sorunsuz çalışır.
    /// </summary>
    public void Draw(Graphics g)
    {
        var state = g.Save();

        float centerX = Position.X + Width / 2f;
        float centerY = Position.Y + Height / 2f;

        g.TranslateTransform(centerX, centerY);
        g.RotateTransform(RotationAngle);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        float drawX = -Width / 2f;
        float drawY = -Height / 2f;

        // 1. Gövde (Altın sarısı elips)
        using (var bodyBrush = new SolidBrush(Color.FromArgb(248, 206, 44)))
        using (var borderPen = new Pen(Color.FromArgb(50, 40, 20), 2f))
        {
            g.FillEllipse(bodyBrush, drawX, drawY, Width - 6, Height);
            g.DrawEllipse(borderPen, drawX, drawY, Width - 6, Height);
        }

        // 2. Karın gölgesi
        using (var bellyBrush = new SolidBrush(Color.FromArgb(235, 175, 20)))
        {
            g.FillEllipse(bellyBrush, drawX + 4, drawY + Height / 2f - 2, (Width - 6) / 2f + 4, Height / 2f);
        }

        // 3. Kanat
        using (var wingBrush = new SolidBrush(Color.FromArgb(255, 240, 130)))
        using (var borderPen = new Pen(Color.FromArgb(50, 40, 20), 1.5f))
        {
            float wingY = VelocityY < 0 ? drawY + 8 : drawY + 4;
            g.FillEllipse(wingBrush, drawX + 2, wingY, 14, 10);
            g.DrawEllipse(borderPen, drawX + 2, wingY, 14, 10);
        }

        // 4. Göz (Büyük beyaz elips ve siyah gözbebeği)
        using (var eyeBrush = new SolidBrush(Color.White))
        using (var pupilBrush = new SolidBrush(Color.Black))
        using (var eyePen = new Pen(Color.FromArgb(50, 40, 20), 1.5f))
        {
            g.FillEllipse(eyeBrush, drawX + Width - 18, drawY + 2, 10, 12);
            g.DrawEllipse(eyePen, drawX + Width - 18, drawY + 2, 10, 12);
            g.FillEllipse(pupilBrush, drawX + Width - 13, drawY + 5, 4, 6);
            // Göz parıltısı
            g.FillEllipse(Brushes.White, drawX + Width - 12, drawY + 5, 2, 2);
        }

        // 5. Gaga (Turuncu)
        using (var beakBrush = new SolidBrush(Color.FromArgb(245, 95, 30)))
        using (var beakPen = new Pen(Color.FromArgb(50, 40, 20), 1.5f))
        {
            PointF[] beakPoints =
            {
                new(drawX + Width - 10, drawY + 10),
                new(drawX + Width + 2, drawY + 14),
                new(drawX + Width - 10, drawY + 19)
            };
            g.FillPolygon(beakBrush, beakPoints);
            g.DrawPolygon(beakPen, beakPoints);
        }

        g.Restore(state);
    }
}
