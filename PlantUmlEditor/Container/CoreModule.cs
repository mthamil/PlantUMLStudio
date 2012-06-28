using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using PlantUmlEditor.Model;
using PlantUmlEditor.Properties;

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

			builder.RegisterType<DiagramBitmapRenderer>().As<IDiagramRenderer>();
			builder.RegisterType<PlantUmlDiagramCompiler>().As<IDiagramCompiler>()
				.WithProperty(c => c.GraphVizLocation, Settings.Default.GraphVizLocation)
				.WithProperty(c => c.PlantUmlExecutable, new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Thirdparty\plantuml.exe")));

			builder.RegisterType<DiagramIOService>().As<IDiagramIOService>();
		}
	}
}