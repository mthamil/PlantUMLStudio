using System.ComponentModel;
using System.Windows.Media;
using PlantUmlEditor.Model;
using Utilities.Mvvm;
using Utilities.PropertyChanged;
using Utilities.Reflection;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a diagram preview.
	/// </summary>
	public class PreviewDiagramViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new diagram preview.
		/// </summary>
		/// <param name="diagram">The underlying diagram</param>
		public PreviewDiagramViewModel(Diagram diagram)
		{
			Diagram = diagram;

			_imagePreview = Property.New(this, p => p.ImagePreview, OnPropertyChanged);
			_codePreview = Property.New(this, p => p.CodePreview, OnPropertyChanged);

			CodePreview = CreatePreview(Diagram.Content);
			Diagram.PropertyChanged += Diagram_PropertyChanged;
		}

		/// <summary>
		/// The diagram image preview.
		/// </summary>
		public ImageSource ImagePreview
		{
			get { return _imagePreview.Value; }
			set { _imagePreview.Value = value; }
		}

		/// <summary>
		/// The diagram code preview.
		/// </summary>
		public string CodePreview
		{
			get { return _codePreview.Value; }
			set { _codePreview.Value = value; }
		}

		private static string CreatePreview(string content)
		{
			// Ignore first @startuml line and select non-empty lines
			return content.Length > 100 ? content.Substring(0, 100) : content;
		}

		void Diagram_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == contentPropertyName)
				CodePreview = CreatePreview(Diagram.Content);
		}
		private static readonly string contentPropertyName = Reflect.PropertyOf<Diagram, string>(p => p.Content).Name;

		/// <summary>
		/// The underlying diagram.
		/// </summary>
		public Diagram Diagram { get; private set; }

		private readonly Property<ImageSource> _imagePreview;
		private readonly Property<string> _codePreview;
	}
}