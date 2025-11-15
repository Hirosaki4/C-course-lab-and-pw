using System;
using System.Collections.Generic;
using System.Text;

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

    // Абстрактний метод
    public abstract void DisplayInfo();

    // Віртуальний метод
    public virtual void Action()
    {
        Console.WriteLine("Це позиція з меню кафе.");
    }
}

// Клас-нащадок: Напій
class Drink : CafeItem
{
    public bool IsCold { get; set; }

    public Drink(string name, double price, bool isCold)
        : base(name, price)
    {
        IsCold = isCold;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Напій: {Name}, Ціна: {Price} грн, Тип: {(IsCold ? "холодний" : "гарячий")}");
    }

    public override void Action()
    {
        Console.WriteLine($"Напій {Name} подається від бармена.");
    }
}

// Клас-нащадок: Десерт
class Dessert : CafeItem
{
    public int Calories { get; set; }

    public Dessert(string name, double price, int calories)
        : base(name, price)
    {
        Calories = calories;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Десерт: {Name}, Ціна: {Price} грн, Калорії: {Calories}");
    }

    public override void Action()
    {
        Console.WriteLine($"Десерт {Name} прикрашається кремом перед подачею.");
    }
}

// Клас-нащадок: Основна страва
class MainDish : CafeItem
{
    public string Meat { get; set; }

    public MainDish(string name, double price, string meat)
        : base(name, price)
    {
        Meat = meat;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Страва: {Name}, Ціна: {Price} грн, М'ясо: {Meat}");
    }

    // не перевизначає Action() → використає віртуальний з базового класу
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        List<CafeItem> menu = new List<CafeItem>()
        {
            new Drink("Лимонад", 45, true),
            new Drink("Капучино", 70, false),
            new Dessert("Чізкейк", 85, 420),
            new MainDish("Стейк", 210, "яловичина")
        };

        foreach (var item in menu)
        {
            item.DisplayInfo();
            item.Action();
            Console.WriteLine();
        }

        Console.ReadLine();
    }
}
