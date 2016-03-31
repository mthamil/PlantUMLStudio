//  PlantUML Studio
//  Copyright 2016 Matthew Hamilton - matthamilton@live.com
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
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using PlantUmlStudio.Core.Dependencies;
using PlantUmlStudio.Core.Dependencies.Update;
using PlantUmlStudio.Core.Imaging;
using SharpEssentials;
using SharpEssentials.Chronology;
using SharpEssentials.Collections;
using SharpEssentials.Concurrency.Processes;
using SharpEssentials.InputOutput;

namespace PlantUmlStudio.Core
{
	/// <summary>
	/// Provides an interface to PlantUML.
	/// </summary>
	public class PlantUml : ComponentUpdateChecker, IExternalComponent, IDiagramCompiler
	{
		/// <summary>
		/// Initializes a new PlantUML wrapper.
		/// </summary>
		/// <param name="clock">The system clock</param>
		/// <param name="renderers">Responsible for converting data to an image</param>
		/// <param name="httpClient">Used for web requests</param>
		public PlantUml(IClock clock, IIndex<ImageFormat, IDiagramRenderer> renderers, HttpClient httpClient) 
			: base(clock, httpClient, StringComparer.OrdinalIgnoreCase)
		{
		    _renderers = renderers;
		}

	    /// <see cref="IDiagramCompiler.CompileToImageAsync"/>
		public async Task<DiagramResult> CompileToImageAsync(string diagramCode, ImageFormat imageFormat, Encoding encoding, CancellationToken cancellationToken)
		{
            var result = await Task.Factory.FromProcess(
				executable: "java",
				arguments: new ArgumentsBuilder()
                                .Arg("splash:no")
                                .Arg("jar", PlantUmlJar)
                                .ArgIf(imageFormat == ImageFormat.SVG, "tsvg")
                                .Arg("graphvizdot", GraphVizExecutable)
                                .Arg("charset", encoding.WebName)
                                .Arg("pipe"), 
				input: new MemoryStream(encoding.GetBytes(diagramCode)), 
				cancellationToken: cancellationToken
			).ConfigureAwait(false);

			var diagramResult = (await HandleErrorStream(result.Error, cancellationToken)
                .ConfigureAwait(false))
                .Select(error => DiagramError.TryParse(error)
                                             .Select(de => new DiagramResult(de.ToEnumerable()))    // Currently only the first error seems to be reported.
                                             .GetOrElse(() => { throw new PlantUmlException(error); }))
                .GetOrElse(() => new DiagramResult(_renderers[imageFormat].Render(result.Output)));

			return diagramResult;
		}
		
		/// <see cref="IDiagramCompiler.CompileToFileAsync"/>
		public Task CompileToFileAsync(FileInfo diagramFile, ImageFormat imageFormat, Encoding encoding)
		{
            return Task.Factory.FromProcess(
				executable: "java",
				arguments: new ArgumentsBuilder()
                                .Arg("splash:no")
                                .Arg("jar", PlantUmlJar)
                                .ArgIf(imageFormat == ImageFormat.SVG, "tsvg")    
                                .Arg("quiet")
                                .Arg("graphvizdot", GraphVizExecutable)
                                .Arg("charset", encoding.WebName)
                                .Value(diagramFile));
		}

		#region Implementation of IExternalComponent

		/// <see cref="IExternalComponent.Name"/>
		public string Name => PlantUmlJar.Name;

	    #endregion

		#region Implementation of IComponentUpdateChecker

        /// <see cref="IComponentUpdateChecker.GetCurrentVersionAsync"/>
        public override async Task<string> GetCurrentVersionAsync(CancellationToken cancellationToken)
        {
            if (!PlantUmlJar.Exists)
                throw new FileNotFoundException("Component not found.", PlantUmlJar.FullName);

            var result = await Task.Factory.FromProcess(
                executable: "java",
                arguments: new ArgumentsBuilder()
                                .Arg("splash:no")
                                .Arg("jar", PlantUmlJar)
                                .Arg("version"),
                input: Stream.Null,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            (await HandleErrorStream(result.Error, cancellationToken).ConfigureAwait(false))
                .Apply(error => { throw new PlantUmlException(error); });

            var output = Encoding.Default.GetString(
                await result.Output.Async().ReadAllBytesAsync(cancellationToken).ConfigureAwait(false));
            var match = LocalVersionPattern.Match(output);
            return match.Groups["version"].Value;
        }

	    #endregion

		private static async Task<Option<string>> HandleErrorStream(Stream errorStream, CancellationToken cancellationToken)
		{
			if (errorStream.Length > 0)
			{
			    var errorBytes = await errorStream.Async().ReadAllBytesAsync(cancellationToken).ConfigureAwait(false);
                var errorMessage = Encoding.Default.GetString(errorBytes);
			    return Option.Some(errorMessage);
			}

		    return Option.None<string>();
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
		public Regex LocalVersionPattern { get; set; }

		private readonly IIndex<ImageFormat, IDiagramRenderer> _renderers;
	}
}