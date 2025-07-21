using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConfigus.Tests
{
    // Группа 8: Специальные форматы
    public class SpecialFormats
    {
        public class CombinedFlags
        {
            [CommandLine(ShortName = 'a')]
            public bool A { get; set; }

            [CommandLine(ShortName = 'b')]
            public bool B { get; set; }
        }

        [Fact]
        public void UnixStyleFlags_ParsesCorrectly()
        {
            var options = new CombinedFlags();
            CommandLineParser.Parse(options, new[] { "-ab" });

            Assert.True(options.A);
            Assert.True(options.B);
        }

        public class EmptyValue
        {
            [CommandLine(ShortName = 'c')]
            public string Config { get; set; } = default!;
        }

        [Fact]
        public void EmptyParameterValue_ParsesAsEmptyString()
        {
            var options = new EmptyValue();
            CommandLineParser.Parse(options, new[] { "-c=" });

            Assert.Equal("", options.Config);
        }
    }

}
