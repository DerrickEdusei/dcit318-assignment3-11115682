using System;
using System.Collections.Generic;

// Record for transaction
public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

// Interface
public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// Implementations
public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction) =>
        Console.WriteLine($"Bank Transfer of {transaction.Amount:C} for {transaction.Category}");
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction) =>
        Console.WriteLine($"Mobile Money payment of {transaction.Amount:C} for {transaction.Category}");
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction) =>
        Console.WriteLine($"Crypto Wallet payment of {transaction.Amount:C} for {transaction.Category}");
}

// Account base class
public class Account
{
    public string AccountNumber { get; }
    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;
    }
}

// Sealed savings account
public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance) { }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: {Balance:C}");
        }
    }
}

// Finance application
public class FinanceApp
{
    private readonly List<Transaction> _transactions = new();

    public void Run()
    {
        var account = new SavingsAccount("ACC123", 1000);

        var t1 = new Transaction(1, DateTime.Now, 50, "Groceries");
        var t2 = new Transaction(2, DateTime.Now, 200, "Utilities");
        var t3 = new Transaction(3, DateTime.Now, 300, "Entertainment");

        new MobileMoneyProcessor().Process(t1);
        account.ApplyTransaction(t1);

        new BankTransferProcessor().Process(t2);
        account.ApplyTransaction(t2);

        new CryptoWalletProcessor().Process(t3);
        account.ApplyTransaction(t3);

        _transactions.AddRange(new[] { t1, t2, t3 });
    }
}

class Program
{
    static void Main()
    {
        var app = new FinanceApp();
        app.Run();
    }
}
