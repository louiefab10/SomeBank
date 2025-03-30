namespace SomeBank;

public class Bank
{
    public static Dictionary<string, Account> Accounts = new Dictionary<string, Account>();

    public static void Start()
    {
        Accounts = Account.LoadFromFile();
        Console.WriteLine("\t Welcome to SomeBank! \n \t Member of PDIC and BancNet. Regulated by the Bangko Sentral ng Pilipinas.");
        while (true)
        {
            Console.WriteLine("\n 1. Already a member? Login.");
            Console.WriteLine("\n 2. New to Some Bank? Create an account today.");
            Console.WriteLine("\n 3. Exit");
            Console.Write("\nSelect an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Login();
                    break;
                case "2":
                    CreateAccount();
                    break;
                case "3":
                    Console.Clear();
                    Console.WriteLine("Closing application, bye for now!");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    private static void ShowMenu(Account account)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\t Welcome to SomeBank Digital!");
            Console.WriteLine("\n SomeBank Menu:");
            Console.WriteLine("1. Check Balance");
            Console.WriteLine("2. Deposit Money");
            Console.WriteLine("3. Withdraw Money");
            Console.WriteLine("4. View Transaction History");
            Console.WriteLine("5. Exit");
            Console.Write("\nSelect an option: ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    account.CheckBalance();
                    Console.Write("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    break;
                case "2":
                    account.Deposit();
                    Console.Write("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    break;
                case "3":
                    account.Withdraw();
                    Console.Write("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    break;
                case "4":
                    account.ShowTransactions();
                    Console.Write("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    break;
                case "5":
                    Console.Clear();
                    Console.WriteLine("Logging out. . . \n");
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    private static void Login()
    {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine();
        if (Accounts.ContainsKey(username))
        {
            Account account = Accounts[username];
            Console.Write("Enter your PIN: ");
            string pin = HidePin();
            if (account.Authenticate(pin))
            {
                ShowMenu(account);
            }
            else
            {
                Console.WriteLine("Invalid PIN, please try again.");
            }
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }

    public static string HidePin()
    {
        string pin = "";
        ConsoleKeyInfo keyInfo;
        while (true)
        {
            keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                break;
            }

            if (keyInfo.Key == ConsoleKey.Backspace && pin.Length > 0)
            {
                pin = pin.Substring(0, (pin.Length - 1));
                Console.Write("\b \b");
                continue;
            }

            if (char.IsDigit(keyInfo.KeyChar))
            {
                pin += keyInfo.KeyChar;
                Console.Write("*");
            }
        }
        Console.WriteLine();
        return pin;
    }

    private static void CreateAccount()
    {
        Console.Write("\nEnter your preferred username: ");
        string username = Console.ReadLine();
        if (!Accounts.ContainsKey(username))
        {
            Console.Write("Create a 4-digit pin: ");
            string pin = HidePin();
            if (pin.Length != 4 || !int.TryParse(pin, out _))
            {
                Console.WriteLine("A pin should only contain numbers and must be exactly 4 digits.");
                return;
            }
            string salt = Account.GenerateSalt();
            string newAccNum = Account.GenerateAccountNumber();
            string encryptedPin = Account.EncryptPin(pin, salt);
            Accounts[username] = new Account(username, newAccNum, encryptedPin, 0, salt);
            Account.SaveToFile();
            Console.Clear();
            Console.WriteLine($"\t Account {username} has been successfully created. Welcome to SomeBank!");
        }
        else
        {
            Console.Write("An account with that username already exists. Please enter a different username.");
            return;
        }
    }
}