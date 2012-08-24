using System;
using System.Windows;
using Utilities.Mvvm.Commands;

namespace Utilities.Controls.Commands
{
	/// <summary>
	/// Creates an displays a window of a given type.
	/// </summary>
	public class OpenWindowCommand : CommandBase
	{
		/// <summary>
		/// Creates and opens a new instance of the specified Window
		/// type.
		/// </summary>
		/// <param name="parameter">
		/// The data context to set on the window. If null, 
		/// the data context will not be set, preventing an override
		/// of any existing context.
		/// </param>
		public override void Execute(object parameter)
		{
			var window = (Window)Activator.CreateInstance(Type);
			if (parameter != null)
				window.DataContext = parameter;
			window.ShowDialog();
		}

		/// <summary>
		/// The type of window to create.
		/// </summary>
		public Type Type { get; set; }
	}
}