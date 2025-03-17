using System;
using System.Collections.Generic;
using System.IO;
namespace SomeBank;

public class Account
{
    public int AccountNumber { get; set; }
    private string Pin  { get; set; }
    private decimal Balance { get; set; }
    private List<string> Transactions { get; set; }
    
    private static string FilePath = "users.txt";

    public Account(int accountNumber, string pin, decimal balance, List<string> transactions)
    {
        AccountNumber = accountNumber;
        Pin = pin;
        Balance = balance;
        Transactions = transactions;
    }

    public bool Authenticate(string inputPin)
    {
        return inputPin == Pin;
    }

    public void Deposit()
    {
        Console.Write("Enter amount to deposit: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal deposit) && deposit > 0)
        {
            Balance += deposit;
            Transactions.Add($"Deposited: +${deposit}");
            
        }
        else
        {
            Console.WriteLine("Please enter a valid number");
        }
    }

    public void Withdraw()
    {
        
    }

    public void CheckBalance()
    {
        
    }

    public void ShowTransactions()
    {
        
    }

    private void SaveToFile()
    {
        
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
                    string pin = data[1];
                    List<string> transactions = data.Length > 3 ? data[3].Split(';').ToList() : new List<string>();
                    accounts[accountNumber] = new Account(accountNumber, pin, balance, transactions);
                }
            }
        }
        
        return accounts;
    }
}