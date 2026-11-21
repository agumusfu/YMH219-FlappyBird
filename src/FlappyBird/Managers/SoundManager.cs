using System.Media;

namespace FlappyBird.Managers;

/// <summary>
/// Harici ses dosyalarına bağımlılık olmadan, bellekte (in-memory) dinamik
/// 8-bit retro ses dalgaları (PCM WAV) sentezleyen ve çalan ses yöneticisi.
/// </summary>
public class SoundManager : IDisposable
{
    private SoundPlayer? _jumpPlayer;
    private SoundPlayer? _scorePlayer;
    private SoundPlayer? _hitPlayer;

    public bool IsMuted { get; set; } = false;

    public SoundManager()
    {
        try
        {
            _jumpPlayer = CreateSoundPlayer(GenerateTone(600, 850, 70));     // Kısa zıplama sesi
            _scorePlayer = CreateSoundPlayer(GenerateScoreJingle());         // Skor kazanma çift tonu
            _hitPlayer = CreateSoundPlayer(GenerateTone(280, 80, 220));      // Çarpışma düşüş tonu
        }
        catch
        {
            // Ses aygıtı bulunamayan ortamlarda sessiz modda devam et
            IsMuted = true;
        }
    }

    public void PlayJump()
    {
        if (IsMuted) return;
        try { _jumpPlayer?.Play(); } catch { }
    }

    public void PlayScore()
    {
        if (IsMuted) return;
        try { _scorePlayer?.Play(); } catch { }
    }

    public void PlayHit()
    {
        if (IsMuted) return;
        try { _hitPlayer?.Play(); } catch { }
    }

    private static SoundPlayer CreateSoundPlayer(byte[] wavBytes)
    {
        var ms = new MemoryStream(wavBytes);
        var player = new SoundPlayer(ms);
        player.Load();
        return player;
    }

    /// <summary>
    /// Belirtilen başlangıç ve bitiş frekansında lineer frekans kaydırmalı (chirp) ses dalgası üretir.
    /// </summary>
    private static byte[] GenerateTone(double startFreq, double endFreq, int durationMs, int sampleRate = 22050)
    {
        int totalSamples = (int)(sampleRate * (durationMs / 1000.0));
        short[] samples = new short[totalSamples];

        double phase = 0;
        for (int i = 0; i < totalSamples; i++)
        {
            double progress = (double)i / totalSamples;
            double currentFreq = startFreq + (endFreq - startFreq) * progress;
            phase += 2 * Math.PI * currentFreq / sampleRate;

            // Yumuşak ses bitişi (Envelope / Fade-out)
            double envelope = 1.0 - progress;
            samples[i] = (short)(Math.Sin(phase) * 16000 * envelope);
        }

        return CreateWavStream(samples, sampleRate);
    }

    /// <summary>
    /// Skor geçildiğinde çalan sevimli 2 tonlu arcade arpeji üretir.
    /// </summary>
    private static byte[] GenerateScoreJingle(int sampleRate = 22050)
    {
        int noteDuration = (int)(sampleRate * 0.08); // 80 ms nota
        int totalSamples = noteDuration * 2;
        short[] samples = new short[totalSamples];

        double[] freqs = { 987.77, 1318.51 }; // B5 ve E6 notaları
        for (int note = 0; note < 2; note++)
        {
            double phase = 0;
            int offset = note * noteDuration;
            for (int i = 0; i < noteDuration; i++)
            {
                phase += 2 * Math.PI * freqs[note] / sampleRate;
                double envelope = 1.0 - ((double)i / noteDuration);
                samples[offset + i] = (short)(Math.Sin(phase) * 15000 * envelope);
            }
        }

        return CreateWavStream(samples, sampleRate);
    }

    /// <summary>
    /// Ham PCM ses örneklerinden standart RIFF/WAVE bayt dizisi oluşturur.
    /// </summary>
    private static byte[] CreateWavStream(short[] samples, int sampleRate)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        int subChunk2Size = samples.Length * 2; // 16-bit = 2 byte

        // RIFF Başlığı
        writer.Write("RIFF"u8.ToArray());
        writer.Write(36 + subChunk2Size);
        writer.Write("WAVE"u8.ToArray());

        // Subchunk1 ("fmt ")
        writer.Write("fmt "u8.ToArray());
        writer.Write(16);             // Subchunk1Size (16 for PCM)
        writer.Write((short)1);       // AudioFormat (1 = PCM)
        writer.Write((short)1);       // NumChannels (1 = Mono)
        writer.Write(sampleRate);     // SampleRate
        writer.Write(sampleRate * 2); // ByteRate (SampleRate * NumChannels * BitsPerSample/8)
        writer.Write((short)2);       // BlockAlign
        writer.Write((short)16);      // BitsPerSample

        // Subchunk2 ("data")
        writer.Write("data"u8.ToArray());
        writer.Write(subChunk2Size);

        foreach (var sample in samples)
        {
            writer.Write(sample);
        }

        return stream.ToArray();
    }

    public void Dispose()
    {
        _jumpPlayer?.Dispose();
        _scorePlayer?.Dispose();
        _hitPlayer?.Dispose();
    }
}
