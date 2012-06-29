using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utilities.Mvvm;
using Utilities.PropertyChanged;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a diagram code editor.
	/// </summary>
	public class CodeEditorViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new code editor.
		/// </summary>
		public CodeEditorViewModel(IEnumerable<ViewModelBase> editorCommands)
		{
			_content = Property.New(this, p => p.Content, OnPropertyChanged);

			_contentIndex = Property.New(this, p => p.ContentIndex, OnPropertyChanged);
			_contentIndex.Value = 0;

			_isModified = Property.New(this, p => IsModified, OnPropertyChanged);

			_editorCommands = Property.New(this, p => p.EditorCommands, OnPropertyChanged);
			EditorCommands = new ObservableCollection<ViewModelBase>(editorCommands);
		}

		/// <summary>
		/// Available operations for a code editor.
		/// </summary>
		public ICollection<ViewModelBase> EditorCommands
		{
			get { return _editorCommands.Value; }
			private set { _editorCommands.Value = value; }
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

		private readonly Property<ICollection<ViewModelBase>> _editorCommands;
	}
}