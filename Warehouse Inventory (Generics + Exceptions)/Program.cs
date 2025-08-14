using System;
using System.Collections.Generic;

// Marker interface
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// Electronic item
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
    }

    public override string ToString() => $"[Electronic] #{Id} {Name} ({Brand}) Qty={Quantity}, Warranty={WarrantyMonths}m";
}

// Grocery item
public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
    }

    public override string ToString() => $"[Grocery]   #{Id} {Name} Qty={Quantity}, Expires={ExpiryDate:yyyy-MM-dd}";
}

// Custom exceptions
public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }

// Generic inventory repository
public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out var item))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public List<T> GetAllItems() => new(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

// Manager
public class WareHouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new();
    private readonly InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Phone", 25, "Samsung", 12));
        _groceries.AddItem(new GroceryItem(1, "Rice (5kg)", 40, DateTime.Today.AddMonths(12)));
        _groceries.AddItem(new GroceryItem(2, "Milk", 15, DateTime.Today.AddDays(20)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
            Console.WriteLine(item);
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var current = repo.GetItemById(id).Quantity;
            repo.UpdateQuantity(id, checked(current + quantity));
            Console.WriteLine($"Stock updated for #{id}: {current} -> {current + quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"IncreaseStock error: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Removed item #{id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RemoveItem error: {ex.Message}");
        }
    }

    public InventoryRepository<ElectronicItem> Electronics => _electronics;
    public InventoryRepository<GroceryItem> Groceries => _groceries;
}

class Program
{
    static void Main()
    {
        var mgr = new WareHouseManager();
        mgr.SeedData();

        Console.WriteLine("Groceries:");
        mgr.PrintAllItems(mgr.Groceries);

        Console.WriteLine("\nElectronics:");
        mgr.PrintAllItems(mgr.Electronics);

        Console.WriteLine("\n— Exception demos —");
        // Duplicate item
        try { mgr.Electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12)); }
        catch (Exception ex) { Console.WriteLine($"Duplicate add: {ex.Message}"); }

        // Remove non-existent
        mgr.RemoveItemById(mgr.Groceries, 999);

        // Invalid quantity
        try { mgr.Electronics.UpdateQuantity(2, -5); }
        catch (Exception ex) { Console.WriteLine($"Invalid quantity: {ex.Message}"); }

        // Normal increase
        mgr.IncreaseStock(mgr.Groceries, 2, 10);
    }
}
