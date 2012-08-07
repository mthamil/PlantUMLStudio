using System;
using System.Reflection;
using Utilities.Mvvm;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents information about the application.
	/// </summary>
	public class AboutViewModel : ViewModelBase
	{
		/// <summary>
		/// The application version.
		/// </summary>
		public Version Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version; }
		}
	}
}