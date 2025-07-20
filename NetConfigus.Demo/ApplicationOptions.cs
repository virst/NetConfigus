using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConfigus.Demo
{
    public class ApplicationOptions
    {
        // Обязательный позиционный аргумент (исходный файл)
        [CommandLine(Position = 0, Description = "Source file path", Required = true)]
        public string SourceFile { get; set; }

        // Опциональный позиционный аргумент (выходной файл)
        [CommandLine(Position = 1, Description = "Destination file path")]
        public string DestinationFile { get; set; }

        // Флаг (булево значение)
        [CommandLine(ShortName = 'v', LongName = "verbose", Description = "Enable verbose output")]
        public bool Verbose { get; set; }

        // Числовой параметр
        [CommandLine(ShortName = 't', LongName = "threads", Description = "Number of threads", Required = false)]
        public int ThreadCount { get; set; } = 1;

        // Параметр с несколькими значениями
        [CommandLine(ShortName = 'i', LongName = "include", Description = "Files to include")]
        public string[] IncludePatterns { get; set; }

        public override string ToString()
        {
            return "ApplicationOptions:\n" +
                   $"  SourceFile: {SourceFile}\n" +
                   $"  DestinationFile: {DestinationFile}\n" +
                   $"  Verbose: {Verbose}\n" +
                   $"  ThreadCount: {ThreadCount}\n" +
                   $"  IncludePatterns: {string.Join(", ", IncludePatterns ?? Array.Empty<string>())}";
        }

        // Метод для вывода справки
        public void PrintHelp()
        {
            Console.WriteLine("Usage: app [options] <source> [destination]");
            Console.WriteLine("Options:");
            Console.WriteLine("  -v, --verbose\tEnable verbose output");
            Console.WriteLine("  -t, --threads=NUM\tNumber of threads (default: 1)");
            Console.WriteLine("  -i, --include=PATTERN\tInclude files matching pattern (multiple allowed)");
        }
    }
}
