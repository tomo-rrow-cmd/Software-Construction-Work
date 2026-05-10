namespace FlashWord;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;
    private Label lblChinese;
    private TextBox txtAnswer;
    private Label lblResult;
    private Button btnNext;
    private Label lblProgress;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.lblChinese = new Label();
        this.txtAnswer = new TextBox();
        this.lblResult = new Label();
        this.btnNext = new Button();
        this.lblProgress = new Label();
        this.SuspendLayout();

        this.lblChinese.AutoSize = true;
        this.lblChinese.Font = new Font("Microsoft YaHei", 24F, FontStyle.Regular, GraphicsUnit.Point);
        this.lblChinese.Location = new Point(100, 80);
        this.lblChinese.Name = "lblChinese";
        this.lblChinese.Size = new Size(200, 40);
        this.lblChinese.TabIndex = 0;
        this.lblChinese.Text = "中文词义";
        this.lblChinese.TextAlign = ContentAlignment.MiddleCenter;

        this.txtAnswer.Font = new Font("Microsoft YaHei", 18F, FontStyle.Regular, GraphicsUnit.Point);
        this.txtAnswer.Location = new Point(100, 150);
        this.txtAnswer.Name = "txtAnswer";
        this.txtAnswer.Size = new Size(300, 38);
        this.txtAnswer.TabIndex = 1;
        this.txtAnswer.KeyDown += txtAnswer_KeyDown;

        this.lblResult.AutoSize = true;
        this.lblResult.Font = new Font("Microsoft YaHei", 16F, FontStyle.Regular, GraphicsUnit.Point);
        this.lblResult.ForeColor = Color.Green;
        this.lblResult.Location = new Point(100, 210);
        this.lblResult.Name = "lblResult";
        this.lblResult.Size = new Size(100, 26);
        this.lblResult.TabIndex = 2;
        this.lblResult.Text = "";
        this.lblResult.TextAlign = ContentAlignment.MiddleCenter;

        this.btnNext.Location = new Point(250, 300);
        this.btnNext.Name = "btnNext";
        this.btnNext.Size = new Size(100, 35);
        this.btnNext.TabIndex = 3;
        this.btnNext.Text = "下一个";
        this.btnNext.UseVisualStyleBackColor = true;
        this.btnNext.Click += btnNext_Click;

        this.lblProgress.AutoSize = true;
        this.lblProgress.Font = new Font("Microsoft YaHei", 10F, FontStyle.Regular, GraphicsUnit.Point);
        this.lblProgress.Location = new Point(100, 310);
        this.lblProgress.Name = "lblProgress";
        this.lblProgress.Size = new Size(80, 17);
        this.lblProgress.TabIndex = 4;
        this.lblProgress.Text = "进度: 0/0";

        this.AutoScaleDimensions = new SizeF(7F, 17F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(500, 400);
        this.Controls.Add(this.lblProgress);
        this.Controls.Add(this.btnNext);
        this.Controls.Add(this.lblResult);
        this.Controls.Add(this.txtAnswer);
        this.Controls.Add(this.lblChinese);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.Name = "Form1";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "背单词程序";
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}