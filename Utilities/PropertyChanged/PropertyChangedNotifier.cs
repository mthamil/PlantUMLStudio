using System.ComponentModel;

namespace Utilities.PropertyChanged
{
	/// <summary>
	/// A base implementation of INotifyPropertyChanged.
	/// </summary>
	public abstract class PropertyChangedNotifier : INotifyPropertyChanged
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
	}
}