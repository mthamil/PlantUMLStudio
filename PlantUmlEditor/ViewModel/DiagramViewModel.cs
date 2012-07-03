using System.ComponentModel;
using System.Windows.Media;
using PlantUmlEditor.Model;
using Utilities.Mvvm;
using Utilities.PropertyChanged;
using Utilities.Reflection;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a diagram.
	/// </summary>
	public class DiagramViewModel : ViewModelBase
	{
		public DiagramViewModel(Diagram diagram)
		{
			_diagram = Property.New(this, p => p.Diagram, OnPropertyChanged);
			Diagram = diagram;
			
			_diagramImage = Property.New(this, p => DiagramImage, OnPropertyChanged);
			_preview = Property.New(this, p => p.Preview, OnPropertyChanged);

			Preview = CreatePreview(Diagram.Content);
			Diagram.PropertyChanged += Diagram_PropertyChanged;
		}

		/// <summary>
		/// The rendered diagram image.
		/// </summary>
		public ImageSource DiagramImage
		{
			get { return _diagramImage.Value; }
			set { _diagramImage.Value = value; }
		}

		/// <summary>
		/// The underlying diagram.
		/// </summary>
		public Diagram Diagram
		{
			get { return _diagram.Value; }
			private set { _diagram.Value = value; }
		}

		/// <summary>
		/// A preview of part of a diagram's content.
		/// </summary>
		public string Preview
		{
			get { return _preview.Value; }
			set { _preview.Value = value; }
		}

		private static string CreatePreview(string content)
		{
			// Ignore first @startuml line and select non-empty lines
			return content.Length > 100 ? content.Substring(0, 100) : content;
		}

		void Diagram_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == contentPropertyName)
				Preview = CreatePreview(Diagram.Content);
		}
		private static readonly string contentPropertyName = Reflect.PropertyOf<Diagram, string>(p => p.Content).Name;

		/// <see cref="object.Equals(object)"/>
		public override bool Equals(object obj)
		{
			var other = obj as DiagramViewModel;
			if (other == null)
				return false;

			return Equals(Diagram, other.Diagram);
		}

		/// <see cref="object.GetHashCode"/>
		public override int GetHashCode()
		{
			return Diagram.GetHashCode();
		}

		private readonly Property<Diagram> _diagram;
		private readonly Property<ImageSource> _diagramImage;
		private readonly Property<string> _preview;
	}
}