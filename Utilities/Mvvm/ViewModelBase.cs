using System.ComponentModel;

namespace Utilities.Mvvm
{
	/// <summary>
	/// This class serves as base class for all ViewModel classes. 
	/// It provides implementations for interfaces that should be implemented by all view models
	/// </summary>
	public abstract class ViewModelBase : DisposableBase, INotifyPropertyChanged
	{
		/// <see cref="DisposableBase.OnDisposing"/>
		protected override void OnDisposing() { }

		#region INotifyPropertyChanged Members

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

		#endregion
	}
}
