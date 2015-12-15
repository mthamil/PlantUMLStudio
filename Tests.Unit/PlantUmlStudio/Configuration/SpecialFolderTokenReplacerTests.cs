using System;
using PlantUmlStudio.Configuration;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.Configuration
{
    public class SpecialFolderTokenReplacerTests
    {
        [Theory]
        [InlineData(@"<ProgramFilesX86>\test\path", @"C:\Program Files (x86)\test\path")]
        [InlineData(@"<Windows>\test\path",         @"C:\Windows\test\path")]
        public void Test_Parse_With_System_Special_Folders(string input, string expected)
        {
            // Act.
            var actual = _underTest.Parse(input);

            // Assert.
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(@"<MyDocuments>\test\path", Environment.SpecialFolder.MyDocuments)]
        [InlineData(@"<UserProfile>\test\path", Environment.SpecialFolder.UserProfile)]
        public void Test_Parse_With_User_Special_Folders(string input, Environment.SpecialFolder expectedFolder)
        {
            // Act.
            var actual = _underTest.Parse(input);

            // Assert.
            Assert.Equal($@"{Environment.GetFolderPath(expectedFolder)}\test\path", actual);
        }

        [Theory]
        [InlineData(@"C:\Program Files\test\path", @"C:\Program Files\test\path")]
        [InlineData(@"C:\",                        @"C:\")]
        [InlineData(@"C:\<WeirdFolder>\stuff",     @"C:\<WeirdFolder>\stuff")]
        [InlineData(@"C:\WeirdFolder>\stuff",      @"C:\WeirdFolder>\stuff")]
        [InlineData(@"C:\<WeirdFolder\stuff",      @"C:\<WeirdFolder\stuff")]
        [InlineData(@"<>",                         @"<>")]
        [InlineData(@"",                           @"")]
        public void Test_Parse_With_No_Special_Folders(string input, string expected)
        {
            // Act.
            var actual = _underTest.Parse(input);

            // Assert.
            Assert.Equal(expected, actual);
        }

        private readonly SpecialFolderTokenReplacer _underTest = new SpecialFolderTokenReplacer();
    }
}