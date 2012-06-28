using System;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Concurrency
{
	/// <summary>
	/// Provides an IProgress&lt;T&gt; that invokes callbacks for each reported progress value.
	/// </summary>
	/// <typeparam name="T">Specifies the type of the progress report value.</typeparam>
	/// <remarks>
	/// This class becomes obsolete in .NET v4.5.  It's implementation is a best guess attempt at matching
	/// the .NET v4.5 Progess class's behavior.
	/// 
	/// Any handler provided to the constructor or event handlers registered with the ProgressChanged 
	/// event are invoked through a SynchronizationContext instance captured when the instance is 
	/// constructed. If there is no current SynchronizationContext at the time of construction, 
	/// the callbacks will be invoked on the ThreadPool.
	/// </remarks>
	public class Progress<T> : IProgress<T>
	{
		/// <summary>
		/// Initializes the Progress&lt;T&gt; object.
		/// </summary>
		public Progress()
		{
			_progressScheduler = SynchronizationContext.Current == null 
				? TaskScheduler.Default 
				: TaskScheduler.FromCurrentSynchronizationContext();
		}

		/// <summary>
		/// Initializes the Progress&lt;T&gt; object with the specified callback.
		/// </summary>
		/// <param name="handler">
		/// A handler to invoke for each reported progress value. This handler will be invoked 
		/// in addition to any delegates registered with the ProgressChanged event. Depending on 
		/// the SynchronizationContext instance captured by the Progress&lt;T&gt; at construction, 
		/// it is possible that this handler instance could be invoked concurrently with itself.
		/// </param>
		public Progress(Action<T> handler)
			: this()
		{
			_handler = handler;
		}

		/// <summary>
		/// Event raised for each reported progress value.
		/// </summary>
		public event EventHandler<ProgressChangedEventArgs<T>> ProgressChanged;

		/// <summary>
		/// Reports a progress change.
		/// </summary>
		/// <param name="value">The value of the updated progress.</param>
		protected virtual void OnReport(T value)
		{
			var localEvent = ProgressChanged;
			Task.Factory.StartNew(() =>
			{
				if (localEvent != null)
					ProgressChanged(this, new ProgressChangedEventArgs<T>(value));
				_handler(value);
			},
			CancellationToken.None,
			TaskCreationOptions.None,
			_progressScheduler);
		}

		#region Implementation of IProgress<in T>

		/// <see cref="IProgress{T}.Report"/>
		void IProgress<T>.Report(T value)
		{
			OnReport(value);
		}

		#endregion

		private readonly Action<T> _handler;
		private readonly TaskScheduler _progressScheduler;
	}

	/// <summary>
	/// Event args used to report progress.
	/// </summary>
	/// <typeparam name="T">Specifies the type of the progress report value.</typeparam>
	public class ProgressChangedEventArgs<T> : EventArgs
	{
		/// <summary>
		/// Initializes new event args with the updated progress value.
		/// </summary>
		/// <param name="value">The updated progres value</param>
		public ProgressChangedEventArgs(T value)
		{
			Value = value;
		}

		/// <summary>
		/// The updated progress value.
		/// </summary>
		public T Value { get; private set; }
	}
}