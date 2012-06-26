using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows.Input;
using Utilities.Reflection;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking delegates. CanExecute and CanExecuteChanged are bound to 
	/// a property of another object.
	/// </summary>
	public class BoundRelayCommand<TSource> : ICommand where TSource : INotifyPropertyChanged
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecuteProperty">An expression referencing a boolean property that determines the command's execution status</param>
		/// <param name="propertyDeclarer">The object that declares the property that triggers a change in command execution status</param>
		public BoundRelayCommand(Action<object> execute, Expression<Func<TSource, bool>> canExecuteProperty, TSource propertyDeclarer)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			if (canExecuteProperty == null)
				throw new ArgumentNullException("canExecuteProperty");

			if (propertyDeclarer == null)
				throw new ArgumentNullException("propertyDeclarer");

			_execute = execute;

			var property = Reflect.PropertyOf(canExecuteProperty);
			_propertyName = property.Name;
			propertyDeclarer.PropertyChanged += propertyDeclarer_PropertyChanged;
			Func<TSource, bool> func = canExecuteProperty.Compile();
			_canExecute = () => func(propertyDeclarer);
		}

		#region ICommand Members

		/// <see cref="ICommand.CanExecute"/>
		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute();
		}

		/// <see cref="ICommand.Execute"/>
		public void Execute(object parameter)
		{
			_execute(parameter);
		}

		/// <see cref="ICommand.CanExecuteChanged"/>
		public event EventHandler CanExecuteChanged;

		private void OnCanExecuteChanged()
		{
			var localEvent = CanExecuteChanged;
			if (localEvent != null)
				localEvent(this, EventArgs.Empty);
		}

		#endregion

		void propertyDeclarer_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == _propertyName)
				OnCanExecuteChanged();
		}

		private readonly Action<object> _execute;
		private readonly Func<bool> _canExecute;
		private readonly string _propertyName;
	}
}