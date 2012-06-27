using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a diagram code editor.
	/// </summary>
	public class CodeEditorViewModel : ViewModelBase
	{
		public CodeEditorViewModel()
		{
			_content = Property.New(this, p => p.Content, OnPropertyChanged);

			_contentIndex = Property.New(this, p => p.ContentIndex, OnPropertyChanged);
			_contentIndex.Value = 0;

			_isModified = Property.New(this, p => IsModified, OnPropertyChanged);
		}

		/// <summary>
		/// The content being edited.
		/// </summary>
		public string Content
		{
			get { return _content.Value; }
			set
			{
				if (_content.TrySetValue(value))
				{
					if (_originalContent != null)
						IsModified = true;//!Equals(_content.Value, _originalContent);	// could be perf issue
					else
						_originalContent = _content.Value;
				}
			}
		}

		/// <summary>
		/// The current index into the content.
		/// </summary>
		public int ContentIndex
		{
			get { return _contentIndex.Value; }
			set { _contentIndex.Value = value; }
		}

		/// <summary>
		/// Whether content has been modified since the last save.
		/// </summary>
		public bool IsModified
		{
			get { return _isModified.Value; }
			set { _isModified.Value = value; }
		}

		private string _originalContent;

		private readonly Property<string> _content;
		private readonly Property<int> _contentIndex;
		private readonly Property<bool> _isModified;
	}
}