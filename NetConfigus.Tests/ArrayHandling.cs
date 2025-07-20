using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConfigus.Tests
{
    // Группа 4: Работа с массивами
    public class ArrayHandling
    {
        public class StringArray
        {
            [CommandLine(ShortName = 'i')]
            public string[] Inputs { get; set; }
        }

        [Fact]
        public void SingleArrayElement_SetsCorrectly()
        {
            var options = new StringArray();
            CommandLineParser.Parse(options, new[] { "-i=file1.txt" });
            Assert.Equal(new[] { "file1.txt" }, options.Inputs);
        }

        [Fact]
        public void MultipleArrayElements_AppendsCorrectly()
        {
            var options = new StringArray();
            CommandLineParser.Parse(options, new[] { "-i=dir1", "-i=dir2", "-i=dir3" });
            Assert.Equal(new[] { "dir1", "dir2", "dir3" }, options.Inputs);
        }

        public class IntArray
        {
            [CommandLine(LongName = "values")]
            public int[] Values { get; set; }
        }

        [Fact]
        public void IntArray_ParsesCorrectly()
        {
            var options = new IntArray();
            CommandLineParser.Parse(options, new[] { "--values=1", "--values=2", "--values=3" });
            Assert.Equal(new[] { 1, 2, 3 }, options.Values);
        }
    }

}
