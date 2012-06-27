using System;
using System.Windows.Media;
using PlantUmlEditor.Converters;
using PlantUmlEditor.Model;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a diagram.
	/// </summary>
	public class DiagramViewModel : ViewModelBase
	{
		public DiagramViewModel()
		{
			_diagram = Property.New(this, p => p.Diagram, OnPropertyChanged);
			_diagramImage = Property.New(this, p => DiagramImage, OnPropertyChanged);
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
			set { _diagram.Value = value; }
		}

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
	}
}