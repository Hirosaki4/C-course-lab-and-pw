using System;
using System.Text;

// ===== Клас-модель =====
public class Book
{
    public string Title { get; set; }      
    public string Author { get; set; }     
    public int Year { get; set; }          
}

// ===== Успадкування від Book =====
public class EBook : Book
{
    public string FileFormat { get; set; } 
    public double FileSize { get; set; }   
}

// ===== Клас-сервіс =====
public class LibraryService
{
    private Book[] books;
    private int count = 0;

    // Конструктор задає розмір масиву
    public LibraryService(int size)
    {
        books = new Book[size];
    }

    // Додати книгу у масив
    public void AddBook(Book book)
    {
        if (count < books.Length)
        {
            books[count] = book;
            count++;
            Console.WriteLine($"Книга \"{book.Title}\" додана до бібліотеки.");
        }
        else
        {
            Console.WriteLine("Місця у бібліотеці більше немає!");
        }
    }

    // Показати всі книги
    public void ShowBooks()
    {
        Console.WriteLine("Список книг у бібліотеці:");
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"{books[i].Title} - {books[i].Author}, {books[i].Year}");
        }
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        LibraryService library = new LibraryService(5); 

        Book book1 = new Book { Title = "Місто", Author = "Валер’ян Підмогильний", Year = 1928 };
        EBook ebook1 = new EBook { Title = "C# для початківців", Author = "Іван Іваненко", Year = 2020, FileFormat = "PDF", FileSize = 5.2 };

        library.AddBook(book1);
        library.AddBook(ebook1);

        library.ShowBooks();
    }
}
