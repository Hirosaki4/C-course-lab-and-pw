using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

// ---- ДЕЛЕГАТИ ----
delegate void InfoDelegate(CafeItem item);
delegate bool FilterDelegate(CafeItem item);
delegate double SumDelegate(CafeItem item);

// ---- Класи предметної області ----

// Абстрактний базовий клас
abstract class CafeItem
{
    public string Name { get; set; }
    public double Price { get; set; }

    public CafeItem(string name, double price)
    {
        Name = name;
        Price = price;
    }

    public abstract void DisplayInfo();
    public virtual void Action() => Console.WriteLine("Це позиція з меню кафе.");
    public virtual void Prepare() => Console.WriteLine($"Страва {Name} готується стандартним способом.");

    public virtual double CalculateDiscount(double percent)
    {
        if (percent < 0 || percent > 100)
            throw new ArgumentException("Процент знижки має бути між 0 і 100.");
        return Price - (Price * percent / 100);
    }
}

class Drink : CafeItem
{
    public bool IsCold { get; set; }
    public Drink(string name, double price, bool isCold) : base(name, price)
    { IsCold = isCold; }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Напій: {Name}, Ціна: {Price} грн, Тип: {(IsCold ? "холодний" : "гарячий")}");
    }

    public override void Prepare()
    {
        Console.WriteLine(IsCold ? $"Напій {Name} охолоджується льодом." : $"Напій {Name} збивається паром.");
    }
}

class Dessert : CafeItem
{
    public int Calories { get; set; }
    public Dessert(string name, double price, int calories) : base(name, price)
    { Calories = calories; }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Десерт: {Name}, Ціна: {Price} грн, Калорії: {Calories}");
    }
}

class MainDish : CafeItem
{
    public string Meat { get; set; }
    public MainDish(string name, double price, string meat) : base(name, price)
    { Meat = meat; }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Страва: {Name}, Ціна: {Price} грн, М'ясо: {Meat}");
    }
}

class Salad : CafeItem
{
    public bool ContainsMayonnaise { get; set; }
    public Salad(string name, double price, bool containsMayonnaise)
        : base(name, price)
    { ContainsMayonnaise = containsMayonnaise; }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Салат: {Name}, Ціна: {Price} грн, Соус: {(ContainsMayonnaise ? "майонез" : "оливкова олія")}");
    }
}

// ---- КЛАС ДЛЯ РОБОТИ З ДЕЛЕГАТАМИ ----
static class CafeProcessor
{
    public static void ProcessItems(List<CafeItem> items, InfoDelegate info)
    {
        foreach (var item in items)
            info(item);
    }

    public static List<CafeItem> FilterItems(List<CafeItem> items, FilterDelegate filter)
    {
        List<CafeItem> result = new List<CafeItem>();
        foreach (var item in items)
            if (filter(item)) result.Add(item);
        return result;
    }

    public static double CalculateSum(List<CafeItem> items, SumDelegate selector)
    {
        double sum = 0;
        foreach (var item in items)
            sum += selector(item);
        return sum;
    }
}

// ---- MAIN ----
class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("uk-UA");

        List<CafeItem> menu = new List<CafeItem>()
        {
            new Drink("Лимонад", 45, true),
            new Dessert("Чізкейк", 85, 420),
            new MainDish("Стейк", 210, "яловичина"),
            new Salad("Цезар", 145, true)
        };

        // ----- ДЕЛЕГАТИ -----
        InfoDelegate showInfo = item => item.DisplayInfo();
        FilterDelegate onlySalads = item => item is Salad;
        FilterDelegate expensive = item => item.Price > 100;
        SumDelegate priceSelector = item => item.Price;
        SumDelegate discountSelector = item => item.CalculateDiscount(20);

        Console.WriteLine("=== ВСІ ПОЗИЦІЇ ===");
        CafeProcessor.ProcessItems(menu, showInfo);
        Console.WriteLine();

        Console.WriteLine("=== ТІЛЬКИ САЛАТИ ===");
        var salads = CafeProcessor.FilterItems(menu, onlySalads);
        CafeProcessor.ProcessItems(salads, showInfo);
        Console.WriteLine();

        Console.WriteLine("=== ДОРОГІ ПОЗИЦІЇ (>100 грн) ===");
        var expensiveItems = CafeProcessor.FilterItems(menu, expensive);
        CafeProcessor.ProcessItems(expensiveItems, showInfo);
        Console.WriteLine();

        Console.WriteLine("=== ЗАГАЛЬНА СУМА МЕНЮ ===");
        Console.WriteLine($"Сума: {CafeProcessor.CalculateSum(menu, priceSelector)} грн\n");

        Console.WriteLine("=== СУМА МЕНЮ ПІСЛЯ 20% ЗНИЖКИ ===");
        Console.WriteLine($"Сума зі знижкою: {CafeProcessor.CalculateSum(menu, discountSelector)} грн\n");

        Console.ReadLine();
    }
}
