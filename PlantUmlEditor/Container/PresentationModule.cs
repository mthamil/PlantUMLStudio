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
using System.Collections.Generic;
using System.IO;
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
using PlantUmlEditor.ViewModel.Notifications;
using Utilities.Chronology;
using Utilities.Clipboard;
using Utilities.Controls.Behaviors.AvalonEdit.Folding;
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
			builder.RegisterType<NotificationsHub>().As<NotificationsHub, INotifications>()
				.SingleInstance();

			builder.RegisterType<ClipboardWrapper>().As<IClipboard>()
				.SingleInstance();

			builder.RegisterType<SettingsPropagator>()
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
				using (var reader = XmlReader.Create(c.Resolve<ISettings>().PlantUmlHighlightingDefinition.OpenRead()))
					return HighlightingLoader.Load(reader, HighlightingManager.Instance);
			}).SingleInstance();

			builder.Register(c => new CodeEditorViewModel(
				c.ResolveNamed<AbstractFoldingStrategy>("PlantUmlFoldingStrategy"), 
				c.Resolve<IHighlightingDefinition>(),
				c.Resolve<SnippetsMenu>(),
				c.Resolve<IClipboard>())).As<ICodeEditor>();

			builder.RegisterType<DiagramEditorViewModel>().As<IDiagramEditor>()
				.WithParameter((p, c) => p.Name == "refreshTimer", (p, c) => new SystemTimer { Interval = TimeSpan.FromSeconds(2) })
				.OnActivating(c =>
				{
					c.Instance.AutoSave = c.Context.Resolve<ISettings>().AutoSaveEnabled;
					c.Instance.AutoSaveInterval = c.Context.Resolve<ISettings>().AutoSaveInterval;
				});

			builder.RegisterType<DiagramExplorerViewModel>().As<IDiagramExplorer>()
				.WithParameter((p, c) => p.Name == "uiScheduler", (p, c) => TaskScheduler.FromCurrentSynchronizationContext())
				.WithProperty(d => d.NewDiagramTemplate, "@startuml \"{0}\"\n\n\n@enduml")
				.OnActivating(c =>
				{
					c.Instance.DiagramLocation = c.Context.Resolve<ISettings>().LastDiagramLocation;
					c.Instance.FileExtension = c.Context.Resolve<ISettings>().DiagramFileExtension;
				});

			builder.RegisterType<DiagramManagerViewModel>().As<DiagramManagerViewModel, IDiagramManager>()
				.OnActivated(c => c.Instance.InitializeAsync())
				.SingleInstance();

			builder.RegisterType<ComponentViewModel>();

			builder.RegisterType<AboutViewModel>()
				.OnActivating(c => c.Instance.LoadComponents());

			builder.RegisterType<SettingsViewModel>();

			builder.RegisterType<RecentFilesMenuViewModel>()
				.WithParameter((p, c) => p.ParameterType == typeof(ICollection<FileInfo>), (p, c) => c.Resolve<ISettings>().RecentFiles);
		}
	}
}