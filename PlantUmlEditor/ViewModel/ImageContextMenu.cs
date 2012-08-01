using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PlantUmlEditor.Properties;
using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Contains operations available on a diagram image.
	/// </summary>
	public class ImageContextMenu : IEnumerable<ICommand>
	{
		public ImageContextMenu()
		{
			_commands = new List<NamedRelayCommand<IDiagramEditor>>
			{
				new NamedRelayCommand<IDiagramEditor>(d => Clipboard.SetImage(d.DiagramImage as BitmapSource))
				{
					Name = Resources.ContextMenu_Image_CopyToClipboard
				},

				new NamedRelayCommand<IDiagramEditor>(d => Process.Start("explorer.exe", "/select," + d.Diagram.ImageFilePath).Dispose())
				{
					Name = Resources.ContextMenu_Image_OpenInExplorer
				},
						
				new NamedRelayCommand<IDiagramEditor>(d => Clipboard.SetText(d.Diagram.ImageFilePath)) 
				{ 
					Name = Resources.ContextMenu_Image_CopyImagePath 
				}
			};
		}

		#region Implementation of IEnumerable

		/// <see cref="IEnumerable{T}.GetEnumerator"/>
		public IEnumerator<ICommand> GetEnumerator()
		{
			return _commands.GetEnumerator();
		}

		/// <see cref="IEnumerable.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		private readonly IEnumerable<ICommand> _commands;
	}
}