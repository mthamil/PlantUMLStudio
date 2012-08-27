using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Autofac;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
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

			builder.RegisterType<PlantUmlFoldRegions>();

			builder.Register(c => new PatternBasedFoldingStrategy(c.Resolve<PlantUmlFoldRegions>()))
				.Named<AbstractFoldingStrategy>("PlantUmlFoldingStrategy")
				.SingleInstance();

			builder.Register(c =>
			{
				using (var stream = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\PlantUML.xshd")))
				using (var reader = new XmlTextReader(stream))
					return HighlightingLoader.Load(reader, HighlightingManager.Instance);
			}).SingleInstance();

			builder.Register(c => new CodeEditorViewModel(
				c.ResolveNamed<AbstractFoldingStrategy>("PlantUmlFoldingStrategy"), 
				c.Resolve<IHighlightingDefinition>(),
				c.Resolve<SnippetsMenu>())).As<ICodeEditor>();

			builder.RegisterType<DiagramEditorViewModel>().As<IDiagramEditor>()
				.WithParameter((p, c) => p.Name == "refreshTimer", (p, c) => new SystemTimer { Interval = TimeSpan.FromSeconds(2) })
				.WithProperty(p => p.AutoSaveInterval, TimeSpan.FromSeconds(30))
				.WithProperty(p => p.AutoSave, true);

			builder.RegisterType<DiagramExplorerViewModel>().As<IDiagramExplorer>()
				.WithParameter((p, c) => p.Name == "uiScheduler", (p, c) => TaskScheduler.FromCurrentSynchronizationContext())
				.WithProperty(d => d.NewDiagramTemplate, "@startuml \"{0}\"\n\n\n@enduml")
				.OnActivating(c =>
				{
					c.Instance.DiagramLocation = c.Context.Resolve<ISettings>().LastDiagramLocation;
					c.Instance.FileExtension = c.Context.Resolve<ISettings>().DiagramFileExtension;
				});;

			builder.RegisterType<DiagramManagerViewModel>()
				.SingleInstance();

			builder.RegisterType<AboutViewModel>();
		}
	}
}