using System;
using System.Windows;
using System.Windows.Input;

namespace Utilities.Controls.Commands
{
	/// <summary>
	/// A Command base class that is also a DependencyObject.
	/// </summary>
	public abstract class DependencyCommandBase : DependencyObject, ICommand
	{
		/// <see cref="ICommand.CanExecute"/>
		public virtual bool CanExecute(object parameter)
		{
			return true;
		}

		/// <see cref="ICommand.Execute"/>
		public abstract void Execute(object parameter);

		/// <see cref="ICommand.CanExecuteChanged"/>
		public virtual event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
	}
}