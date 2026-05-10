using Microsoft.EntityFrameworkCore;

namespace FlashWord;

public class WordContext : DbContext
{
    public DbSet<Word> Words { get; set; }

    public string DbPath { get; }

    public WordContext()
    {
        var folder = Environment.CurrentDirectory;
        DbPath = Path.Combine(folder, "words.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Word>().HasData(
            new Word { Id = 1, English = "apple", Chinese = "苹果" },
            new Word { Id = 2, English = "banana", Chinese = "香蕉" },
            new Word { Id = 3, English = "orange", Chinese = "橙子" },
            new Word { Id = 4, English = "computer", Chinese = "电脑" },
            new Word { Id = 5, English = "book", Chinese = "书" },
            new Word { Id = 6, English = "phone", Chinese = "手机" },
            new Word { Id = 7, English = "table", Chinese = "桌子" },
            new Word { Id = 8, English = "chair", Chinese = "椅子" },
            new Word { Id = 9, English = "window", Chinese = "窗户" },
            new Word { Id = 10, English = "door", Chinese = "门" }
        );
    }
}