using System;
using System.IO;
using System.Threading;

class Timer
{
    private System.Timers.Timer timer1;

    public event EventHandler? Tick;

    public Timer()
    {
        timer1 = new System.Timers.Timer(1000);
        timer1.Elapsed += OnElapsed;
        timer1.AutoReset = true;
    }

    private void OnElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (Tick != null)
        {
            Tick(this, EventArgs.Empty);
        }
    }

    public void Start()
    {
        timer1.Start();
    }

    public void Stop()
    {
        timer1.Stop();
    }
}

class Clock
{
    public Clock(Timer timer)
    {
        timer.Tick += ShowTime;
    }

    private void ShowTime(object? sender, EventArgs e)
    {
        Console.WriteLine("Текущее время: " + DateTime.Now.ToLongTimeString());
    }
}

class Counter
{
    public int Value { get; private set; }

    public Counter(Timer timer)
    {
        timer.Tick += Count;
    }

    private void Count(object? sender, EventArgs e)
    {
        Value++;
        Console.WriteLine("Счетчик: " + Value);
    }
}

class BankAccount
{
    private decimal balance;

    public decimal Balance
    {
        get { return balance; }
    }

    public event Action<decimal>? BalanceChanged;

    public void Deposit(decimal amount)
    {
        balance += amount;

        if (BalanceChanged != null)
        {
            BalanceChanged(balance);
        }
    }

    public void Withdraw(decimal amount)
    {
        balance -= amount;

        if (BalanceChanged != null)
        {
            BalanceChanged(balance);
        }
    }
}

class Logger
{
    private string filePath;

    public Logger(string path)
    {
        filePath = path;
    }

    public void Subscribe(BankAccount account)
    {
        account.BalanceChanged += WriteLog;
    }

    private void WriteLog(decimal balance)
    {
        string text = DateTime.Now + " | Баланс: " + balance + Environment.NewLine;
        File.AppendAllText(filePath, text);
    }
}

class Button
{
    private EventHandler? click;
    private int maxHandlers = 3;

    public string Text { get; set; }

    public Button(string text)
    {
        Text = text;
    }

    public event EventHandler Click
    {
        add
        {
            if (value == null)
                return;

            if (click != null)
            {
                Delegate[] list = click.GetInvocationList();

                if (Array.IndexOf(list, value) >= 0)
                {
                    Console.WriteLine("Одинаковый обработчик нельзя добавить дважды");
                    return;
                }

                if (list.Length >= maxHandlers)
                {
                    Console.WriteLine("Нельзя добавить больше трех обработчиков");
                    return;
                }
            }

            click += value;
        }
        remove
        {
            click -= value;
        }
    }

    public void PerformClick()
    {
        if (click != null)
        {
            click(this, EventArgs.Empty);
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("1. Timer, Clock, Counter");

        Timer timer = new Timer();
        Clock clock = new Clock(timer);
        Counter counter = new Counter(timer);

        timer.Start();
        Thread.Sleep(5000);
        timer.Stop();

        Console.WriteLine("Итоговое значение счетчика: " + counter.Value);

        Console.WriteLine();
        Console.WriteLine("2. BankAccount, Logger");

        string path = "balance_log.txt";

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        BankAccount account = new BankAccount();
        Logger logger = new Logger(path);

        logger.Subscribe(account);

        account.Deposit(1000);
        account.Withdraw(250);
        account.Deposit(500);

        Console.WriteLine("Текущий баланс: " + account.Balance);
        Console.WriteLine("Изменения записаны в файл: " + path);

        Console.WriteLine();
        Console.WriteLine("3. Button и аксессоры события");

        Button button = new Button("Моя кнопка");

        button.Click += ShowText;
        button.Click += ChangeColor;
        button.Click += ShowMessage;
        button.Click += AnotherHandler;
        button.Click += ShowText;

        Console.WriteLine("Нажатие кнопки 1:");
        button.PerformClick();

        button.Click -= ShowMessage;
        button.Click += AnotherHandler;

        Console.WriteLine("Нажатие кнопки 2:");
        button.PerformClick();

        Console.ReadLine();
    }

    static void ShowText(object? sender, EventArgs e)
    {
        Button button = (Button)sender!;
        Console.WriteLine("Текст кнопки: " + button.Text);
    }

    static void ChangeColor(object? sender, EventArgs e)
    {
        ConsoleColor oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Цвет текста изменен");
        Console.ForegroundColor = oldColor;
    }

    static void ShowMessage(object? sender, EventArgs e)
    {
        Console.WriteLine("Обработчик ShowMessage выполнен");
    }

    static void AnotherHandler(object? sender, EventArgs e)
    {
        Console.WriteLine("Обработчик AnotherHandler выполнен");
    }
}