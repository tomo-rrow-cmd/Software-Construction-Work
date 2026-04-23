using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebExtractor
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static readonly Regex phoneRegex = new Regex(@"1[3-9]\d{9}", RegexOptions.Compiled);
        private static readonly Regex emailRegex = new Regex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", RegexOptions.Compiled);

        public Form1()
        {
            InitializeComponent();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        private async void btnFetch_Click(object sender, EventArgs e)
        {
            string input = txtUrl.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("请输入内容！");
                return;
            }

            btnFetch.Enabled = false;
            txtResult.Text = "处理中...";

            try
            {
                string content;
                input = CleanFilePath(input);

                if (File.Exists(input))
                {
                    // 尝试多种编码读取文件
                    try
                    {
                        // 首先尝试 UTF-8
                        content = await Task.Run(() => File.ReadAllText(input, Encoding.UTF8));
                    }
                    catch
                    {
                        try
                        {
                            // 然后尝试 GBK
                            content = await Task.Run(() => File.ReadAllText(input, Encoding.GetEncoding("GBK")));
                        }
                        catch
                        {
                            try
                            {
                                // 最后尝试 GB2312
                                content = await Task.Run(() => File.ReadAllText(input, Encoding.GetEncoding("GB2312")));
                            }
                            catch
                            {
                                // 兜底使用默认编码
                                content = await Task.Run(() => File.ReadAllText(input));
                            }
                        }
                    }
                }
                else
                {
                    if (!input.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        input = "https://" + input;
                    }
                    content = await httpClient.GetStringAsync(input);
                }

                var phones = ExtractPhones(content);
                var emails = ExtractEmails(content);
                ShowResult(phones, emails);
            }
            catch (HttpRequestException ex)
            {
                txtResult.Text = $"网络请求失败：{ex.Message}";
            }
            catch (UnauthorizedAccessException ex)
            {
                txtResult.Text = $"文件访问被拒绝：{ex.Message}";
            }
            catch (IOException ex)
            {
                txtResult.Text = $"文件读取错误：{ex.Message}";
            }
            catch (Exception ex)
            {
                txtResult.Text = $"发生错误：{ex.Message}";
            }
            finally
            {
                btnFetch.Enabled = true;
            }
        }

        // 自动清理 file:// 前缀，保证文件路径正确
        private string CleanFilePath(string input)
        {
            if (input.StartsWith("file:///", StringComparison.OrdinalIgnoreCase))
                input = input.Substring(8).Replace("/", "\\");
            if (input.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
                input = input.Substring(7).Replace("/", "\\");
            return input.Trim();
        }

        // 手机号提取
        private HashSet<string> ExtractPhones(string text)
        {
            HashSet<string> list = new HashSet<string>();
            var matches = phoneRegex.Matches(text);
            foreach (Match m in matches) list.Add(m.Value);
            return list;
        }

        // 邮箱提取
        private HashSet<string> ExtractEmails(string text)
        {
            HashSet<string> list = new HashSet<string>();
            var matches = emailRegex.Matches(text);
            foreach (Match m in matches) list.Add(m.Value);
            return list;
        }

        // 显示结果
        private void ShowResult(HashSet<string> phones, HashSet<string> emails)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("提取完成！");
            sb.AppendLine($"\n【手机号】({phones.Count} 个)");
            foreach (var p in phones) sb.AppendLine(p);
            sb.AppendLine($"\n【邮箱】({emails.Count} 个)");
            foreach (var e in emails) sb.AppendLine(e);
            txtResult.Text = sb.ToString();
        }
    }
}