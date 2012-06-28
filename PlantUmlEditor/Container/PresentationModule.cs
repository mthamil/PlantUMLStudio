using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using PlantUmlEditor.Model;
using PlantUmlEditor.Properties;
using PlantUmlEditor.ViewModel;
using Utilities.Chronology;

namespace PlantUmlEditor.Container
{
	/// <summary>
	/// Configures presentation related objects.
	/// </summary>
	public class PresentationModule : Module
	{
		/// <see cref="Module.Load"/>
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<ProgressViewModel>().As<IProgressViewModel>()
				.SingleInstance();

			builder.Register(c => new DefaultSnippets().SnippetCategories)
				.Named<IEnumerable<SnippetCategoryViewModel>>("DefaultSnippets")
				.SingleInstance();

			builder.Register<Func<Diagram, DiagramViewModel>>(c =>
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
				var snippets = c.ResolveNamed<IEnumerable<SnippetCategoryViewModel>>("DefaultSnippets");
				var renderer = c.Resolve<IDiagramRenderer>();
				var diagramIO = c.Resolve<IDiagramIOService>();
				var progress = c.Resolve<IProgressViewModel>();
				return diagram =>
					new DiagramEditorViewModel(
						diagram,
						progress,
						snippets,
						renderer,
						diagramIO,
						new SystemTimersTimer())
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