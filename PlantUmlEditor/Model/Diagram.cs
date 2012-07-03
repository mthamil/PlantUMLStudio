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
		}

		/// <summary>
		/// The diagram's full file path.
		/// </summary>
        public string DiagramFilePath { get; set; }

		/// <summary>
		/// Just the diagram's file name.
		/// </summary>
		public string DiagramFileNameOnly
		{
			get { return Path.GetFileName(DiagramFilePath); }
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
			set { _content.Value = value; }
		}

		/// <see cref="object.Equals(object)"/>
        public override bool Equals(object obj)
        {
            var other = obj as Diagram;
			if (other == null)
				return false;

            return other.DiagramFilePath == DiagramFilePath;
        }

		/// <see cref="object.GetHashCode"/>
        public override int GetHashCode()
        {
            return DiagramFilePath.GetHashCode();
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
