using System;
using System.IO;
using PlantUmlEditor.Properties;

namespace PlantUmlEditor.Configuration
{
	/// <summary>
	/// An adapter around the generated .NET Settings class.
	/// </summary>
	public class DotNetSettings : ISettings
	{
		internal DotNetSettings(Settings settings)
		{
			_settings = settings;

			GraphVizExecutable = new FileInfo(_settings.GraphVizLocation);

			LastDiagramLocation = new DirectoryInfo(String.IsNullOrEmpty(_settings.LastPath)
									? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"PlantUmlEditor\samples\")
									: _settings.LastPath);

		}

		/// <see cref="ISettings.GraphVizExecutable"/>
		public FileInfo GraphVizExecutable
		{
			get;
			set;
		}

		/// <see cref="ISettings.LastDiagramLocation"/>
		public DirectoryInfo LastDiagramLocation
		{
			get;
			set;
		}

		/// <see cref="ISettings.Save"/>
		public void Save()
		{
			_settings.LastPath = LastDiagramLocation.FullName;
			_settings.GraphVizLocation = GraphVizExecutable.FullName;

			_settings.Save();
		}

		private readonly Settings _settings;
	}
}