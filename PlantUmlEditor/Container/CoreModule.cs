//  PlantUML Editor
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
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
using System.Threading.Tasks;
using Autofac;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Core;
using PlantUmlEditor.Core.Dependencies;
using PlantUmlEditor.Core.Imaging;
using PlantUmlEditor.Core.InputOutput;
using PlantUmlEditor.Core.Security;
using PlantUmlEditor.Model;
using PlantUmlEditor.Model.Snippets;
using PlantUmlEditor.Properties;
using Utilities.Chronology;
using Utilities.InputOutput;
using Module = Autofac.Module;

namespace PlantUmlEditor.Container
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

			builder.Register(c => new DotNetSettings(
				        Settings.Default,
				        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"PlantUmlEditor\samples\"))))
			       .As<ISettings>()
			       .SingleInstance();

			builder.RegisterType<FileSystemWatcherAdapter>().As<IFileSystemWatcher>();
			builder.RegisterType<DirectoryMonitor>().As<IDirectoryMonitor>()
			       .WithProperty(p => p.FileCreationWaitTimeout, TimeSpan.FromSeconds(2))
			       .OnActivating(c => c.Instance.Filter = "*" + c.Context.Resolve<ISettings>().DiagramFileExtension);

			builder.RegisterType<BitmapRenderer>().As<IDiagramRenderer>();

			builder.RegisterType<GraphViz>().As<IExternalComponent>()
			       .OnActivating(c =>
			       {
				       c.Instance.GraphVizExecutable = c.Context.Resolve<ISettings>().GraphVizExecutable;
				       c.Instance.LocalVersionMatchingPattern = c.Context.Resolve<ISettings>().GraphVizLocalVersionPattern;
			       });

			builder.RegisterType<PlantUml>().As<IDiagramCompiler, IExternalComponent>()
			       .OnActivating(c =>
			       {
				       c.Instance.PlantUmlJar = c.Context.Resolve<ISettings>().PlantUmlJar;
				       c.Instance.GraphVizExecutable = c.Context.Resolve<ISettings>().GraphVizExecutable;
				       c.Instance.LocalVersionMatchingPattern = c.Context.Resolve<ISettings>().PlantUmlLocalVersionPattern;
				       c.Instance.LocalLocation = c.Context.Resolve<ISettings>().PlantUmlJar;
				       c.Instance.RemoteLocation = c.Context.Resolve<ISettings>().PlantUmlDownloadLocation;
				       c.Instance.VersionLocation = c.Context.Resolve<ISettings>().PlantUmlVersionSource;
				       c.Instance.RemoteVersionMatchingPattern = c.Context.Resolve<ISettings>().PlantUmlRemoteVersionPattern;
			       });

			builder.RegisterType<DiagramIOService>().As<IDiagramIOService>()
			       .OnActivating(c => c.Instance.FileFilter = "*" + c.Context.Resolve<ISettings>().DiagramFileExtension);

			builder.RegisterType<SnippetParser>().As<ISnippetParser>();

			builder.RegisterType<SnippetProvider>()
			       .WithProperty(p => p.SnippetLocation, new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"snippets\")))
			       .OnActivating(c => c.Instance.Load())
			       .SingleInstance();

			builder.RegisterType<WindowsSecurityService>().As<ISecurityService>()
			       .SingleInstance();
		}
	}
}