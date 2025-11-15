using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

// ---- ДЕЛЕГАТИ ----
delegate void InfoDelegate(CafeItem item);
delegate bool FilterDelegate(CafeItem item);
delegate double SumDelegate(CafeItem item);

// ---- БАЗОВІ КЛАСИ ----
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
}

class Drink : CafeItem
{
    public bool IsCold { get; set; }
    public Drink(string name, double price, bool isCold) : base(name, price) { IsCold = isCold; }

    public override void DisplayInfo() =>
        Console.WriteLine($"Напій: {Name}, Ціна: {Price}, {(IsCold ? "холодний" : "гарячий")}");
}

class Dessert : CafeItem
{
    public int Calories { get; set; }
    public Dessert(string name, double price, int calories) : base(name, price) { Calories = calories; }

    public override void DisplayInfo() =>
        Console.WriteLine($"Десерт: {Name}, Ціна: {Price}, Калорії: {Calories}");
}

class Salad : CafeItem
{
    public bool ContainsMayonnaise { get; set; }
    public Salad(string name, double price, bool containsMayonnaise)
        : base(name, price) { ContainsMayonnaise = containsMayonnaise; }

    public override void DisplayInfo() =>
        Console.WriteLine($"Салат: {Name}, Ціна: {Price}, Соус: {(ContainsMayonnaise ? "майонез" : "оливкова олія")}");
}

// ---- КЛАС ГЕНЕРАТОР ПОДІЙ ----
class CafeManager
{
    public List<CafeItem> Menu { get; private set; } = new List<CafeItem>();

    // Події
    public event Action<CafeItem> ItemAdded;
    public event Action<CafeItem> ItemRemoved;
    public event Action<CafeItem> ItemUpdated;

    // Методи роботи з подіями
    public void AddItem(CafeItem item)
    {
        Menu.Add(item);
        SafeInvoke(ItemAdded, item);
    }

    public void RemoveItem(CafeItem item)
    {
        if (Menu.Remove(item))
            SafeInvoke(ItemRemoved, item);
        else
            Console.WriteLine("Спроба видалення елемента, якого немає у меню.");
    }

    public void UpdateItem(CafeItem oldItem, CafeItem newItem)
    {
        int index = Menu.IndexOf(oldItem);
        if (index >= 0)
        {
            Menu[index] = newItem;
            SafeInvoke(ItemUpdated, newItem);
        }
        else
            Console.WriteLine("Неможливо оновити: елемент не знайдено.");
    }

    // Безпечний виклик подій з обробкою виключень
    private void SafeInvoke(Action<CafeItem> evt, CafeItem item)
    {
        if (evt == null) return;
        foreach (var handler in evt.GetInvocationList())
        {
            try { handler.DynamicInvoke(item); }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при виклику обробника події: {ex.Message}");
            }
        }
    }
}

// ---- КЛАС СЛУХАЧ ПОДІЙ ----
class EventLogger
{
    public void OnItemAdded(CafeItem item)
    {
        Console.WriteLine($"[LOG] Додано позицію: {item.Name}");
    }
    public void OnItemRemoved(CafeItem item)
    {
        Console.WriteLine($"[LOG] Видалено позицію: {item.Name}");
    }
    public void OnItemUpdated(CafeItem item)
    {
        Console.WriteLine($"[LOG] Змінено позицію: {item.Name}");
    }
}

// ---- ДЕЛЕГАТИ У ВЗАЄМОДІЇ З ПОДІЯМИ ----
static class CafeProcessor
{
    public static void Process(List<CafeItem> items, InfoDelegate info)
    {
        foreach (var item in items)
            info(item);
    }
}

// ---- MAIN ----
class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("uk-UA");

        CafeManager manager = new CafeManager();
        EventLogger logger = new EventLogger();

        // Підписка на події
        manager.ItemAdded += logger.OnItemAdded;
        manager.ItemRemoved += logger.OnItemRemoved;
        manager.ItemUpdated += logger.OnItemUpdated;

        // Делегат для виведення інформації
        InfoDelegate show = item => item.DisplayInfo();

        // --- Робота з подіями ---
        var lemonade = new Drink("Лимонад", 45, true);
        var cheesecake = new Dessert("Чізкейк", 85, 420);
        var salad = new Salad("Цезар", 145, true);

        manager.AddItem(lemonade);
        manager.AddItem(cheesecake);
        manager.AddItem(salad);

        Console.WriteLine("\n=== МЕНЮ ===");
        CafeProcessor.Process(manager.Menu, show);

        // Зміна елемента
        manager.UpdateItem(salad, new Salad("Цезар XXL", 175, true));

        // Видалення
        manager.RemoveItem(lemonade);

        Console.WriteLine("\n=== ОНОВЛЕНЕ МЕНЮ ===");
        CafeProcessor.Process(manager.Menu, show);

        Console.ReadLine();
    }
}
