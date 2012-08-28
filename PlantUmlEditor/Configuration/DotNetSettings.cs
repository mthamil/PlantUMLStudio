using System;
using System.IO;
using System.Text.RegularExpressions;
using PlantUmlEditor.Properties;

namespace PlantUmlEditor.Configuration
{
	/// <summary>
	/// An adapter around the generated .NET Settings class.
	/// </summary>
	public class DotNetSettings : ISettings
	{
		internal DotNetSettings(Settings settings, DirectoryInfo defaultDiagramLocation)
		{
			_settings = settings;

			GraphVizExecutable = new FileInfo(_settings.GraphVizLocation);
			PlantUmlJar = new FileInfo(_settings.PlantUmlLocation);

			LastDiagramLocation = String.IsNullOrEmpty(_settings.LastPath)
				? defaultDiagramLocation
				: new DirectoryInfo(_settings.LastPath);

			PlantUmlDownloadLocation = settings.DownloadUrl;
			PlantUmlVersionSource = settings.PlantUmlVersionSource;
			PlantUmlVersionPattern = new Regex(settings.PlantUmlVersionPattern);
			DiagramFileExtension = settings.PlantUmlFileExtension;
		}

		/// <see cref="ISettings.GraphVizExecutable"/>
		public FileInfo GraphVizExecutable { get; set; }

		/// <see cref="ISettings.PlantUmlJar"/>
		public FileInfo PlantUmlJar { get; set; }

		/// <see cref="ISettings.LastDiagramLocation"/>
		public DirectoryInfo LastDiagramLocation { get; set; }

		/// <see cref="ISettings.PlantUmlDownloadLocation"/>
		public Uri PlantUmlDownloadLocation { get; private set; }

		/// <see cref="ISettings.PlantUmlVersionSource"/>
		public Uri PlantUmlVersionSource { get; private set; }

		/// <see cref="ISettings.PlantUmlVersionPattern"/>
		public Regex PlantUmlVersionPattern { get; private set; }

		/// <see cref="ISettings.DiagramFileExtension"/>
		public string DiagramFileExtension { get; private set; }

		/// <see cref="ISettings.Save"/>
		public void Save()
		{
			_settings.LastPath = LastDiagramLocation.FullName;
			_settings.GraphVizLocation = GraphVizExecutable.FullName;
			_settings.PlantUmlLocation = PlantUmlJar.FullName;

			_settings.Save();
		}

		private readonly Settings _settings;
	}
}