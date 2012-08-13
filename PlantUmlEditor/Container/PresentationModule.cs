using System;
using System.IO;
using System.Reflection;
using Autofac;
using ICSharpCode.AvalonEdit.Folding;
using PlantUmlEditor.Configuration;
using PlantUmlEditor.Core;
using PlantUmlEditor.Model;
using PlantUmlEditor.Model.Snippets;
using PlantUmlEditor.ViewModel;
using Utilities.Chronology;
using Utilities.Controls.Behaviors.AvalonEdit;
using Module = Autofac.Module;

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
			builder.RegisterType<ProgressViewModel>().As<IProgressViewModel, IProgressRegistration>()
				.SingleInstance();

			builder.RegisterType<PreviewDiagramViewModel>()
				.OnActivating(c => c.Instance.ImagePreview =
					c.Context.Resolve<IDiagramRenderer>().Render(c.Parameters.TypedAs<Diagram>()));	// Perform an initial render of the diagram.

			builder.Register(c => new SnippetsMenu(c.Resolve<SnippetProvider>().Snippets))
				.SingleInstance();

			builder.RegisterType<EditorContextMenu>()
				.SingleInstance();

			builder.RegisterType<PlantUmlFoldRegions>();

			builder.Register(c => new PatternBasedFoldingStrategy(c.Resolve<PlantUmlFoldRegions>()))
				.Named<AbstractFoldingStrategy>("PlantUmlFoldingStrategy")
				.SingleInstance();

			builder.Register(c => new CodeEditorViewModel(
				c.ResolveNamed<AbstractFoldingStrategy>("PlantUmlFoldingStrategy"), 
				new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\PlantUML.xshd")),
				c.Resolve<EditorContextMenu>(),
				c.Resolve<SnippetsMenu>())).As<ICodeEditor>();

			builder.RegisterType<DiagramEditorViewModel>().As<IDiagramEditor>()
				.WithParameter((p, c) => p.Name == "refreshTimer", (p, c) => new SystemTimer { Interval = TimeSpan.FromSeconds(2) })
				.WithProperty(p => p.AutoSaveInterval, TimeSpan.FromSeconds(30))
				.WithProperty(p => p.AutoSave, true)
				.WithProperty(p => p.ImageCommands, new ImageContextMenu());

			builder.RegisterType<DiagramExplorerViewModel>().As<IDiagramExplorer>()
				.WithProperty(d => d.NewDiagramTemplate, "@startuml \"{0}\"\n\n\n@enduml")
				.OnActivating(c => c.Instance.DiagramLocation = c.Context.Resolve<ISettings>().LastDiagramLocation);

			builder.RegisterType<DiagramManagerViewModel>()
				.SingleInstance();

			builder.RegisterType<AboutViewModel>();
		}
	}
}