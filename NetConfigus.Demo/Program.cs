using NetConfigus.Demo;
using NetConfigus;
using System;
using System.Threading;

namespace NetConfigus.Demo;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            App(args);
            return;
        }

        string[] prm = ["input.txt", "output.txt", "-v", "--threads = 4"];
        var options = new ApplicationOptions();
        CommandLineParser.Parse(options, prm);
        Console.WriteLine(options);
    }
    static void App(string[] args)
    {
        var options = new ApplicationOptions();

        try
        {
            CommandLineParser.Parse(options, args);

            if (options.Verbose)
            {
                Console.WriteLine($"Processing file: {options.SourceFile}");
                Console.WriteLine($"Thread count: {options.ThreadCount}");

                if (options.IncludePatterns != null)
                {
                    Console.WriteLine("Include patterns: " +
                        string.Join(", ", options.IncludePatterns));
                }
            }

            // Здесь основная логика приложения
            ProcessFiles(options);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            options.PrintHelp();
            Environment.Exit(1);
        }
    }

    static void ProcessFiles(ApplicationOptions options)
    {
        // Реализация обработки файлов
        Console.WriteLine($"Processing {options.SourceFile}...");
    }
}