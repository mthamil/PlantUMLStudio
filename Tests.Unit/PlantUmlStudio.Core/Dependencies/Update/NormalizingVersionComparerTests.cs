using System.Collections.Generic;
using PlantUmlStudio.Core.Dependencies.Update;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.Core.Dependencies.Update
{
    public class NormalizingVersionComparerTests
    {
        [Theory]
        [InlineData("2.3.4", "2.3.4", true)]
        [InlineData("2.3",   "2.3.0", true)]
        [InlineData("2",     "2.0",   true)]
        [InlineData("2",     "2.0.0", true)]
        [InlineData("2.0",   "2.0.0", true)]
        [InlineData(".3.4",  "0.3.4", true)]
        [InlineData("2.3.5", "2.3.4", false)]
        [InlineData("2.1.4", "2.3.4", false)]
        [InlineData("1.3.4", "2.3.4", false)]
        [InlineData("2.3.4", "",      false)]
        public void Test_Equals(string version1, string version2, bool expected)
        {
            // Act.
            var actual = _underTest.Equals(version1, version2);

            // Assert.
            Assert.Equal(expected, actual);
        }

        private readonly NormalizingVersionComparer _underTest = new NormalizingVersionComparer();
    }
}