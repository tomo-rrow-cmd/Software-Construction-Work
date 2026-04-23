using System.Text;
using System.Windows.Forms;

namespace WebExtractor
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // 注册GBK/GB2312编码提供程序
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}