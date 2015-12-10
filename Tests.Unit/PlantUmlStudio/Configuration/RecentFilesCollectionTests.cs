using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlantUmlStudio.Configuration;
using SharpEssentials.InputOutput;
using SharpEssentials.Testing;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.Configuration
{
	public class RecentFilesCollectionTests
	{
		[Theory]
		[InlineData(new[] { @"C:\newFile" },				  new[] { @"C:\newFile" }, 2)]
		[InlineData(new[] { @"C:\newFile2", @"C:\newFile1" }, new[] { @"C:\newFile1", @"C:\newFile2" }, 2)]
		[InlineData(new[] { @"C:\newFile3", @"C:\newFile2" }, new[] { @"C:\newFile1", @"C:\newFile2", @"C:\newFile3" }, 2)]
		[InlineData(new string[0],							  new[] { @"C:\newFile" }, 0)]
		public void Test_Add(IEnumerable<string> expected, IEnumerable<string> inputPaths, int maximum)
		{
			// Arrange.
			var expectedFiles = expected.Select(fileName => new FileInfo(fileName));
			var inputFiles = inputPaths.Select(fileName => new FileInfo(fileName));
			recentFiles.MaximumCount = maximum;

			// Act.
			foreach (var inputFile in inputFiles)
				recentFiles.Add(inputFile);
			
			// Assert.
			AssertThat.SequenceEqual(expectedFiles, recentFiles, FileSystemInfoPathEqualityComparer.Instance);
		}

		[Fact]
		public void Test_Add_MovesAlreadyExistingFileToTop()
		{
			// Arrange.
			recentFiles.MaximumCount = 10;
			recentFiles.Add(new FileInfo(@"C:\file1"));
			recentFiles.Add(new FileInfo(@"C:\file2"));

			// Act.
			recentFiles.Add(new FileInfo(@"C:\file1"));

			// Assert.
			AssertThat.SequenceEqual(new[] { @"C:\file1", @"C:\file2" }, recentFiles.Select(f => f.FullName));
		}

		[Fact]
		public void Test_RecentFilesTrimmed_WhenMaximumCountChanged()
		{
			// Arrange.
			recentFiles.MaximumCount = 3;
			foreach (var recentFile in new[] { @"C:\file1", @"C:\file2", @"C:\file3" })
				recentFiles.Add(new FileInfo(recentFile));

			// Act.
			recentFiles.MaximumCount = 1;

			// Assert.
			AssertThat.SequenceEqual(new[] { new FileInfo(@"C:\file3") }, recentFiles, FileSystemInfoPathEqualityComparer.Instance);
		}

		[Fact]
		public void Test_MaximumCount_Changes()
		{
			// Arrange.
			recentFiles.MaximumCount = 0;

			// Act/Assert.
			AssertThat.PropertyChanged(recentFiles,
				r => r.MaximumCount,
				() => recentFiles.MaximumCount = 15);

			Assert.Equal(15, recentFiles.MaximumCount);
		}

		private readonly RecentFilesCollection recentFiles = new RecentFilesCollection();
	}
}