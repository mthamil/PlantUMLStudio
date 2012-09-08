using System;
using System.Threading;
using System.Windows.Input;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command that performs cancellation using a CancellationTokenSource.
	/// </summary>
	public class CancelTaskCommand : ICommand
	{
		/// <summary>
		/// Initializes a command with a cancellation token source.
		/// </summary>
		/// <param name="cancellationTokenSource">Object that performs cancellation</param>
		public CancelTaskCommand(CancellationTokenSource cancellationTokenSource)
		{
			_cancellationTokenSource = cancellationTokenSource;
		}

		/// <see cref="ICommand.Execute"/>
		public void Execute(object parameter)
		{
			_cancellationTokenSource.Cancel();
			OnCanExecuteChanged();
		}

		/// <see cref="ICommand.Execute"/>
		public bool CanExecute(object parameter)
		{
			return !_cancellationTokenSource.IsCancellationRequested;
		}

		/// <see cref="ICommand.CanExecuteChanged"/>
		public event EventHandler CanExecuteChanged
		{
			add
			{
				_canExecuteChanged += value;
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				_canExecuteChanged -= value;
				CommandManager.RequerySuggested -= value;
			}
		}

		private void OnCanExecuteChanged()
		{
			if (_canExecuteChanged != null)
				_canExecuteChanged(this, EventArgs.Empty);
		}

		private event EventHandler _canExecuteChanged;

		private readonly CancellationTokenSource _cancellationTokenSource;
	}
}