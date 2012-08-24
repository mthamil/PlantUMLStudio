using System;
using System.Windows.Input;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// Provides a base class for commands.
	/// </summary>
	public abstract class CommandBase : ICommand
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