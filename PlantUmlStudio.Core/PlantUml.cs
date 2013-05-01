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
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Autofac.Features.Indexed;
using PlantUmlStudio.Core.Dependencies;
using PlantUmlStudio.Core.Dependencies.Update;
using PlantUmlStudio.Core.Imaging;
using Utilities;
using Utilities.Chronology;
using Utilities.Concurrency.Processes;
using Utilities.InputOutput;
using Utilities.Net;

namespace PlantUmlStudio.Core
{
	/// <summary>
	/// Provides an interface to PlantUML.
	/// </summary>
	public class PlantUml : ComponentUpdateChecker, IExternalComponent, IDiagramCompiler
	{
		/// <summary>
		/// Initializes the PlantUML wrapper.
		/// </summary>
		/// <param name="clock">The system clock</param>
		/// <param name="renderers">Responsible for converting data to an image</param>
		public PlantUml(IClock clock, IIndex<ImageFormat, IDiagramRenderer> renderers) 
			: base(clock)
		{
			_renderers = renderers;
		}

		/// <see cref="IDiagramCompiler.CompileToImageAsync"/>
		public async Task<ImageSource> CompileToImageAsync(string diagramCode, ImageFormat imageFormat, CancellationToken cancellationToken)
		{
			var result = await Task.Factory.FromProcess(
				executable: "java",
				arguments: String.Format(@"-jar ""{0}"" {1} -quiet -graphvizdot ""{2}"" -pipe", 
								PlantUmlJar.FullName, 
								imageFormat == ImageFormat.SVG ? "-tsvg" : string.Empty, 
								GraphVizExecutable.FullName), 
				input: new MemoryStream(Encoding.Default.GetBytes(diagramCode)), 
				cancellationToken: cancellationToken
			).ConfigureAwait(false);

			await HandleErrorStream(result.Error, cancellationToken).ConfigureAwait(false);

			return _renderers[imageFormat].Render(result.Output);
		}
		
		/// <see cref="IDiagramCompiler.CompileToFileAsync"/>
		public Task CompileToFileAsync(FileInfo diagramFile, ImageFormat imageFormat)
		{
			return Task.Factory.FromProcess(
				executable: "java",
				arguments: String.Format(@"-jar ""{0}"" {1} -quiet -graphvizdot ""{2}"" ""{3}""", 
								PlantUmlJar.FullName,
								imageFormat == ImageFormat.SVG ? "-tsvg" : string.Empty, 
								GraphVizExecutable.FullName, 
								diagramFile.FullName));
		}

		#region Implementation of IExternalComponent

		/// <see cref="IExternalComponent.Name"/>
		public string Name { get { return PlantUmlJar.Name; } }

		/// <see cref="IExternalComponent.GetCurrentVersionAsync"/>
		public async Task<string> GetCurrentVersionAsync()
		{
			var result = await Task.Factory.FromProcess(
				executable: "java",
				arguments: String.Format(@"-jar ""{0}"" -version", PlantUmlJar.FullName),
				input: Stream.Null
			).ConfigureAwait(false);

			await HandleErrorStream(result.Error, CancellationToken.None).ConfigureAwait(false);

			var output = Encoding.Default.GetString(
				await result.Output.Async().ReadAllBytesAsync(CancellationToken.None).ConfigureAwait(false));
			var match = LocalVersionMatchingPattern.Match(output);
			return match.Groups["version"].Value;
		}

		#endregion

		#region Implementation of IComponentUpdateChecker

		/// <summary>
		/// The location of the latest PlantUML version number.
		/// </summary>
		public Uri VersionLocation { get; set; }

		/// <summary>
		/// Pattern used to find the latest version.
		/// </summary>
		public Regex RemoteVersionMatchingPattern { get; set; }

		/// <see cref="IComponentUpdateChecker.HasUpdateAsync"/>
		public override async Task<Option<string>> HasUpdateAsync(CancellationToken cancellationToken)
		{
			string currentVersion = await GetCurrentVersionAsync().ConfigureAwait(false);

			// Scrape the PlantUML downloads page for the latest version number.
			using (var client = new WebClient())
			{
				var downloadPage = await client.Async().DownloadStringAsync(VersionLocation, cancellationToken: cancellationToken).ConfigureAwait(false);
				var match = RemoteVersionMatchingPattern.Match(downloadPage);
				if (match.Success)
				{
					string remoteVersion = match.Groups["version"].Value;
					bool versionsNotEqual = String.Compare(remoteVersion, currentVersion, true, CultureInfo.InvariantCulture) != 0;
					if (versionsNotEqual)
						return remoteVersion;
				}
			}
			
			return Option<string>.None();
		}

		#endregion

		private static async Task HandleErrorStream(Stream errorStream, CancellationToken cancellationToken)
		{
			if (errorStream.Length > 0)
			{
				string errorMessage = Encoding.Default.GetString(
					await errorStream.Async().ReadAllBytesAsync(cancellationToken).ConfigureAwait(false));
				throw new PlantUmlException(errorMessage);
			}
		}

		/// <summary>
		/// The location of the GraphViz executable.
		/// </summary>
		public FileInfo GraphVizExecutable { get; set; }

		/// <summary>
		/// The location of the plantuml.jar file.
		/// </summary>
		public FileInfo PlantUmlJar { get; set; }

		/// <summary>
		/// Pattern used to extract the current version.
		/// </summary>
		public Regex LocalVersionMatchingPattern { get; set; }

		private readonly IIndex<ImageFormat, IDiagramRenderer> _renderers;
	}
}