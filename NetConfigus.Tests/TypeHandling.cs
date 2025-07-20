using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConfigus.Tests
{
    // Группа 3: Обработка типов данных
    public class TypeHandling
    {
        public class IntType
        {
            [CommandLine(ShortName = 'p')]
            public int Port { get; set; }
        }

        [Fact]
        public void IntTypeArg_ParsesCorrectly()
        {
            var options = new IntType();
            CommandLineParser.Parse(options, new[] { "-p=8080" });
            Assert.Equal(8080, options.Port);
        }

        public class DoubleType
        {
            [CommandLine(ShortName = 'r')]
            public double Ratio { get; set; }
        }

        [Fact]
        public void DoubleTypeArg_ParsesCorrectly()
        {
            var options = new DoubleType();
            CommandLineParser.Parse(options, new[] { "-r=0.75" });
            Assert.Equal(0.75, options.Ratio);
        }

        public class BoolFlags
        {
            [CommandLine(ShortName = 'e')]
            public bool Enabled { get; set; }
        }

        [Theory]
        [InlineData(new[] { "-e" }, true)]
        [InlineData(new[] { "-e=false" }, false)]
        [InlineData(new[] { "--enabled=true" }, true)]
        public void BoolFlags_ParsesCorrectly(string[] args, bool expected)
        {
            var options = new BoolFlags();
            CommandLineParser.Parse(options, args);
            Assert.Equal(expected, options.Enabled);
        }
    }

}
