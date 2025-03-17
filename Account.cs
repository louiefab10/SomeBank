using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;

namespace SomeBank;

public class Account
{
    public int AccountNumber { get; set; }
    private string HashedPin  { get; set; }
    private decimal Balance { get; set; }
    private List<string> Transactions { get; set; }
    
    private static string FilePath = "users.txt";

    public Account(int accountNumber, string hashedPin, decimal balance, List<string> transactions = null)
    {
        AccountNumber = accountNumber;
        HashedPin = hashedPin;
        Balance = balance;
        Transactions = transactions ?? new List<string>();
    }

    public bool Authenticate(string inputPin)
    {
        string encryptedPin = EncryptPin(inputPin);
        return encryptedPin == HashedPin;
    }

    public static string EncryptPin(string pin)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] hashBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(pin));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public void Deposit()
    {
        Console.Write("How much would you like to deposit?: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal deposit) && deposit > 0)
        {
            Balance += deposit;
            Transactions.Add($"Deposited: +${deposit}");
            SaveToFile();
            Console.WriteLine($"${deposit:F2} deposited successfully!");
        }
        else
        {
            Console.WriteLine("Please enter a valid number");
        }
    }

    public void Withdraw()
    {
        Console.Write("How much would you like to withdraw?: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal withdraw) && withdraw > 0)
        {
            if (withdraw <= Balance)
            {
                Balance -= withdraw;
                Transactions.Add($"Withdrawn: -${withdraw:F2}");
                SaveToFile();
                Console.WriteLine($"${withdraw:F2} withdrawn successfully!");
            }
            else
            {
                Console.WriteLine("Insufficient funds!");
            }
        }
        else
        {
            Console.WriteLine("Invalid amount!");
        }
    }

    public void CheckBalance()
    {
        Console.WriteLine($"\nYou currently have ${Balance:F2} in your savings account.");
    }

    public void ShowTransactions()
    {
        Console.WriteLine("\nTransaction History: ");
        if (Transactions.Count == 0)
        {
            Console.WriteLine("No transactions yet!");
        }
        else
        {
            foreach (string transaction in Transactions)
            {
                Console.WriteLine(transaction);
            }
        }
    }

    public static void SaveToFile()
    {
        List<string> lines = new List<string>();
        foreach (var account in Bank.Accounts.Values)
        {
            string transactionHistory = account.Transactions != null ? string.Join(";", account.Transactions) : "";
            lines.Add($"{account.AccountNumber}, {account.HashedPin}, {account.Balance}, {transactionHistory}");
        }
        File.WriteAllLines(FilePath, lines);
    }

    public static Dictionary<int, Account> LoadFromFile()
    {
        Dictionary<int, Account> accounts = new Dictionary<int, Account>();

        if (File.Exists(FilePath))
        {
            foreach (string line in File.ReadAllLines(FilePath))
            {
                string[] data = line.Split(',');
                if (data.Length >= 3 && 
                    int.TryParse(data[0], out int accountNumber) &&
                    decimal.TryParse(data[2], out decimal balance))
                {
                    string hashedPin = data[1];
                    List<string> transactions = data.Length > 3 ? data[3].Split(';').ToList() : new List<string>();
                    accounts[accountNumber] = new Account(accountNumber, hashedPin, balance, transactions);
                }
            }
        }
        
        return accounts;
    }
}