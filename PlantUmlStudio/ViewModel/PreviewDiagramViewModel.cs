//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using PlantUmlStudio.Core;
using SharpEssentials.Controls.Mvvm;
using SharpEssentials.Observable;

namespace PlantUmlStudio.ViewModel
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
			// Select first few lines, but skip initial whitespace.
			var lines = content.Trim().Split(Delimiters, MaxPreviewLines + 1);
			return String.Join("\n", lines.Take(Math.Min(MaxPreviewLines, lines.Length)));
		}

		void Diagram_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Diagram.Content))
				CodePreview = CreatePreview(Diagram.Content);
		}

		/// <summary>
		/// The underlying diagram.
		/// </summary>
		public Diagram Diagram { get; }

		private readonly Property<ImageSource> _imagePreview;
		private readonly Property<string> _codePreview;
		private static readonly char[] Delimiters = { '\n' };
		private const int MaxPreviewLines = 5;
	}
}