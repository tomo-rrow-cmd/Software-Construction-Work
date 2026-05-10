using FlashWord;

namespace FlashWord;

static class Program
{
    [STAThread]
    static void Main()
    {
        using var context = new WordContext();
        context.Database.EnsureCreated();

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}