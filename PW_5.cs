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
    private readonly SemaphoreSlim limiter = new SemaphoreSlim(1);

    public event Action<MusicItem> ItemAdded;
    public event Action<MusicItem> ItemRemoved;

    public void Add(MusicItem item)
    {
        lock (locker) 
        {
            Items.Add(item);
        }
        ItemAdded?.Invoke(item);
    }

    public void Remove(MusicItem item)
    {
        lock (locker)
        {
            Items.Remove(item);
        }
        ItemRemoved?.Invoke(item);
    }

    public List<MusicItem> GetItems()
    {
        lock (locker)
        {
            return Items.ToList();
        }
    }


    public async Task<double> CalculateAverageRatingParallel()
    {
        await limiter.WaitAsync(); // SemaphoreSlim
        try
        {
            return await Task.Run(() =>
            {
                lock (locker)
                {
                    Console.WriteLine("📌 Розрахунок середнього рейтингу...");
                    Thread.Sleep(1000);
                    return Items.Average(i => i.Rating);
                }
            });
        }
        finally
        {
            limiter.Release();
        }
    }

    public void PlayAllParallel()
    {
        Parallel.ForEach(GetItems(), item =>
        {
            lock (locker)
            {
                Console.WriteLine($"▶ Відтворення: {item.Title}");
            }
            item.Play();
            Thread.Sleep(400);
        });
    }
}

class Program
{
    static async Task Main()
    {
        MusicLibrary lib = new MusicLibrary();
        lib.ItemAdded += i => Console.WriteLine($"➕ Додано: {i.Title}");
        lib.ItemRemoved += i => Console.WriteLine($"❌ Видалено: {i.Title}");

        // Додавання у різних потоках
        var t1 = Task.Run(() => lib.Add(new Song("Warriors", 230, 9.2, "Imagine Dragons")));
        var t2 = Task.Run(() => lib.Add(new Song("Legends Never Die", 255, 9.7, "Against The Current")));
        var t3 = Task.Run(() => lib.Add(new Podcast("Tech News", 1800, 8.8, "John Doe")));
        var t4 = Task.Run(() => lib.Add(new Podcast("History Talks", 2100, 9.0, "James Smith")));

        await Task.WhenAll(t1, t2, t3, t4);

        Console.WriteLine("\n🎧 Паралельне відтворення треків:");
        lib.PlayAllParallel();

        Console.WriteLine("\n📊 Паралельний розрахунок середнього рейтингу:");
        double avg = await lib.CalculateAverageRatingParallel();
        Console.WriteLine($"⭐ Середній рейтинг = {avg:F2}");

        // Видалення у потоках
        Task.Run(() => lib.Remove(lib.GetItems().First()));
        Task.Run(() => lib.Remove(lib.GetItems().Last()));

        Thread.Sleep(800);
        Console.WriteLine("\n📌 Робота завершена стабільно.");
    }
}
