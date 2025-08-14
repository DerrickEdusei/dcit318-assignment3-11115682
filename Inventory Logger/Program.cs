using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

// Marker interface
public interface IInventoryEntity { int Id { get; } }

// Immutable record (requires C# 9+)
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// Generic logger using plain-text serialization for .NET Framework compatibility
public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;
    private readonly Func<string, T> _deserialize;
    private readonly Func<T, string> _serialize;

    public InventoryLogger(string filePath, Func<T, string> serialize, Func<string, T> deserialize)
    {
        _filePath = filePath;
        _serialize = serialize;
        _deserialize = deserialize;
    }

    public void Add(T item) => _log.Add(item);
    public List<T> GetAll() => new(_log);

    public void SaveToFile()
    {
        try
        {
            using (var sw = new StreamWriter(_filePath, false))
            {
                foreach (var item in _log)
                    sw.WriteLine(_serialize(item));
            }
            Console.WriteLine($"Saved {_log.Count} items to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Save error: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        _log.Clear();
        try
        {
            if (!File.Exists(_filePath)) { Console.WriteLine("No file to load."); return; }

            using (var sr = new StreamReader(_filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    _log.Add(_deserialize(line));
                }
            }
            Console.WriteLine($"Loaded {_log.Count} items from {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Load error: {ex.Message}");
        }
    }
}

// App
public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp()
    {
        // Serialization: Id|Name|Quantity|DateAdded (round-trip "O" format)
        _logger = new InventoryLogger<InventoryItem>(
            filePath: "inventory.txt",
            serialize: item => $"{item.Id}|{item.Name}|{item.Quantity}|{item.DateAdded.ToString("O", CultureInfo.InvariantCulture)}",
            deserialize: line =>
            {
                var parts = line.Split('|');
                return new InventoryItem(
                    int.Parse(parts[0]),
                    parts[1],
                    int.Parse(parts[2]),
                    DateTime.Parse(parts[3], null, DateTimeStyles.RoundtripKind)
                );
            });
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Stapler", 12, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Printer Paper A4", 500, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Marker Pen", 48, DateTime.Now));
        _logger.Add(new InventoryItem(4, "USB Flash 32GB", 20, DateTime.Now));
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
            Console.WriteLine($"#{item.Id} {item.Name} — Qty={item.Quantity}, Added={item.DateAdded:yyyy-MM-dd HH:mm:ss}");
    }
}

class Program
{
    static void Main()
    {
        // First run: seed + save
        var app = new InventoryApp();
        app.SeedSampleData();
        app.SaveData();

        // Simulate new session: new app instance then load & print
        var newSession = new InventoryApp();
        newSession.LoadData();
        newSession.PrintAllItems();
    }
}
