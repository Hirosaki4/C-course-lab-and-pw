using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

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

    // Віртуальні методи
    public virtual void Action()
    {
        Console.WriteLine("Це позиція з меню кафе.");
    }

    public virtual void Prepare()
    {
        Console.WriteLine($"Страва {Name} готується стандартним способом.");
    }

    public virtual double CalculateDiscount(double percent)
    {
        if (percent < 0 || percent > 100)
            throw new ArgumentException("Процент знижки має бути між 0 і 100.");

        return Price - (Price * percent / 100);
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

    public override void Prepare()
    {
        Console.WriteLine(IsCold ? $"Напій {Name} охолоджується льодом." : $"Напій {Name} збивається паром.");
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
        Console.WriteLine($"Десерт {Name} прикрашається кремом.");
    }

    public override void Prepare()
    {
        Console.WriteLine($"Десерт {Name} охолоджується у холодильнику перед подачею.");
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
}

// НОВИЙ клас-нащадок: Салат
class Salad : CafeItem
{
    public bool ContainsMayonnaise { get; set; }

    public Salad(string name, double price, bool containsMayonnaise)
        : base(name, price)
    {
        ContainsMayonnaise = containsMayonnaise;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Салат: {Name}, Ціна: {Price} грн, Соус: {(ContainsMayonnaise ? "майонез" : "оливкова олія")}");
    }

    public override void Prepare()
    {
        Console.WriteLine($"Салат {Name} нарізається та змішується з овочами.");
    }

    public override double CalculateDiscount(double percent)
    {
        // Наприклад — салати не можна скидати більше ніж на 30%
        if (percent > 30)
            throw new InvalidOperationException("Салат не можна скидати більше ніж на 30%.");

        return base.CalculateDiscount(percent);
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("uk-UA");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("uk-UA");

        List<CafeItem> menu = new List<CafeItem>()
        {
            new Drink("Лимонад", 45, true),
            new Dessert("Торт Наполеон", 92, 520),
            new MainDish("Стейк", 210, "яловичина"),
            new Salad("Цезар", 145, true)
        };

        foreach (var item in menu)
        {
            item.DisplayInfo();
            item.Action();
            item.Prepare();

            // Використання виключень
            try
            {
                double newPrice = item.CalculateDiscount(35);
                Console.WriteLine($"Ціна після знижки: {newPrice} грн");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при обчисленні знижки: {ex.Message}");
            }

            Console.WriteLine();
        }

        Console.ReadLine();
    }
}
