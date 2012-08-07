using System;
using System.Windows;
using Utilities.Mvvm.Commands;

namespace PlantUmlEditor.ViewModel.Commands
{
	/// <summary>
	/// Creates an displays a window of a given type.
	/// </summary>
	public class OpenWindowCommand : RelayCommand<Type>
	{
		public OpenWindowCommand()
			: base(windowType => 
				((Window)Activator.CreateInstance(windowType)).ShowDialog()) { }
	}
}