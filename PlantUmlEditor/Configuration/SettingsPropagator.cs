﻿//  PlantUML Editor
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
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
using System.ComponentModel;
using PlantUmlEditor.ViewModel;
using Utilities.Reflection;

namespace PlantUmlEditor.Configuration
{
	/// <summary>
	/// Responsible for updating objects in response to settings changes or
	/// capturing events that should change settings.
	/// </summary>
	public class SettingsPropagator
	{
		public SettingsPropagator(ISettings settings, IDiagramManager diagramManager)
		{
			_settings = settings;
			_diagramManager = diagramManager;

			_settings.PropertyChanged += settings_PropertyChanged;
			_diagramManager.DiagramClosed += diagramManager_DiagramClosed;
		}

		private void settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			Action<IDiagramEditor, ISettings> editorUpdate;
			if (editorUpdates.TryGetValue(e.PropertyName, out editorUpdate))
			{
				foreach (var editor in _diagramManager.OpenDiagrams)
					editorUpdate(editor, _settings);
			}
		}

		private void diagramManager_DiagramClosed(object sender, DiagramClosedEventArgs e)
		{
			_settings.RecentFiles.Add(e.Diagram.File);
		}

		private readonly ISettings _settings;
		private readonly IDiagramManager _diagramManager;

		/// <summary>
		/// A mapping of settings property names to the changes that should be applied to diagram editors if such a setting changes.
		/// </summary>
		private static readonly IDictionary<string, Action<IDiagramEditor, ISettings>> editorUpdates = new Dictionary<string, Action<IDiagramEditor, ISettings>>
		{
			{ Reflect.PropertyOf<ISettings>(s => s.AutoSaveEnabled).Name, (ed, s) => ed.AutoSave = s.AutoSaveEnabled },
			{ Reflect.PropertyOf<ISettings>(s => s.AutoSaveInterval).Name, (ed, s) => ed.AutoSaveInterval = s.AutoSaveInterval },
			{ Reflect.PropertyOf<ISettings>(s => s.HighlightCurrentLine).Name, (ed, s) => ed.CodeEditor.HighlightCurrentLine = s.HighlightCurrentLine },
			{ Reflect.PropertyOf<ISettings>(s => s.ShowLineNumbers).Name, (ed, s) => ed.CodeEditor.ShowLineNumbers = s.ShowLineNumbers }
		};
	}
}