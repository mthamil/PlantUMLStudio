using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using Autofac;
using ICSharpCode.AvalonEdit.Folding;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Model;
using PlantUmlEditor.Model.Snippets;
using PlantUmlEditor.Properties;
using PlantUmlEditor.ViewModel;
using Utilities.Chronology;
using Utilities.Controls.Behaviors.AvalonEdit;
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

			builder.Register<IEnumerable<ViewModelBase>>(c =>
			{
				var snippetProvider = c.Resolve<SnippetProvider>();
				var snippetRoot = new SnippetCategoryViewModel(Resources.ContextMenu_Code_Snippets);
				foreach (var snippet in SnippetCategoryViewModel.BuildTree(snippetProvider.Snippets))
					snippetRoot.Snippets.Add(snippet);
				return new List<ViewModelBase> { snippetRoot };
			})
			.Named<IEnumerable<ViewModelBase>>("EditorContextMenu")
			.SingleInstance();

			builder.RegisterType<PlantUmlFoldingStrategy>().As<AbstractFoldingStrategy>()
				.SingleInstance();

			builder.Register(c => new CodeEditorViewModel(c.Resolve<AbstractFoldingStrategy>(), c.ResolveNamed<IEnumerable<ViewModelBase>>("EditorContextMenu"))).As<ICodeEditor>();

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
				.WithProperty(d => d.NewDiagramTemplate, "@startuml \"{0}\"\n\n\n@enduml")
				.OnActivating(c => c.Instance.DiagramLocation = c.Context.Resolve<ISettings>().LastDiagramLocation);

			builder.RegisterType<DiagramManagerViewModel>()
				.SingleInstance();
		}
	}
}