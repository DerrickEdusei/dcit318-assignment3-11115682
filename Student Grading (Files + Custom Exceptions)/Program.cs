using System;
using System.Collections.Generic;
using System.IO;

// Student
public class Student
{
    public int Id;
    public string FullName;
    public int Score;

    public Student(int id, string fullName, int score)
    {
        Id = id; FullName = fullName; Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70) return "B";
        if (Score >= 60) return "C";
        if (Score >= 50) return "D";
        return "F";
    }
}

// Custom exceptions
public class InvalidScoreFormatException : Exception { public InvalidScoreFormatException(string m) : base(m) { } }
public class MissingFieldException : Exception { public MissingFieldException(string m) : base(m) { } }

// Processor
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var list = new List<Student>();

        using (var sr = new StreamReader(inputFilePath))
        {
            string line;
            int lineNo = 0;
            while ((line = sr.ReadLine()) != null)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                if (parts.Length < 3)
                    throw new MissingFieldException($"Line {lineNo}: Expected 3 fields (Id, FullName, Score). Found {parts.Length}.");

                // Trim parts
                for (int i = 0; i < parts.Length; i++) parts[i] = parts[i].Trim();

                if (!int.TryParse(parts[0], out int id))
                    throw new InvalidScoreFormatException($"Line {lineNo}: Invalid Id '{parts[0]}'.");

                if (!int.TryParse(parts[2], out int score))
                    throw new InvalidScoreFormatException($"Line {lineNo}: Invalid Score '{parts[2]}'.");

                list.Add(new Student(id, parts[1], score));
            }
        }

        return list;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var sw = new StreamWriter(outputFilePath, false))
        {
            foreach (var s in students)
            {
                sw.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
            }
        }
    }
}

class Program
{
    static void Main()
    {
        Console.Write("Enter input .txt path: ");
        var inputPath = Console.ReadLine();

        Console.Write("Enter output .txt path: ");
        var outputPath = Console.ReadLine();

        var processor = new StudentResultProcessor();

        try
        {
            var students = processor.ReadStudentsFromFile(inputPath);
            processor.WriteReportToFile(students, outputPath);
            Console.WriteLine("Report generated successfully.");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: Input file not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Format error: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Missing data: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
