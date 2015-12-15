//  PlantUML Studio
//  Copyright 2014 Matthew Hamilton - matthamilton@live.com
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
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using PlantUmlStudio.Configuration;
using PlantUmlStudio.Container.Support;
using PlantUmlStudio.Core;
using PlantUmlStudio.Core.Dependencies;
using PlantUmlStudio.Core.Imaging;
using PlantUmlStudio.Core.InputOutput;
using SharpEssentials.Chronology;
using SharpEssentials.InputOutput;

namespace PlantUmlStudio.Container
{
	/// <summary>
	/// Configures the application's core objects.
	/// </summary>
	public class CoreModule : Module
	{
		/// <see cref="Module.Load"/>
		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(c => TaskScheduler.Default);

			builder.RegisterType<SystemTimer>().As<ITimer>();

			builder.RegisterType<SystemClock>().As<IClock>()
			       .SingleInstance();

		    builder.RegisterType<HttpClient>()
		           .SingleInstance();

		    builder.Register(c => Settings.Default);

		    builder.RegisterType<DotNetSettings>()
                   .FindConstructorsWith(new NonPublicConstructorFinder())
		           .As<ISettings>()
		           .SingleInstance();

			builder.RegisterType<FileSystemWatcherAdapter>().As<IFileSystemWatcher>();
			builder.RegisterType<DirectoryMonitor>().As<IDirectoryMonitor>()
			       .WithProperty(p => p.FileCreationWaitTimeout, TimeSpan.FromSeconds(2))
                   .ApplySettings((settings, instance) => instance.Filter = "*" + settings.DiagramFileExtension);

			builder.RegisterType<BitmapRenderer>().As<IDiagramRenderer>()
			       .Keyed<IDiagramRenderer>(ImageFormat.PNG);

			builder.RegisterType<SvgRenderer>().As<IDiagramRenderer>()
				   .Keyed<IDiagramRenderer>(ImageFormat.SVG);

			builder.RegisterType<GraphViz>().As<IExternalComponent>()
                   .ApplySettings((settings, instance) =>
                   {
                       instance.GraphVizExecutable = settings.GraphVizExecutable;

                       instance.LocalVersionPattern = settings.GraphVizLocalVersionPattern;
                       instance.DownloadLocation = settings.GraphVizDownloadLocation;
                       instance.VersionSource = settings.GraphVizVersionSource;
                       instance.RemoteVersionPattern = settings.GraphVizRemoteVersionPattern;
                   });

			builder.RegisterType<PlantUml>().As<IDiagramCompiler, IExternalComponent>()
                   .ApplySettings((settings, instance) =>
                   {
                       instance.PlantUmlJar = settings.PlantUmlJar;
                       instance.GraphVizExecutable = settings.GraphVizExecutable;
                       instance.LocalLocation = settings.PlantUmlJar;

                       instance.LocalVersionPattern = settings.PlantUmlLocalVersionPattern;
                       instance.DownloadLocation = settings.PlantUmlDownloadLocation;
                       instance.VersionSource = settings.PlantUmlVersionSource;
                       instance.RemoteVersionPattern = settings.PlantUmlRemoteVersionPattern;
                   });

			builder.RegisterType<DiagramIOService>().As<IDiagramIOService>()
                   .ApplySettings((settings, instance) => instance.FileFilter = "*" + settings.DiagramFileExtension);
		}
	}
}