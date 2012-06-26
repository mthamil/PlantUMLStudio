using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using PlantUmlEditor.Model;
using PlantUmlEditor.Properties;
using PlantUmlEditor.ViewModel;
using Utilities.Chronology;

namespace PlantUmlEditor.Container
{
	/// <summary>
	/// Configures the application's container.
	/// </summary>
	public class MainModule : Module
	{
		/// <see cref="Module.Load"/>
		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(c => new DefaultSnippets().Snippets)
				.Named<IEnumerable<SnippetCategory>>("DefaultSnippets")
				.SingleInstance();

			builder.RegisterType<DiagramFileReader>().As<IDiagramReader>();
			builder.RegisterType<DiagramBitmapRenderer>().As<IDiagramRenderer>();
			builder.RegisterType<PlantUmlDiagramCompiler>().As<IDiagramCompiler>()
				.WithProperty(c => c.GraphVizLocation, Settings.Default.GraphVizLocation)
				.WithProperty(c => c.PlantUmlExecutable, new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Thirdparty\plantuml.exe")));

			builder.Register(c => TaskScheduler.Default);

			builder.Register<Func<DiagramFile, DiagramViewModel>>(c =>
			{
				var diagramRenderer = c.Resolve<IDiagramRenderer>();
				return diagram => new DiagramViewModel 
				{ 
					Diagram = diagram,
					DiagramImage = diagramRenderer.Render(diagram)	// Perform an initial render of the diagram.
				};
			});

			builder.Register<Func<DiagramViewModel, DiagramEditorViewModel>>(c =>
			{
				// Have to resolve these outside of the lambda.
				var snippets = c.ResolveNamed<IEnumerable<SnippetCategory>>("DefaultSnippets");
				var renderer = c.Resolve<IDiagramRenderer>();
				var compiler = c.Resolve<IDiagramCompiler>();
				return diagram =>
					new DiagramEditorViewModel(
						diagram,
						snippets,
						renderer,
						compiler,
						new SystemTimersTimer(),
						TaskScheduler.Default)
					{
						RefreshIntervalSeconds = 5,
						AutoRefresh = true
					};
			});

			builder.RegisterType<DiagramsViewModel>()
				.WithProperty(d => d.DiagramLocation,	// Initialize the diagram location.
					new DirectoryInfo(string.IsNullOrEmpty(Settings.Default.LastPath)
									? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"PlantUmlEditor\samples\")
									: Settings.Default.LastPath))
				.SingleInstance();
		}
	}
}