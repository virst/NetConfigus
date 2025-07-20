using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConfigus.Tests
{
    // Группа 7: Валидация атрибутов
    public class AttributeValidation
    {
        public class InvalidBoolPosition
        {
            [CommandLine(Position = 0)]
            public bool Flag { get; set; }
        }

        [Fact]
        public void BoolWithPosition_ThrowsOnParse()
        {
            var options = new InvalidBoolPosition();
            Assert.Throws<InvalidOperationException>(() =>
                CommandLineParser.Parse(options, new[] { "true" }));
        }

        public class DuplicatePositions
        {
            [CommandLine(Position = 0)]
            public string A { get; set; }

            [CommandLine(Position = 0)]
            public string B { get; set; }
        }

        [Fact]
        public void DuplicatePositions_ThrowsOnParse()
        {
            var options = new DuplicatePositions();
            Assert.Throws<InvalidOperationException>(() =>
                CommandLineParser.Parse(options, new[] { "value" }));
        }
    }

}
