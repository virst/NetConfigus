using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConfigus.Tests
{
    // Группа 2: Комбинации аргументов
    public class ArgumentCombinations
    {
        public class PositionalAndNamed
        {
            [CommandLine(Position = 0)]
            public string Source { get; set; }

            [CommandLine(ShortName = 'b')]
            public bool Backup { get; set; }
        }

        [Fact]
        public void PositionalAndNamedArgs_SetsBothProperties()
        {
            var options = new PositionalAndNamed();
            CommandLineParser.Parse(options, new[] { "src.txt", "-b" });
            Assert.Equal("src.txt", options.Source);
            Assert.True(options.Backup);
        }

        public class MultipleNamed
        {
            [CommandLine(ShortName = 't')]
            public int Threads { get; set; }

            [CommandLine(LongName = "log")]
            public string LogLevel { get; set; }
        }

        [Fact]
        public void MultipleNamedArgs_SetsAllProperties()
        {
            var options = new MultipleNamed();
            CommandLineParser.Parse(options, new[] { "-t=4", "--log=debug" });
            Assert.Equal(4, options.Threads);
            Assert.Equal("debug", options.LogLevel);
        }

        public class MixedFormats
        {
            [CommandLine(ShortName = 's')]
            public string Size { get; set; }

            [CommandLine(LongName = "color")]
            public bool Color { get; set; }
        }

        [Fact]
        public void MixedFormatArgs_SetsProperties()
        {
            var options = new MixedFormats();
            CommandLineParser.Parse(options, new[] { "-s", "10MB", "--color" });
            Assert.Equal("10MB", options.Size);
            Assert.True(options.Color);
        }
    }

}
