//  PlantUML Editor
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
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Control = System.Windows.Controls.Control;

namespace Utilities.Controls
{
	/// <summary>
	/// Encapsulates a file system browser dialog for use in XAML.
	/// </summary>
	[DesignTimeVisible(false)]
	public class FileSystemPicker : Control
	{
		/// <summary>
		/// Initializes the control.
		/// </summary>
		public FileSystemPicker()
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
				typeof(FileSystemPicker),
				new FrameworkPropertyMetadata(OnTriggerChanged));

		/// <summary>
		/// The <see cref="Trigger"/> property changed handler. Whenever this property changes the handler will be executed.
		/// When the property changes to true, a dialog will be shown.
		/// </summary>
		private static void OnTriggerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var filePicker = (FileSystemPicker)dependencyObject;
			if (!filePicker.Trigger)
				return;

			switch (filePicker.Mode)
			{
				case FilePickerMode.Directory:
					using (var folderDialog = new FolderBrowserDialog())
					{
						folderDialog.ShowNewFolderButton = true;
						folderDialog.RootFolder = Environment.SpecialFolder.Desktop;
						if (filePicker.InitialLocation != null)
							folderDialog.SelectedPath = filePicker.InitialLocation.FullName;

						ShowDialog(filePicker, folderDialog, d => new DirectoryInfo(d.SelectedPath));
					}
					break;

				case FilePickerMode.Open:
				case FilePickerMode.Save:
					using (var fileDialog = filePicker.Mode == FilePickerMode.Save ? new SaveFileDialog() : (FileDialog)new OpenFileDialog())
					{
						if (filePicker.InitialLocation != null)
							fileDialog.InitialDirectory = filePicker.InitialLocation.FullName;

						if (filePicker.InitialFileName != null)
							fileDialog.FileName = filePicker.InitialFileName;

						fileDialog.Filter = filePicker.Filter;
						fileDialog.AddExtension = true;

						ShowDialog(filePicker, fileDialog, d => new FileInfo(d.FileName));
					}
					break;

				default:
					throw new InvalidOperationException(String.Format("{0}.{1} is unsupported.", typeof(FilePickerMode).Name, filePicker.Mode));
			}
		}

		private static void ShowDialog<TDialog, TFileSystemInfo>(FileSystemPicker filePicker, TDialog dialog, Func<TDialog, TFileSystemInfo> resultRetriever)
			where TDialog : CommonDialog 
			where TFileSystemInfo : FileSystemInfo
		{
			var result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				filePicker.SelectedPath = resultRetriever(dialog);
				var resultConverter = filePicker.GetConverter();
				var convertedResult = resultConverter(filePicker.SelectedPath);
				if (filePicker.AffirmativeCommand != null && filePicker.AffirmativeCommand.CanExecute(convertedResult))
					filePicker.AffirmativeCommand.Execute(convertedResult);
			}
		}

		/// <summary>
		/// The selected file system path.
		/// </summary>
		public FileSystemInfo SelectedPath
		{
			get { return (FileSystemInfo)GetValue(SelectedPathProperty); }
			set { SetValue(SelectedPathProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for the chosen file system path.
		/// </summary>
		public static readonly DependencyProperty SelectedPathProperty =
			DependencyProperty.Register("SelectedPath",
				typeof(FileSystemInfo),
				typeof(FileSystemPicker),
				new FrameworkPropertyMetadata(null));

		/// <summary>
		/// The initial file name.
		/// </summary>
		public string InitialFileName
		{
			get { return (string)GetValue(InitialFileNameProperty); }
			set { SetValue(InitialFileNameProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for the initial file name.
		/// </summary>
		public static readonly DependencyProperty InitialFileNameProperty =
			DependencyProperty.Register("InitialFileName",
				typeof(string),
				typeof(FileSystemPicker));

		/// <summary>
		/// The initial directory.
		/// </summary>
		public DirectoryInfo InitialLocation
		{
			get { return (DirectoryInfo)GetValue(InitialLocationProperty); }
			set { SetValue(InitialLocationProperty, value); }
		}

		/// <summary>
		/// DependencyProperty for the initial directory.
		/// </summary>
		public static readonly DependencyProperty InitialLocationProperty =
			DependencyProperty.Register("InitialLocation",
				typeof(DirectoryInfo),
				typeof(FileSystemPicker),
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
				typeof(FileSystemPicker));

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
				typeof(FileSystemPicker));

		/// <summary>
		/// The command to be executed when an affirmative choice is made.
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
			DependencyProperty.Register(
			"AffirmativeCommand",
				typeof(ICommand),
				typeof(FileSystemPicker));

		/// <summary>
		/// An optional value converter to be used on the parameter being passed to <see cref="AffirmativeCommand"/>.
		/// </summary>
		public IValueConverter AffirmativeCommandConverter
		{
			get { return (IValueConverter)GetValue(AffirmativeCommandConverterProperty); }
			set { SetValue(AffirmativeCommandConverterProperty, value); }
		}

		/// <summary>
		/// The AffirmativeCommandConverter dependency property.
		/// </summary>
		public static readonly DependencyProperty AffirmativeCommandConverterProperty =
			DependencyProperty.Register("AffirmativeCommandConverter", 
				typeof(IValueConverter), 
				typeof(FileSystemPicker), 
				new PropertyMetadata(default(IValueConverter)));

		private Func<FileSystemInfo, object> GetConverter()
		{
			var converter = AffirmativeCommandConverter;
			return converter == null
				? new Func<FileSystemInfo, object>(r => r)
				: r => converter.Convert(r, typeof(object), null, CultureInfo.CurrentUICulture);
		}
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