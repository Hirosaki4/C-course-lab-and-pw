using System;
using System.Collections.Generic;
using System.Linq;

// === БАЗОВИЙ КЛАС ===
public abstract class MusicItem : ICloneable, IComparable<MusicItem>
{
    public string Title { get; set; }
    public int Duration { get; set; } // у секундах
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

// === ПОХІДНІ КЛАСИ ===
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

// === КОЛЕКЦІЯ З ПОДІЯМИ ТА ІТЕРАТОРОМ ===
public class MusicLibrary
{
    public List<MusicItem> Items = new List<MusicItem>();

    public event Action<MusicItem> ItemAdded;
    public event Action<MusicItem> ItemRemoved;

    public void Add(MusicItem item)
    {
        Items.Add(item);
        ItemAdded?.Invoke(item);
    }

    public void Remove(MusicItem item)
    {
        Items.Remove(item);
        ItemRemoved?.Invoke(item);
    }

    // === ВЛАСНИЙ ІТЕРАТОР (лише елементи з рейтингом вище заданого) ===
    public IEnumerable<MusicItem> GetHighRated(double minRating)
    {
        foreach (var item in Items)
        {
            if (item.Rating >= minRating)
                yield return item;
        }
    }
}

// === ГОЛОВНА ПРОГРАМА (ДЕМОНСТРАЦІЯ) ===
class Program
{
    static void Main()
    {
        MusicLibrary lib = new MusicLibrary();

        lib.ItemAdded += i => Console.WriteLine($"➕ Додано: {i.Title}");
        lib.ItemRemoved += i => Console.WriteLine($"❌ Видалено: {i.Title}");

        lib.Add(new Song("Warriors", 230, 9.2, "Imagine Dragons"));
        lib.Add(new Song("Legends Never Die", 255, 9.7, "Against The Current"));
        lib.Add(new Podcast("Tech News", 1800, 8.8, "John Doe"));
        lib.Add(new Podcast("History Talks", 2100, 9.0, "James Smith"));

        Console.WriteLine("\n📌 КОРИСТУВАЦЬКИЙ ІТЕРАТОР (рейтинг ≥ 9):");
        foreach (var item in lib.GetHighRated(9))
            Console.WriteLine(item);

        // === LINQ ЗАПИТИ ===

        Console.WriteLine("\n📌 LINQ: усі пісні відсортовані за тривалістю:");
        var sortedSongs = lib.Items
            .Where(x => x is Song)
            .OrderBy(x => x.Duration);

        foreach (var item in sortedSongs)
            Console.WriteLine(item);

        Console.WriteLine("\n📌 LINQ: назви всіх треків з рейтингом вище 9:");
        var titles = lib.Items
            .Where(x => x.Rating > 9)
            .Select(x => x.Title);

        foreach (var t in titles)
            Console.WriteLine("⭐ " + t);

        Console.WriteLine("\n📌 LINQ: середня тривалість всіх об'єктів:");
        double avgDuration = lib.Items.Average(x => x.Duration);
        Console.WriteLine($"⏳ Середня тривалість: {avgDuration:F1} сек");

        Console.WriteLine("\n📌 LINQ: найдовший об'єкт:");
        var longest = lib.Items.OrderByDescending(x => x.Duration).First();
        Console.WriteLine("🏆 " + longest);

        Console.WriteLine("\n📌 Відтворення всіх об'єктів:");
        foreach (var item in lib.Items)
            item.Play();
    }
}
