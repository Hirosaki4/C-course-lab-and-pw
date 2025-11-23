using System;
using System.Collections.Generic;

// === БАЗОВИЙ КЛАС З ІНТЕРФЕЙСАМИ ===
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

    // Клонування
    public virtual object Clone()
    {
        return MemberwiseClone();
    }

    // Порівняння — за рейтингом
    public int CompareTo(MusicItem other)
    {
        if (other == null) return 1;
        return Rating.CompareTo(other.Rating);
    }

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

    public override void Play()
    {
        Console.WriteLine($"🎵 Відтворюється пісня: {Title} — {Artist}");
    }

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

    public override void Play()
    {
        Console.WriteLine($"🎤 Відтворюється подкаст: {Title} — ведучий {Host}");
    }

    public override string ToString() =>
        $"Podcast: {base.ToString()} | Host: {Host}";
}

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
}

class Program
{
    static void Main()
    {
        MusicLibrary lib = new MusicLibrary();

        // Підписка на події
        lib.ItemAdded += item => Console.WriteLine($"➕ Додано: {item.Title}");
        lib.ItemRemoved += item => Console.WriteLine($"❌ Видалено: {item.Title}");

        // Додавання елементів
        var s1 = new Song("Warriors", 230, 9.2, "Imagine Dragons");
        var s2 = new Song("Legends Never Die", 255, 9.7, "Against The Current");
        var p1 = new Podcast("Tech News", 1800, 8.8, "John Doe");

        lib.Add(s1);
        lib.Add(s2);
        lib.Add(p1);

        Console.WriteLine("\n📌 СОРТУВАННЯ ЗА РЕЙТИНГОМ:");
        lib.Items.Sort(); // IComparable працює тут автоматично
        lib.Items.ForEach(i => Console.WriteLine(i));

        Console.WriteLine("\n📌 КЛОНУВАННЯ ЕЛЕМЕНТА:");
        MusicItem clone = (MusicItem)s2.Clone();
        clone.Title = "Legends Never Die — CLONE";
        Console.WriteLine("Оригінал: " + s2);
        Console.WriteLine("Клон:     " + clone);

        Console.WriteLine("\n📌 ВІДТВОРЕННЯ ВСІХ ЕЛЕМЕНТІВ:");
        foreach (var item in lib.Items)
            item.Play();
    }
}
