using System;
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
			if (e.PropertyName == autoSaveEnabledName)
				AutoSaveEnabledChanged();
			else if (e.PropertyName == autoSaveIntervalName)
				AutoSaveIntervalChanged();
		}

		private void AutoSaveEnabledChanged()
		{
			foreach (var editor in _diagramManager.OpenDiagrams)
				editor.AutoSave = _settings.AutoSaveEnabled;
		}

		private void AutoSaveIntervalChanged()
		{
			foreach (var editor in _diagramManager.OpenDiagrams)
				editor.AutoSaveInterval = _settings.AutoSaveInterval;
		}

		private void diagramManager_DiagramClosed(object sender, DiagramClosedEventArgs e)
		{
			_settings.RecentFiles.Add(e.Diagram.File);
		}

		private readonly ISettings _settings;
		private readonly IDiagramManager _diagramManager;

		private static readonly string autoSaveEnabledName = Reflect.PropertyOf<ISettings>(s => s.AutoSaveEnabled).Name;
		private static readonly string autoSaveIntervalName = Reflect.PropertyOf<ISettings>(s => s.AutoSaveInterval).Name;
	}
}