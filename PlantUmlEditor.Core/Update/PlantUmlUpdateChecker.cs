using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Chronology;
using Utilities.Net;

namespace PlantUmlEditor.Core.Update
{
	/// <summary>
	/// Checks for updates of PlantUML.
	/// </summary>
	public class PlantUmlUpdateChecker : DependencyUpdateChecker
	{
		/// <summary>
		/// Initializes a new update checker for PlantUML.
		/// </summary>
		/// <param name="clock">The system clock</param>
		/// <param name="plantUml">The PlantUML API</param>
		public PlantUmlUpdateChecker(IClock clock, IDiagramCompiler plantUml)
			: base(clock)
		{
			_plantUml = plantUml;
		}

		/// <summary>
		/// The location of the latest PlantUML version number.
		/// </summary>
		public Uri VersionLocation { get; set; }

		/// <see cref="IDependencyUpdateChecker.HasUpdateAsync"/>
		public override async Task<bool> HasUpdateAsync()
		{
			string currentVersion = await _plantUml.GetCurrentVersion();

			// Scrape the PlantUML downloads page for the latest version number.
			using (var client = new WebClient())
			{
				var serverVersionBytes = await client.Async().DownloadDataAsync(VersionLocation, CancellationToken.None);
				var downloadPage = Encoding.Unicode.GetString(Encoding.Convert(
					Encoding.UTF8,
					Encoding.Unicode, serverVersionBytes));
				int versionIndex = downloadPage.IndexOf(versionToken);
				if (versionIndex > -1)
				{
					int versionEndIndex = downloadPage.IndexOf(')', versionIndex + versionToken.Length);
					if (versionEndIndex > -1)
					{
						string latestVersion = downloadPage.Substring(versionIndex + versionToken.Length, versionEndIndex - (versionIndex + versionToken.Length));
						bool versionsNotEqual = String.Compare(latestVersion, currentVersion, true, CultureInfo.InvariantCulture) != 0;
						return versionsNotEqual;
					}
				}
			}

			return false;
		}

		private readonly IDiagramCompiler _plantUml;

		private const string versionToken = "PlantUML compiled Jar (Version ";
	}
}