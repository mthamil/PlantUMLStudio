using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Control = System.Windows.Controls.Control;

namespace Utilities.Controls
{
	/// <summary>
	/// Encapsulates a file dialog for use by MVVM.
	/// </summary>
	[DesignTimeVisible(false)]
	public class FilePicker : Control
	{
		/// <summary>
        /// Initializes the control.
        /// </summary>
		public FilePicker()
        {
            // The control doesn't have any specific rendering of its own.
            Visibility = Visibility.Hidden;
        }

		/// <summary>
		/// Property to trigger the prompt for a file.
		/// </summary>
		public bool Trigger
		{
			get { return (bool)GetValue(TriggerProperty); }
			set { SetValue(TriggerProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for "Trigger". This also overrides the PropertyChangedCallback to trigger the message box display.
		/// </summary>
		public static readonly DependencyProperty TriggerProperty = 
			DependencyProperty.Register("Trigger", 
			typeof(bool), 
			typeof(FilePicker),
			new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTriggerChanged)));

		/// <summary>
		/// The "Trigger" propery changed override. Whenever the "Trigger" property changes to true or false this will be executed.
		/// When the property changes to true, the message box will be shown.
		/// </summary>
		private static void OnTriggerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var filePicker = (FilePicker)dependencyObject;
			if (!filePicker.Trigger) 
				return;

			switch (filePicker.Mode)
			{
				case FilePickerMode.Directory:
					using (var folderDialog = new FolderBrowserDialog())
					{
						folderDialog.ShowNewFolderButton = true;
						folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
						if (filePicker.InitialLocationUri.IsAbsoluteUri)
							folderDialog.SelectedPath = filePicker.InitialLocationUri.LocalPath;
						var result = folderDialog.ShowDialog();
						if (result == DialogResult.OK)
						{
							filePicker.SelectedUri = new Uri(folderDialog.SelectedPath, UriKind.Absolute);
							if (filePicker.AffirmativeCommand != null && filePicker.AffirmativeCommand.CanExecute(filePicker.SelectedUri))
								filePicker.AffirmativeCommand.Execute(filePicker.SelectedUri);
						}
					}
					break;

				case FilePickerMode.Open:
				case FilePickerMode.Save:
					using (var fileDialog = filePicker.Mode == FilePickerMode.Save ? new SaveFileDialog() : (FileDialog)new OpenFileDialog())
					{
						if (filePicker.InitialLocationUri.IsAbsoluteUri)
							fileDialog.InitialDirectory = filePicker.InitialLocationUri.LocalPath;
						fileDialog.Filter = filePicker.Filter;
						var result = fileDialog.ShowDialog();
						if (result == DialogResult.OK)
						{
							filePicker.SelectedUri = new Uri(fileDialog.FileName, UriKind.Absolute);
							if (filePicker.AffirmativeCommand != null && filePicker.AffirmativeCommand.CanExecute(filePicker.SelectedUri))
								filePicker.AffirmativeCommand.Execute(filePicker.SelectedUri);
						}
					}
					break;
			}
		}

		/// <summary>
		/// The URI of the file chosen.
		/// </summary>
		public Uri SelectedUri
		{
			get { return (Uri)GetValue(SelectedUriProperty); }
			set { SetValue(SelectedUriProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for the URI of the file chosen.
		/// </summary>
		public static readonly DependencyProperty SelectedUriProperty =
			DependencyProperty.Register("SelectedUri",
			typeof(Uri),
			typeof(FilePicker),
			new FrameworkPropertyMetadata(null));

		/// <summary>
		/// The URI of the initial directory.
		/// </summary>
		public Uri InitialLocationUri
		{
			get { return (Uri)GetValue(InitialLocationUriProperty); }
			set { SetValue(InitialLocationUriProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for the URI of the initial directory.
		/// </summary>
		public static readonly DependencyProperty InitialLocationUriProperty =
			DependencyProperty.Register("InitialLocationUri",
			typeof(Uri),
			typeof(FilePicker),
			new FrameworkPropertyMetadata(null));

		/// <summary>
		/// The file filter.
		/// </summary>
		public string Filter
		{
			get { return (string)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for the file filter.
		/// </summary>
		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register("Filter",
			typeof(string),
			typeof(FilePicker));

		/// <summary>
		/// The picker mode.
		/// </summary>
		public FilePickerMode Mode
		{
			get { return (FilePickerMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for the URI of the file chosen.
		/// </summary>
		public static readonly DependencyProperty ModeProperty =
			DependencyProperty.Register("Mode",
			typeof(FilePickerMode),
			typeof(FilePicker));

		/// <summary>
		/// The command to be executed when an affirmativee choice is made.
		/// The selected URI is passed as the command parameter.
		/// </summary>
		public ICommand AffirmativeCommand
		{
			get { return (ICommand)GetValue(AffirmativeCommandProperty); }
			set { SetValue(AffirmativeCommandProperty, value); }
		}

		/// <summary>
		/// The affirmative command property.
		/// </summary>
		public static readonly DependencyProperty AffirmativeCommandProperty =
			DependencyProperty.RegisterAttached(
			"AffirmativeCommand",
			typeof(ICommand),
			typeof(FilePicker));
	}

	/// <summary>
	/// Determines the file picker mode.
	/// </summary>
	public enum FilePickerMode
	{
		/// <summary>
		/// The file is being chosen for reading.
		/// </summary>
		Open,

		/// <summary>
		/// The file is being chosen for writing.
		/// </summary>
		Save,

		/// <summary>
		/// A directory is being selected.
		/// </summary>
		Directory
	}
}