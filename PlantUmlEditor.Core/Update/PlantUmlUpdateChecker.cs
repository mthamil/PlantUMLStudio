using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
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

		/// <summary>
		/// Pattern used to find the latest version.
		/// </summary>
		public Regex VersionMatchingPattern { get; set; }

		/// <see cref="IDependencyUpdateChecker.HasUpdateAsync"/>
		public override async Task<bool> HasUpdateAsync()
		{
			string currentVersion = await _plantUml.GetCurrentVersion();

			// Scrape the PlantUML downloads page for the latest version number.
			using (var client = new WebClient())
			{
				var downloadPage = await client.Async().DownloadStringAsync(VersionLocation, CancellationToken.None);
				var match = VersionMatchingPattern.Match(downloadPage);
				if (match.Success)
				{
					bool versionsNotEqual = String.Compare(match.Groups["version"].Value, currentVersion, true, CultureInfo.InvariantCulture) != 0;
					return versionsNotEqual;
				}
			}

			return false;
		}

		private readonly IDiagramCompiler _plantUml;
	}
}