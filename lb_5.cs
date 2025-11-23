using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public abstract class MusicItem : ICloneable, IComparable<MusicItem>
{
    public string Title { get; set; }
    public int Duration { get; set; }
    public double Rating { get; set; }

    public MusicItem(string title, int duration, double rating)
    {
        Title = title;
        Duration = duration;
        Rating = rating;
    }

    public abstract void Play();

    public virtual object Clone() => MemberwiseClone();

    public int CompareTo(MusicItem other) => Rating.CompareTo(other.Rating);

    public override string ToString() =>
        $"{Title} | {Duration}s | Rating: {Rating}";
}

public class Song : MusicItem
{
    public string Artist { get; set; }

    public Song(string title, int duration, double rating, string artist)
        : base(title, duration, rating)
    {
        Artist = artist;
    }

    public override void Play() =>
        Console.WriteLine($"🎵 Пісня: {Title} — {Artist}");

    public override string ToString() =>
        $"Song: {base.ToString()} | Artist: {Artist}";
}

public class Podcast : MusicItem
{
    public string Host { get; set; }

    public Podcast(string title, int duration, double rating, string host)
        : base(title, duration, rating)
    {
        Host = host;
    }

    public override void Play() =>
        Console.WriteLine($"🎤 Подкаст: {Title} — ведучий {Host}");

    public override string ToString() =>
        $"Podcast: {base.ToString()} | Host: {Host}";
}

public class MusicLibrary
{
    private readonly List<MusicItem> Items = new List<MusicItem>();
    private readonly object locker = new object();

    public event Action<MusicItem> ItemAdded;
    public event Action<MusicItem> ItemRemoved;

    public void Add(MusicItem item)
    {
        lock (locker)
            Items.Add(item);
        ItemAdded?.Invoke(item);
    }

    public void Remove(MusicItem item)
    {
        lock (locker)
            Items.Remove(item);
        ItemRemoved?.Invoke(item);
    }

    public List<MusicItem> GetItems()
    {
        lock (locker)
            return Items.ToList();
    }

    public async Task SaveToDatabaseAsync(CancellationToken token)
    {
        Console.WriteLine("💾 Початок збереження у базу...");
        await Task.Delay(500, token);

        lock (locker)
        {
            Console.WriteLine($"💽 Збережено {Items.Count} записів у базу даних.");
        }
        Console.WriteLine("✔ Завершено.");
    }

    public async Task<double> AnalyzeLibraryAsync(CancellationToken token)
    {
        Console.WriteLine("📊 Початок аналітики...");
        await Task.Delay(700, token);

        lock (locker)
        {
            return Items.Average(i => i.Duration);
        }
    }

    public async Task PlayAllAsync(CancellationToken token)
    {
        Console.WriteLine("🎧 Початок відтворення...");
        foreach (var item in GetItems())
        {
            token.ThrowIfCancellationRequested();
            Console.WriteLine($"▶ {item.Title}");
            item.Play();
            await Task.Delay(400, token);
        }
        Console.WriteLine("✔ Всі елементи відтворено.");
    }
}

class Program
{
    static async Task Main()
    {
        MusicLibrary lib = new MusicLibrary();
        lib.ItemAdded += i => Console.WriteLine($"➕ Додано: {i.Title}");
        lib.ItemRemoved += i => Console.WriteLine($"❌ Видалено: {i.Title}");

        lib.Add(new Song("Warriors", 230, 9.2, "Imagine Dragons"));
        lib.Add(new Song("Legends Never Die", 255, 9.7, "Against The Current"));
        lib.Add(new Podcast("Tech News", 1800, 8.8, "John Doe"));
        lib.Add(new Podcast("History Talks", 2100, 9.0, "James Smith"));

        CancellationTokenSource cts = new CancellationTokenSource();

        Console.WriteLine("\n🚀 Запуск трьох асинхронних операцій...");

        Task playTask = lib.PlayAllAsync(cts.Token);
        Task saveTask = lib.SaveToDatabaseAsync(cts.Token);
        Task<double> analyzeTask = lib.AnalyzeLibraryAsync(cts.Token);

        Task monitor = Task.Run(async () =>
        {
            while (!playTask.IsCompleted || !saveTask.IsCompleted || !analyzeTask.IsCompleted)
            {
                Console.WriteLine($"⏳ Статус: Play={playTask.Status}, Save={saveTask.Status}, Analyze={analyzeTask.Status}");
                await Task.Delay(300);
            }
        });

        await Task.Delay(1200);
        cts.Cancel(); 

        try
        {
            await Task.WhenAll(playTask, saveTask, analyzeTask);
            Console.WriteLine($"📌 Середня тривалість = {analyzeTask.Result:F1} сек");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("❗ Операція була скасована користувачем.");
        }

        await monitor;
        Console.WriteLine("\n🏁 Програма завершена стабільно.");
    }
}
