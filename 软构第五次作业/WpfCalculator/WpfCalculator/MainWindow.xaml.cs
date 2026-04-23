using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace WpfCalculator
{
    public partial class MainWindow : Window
    {
        // 记录当前输入的表达式
        private string currentExpression = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        // 数字按钮点击事件（0~9）
        private void NumButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取点击的数字内容
            string num = (sender as Button).Content.ToString();
            // 拼接数字到表达式
            currentExpression += num;
            // 更新文本框显示
            txtDisplay.Text = currentExpression;
        }

        // 运算符按钮点击事件（+、-、×、÷）
        private void OperButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取点击的运算符内容
            string oper = (sender as Button).Content.ToString()
                .Replace("×", "*")
                .Replace("÷", "/");
            // 拼接运算符到表达式（避免开头直接输入运算符）
            if (!string.IsNullOrEmpty(currentExpression) && !IsOperator(currentExpression[currentExpression.Length - 1]))
            {
                currentExpression += oper;
                txtDisplay.Text = currentExpression;
            }
        }

        // 清空按钮点击事件（C）
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // 重置表达式和文本框
            currentExpression = "";
            txtDisplay.Text = "";
        }

        // 等号按钮点击事件（=）
        private void EqualButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(currentExpression))
                    return;

                var lambda = DynamicExpressionParser.ParseLambda(
                    new ParsingConfig(),     // 解析配置（使用默认配置）
                    typeof(double),          // 表达式返回值类型
                    currentExpression,       // 要计算的表达式字符串
                    null                     // 参数列表（本计算器无参数，传 null）
                );

                // 编译并动态执行表达式
                var result = lambda.Compile().DynamicInvoke();

                // 格式化输出：保留两位小数（避免过长小数）
                string finalText = $"{currentExpression}={Math.Round(Convert.ToDouble(result), 2)}";
                txtDisplay.Text = finalText;

                // 更新当前表达式为计算结果（支持连续计算，例如 3+5=8 后点击 +2 继续）
                currentExpression = Convert.ToDouble(result).ToString();
            }
            catch (Exception)
            {
                // 处理异常（除数为零、表达式格式错误等）
                txtDisplay.Text = "错误";
                currentExpression = "";
            }
        }

        // 辅助方法：判断字符是否为运算符（*、/、+、-）
        private bool IsOperator(char c)
        {
            return c == '*' || c == '/' || c == '+' || c == '-';
        }
    }
}
