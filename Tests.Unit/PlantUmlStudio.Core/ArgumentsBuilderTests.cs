using System.IO;
using PlantUmlStudio.Core;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.Core
{
    public class ArgumentsBuilderTests
    {
        [Fact]
        public void Test_Arg()
        {
            // Arrange.
            var args = new ArgumentsBuilder()
                            .Arg("flag", "value");

            // Act.
            var actual = args.ToString();

            // Assert.
            Assert.Equal("-flag value", actual);
        }

        [Fact]
        public void Test_Arg_With_Nondefault_Prefix()
        {
            // Arrange.
            var args = new ArgumentsBuilder("/")
                            .Arg("flag", "value");

            // Act.
            var actual = args.ToString();

            // Assert.
            Assert.Equal("/flag value", actual);
        }

        [Fact]
        public void Test_Arg_Without_Value()
        {
            // Arrange.
            var args = new ArgumentsBuilder()
                            .Arg("flag");

            // Act.
            var actual = args.ToString();

            // Assert.
            Assert.Equal("-flag", actual);
        }

        [Fact]
        public void Test_Value()
        {
            // Arrange.
            var args = new ArgumentsBuilder()
                            .Value("value");

            // Act.
            var actual = args.ToString();

            // Assert.
            Assert.Equal("value", actual);
        }

        [Theory]
        [InlineData(true, "-flag value")]
        [InlineData(false, "")]
        public void Test_ArgIf(bool precondition, string expected)
        {
            // Arrange.
            var args = new ArgumentsBuilder()
                .ArgIf(precondition, "flag", "value");

            // Act.
            var actual = args.ToString();

            // Assert.
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test_Arg_With_File()
        {
            // Arrange.
            var args = new ArgumentsBuilder()
                            .Arg("flag", new FileInfo(@"C:\test\testfile.txt"));

            // Act.
            var actual = args.ToString();

            // Assert.
            Assert.Equal(@"-flag ""C:\test\testfile.txt""", actual);
        }

        [Fact]
        public void Test_Value_With_File()
        {
            // Arrange.
            var args = new ArgumentsBuilder()
                            .Value(new FileInfo(@"C:\test\testfile.txt"));

            // Act.
            var actual = args.ToString();

            // Assert.
            Assert.Equal(@"""C:\test\testfile.txt""", actual);
        }

        [Fact]
        public void Test_Complex_Args()
        {
            // Arrange.
            var args = new ArgumentsBuilder()
                            .Arg("flag1", "a")
                            .ArgIf(false, "flag2", "b")
                            .Arg("flag3", new FileInfo(@"C:\test\testfile.txt"))
                            .Arg("flag4")
                            .ArgIf(true, "flag5", "c")
                            .Value("d");

            // Act.
            var actual = args.ToString();

            // Assert.
            Assert.Equal(@"-flag1 a -flag3 ""C:\test\testfile.txt"" -flag4 -flag5 c d", actual);
        }
    }
}