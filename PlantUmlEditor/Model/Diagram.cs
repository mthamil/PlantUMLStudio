using System;
using System.ComponentModel;
using System.IO;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Represents a diagram.
	/// </summary>
    public class Diagram : INotifyPropertyChanged
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
		public string DiagramFileNameOnly
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
		public string ImageFileNameOnly
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

		#region Implementation of INotifyPropertyChanged

		/// <see cref="INotifyPropertyChanged.PropertyChanged"/>
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			var localEvent = PropertyChanged;
			if (localEvent != null)
				localEvent(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		private readonly Property<string> _content;
    }
}
