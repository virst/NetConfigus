using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConfigus.Tests
{
    public class EdgeCases
    {
        public class IntBounds
        {
            [CommandLine(ShortName = 's')]
            public int Size { get; set; }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public void IntBoundaryValues_ParseCorrectly(int value)
        {
            var options = new IntBounds();
            CommandLineParser.Parse(options, new[] { $"-s={value}" });

            Assert.Equal(value, options.Size);
        }

        public class StringEdgeCases
        {
            [CommandLine(Position = 0)]
            public string Path { get; set; }
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("C:\\Long\\Path\\With\\Special\\Chars")]
        public void StringEdgeCases_ParseCorrectly(string value)
        {
            var options = new StringEdgeCases();
            CommandLineParser.Parse(options, new[] { value });

            Assert.Equal(value, options.Path);
        }
    }

}
