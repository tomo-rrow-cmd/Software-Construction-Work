using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchApp
{
    public class Form1 : Form
    {
        private TextBox keywordTextBox = null!;
        private Button searchButton = null!;
        private TextBox baiduResultTextBox = null!;
        private TextBox bingResultTextBox = null!;
        private Label statusLabel = null!;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "多搜索引擎搜索工具";
            this.Size = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label keywordLabel = new Label
            {
                Text = "请输入搜索关键字：",
                Location = new System.Drawing.Point(30, 30),
                AutoSize = true
            };

            keywordTextBox = new TextBox
            {
                Location = new System.Drawing.Point(30, 60),
                Width = 600,
                Font = new System.Drawing.Font("Microsoft YaHei", 10)
            };

            searchButton = new Button
            {
                Text = "搜索",
                Location = new System.Drawing.Point(650, 58),
                Width = 80,
                Height = 30,
                Font = new System.Drawing.Font("Microsoft YaHei", 9)
            };
            searchButton.Click += SearchButton_Click;

            Label baiduLabel = new Label
            {
                Text = "百度搜索结果：",
                Location = new System.Drawing.Point(30, 110),
                AutoSize = true,
                ForeColor = System.Drawing.Color.Blue
            };

            baiduResultTextBox = new TextBox
            {
                Location = new System.Drawing.Point(30, 140),
                Width = 820,
                Height = 180,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new System.Drawing.Font("Microsoft YaHei", 9)
            };

            Label bingLabel = new Label
            {
                Text = "必应搜索结果：",
                Location = new System.Drawing.Point(30, 340),
                AutoSize = true,
                ForeColor = System.Drawing.Color.Green
            };

            bingResultTextBox = new TextBox
            {
                Location = new System.Drawing.Point(30, 370),
                Width = 820,
                Height = 180,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new System.Drawing.Font("Microsoft YaHei", 9)
            };

            statusLabel = new Label
            {
                Text = "就绪",
                Location = new System.Drawing.Point(30, 560),
                AutoSize = true,
                ForeColor = System.Drawing.Color.Gray
            };

            this.Controls.Add(keywordLabel);
            this.Controls.Add(keywordTextBox);
            this.Controls.Add(searchButton);
            this.Controls.Add(baiduLabel);
            this.Controls.Add(baiduResultTextBox);
            this.Controls.Add(bingLabel);
            this.Controls.Add(bingResultTextBox);
            this.Controls.Add(statusLabel);
        }

        private async void SearchButton_Click(object? sender, EventArgs e)
        {
            string keyword = keywordTextBox.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show("请输入搜索关键字！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            searchButton.Enabled = false;
            keywordTextBox.Enabled = false;
            statusLabel.Text = "正在搜索...";
            baiduResultTextBox.Clear();
            bingResultTextBox.Clear();

            try
            {
                Task<string> baiduTask = SearchBaiduAsync(keyword);
                Task<string> bingTask = SearchBingAsync(keyword);

                await Task.WhenAll(baiduTask, bingTask);

                string baiduResult = await baiduTask;
                string bingResult = await bingTask;

                baiduResultTextBox.Text = baiduResult;
                bingResultTextBox.Text = bingResult;

                statusLabel.Text = $"搜索完成 - {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                statusLabel.Text = "搜索出错";
                MessageBox.Show($"搜索过程中出现错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                searchButton.Enabled = true;
                keywordTextBox.Enabled = true;
            }
        }

        private async Task<string> SearchBaiduAsync(string keyword)
        {
            string searchUrl = $"https://www.baidu.com/s?wd={Uri.EscapeDataString(keyword)}";

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

            try
            {
                string html = await client.GetStringAsync(searchUrl);
                return ExtractTextContent(html, 200, "百度");
            }
            catch (Exception ex)
            {
                return $"[百度搜索失败] {ex.Message}";
            }
        }

        private async Task<string> SearchBingAsync(string keyword)
        {
            string searchUrl = $"https://www.bing.com/search?q={Uri.EscapeDataString(keyword)}";

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

            try
            {
                string html = await client.GetStringAsync(searchUrl);
                return ExtractTextContent(html, 200, "必应");
            }
            catch (Exception ex)
            {
                return $"[必应搜索失败] {ex.Message}";
            }
        }

        private string ExtractTextContent(string html, int maxLength, string source)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return $"[{source}] 未能获取到内容";
            }

            string textOnly = Regex.Replace(html, "<script[^>]*>.*?</script>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            textOnly = Regex.Replace(textOnly, "<style[^>]*>.*?</style>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            textOnly = Regex.Replace(textOnly, "<[^>]+>", " ");
            textOnly = Regex.Replace(textOnly, "&nbsp;", " ");
            textOnly = Regex.Replace(textOnly, "&amp;", "&");
            textOnly = Regex.Replace(textOnly, "&lt;", "<");
            textOnly = Regex.Replace(textOnly, "&gt;", ">");
            textOnly = Regex.Replace(textOnly, "&quot;", "\"");
            textOnly = Regex.Replace(textOnly, @"&#\d+;", "");
            textOnly = System.Text.RegularExpressions.Regex.Replace(textOnly, @"\s+", " ");

            textOnly = textOnly.Trim();

            if (textOnly.Length <= maxLength)
            {
                return $"[{source}] {textOnly}";
            }

            string truncated = textOnly.Substring(0, maxLength);
            return $"[{source}] {truncated}...";
        }

        [STAThread]
        public static void Main()
        {
            Application.Run(new Form1());
        }
    }
}