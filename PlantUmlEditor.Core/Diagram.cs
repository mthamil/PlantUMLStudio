using System;
using System.IO;
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
		/// Tje file path of the diagram's compiled image output.
		/// </summary>
        public string ImageFilePath { get; set; }

		/// <summary>
		/// Just the diagram image's file name.
		/// </summary>
		public string ImageFileName
		{
			get { return Path.GetFileName(ImageFilePath); }
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

            return other.File.FullName == File.FullName;
        }

		/// <see cref="object.GetHashCode"/>
        public override int GetHashCode()
        {
            return File.FullName.GetHashCode();
        }

		private readonly Property<string> _content;
    }
}
