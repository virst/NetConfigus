namespace NetConfigus.Tests
{
    public class FilesAndEnvironment
    {
        #region Вспомогательные классы для тестов
        public class TestConfig
        {
            [CommandLine(LongName = "stringValue", EnvironmentVariableName = "ENV_OVERRIDE")]
            public string StringValue { get; set; } = "default";

            [CommandLine(LongName = "intValue")]
            public int IntValue { get; set; }
        }

        public class PositionalConfig
        {
            [CommandLine(Position = 0, Required = true)]
            public string Arg1 { get; set; } = "";

            [CommandLine(Position = 1)]
            public string Arg2 { get; set; } = "";
        }

        public class ArrayConfig
        {
            [CommandLine(LongName = "array")]
            public string[] ArrayValue { get; set; } = Array.Empty<string>();
        }

        public class RequiredConfig
        {
            [CommandLine(Required = true, LongName = "requiredProp")]
            public string RequiredProperty { get; set; } = "";
        }
        #endregion

        [Fact]
        public void Load_WithConfigFile_ShouldLoadValues()
        {
            // Arrange
            var configFile = Path.GetTempFileName();
            File.WriteAllText(configFile, "{\"StringValue\":\"fileValue\", \"IntValue\": 42}");

            // Act
            TestConfig config = CommandLineParser.Load<TestConfig>(configFile, null);

            // Assert
            Assert.Equal("fileValue", config.StringValue);
            Assert.Equal(42, config.IntValue);
            File.Delete(configFile);
        }

        [Fact]
        public void Load_WithEnvironmentVariable_ShouldOverrideFile()
        {
            // Arrange
            var configFile = Path.GetTempFileName();
            File.WriteAllText(configFile, "{\"StringValue\":\"fileValue\"}");
            Environment.SetEnvironmentVariable("ENV_OVERRIDE", "envValue");

            // Act
            TestConfig config = CommandLineParser.Load<TestConfig>(configFile, null);

            // Assert
            Assert.Equal("envValue", config.StringValue);
            File.Delete(configFile);
            Environment.SetEnvironmentVariable("ENV_OVERRIDE", null);
        }

        [Fact]
        public void Load_WithCommandLineArgs_ShouldOverrideEnvironment()
        {
            // Arrange
            Environment.SetEnvironmentVariable("ENV_VAR", "envValue");
            string[] args = ["--stringValue", "cliValue"];

            // Act
            TestConfig config = CommandLineParser.Load<TestConfig>(null, args);

            // Assert
            Assert.Equal("cliValue", config.StringValue);
            Environment.SetEnvironmentVariable("ENV_VAR", null);
        }

        [Fact]
        public void Load_RequiredFileMissing_ShouldThrowException()
        {
            // Arrange
            string missingFile = "non_existent.json";

            // Act & Assert
            Assert.Throws<Exception>(() =>
                CommandLineParser.Load<TestConfig>(missingFile, null, isConfigFileRequired: true));
        }

        [Fact]
        public void Load_WithPositionalArgs_ShouldSetValues()
        {
            // Arrange
            string[] args = ["posValue1", "posValue2"];

            // Act
            var config = CommandLineParser.Load<PositionalConfig>(null, args);

            // Assert
            Assert.Equal("posValue1", config.Arg1);
            Assert.Equal("posValue2", config.Arg2);
        }

        [Fact]
        public void Load_WithArrayValues_ShouldAppendElements()
        {
            // Arrange
            string[] args = ["--array", "val1", "--array=val2"];

            // Act
            var config = CommandLineParser.Load<ArrayConfig>(null, args);

            // Assert
            Assert.Equal(["val1", "val2"], config.ArrayValue);
        }

        [Fact]
        public void Load_MissingRequiredProperty_ShouldThrow()
        {
            // Arrange
            string[] args = Array.Empty<string>();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                CommandLineParser.Load<RequiredConfig>(null, args));
            Assert.Contains("Required property", ex.Message);
        }

    }
}
