using System;
using System.Timers;

namespace Utilities.Chronology
{
	/// <summary>
	/// Wraps a timer based on the windows threading timer.
	/// </summary>
	public class SystemTimer : ITimer, IDisposable
	{
		#region ITimer Members

		/// <see cref="ITimer.Interval"/>
		public TimeSpan Interval { get; set; }

		/// <see cref="ITimer.Start"/>
		public void Start()
		{
			lock (_syncObject)
			{
				if (!TryStart())
					throw new InvalidOperationException("Timer is already started.");
			}
		}

		/// <see cref="ITimer.TryStart"/>
		public bool TryStart()
		{
			lock (_syncObject)
			{
				if (_running)
					return false;

				if (Interval.TotalMilliseconds <= Int32.MaxValue)
				{
					_timer.Interval = Interval.TotalMilliseconds;
					_largeIntervalRemaining = -1;
				}
				else
				{
					// workaround to allow the timer to have an interval greater than Int32.Max miliseconds
					_largeIntervalRemaining = _timer.Interval = Int32.MaxValue;
				}

				_timer.Elapsed += timer_Elapsed;
				try
				{
					_timer.Start();
					_running = true;
				}
				catch (Exception)
				{
					_timer.Elapsed -= timer_Elapsed;
					throw;
				}

				return true;
			}
		}

		/// <see cref="ITimer.Stop"/>	
		public void Stop()
		{
			lock (_syncObject)
			{
				if (!TryStop())
					throw new InvalidOperationException("Timer is already stopped.");
			}
		}

		/// <see cref="ITimer.TryStop"/>
		public bool TryStop()
		{
			lock (_syncObject)
			{
				if (!_running)
					return false;

				_largeIntervalRemaining = 0;

				_timer.Elapsed -= timer_Elapsed;
				_timer.Stop();
				_running = false;

				return true;
			}
		}

		/// <see cref="ITimer.Restart"/>
		public void Restart()
		{
			lock (_syncObject)
			{
				TryStop();
				Start();
			}
		}

		/// <see cref="ITimer.Started"/>
		public bool Started
		{
			get 
			{
				lock (_syncObject)
				{
					return _running;
				}
			}
		}

		/// <see cref="ITimer.Elapsed"/>
		public event EventHandler Elapsed;

		#endregion

		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			lock (_syncObject)
			{
				// if we have a large interval we need to count down
				if (_largeIntervalRemaining != -1)
				{
					// subtract the current interval from the total
					_largeIntervalRemaining -= Interval.TotalMilliseconds;
					// if we're left with none, we're done...otherwise see how much time is left
					if (_largeIntervalRemaining != 0)
					{
						// if we're left with some, reset the interval if the remainder
						// is less than the current interval
						if (_largeIntervalRemaining <= Int32.MaxValue)
							_timer.Interval = _largeIntervalRemaining;

						return;
					}
					else
					{
						_largeIntervalRemaining = -1;
					}
				}
			}

			EventHandler handler = Elapsed;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		#region IDisposable Members

		/// <see cref="System.IDisposable.Dispose"/>
		public void Dispose()
		{
			Dispose(true);

			// Take this object off the finalization queue and prevent
			// finalization code for this object from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose method.
		/// </summary>
		/// <param name="disposing">
		/// If disposing equals true, the method has been called directly or
		/// indirectly by a user's code. Managed and unmanaged resources can be
		/// disposed.  If disposing equals false, the method has been called by
		/// the runtime from inside the finalizer and you should not reference 
		/// other objects. Only unmanaged resources can be disposed.
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			lock (_syncObject)
			{
				if (_disposed == false)
				{
					if (disposing)
					{
						if (_running)
							_timer.Elapsed -= timer_Elapsed;

						// Dispose managed resources here.
						_timer.Dispose();
						_running = false;
					}

					_disposed = true;
				}
			}
		}

		private bool _disposed;

		#endregion

		/// <summary>
		/// The number of milliseconds remaining in the countdown 
		/// when the interval is large (> Int32 Max milliseconds).
		/// </summary>
		private double _largeIntervalRemaining = -1;

		/// <summary>
		/// True if the timer is actively running
		/// </summary>
		private bool _running;

		/// <summary>
		/// The underlying timer for which this object acts as a wrapper.
		/// </summary>
		private readonly Timer _timer = new Timer();

		/// <summary>
		/// Object used for locking.
		/// </summary>
		private readonly object _syncObject = new object();
	}
}
