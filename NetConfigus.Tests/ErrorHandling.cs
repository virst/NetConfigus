using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConfigus.Tests
{
    // Группа 5: Обработка ошибок
    public class ErrorHandling
    {
        public class RequiredParam
        {
            [CommandLine(Position = 0, Required = true)]
            public string Target { get; set; } = default!;
        }

        [Fact]
        public void MissingRequiredParam_ThrowsArgumentException()
        {
            var options = new RequiredParam();
            Assert.Throws<ArgumentException>(() =>
                CommandLineParser.Parse(options, Array.Empty<string>()));
        }

        public class ConflictParam
        {
            [CommandLine(Position = 0, ShortName = 'f')]
            public string File { get; set; } = default!;

        }

        [Fact]
        public void PositionalAndNamedConflict_ThrowsInvalidOperationException()
        {
            var options = new ConflictParam();
            Assert.Throws<InvalidOperationException>(() =>
                CommandLineParser.Parse(options, new[] { "data.txt", "-f=new.txt" }));
        }

        public class InvalidType
        {
            [CommandLine(ShortName = 'n')]
            public int Number { get; set; }
        }

        [Fact]
        public void InvalidTypeConversion_ThrowsFormatException()
        {
            var options = new InvalidType();
            Assert.Throws<FormatException>(() =>
                CommandLineParser.Parse(options, new[] { "-n=text" }));
        }
    }

}
