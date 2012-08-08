using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Core;
using PlantUmlEditor.Core.InputOutput;
using PlantUmlEditor.Model;
using PlantUmlEditor.Model.Snippets;
using PlantUmlEditor.Properties;
using Utilities.Chronology;

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

			builder.Register(c => new DotNetSettings(
					Settings.Default,
					new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"PlantUmlEditor\samples\"))))
				.As<ISettings>()
				.SingleInstance();

			builder.RegisterType<SystemTimer>().As<ITimer>();

			builder.RegisterType<DiagramBitmapRenderer>().As<IDiagramRenderer>();

			builder.RegisterType<PlantUml>().As<IDiagramCompiler>()
				.WithProperty(c => c.PlantUmlJar, new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Thirdparty\plantuml.jar")))
				.OnActivating(c => c.Instance.GraphVizExecutable = c.Context.Resolve<ISettings>().GraphVizExecutable);

			builder.RegisterType<DiagramIOService>().As<IDiagramIOService>();

			builder.RegisterType<SnippetParser>().As<ISnippetParser>();

			builder.RegisterType<SnippetProvider>()
				.WithProperty(p => p.SnippetLocation, new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"snippets\")))
				.OnActivating(c => c.Instance.Load())
				.SingleInstance();
		}
	}
}