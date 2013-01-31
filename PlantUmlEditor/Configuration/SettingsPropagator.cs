using System;
using System.ComponentModel;
using PlantUmlEditor.ViewModel;
using Utilities.Reflection;

namespace PlantUmlEditor.Configuration
{
	/// <summary>
	/// Responsible for updating objects in response to settings changes.
	/// </summary>
	public class SettingsPropagator
	{
		public SettingsPropagator(ISettings settings, Lazy<IDiagramManager> diagramManager)
		{
			_settings = settings;
			_diagramManager = diagramManager;

			_settings.PropertyChanged += settings_PropertyChanged;
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
			foreach (var editor in _diagramManager.Value.OpenDiagrams)
				editor.AutoSave = _settings.AutoSaveEnabled;
		}

		private void AutoSaveIntervalChanged()
		{
			foreach (var editor in _diagramManager.Value.OpenDiagrams)
				editor.AutoSaveInterval = _settings.AutoSaveInterval;
		}

		private readonly ISettings _settings;
		private readonly Lazy<IDiagramManager> _diagramManager;

		private static readonly string autoSaveEnabledName = Reflect.PropertyOf<ISettings>(s => s.AutoSaveEnabled).Name;
		private static readonly string autoSaveIntervalName = Reflect.PropertyOf<ISettings>(s => s.AutoSaveInterval).Name;
	}
}