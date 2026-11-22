using FlappyBird.Engine;
using FlappyBird.Models;

namespace FlappyBird;

public partial class MainForm : Form
{
    private readonly GameEngine _engine;

    public MainForm()
    {
        InitializeComponent();

        // 1. Oyun motorunu çalışma alanının boyutlarıyla başlat
        _engine = new GameEngine(gameCanvas.ClientSize.Width, gameCanvas.ClientSize.Height);

        // 2. Motor olaylarına abone ol (Event Subscription)
        _engine.ScoreUpdated += OnScoreUpdated;
        _engine.GameOverTriggered += OnGameOverTriggered;
        _engine.StateChanged += OnStateChanged;

        // 3. Arayüz ilk değerlerini ayarla
        cmbDifficulty.SelectedIndex = 1; // Varsayılan: Normal
        UpdateScoreDisplay(0);

        // 4. Zamanlayıcıyı (Timer) başlat
        gameTimer.Start();
    }

    private void GameCanvas_Paint(object? sender, PaintEventArgs e)
    {
        _engine.Draw(e.Graphics);
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        _engine.Update();
        gameCanvas.Invalidate();
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Up)
        {
            _engine.Flap();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.P)
        {
            _engine.TogglePause();
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.R)
        {
            _engine.Reset();
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.M)
        {
            chkSound.Checked = !chkSound.Checked;
            e.Handled = true;
        }
    }

    private void GameCanvas_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _engine.Flap();
        }
    }

    private void BtnNewGame_Click(object? sender, EventArgs e)
    {
        _engine.Reset();
        gameCanvas.Focus();
    }

    private void BtnPause_Click(object? sender, EventArgs e)
    {
        _engine.TogglePause();
        gameCanvas.Focus();
    }

    private void CmbDifficulty_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var selected = cmbDifficulty.SelectedIndex switch
        {
            0 => DifficultyLevel.Easy,
            2 => DifficultyLevel.Hard,
            _ => DifficultyLevel.Normal
        };

        _engine.SetDifficulty(selected);
        gameCanvas.Focus();
    }

    private void ChkSound_CheckedChanged(object? sender, EventArgs e)
    {
        _engine.SoundManager.IsMuted = !chkSound.Checked;
        chkSound.Text = chkSound.Checked ? "Ses Açık" : "Sessiz";
        gameCanvas.Focus();
    }

    private void OnScoreUpdated(int score)
    {
        if (InvokeRequired)
        {
            Invoke(new Action<int>(OnScoreUpdated), score);
            return;
        }
        UpdateScoreDisplay(score);
    }

    private void UpdateScoreDisplay(int score)
    {
        lblCurrentScore.Text = $"Skor: {score}";
        lblHighScore.Text = $"En Yüksek: {_engine.ScoreManager.HighScore}";
    }

    private void OnGameOverTriggered()
    {
        if (InvokeRequired)
        {
            Invoke(new Action(OnGameOverTriggered));
            return;
        }
        lblStatusInfo.Text = "OYUN BİTTİ! Tekrar oynamak için Boşluk veya 'Yeni Oyun'a tıklayın.";
        UpdateScoreDisplay(_engine.ScoreManager.CurrentScore);
    }

    private void OnStateChanged(GameState state)
    {
        if (InvokeRequired)
        {
            Invoke(new Action<GameState>(OnStateChanged), state);
            return;
        }

        switch (state)
        {
            case GameState.Ready:
                lblStatusInfo.Text = "Hazır - Başlamak için Boşluk tuşuna basın veya ekrana tıklayın";
                btnPause.Text = "Duraklat (P)";
                break;
            case GameState.Playing:
                lblStatusInfo.Text = "Oyun Devam Ediyor | Boşluk: Zıpla | P: Duraklat";
                btnPause.Text = "Duraklat (P)";
                break;
            case GameState.Paused:
                lblStatusInfo.Text = "Oyun Duraklatıldı | Devam etmek için 'P' veya Boşluk'a basın";
                btnPause.Text = "Devam Et (P)";
                break;
            case GameState.GameOver:
                lblStatusInfo.Text = "OYUN BİTTİ! Yeniden başlamak için Boşluk veya R tuşuna basın";
                btnPause.Text = "Duraklat (P)";
                break;
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        gameTimer.Stop();
        _engine.Dispose();
    }
}

/// <summary>
/// Çizim titremelerini (flickering) önleyen çift arabelleğe (double-buffered) sahip özel tuval.
/// </summary>
public sealed class GameCanvas : Panel
{
    public GameCanvas()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint |
                 ControlStyles.OptimizedDoubleBuffer, true);
        UpdateStyles();
    }
}
