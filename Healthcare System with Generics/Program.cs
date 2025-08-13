using System;
using System.Collections.Generic;
using System.Linq;

public class Repository<T>
{
    private readonly List<T> items = new();

    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => new(items);
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

public class Patient
{
    public int Id;
    public string Name;
    public int Age;
    public string Gender;

    public Patient(int id, string name, int age, string gender)
    {
        Id = id; Name = name; Age = age; Gender = gender;
    }

    public override string ToString() => $"{Id}: {Name}, {Age}, {Gender}";
}

public class Prescription
{
    public int Id;
    public int PatientId;
    public string MedicationName;
    public DateTime DateIssued;

    public Prescription(int id, int patientId, string medName, DateTime dateIssued)
    {
        Id = id; PatientId = patientId; MedicationName = medName; DateIssued = dateIssued;
    }

    public override string ToString() => $"{Id}: {MedicationName} ({DateIssued:d})";
}

public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new();
    private Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        _patientRepo.Add(new Patient(1, "John Doe", 30, "Male"));
        _patientRepo.Add(new Patient(2, "Jane Smith", 25, "Female"));

        _prescriptionRepo.Add(new Prescription(1, 1, "Paracetamol", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Amoxicillin", DateTime.Now.AddDays(-5)));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap = _prescriptionRepo.GetAll()
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public void PrintAllPatients()
    {
        foreach (var p in _patientRepo.GetAll())
            Console.WriteLine(p);
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        if (_prescriptionMap.ContainsKey(id))
        {
            foreach (var pres in _prescriptionMap[id])
                Console.WriteLine(pres);
        }
        else
        {
            Console.WriteLine("No prescriptions found.");
        }
    }
}

class Program
{
    static void Main()
    {
        var app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();

        Console.WriteLine("Patients:");
        app.PrintAllPatients();

        Console.WriteLine("\nPrescriptions for Patient 1:");
        app.PrintPrescriptionsForPatient(1);
    }
}
