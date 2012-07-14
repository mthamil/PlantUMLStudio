using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Autofac;
using PlantUmlEditor.Model;
using PlantUmlEditor.Properties;
using PlantUmlEditor.ViewModel;
using Utilities.Chronology;
using Utilities.Mvvm;
using Utilities.Mvvm.Commands;

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

			// Diagram view-model factory.
			builder.Register<Func<Diagram, PreviewDiagramViewModel>>(c =>
			{
				var diagramRenderer = c.Resolve<IDiagramRenderer>();
				return diagram => new PreviewDiagramViewModel(diagram)
				{
					ImagePreview = diagramRenderer.Render(diagram)	// Perform an initial render of the diagram.
				};
			});

			builder.Register(c => new DefaultSnippets().SnippetCategories)
				.Named<IEnumerable<SnippetCategoryViewModel>>("DefaultSnippets")
				.SingleInstance();

			builder.Register<IEnumerable<ViewModelBase>>(c =>
			{
				var snippets = c.ResolveNamed<IEnumerable<SnippetCategoryViewModel>>("DefaultSnippets");
				var snippetRoot = new SnippetCategoryViewModel(Resources.ContextMenu_Code_Snippets);
				foreach (var snippet in snippets)
					snippetRoot.Snippets.Add(snippet);
				return new List<ViewModelBase> { snippetRoot };
			})
			.Named<IEnumerable<ViewModelBase>>("EditorContextMenu");

			builder.Register(c => new CodeEditorViewModel(c.ResolveNamed<IEnumerable<ViewModelBase>>("EditorContextMenu")));

			builder.RegisterType<DiagramEditorViewModel>().As<IDiagramEditor>()
				.WithParameter((p, c) => p.Name == "refreshTimer", (p, c) => new SystemTimersTimer { Interval = TimeSpan.FromSeconds(2) })
				.WithProperty(p => p.AutoSaveInterval, TimeSpan.FromSeconds(30))
				.WithProperty(p => p.AutoSave, true)
				.WithProperty(p => p.ImageCommands, new List<NamedRelayCommand<IDiagramEditor>>
					{
						new NamedRelayCommand<IDiagramEditor>(d => Clipboard.SetImage(d.DiagramImage as BitmapSource))
						{
							Name = Resources.ContextMenu_Image_CopyToClipboard
						},

						new NamedRelayCommand<IDiagramEditor>(d => Process.Start("explorer.exe", "/select," + d.Diagram.ImageFilePath).Dispose())
						{
							Name = Resources.ContextMenu_Image_OpenInExplorer
						},
						
						new NamedRelayCommand<IDiagramEditor>(d => Clipboard.SetText(d.Diagram.ImageFilePath)) 
						{ 
							Name = Resources.ContextMenu_Image_CopyImagePath 
						}
					});

			builder.RegisterType<DiagramExplorerViewModel>().As<IDiagramExplorer>()
				.WithProperty(d => d.DiagramLocation,	// Initialize the diagram location.
					new DirectoryInfo(string.IsNullOrEmpty(Settings.Default.LastPath)
									? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"PlantUmlEditor\samples\")
									: Settings.Default.LastPath))
				.WithProperty(d => d.NewDiagramTemplate, String.Format(
					@"@startuml ""{{0}}""{0}{1}{2}@enduml", Environment.NewLine, Environment.NewLine, Environment.NewLine));

			builder.RegisterType<DiagramManagerViewModel>()
				.SingleInstance();
		}
	}
}