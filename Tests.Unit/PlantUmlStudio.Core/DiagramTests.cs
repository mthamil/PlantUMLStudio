﻿using System.IO;
using System.Text;
using PlantUmlStudio.Core;
using PlantUmlStudio.Core.Imaging;
using SharpEssentials.Testing;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.Core
{
	public class DiagramTests
	{
        [Fact]
        public void Test_Default_Encoding_Is_Utf8()
        {
            // Arrange.
            var diagram = new Diagram();

            // Act.
            var actual = diagram.Encoding;

            // Assert.
            Assert.Equal(Encoding.UTF8, actual);
        }

        [Fact]
		public void Test_PNG_ImageFormat_Detection()
		{
			// Arrange.
			var diagram = new Diagram();

			// Act.
			diagram.ImageFile = new FileInfo("image.png");

			// Assert.
			Assert.Equal(ImageFormat.PNG, diagram.ImageFormat);
		}

		[Fact]
		public void Test_SVG_ImageFormat_Detection()
		{
			// Arrange.
			var diagram = new Diagram();

			// Act.
			diagram.ImageFile = new FileInfo("image.svg");

			// Assert.
			Assert.Equal(ImageFormat.SVG, diagram.ImageFormat);
		}

		[Fact]
		public void Test_ImageFormat_Default()
		{
			// Arrange.
			var diagram = new Diagram();

			// Act.
			diagram.ImageFile = new FileInfo("image");

			// Assert.
			Assert.Equal(ImageFormat.PNG, diagram.ImageFormat);
		}

		[Fact]
		public void Test_Content_RaisesPropertyChange()
		{
			// Arrange.
			var diagram = new Diagram();

			// Act/Assert.
			AssertThat.PropertyChanged(diagram, p => p.Content, () => diagram.Content = "blargh");
		}

		[Fact]
		public void Test_ImageFile_RaisesPropertyChange()
		{
			// Arrange.
			var diagram = new Diagram
			{
				ImageFile = new FileInfo("image.svg")
			};

			// Act/Assert.
			AssertThat.PropertyChanged(diagram, p => p.ImageFile, () => diagram.ImageFile = new FileInfo("image.png"));
		}

		[Theory]
		[InlineData(true, "image2.svg", @"
				@startuml image2.svg

				title Class Diagram")]
		[InlineData(false, "image.png", "title Class Diagram")]
		[InlineData(false, "image.png", "")]
		public void Test_TryDeduceImageFile(bool expected, string expectedImageFileName, string content)
		{
			// Arrange.
			var diagram = new Diagram
			{
				File = new FileInfo("diagram.puml"),
				ImageFile = new FileInfo("image.png")
			};

			diagram.Content = content;

			// Act.
			bool actual = diagram.TryDeduceImageFile();

			// Assert.
			Assert.Equal(expected, actual);
			Assert.Equal(expectedImageFileName, diagram.ImageFile.Name);
		}
	}
}