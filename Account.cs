using System.Security.Cryptography;
using System.Text;

namespace SomeBank;

public class Account
{
    public string Username { get; set; }
    public string AccountNumber { get; set; }
    private string HashedPin  { get; set; }
    private decimal Balance { get; set; }
    private List<string> Transactions { get; set; }
    private static string filePath = "users.txt";
    private string Salt { get; set; }

    public Account(string username, string accountNumber, string hashedPin, decimal balance, string salt, List<string> transactions = null)
    {
        Username = username;
        AccountNumber = accountNumber;
        HashedPin = hashedPin;
        Balance = balance;
        Salt = salt;
        Transactions = transactions ?? new List<string>();
    }

    public bool Authenticate(string inputPin)
    {
        string encryptedPin = EncryptPin(inputPin, Salt);
        return encryptedPin == HashedPin;
    }

    public static string GenerateAccountNumber()
    {
        string prefix = "SB";
        string yyyy = DateTime.Now.ToString("yyyy");
        
        
        var accounts = LoadFromFile();
        var existingNum = accounts.Values
            .Where(a => a.AccountNumber.StartsWith($"{prefix}{yyyy}"))
            .Select(u => int.Parse(u.AccountNumber.Substring(6, 4))).ToList();
        int num = (existingNum.Count > 0) ? existingNum.Max() + 1 : 1;
        string accountNumber = num.ToString("D4");

        return $"{prefix}{yyyy}{accountNumber}";
    }

    public static string EncryptPin(string pin, string salt)
    {
        using (var sha256Hash = SHA256.Create())
        {
            byte[] saltedPin = Encoding.UTF8.GetBytes(pin + salt);
            byte[] hashedPin = sha256Hash.ComputeHash(saltedPin);
            return Convert.ToHexString(hashedPin).ToLower();
        }

        //return pin;
    }

    public static string GenerateSalt()
    {
        //string slt = "salt";
        
        byte[] salt = new byte[16];
        // using (var rng = new RNGCryptoServiceProvider())
        // {
        //     rng.GetBytes(salt);
        // }
        RandomNumberGenerator.Fill(salt);
        return Convert.ToBase64String(salt);
        //return slt;
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
            lines.Add($"{account.Username},{account.AccountNumber},{account.HashedPin},{account.Balance},{account.Salt},{transactionHistory}");
        }
        File.WriteAllLines(filePath, lines);
    }

    public static Dictionary<string, Account> LoadFromFile()
    {
        Dictionary<string, Account> accounts = new Dictionary<string, Account>();

        if (File.Exists(filePath))
        {
            foreach (string line in File.ReadAllLines(filePath))
            {
                string[] data = line.Split(',');
                if (data.Length >= 4)
                {
                    string username = data[0];
                    string accountNumber = data[1];
                    string hashedPin = data[2];
                    decimal balance = decimal.Parse(data[3]);
                    string salt = data[4];
                    accounts[username] = new Account(username, accountNumber, hashedPin, balance, salt);
                }
            }
        }
        
        return accounts;
    }
}