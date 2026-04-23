using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 软构第四次作业
{
    public partial class Form1 : Form
    {
        private string file1Path = "";
        private string file2Path = "";
        private Label lblStatus;
        private TextBox txtFile1;
        private TextBox txtFile2;
        private Button btnMerge;
        private ProgressBar progressBar;
        private ComboBox cboEncoding;

        public Form1()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "文件合并器";
            this.Size = new System.Drawing.Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 文件1选择区域
            Label lbl1 = new Label { Text = "文件1:", Top = 20, Left = 20, Width = 50 };
            txtFile1 = new TextBox { Top = 18, Left = 80, Width = 400, ReadOnly = true };
            Button btnSelect1 = new Button { Text = "浏览...", Top = 15, Left = 490, Width = 80 };

            // 文件2选择区域
            Label lbl2 = new Label { Text = "文件2:", Top = 60, Left = 20, Width = 50 };
            txtFile2 = new TextBox { Top = 58, Left = 80, Width = 400, ReadOnly = true };
            Button btnSelect2 = new Button { Text = "浏览...", Top = 55, Left = 490, Width = 80 };

            // 编码选择
            Label lblEncoding = new Label { Text = "编码:", Top = 100, Left = 20, Width = 50 };
            cboEncoding = new ComboBox
            {
                Top = 98,
                Left = 80,
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboEncoding.Items.AddRange(new object[] { "UTF-8", "GBK (简体中文)", "Unicode", "ASCII" });
            cboEncoding.SelectedIndex = 0; // 默认 UTF-8

            // 进度条
            progressBar = new ProgressBar
            {
                Top = 140,
                Left = 20,
                Width = 550,
                Height = 20,
                Style = ProgressBarStyle.Marquee, // 不确定进度时使用动画
                Visible = false
            };

            // 合并按钮
            btnMerge = new Button
            {
                Text = "开始合并",
                Top = 180,
                Left = 20,
                Width = 550,
                Height = 40,
                BackColor = System.Drawing.Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };

            // 状态标签
            lblStatus = new Label
            {
                Text = "请选择两个文件，然后点击合并...",
                Top = 240,
                Left = 20,
                Width = 550,
                Height = 40,
                AutoSize = false
            };

            // 添加控件
            this.Controls.Add(lbl1);
            this.Controls.Add(txtFile1);
            this.Controls.Add(btnSelect1);
            this.Controls.Add(lbl2);
            this.Controls.Add(txtFile2);
            this.Controls.Add(btnSelect2);
            this.Controls.Add(lblEncoding);
            this.Controls.Add(cboEncoding);
            this.Controls.Add(progressBar);
            this.Controls.Add(btnMerge);
            this.Controls.Add(lblStatus);

            // 绑定事件
            btnSelect1.Click += (s, e) => SelectFile(ref file1Path, txtFile1);
            btnSelect2.Click += (s, e) => SelectFile(ref file2Path, txtFile2);
            btnMerge.Click += async (s, e) => await MergeFilesAsync();
        }

        private void SelectFile(ref string path, TextBox txtBox)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                ofd.Title = "选择要合并的文件";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    path = ofd.FileName;
                    txtBox.Text = path;
                }
                else
                {
                    // 用户取消，清空对应记录
                    path = "";
                    txtBox.Text = "";
                }
            }
        }

        private async Task MergeFilesAsync()
        {
            if (string.IsNullOrEmpty(file1Path) || string.IsNullOrEmpty(file2Path))
            {
                MessageBox.Show("请先选择两个文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 获取用户选择的编码
            Encoding encoding = GetEncodingFromComboBox();

            // 准备目标路径
            string targetPath;
            SaveFileDialog sfd = null;
            try
            {
                // 默认保存到程序目录下的 Data 文件夹
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                string dataFolder = Path.Combine(exePath, "Data");
                if (!Directory.Exists(dataFolder))
                    Directory.CreateDirectory(dataFolder);

                string newFileName = $"Merged_{DateTime.Now:yyyyMMdd_HHmmss_fff}.txt";
                targetPath = Path.Combine(dataFolder, newFileName);
            }
            catch (UnauthorizedAccessException)
            {
                // 无权限创建文件夹，弹出保存对话框让用户自选位置
                sfd = new SaveFileDialog
                {
                    Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*",
                    Title = "保存合并后的文件",
                    FileName = $"Merged_{DateTime.Now:yyyyMMdd_HHmmss_fff}.txt"
                };
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;
                targetPath = sfd.FileName;
            }

            // UI 状态：禁用按钮，显示进度条
            btnMerge.Enabled = false;
            progressBar.Visible = true;
            lblStatus.Text = "正在合并文件，请稍候...";

            bool success = false;
            try
            {
                await Task.Run(() => MergeFilesCore(file1Path, file2Path, targetPath, encoding));
                success = true;
                lblStatus.Text = $"合并成功！保存至: {targetPath}";
                MessageBox.Show($"合并完成！\n路径：{targetPath}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "合并失败：" + ex.Message;
                MessageBox.Show($"操作失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 恢复 UI
                btnMerge.Enabled = true;
                progressBar.Visible = false;
                if (!success)
                    lblStatus.Text = "合并失败，请重试。";
            }
        }

        private void MergeFilesCore(string file1, string file2, string outputPath, Encoding encoding)
        {
            // 流式合并，避免内存占用过大
            using (var writer = new StreamWriter(outputPath, false, encoding))
            {
                // 写入第一个文件内容
                using (var reader = new StreamReader(file1, encoding, detectEncodingFromByteOrderMarks: true))
                {
                    char[] buffer = new char[8192];
                    int bytesRead;
                    while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }

                // 写入分割线（前后加换行，确保独立成行）
                writer.WriteLine(); // 确保第一个文件末尾有换行
                writer.WriteLine("--- 合并分割线 ---");
                writer.WriteLine(); // 分割线后换行，与第二个文件内容分开

                // 写入第二个文件内容
                using (var reader = new StreamReader(file2, encoding, detectEncodingFromByteOrderMarks: true))
                {
                    char[] buffer = new char[8192];
                    int bytesRead;
                    while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        private Encoding GetEncodingFromComboBox()
        {
            switch (cboEncoding.SelectedIndex)
            {
                case 0: return Encoding.UTF8;
                case 1: return Encoding.GetEncoding("GBK");
                case 2: return Encoding.Unicode;
                case 3: return Encoding.ASCII;
                default: return Encoding.UTF8;
            }
        }
    }
}