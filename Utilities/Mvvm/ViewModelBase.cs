using System;
using System.ComponentModel;

namespace Utilities.Mvvm
{
	/// <summary>
	/// This class serves as base class for all ViewModel classes. 
	/// It provides implementations for interfaces that should be implemented by all view models
	/// </summary>
	public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
	{
		/// <see cref="INotifyPropertyChanged.PropertyChanged"/>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		/// <param name="propertyName">The name of the property that changed</param>
		protected void OnPropertyChanged(string propertyName)
		{
			var localEvent = PropertyChanged;
			if (localEvent != null)
				localEvent(this, new PropertyChangedEventArgs(propertyName));
		}

		#region IDisposable Members

		/// <see cref="IDisposable.Dispose"/>
		public void Dispose()
		{
			Dispose(true);
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Implements the actual disposal logic.  Subclasses should
		/// override this method to clean up resources.
		/// </summary>
		/// <param name="disposing">Whether the class is disposing from the Dispose() method</param>
		protected virtual void Dispose(bool disposing)
		{
			// Implementing classes should override this method
		}

		#endregion

		/// <summary>
		/// Use C# destructor syntax for finalization code.
		/// This destructor will run only if the Dispose method
		/// does not get called.
		/// It gives your base class the opportunity to finalize.
		/// Do not provide destructors in types derived from this class.
		/// </summary>
		~ViewModelBase()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
	}
}
