using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConfigus.Tests
{
    // Группа 6: Продвинутые сценарии
    public class AdvancedScenarios
    {
        public class ComplexConfig
        {
            [CommandLine(Position = 0, Required = true)]
            public string Source { get; set; } = default!;

            [CommandLine(Position = 1)]
            public string Destination { get; set; } = default!;

            [CommandLine(ShortName = 'b')]
            public bool Backup { get; set; }

            [CommandLine(LongName = "threads")]
            public int ThreadCount { get; set; }

            [CommandLine(ShortName = 'l')]
            public string[] Logs { get; set; } = [];
        }

        [Fact]
        public void ComplexConfiguration_ParsesAllProperties()
        {
            var options = new ComplexConfig();
            CommandLineParser.Parse(options, new[] { "src", "dest", "-b", "--threads=4", "-l=log1", "-l=log2" });

            Assert.Equal("src", options.Source);
            Assert.Equal("dest", options.Destination);
            Assert.True(options.Backup);
            Assert.Equal(4, options.ThreadCount);
            Assert.Equal(new[] { "log1", "log2" }, options.Logs);
        }

        public class CaseInsensitive
        {
            [CommandLine(LongName = "verbose")]
            public bool Verbose { get; set; }
        }

        [Fact]
        public void CaseInsensitiveLongName_ParsesCorrectly()
        {
            var options = new CaseInsensitive();
            CommandLineParser.Parse(options, new[] { "--VERBOSE=true" });
            Assert.True(options.Verbose);
        }

        public class IgnoredPositionals
        {
            [CommandLine(Position = 0)]
            public string A { get; set; } = default!;

            [CommandLine(Position = 1)]
            public string B { get; set; } = default!;
        }

        [Fact]
        public void PositionalsAfterNamed_AreIgnored()
        {
            var options = new IgnoredPositionals();
            CommandLineParser.Parse(options, new[] { "first", "-x=val", "second" });

            Assert.Equal("first", options.A);
            Assert.Null(options.B);
        }
    }

}
