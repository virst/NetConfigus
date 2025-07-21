using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConfigus.Tests
{
    // Группа 1: Базовые сценарии
    public class BasicScenarios
    {
        public class SinglePositional
        {
            [CommandLine(Position = 0)]
            public string Source { get; set; } = default!;
        }

        [Fact]
        public void SinglePositionalArg_SetsProperty()
        {
            var options = new SinglePositional();
            CommandLineParser.Parse(options, new[] { "file.txt" });
            Assert.Equal("file.txt", options.Source);
        }

        public class SingleNamedShort
        {
            [CommandLine(ShortName = 'v')]
            public bool Verbose { get; set; }
        }

        [Fact]
        public void SingleNamedShortArg_SetsProperty()
        {
            var options = new SingleNamedShort();
            CommandLineParser.Parse(options, new[] { "-v" });
            Assert.True(options.Verbose);
        }

        public class SingleNamedLong
        {
            [CommandLine(LongName = "output")]
            public string Output { get; set; } = default!;
        }

        [Fact]
        public void SingleNamedLongArg_SetsProperty()
        {
            var options = new SingleNamedLong();
            CommandLineParser.Parse(options, new[] { "--output=result.txt" });
            Assert.Equal("result.txt", options.Output);
        }
    }

}
