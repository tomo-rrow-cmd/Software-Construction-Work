namespace FlashWord;

public partial class Form1 : Form
{
    private List<Word> words = new();
    private int currentIndex = 0;
    private Random random = new();

    public Form1()
    {
        InitializeComponent();
        LoadWords();
    }

    private void LoadWords()
    {
        using var context = new WordContext();
        words = context.Words.ToList().OrderBy(w => random.Next()).ToList();
        if (words.Count > 0)
        {
            ShowCurrentWord();
        }
    }

    private void ShowCurrentWord()
    {
        if (currentIndex < words.Count)
        {
            lblChinese.Text = words[currentIndex].Chinese;
            lblProgress.Text = $"进度: {currentIndex + 1}/{words.Count}";
        }
        else
        {
            lblChinese.Text = "已完成全部单词!";
            lblProgress.Text = "进度: 已完成";
        }
        txtAnswer.Clear();
        lblResult.Text = "";
        txtAnswer.Focus();
    }

    private void txtAnswer_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            btnNext_Click(this, EventArgs.Empty);
            e.SuppressKeyPress = true;
        }
    }

    private void CheckAnswer()
    {
        if (currentIndex >= words.Count) return;

        string userAnswer = txtAnswer.Text.Trim();
        string correctAnswer = words[currentIndex].English;

        if (string.Equals(userAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase))
        {
            lblResult.Text = "✓ 正确";
            lblResult.ForeColor = Color.Green;
        }
        else
        {
            lblResult.Text = $"✗ 错误，正确答案: {correctAnswer}";
            lblResult.ForeColor = Color.Red;
        }
    }

    private void btnNext_Click(object? sender, EventArgs e)
    {
        CheckAnswer();
        Task.Delay(1000).Wait();
        currentIndex++;
        ShowCurrentWord();
    }
}