//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using PlantUmlEditor.Core.Imaging;
using Utilities.InputOutput;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.Core
{
	/// <summary>
	/// Represents a diagram.
	/// </summary>
    public class Diagram : PropertyChangedNotifier
    {
		/// <summary>
		/// Initializes a new diagram.
		/// </summary>
		public Diagram()
		{
			_content = Property.New(this, p => p.Content, OnPropertyChanged);
			_imageFile = Property.New(this, p => p.ImageFile, OnPropertyChanged)
			                     .EqualWhen(fileComparer.Equals);

			_content.Value = string.Empty;
		}

		/// <summary>
		/// The diagram file.
		/// </summary>
        public FileInfo File { get; set; }

		/// <summary>
		/// Just the diagram's file name.
		/// </summary>
		public string DiagramFileName
		{
			get { return File.Name; }
		}

		/// <summary>
		/// The file where a diagram's compiled image output is stored.
		/// </summary>
        public FileInfo ImageFile 
		{
			get { return _imageFile.Value; }
			set 
			{
				if (_imageFile.TrySetValue(value))
					ImageFormat = DetermineFormat(ImageFile.FullName);
			}
		}

		/// <summary>
		/// Attempts to analyze a diagram's content and extract the image file path again.
		/// If there is no image file path, no change is made.
		/// </summary>
		/// <returns>True if image file was path was found</returns>
		public bool TryRefreshImageFile()
		{
			var match = diagramImagePathPattern.Match(Content);
			if (match.Success && match.Groups.Count > 1)
			{
				string imageFileName = match.Groups[1].Value;
				var imageFilePath = Path.IsPathRooted(imageFileName)
										? Path.GetFullPath(imageFileName)
										: Path.GetFullPath(Path.Combine(File.DirectoryName, imageFileName));


				ImageFile = new FileInfo(imageFilePath);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Just the diagram image's file name.
		/// </summary>
		public string ImageFileName
		{
			get { return ImageFile.Name; }
		}

		/// <summary>
		/// The format of the diagram's image output.
		/// </summary>
		public ImageFormat ImageFormat { get; set; }

		private ImageFormat DetermineFormat(string imagePath)
		{
			switch (Path.GetExtension(imagePath))
			{
				case ".png":
					return ImageFormat.PNG;
				case ".svg":
					return ImageFormat.SVG;
				default:
					return ImageFormat.PNG;
			}
		}

		/// <summary>
		/// The diagram's content.
		/// </summary>
        public string Content
		{
			get { return _content.Value; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_content.Value = value; 
			}
		}

		/// <see cref="object.Equals(object)"/>
        public override bool Equals(object obj)
        {
            var other = obj as Diagram;
			if (other == null)
				return false;

			return fileComparer.Equals(other.File, File);
        }

		/// <see cref="object.GetHashCode"/>
        public override int GetHashCode()
        {
            return File.FullName.GetHashCode();
        }

		private readonly Property<string> _content;
		private readonly Property<FileInfo> _imageFile;

		private static readonly Regex diagramImagePathPattern = new Regex(@"@startuml\s*(?:"")*([^\r\n""]*)",
			RegexOptions.IgnoreCase |
			RegexOptions.Multiline |
			RegexOptions.IgnorePatternWhitespace |
			RegexOptions.Compiled);

		private static IEqualityComparer<FileInfo> fileComparer = FileInfoPathEqualityComparer.Instance;
    }
}
