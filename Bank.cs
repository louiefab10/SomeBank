using System;
using System.Collections.Generic;
namespace SomeBank;

public class Bank
{
    public static Dictionary<int, Account> Accounts = new Dictionary<int, Account>();

    public static void Start()
    {
        Accounts = Account.LoadFromFile();
        Console.WriteLine("Welcome to SomeBank! \n Member of PDIC and BancNet. Regulated by the Bangko Sentral ng Pilipinas.");
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
                    break;
                case "2":
                    account.Deposit();
                    break;
                case "3":
                    account.Withdraw();
                    break;
                case "4":
                    account.ShowTransactions();
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
        Console.WriteLine("\n Enter your account number: ");
        if (int.TryParse(Console.ReadLine(), out int accountNumber) && Accounts.ContainsKey(accountNumber))
        {
            Account account = Accounts[accountNumber];
            Console.Write("Enter your PIN: ");
            string pin = Console.ReadLine();
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

    private static void CreateAccount()
    {
        Console.Write("\nEnter a new account number: ");
        if (int.TryParse(Console.ReadLine(), out int accountNumber) && !Accounts.ContainsKey(accountNumber))
        {
            Console.Write("Create a 4-digit PIN: ");
            string pin = Console.ReadLine();
            if (pin.Length != 4 || !int.TryParse(pin, out _))
            {
                Console.WriteLine("A PIN must only contain 4 digits, please try again.");
                return;
            }

            string hashedPin = Account.EncryptPin(pin);
            Accounts[accountNumber] = new Account(accountNumber, hashedPin, 0);
            Account.SaveToFile();
            Console.WriteLine("Account has been successfully created. Welcome to SomeBank!");
        }
        else
        {
            Console.WriteLine("Account number is invalid or already exists.");
        }
    }
}