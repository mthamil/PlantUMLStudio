using System;
using System.Timers;

namespace Utilities.Chronology
{
	/// <summary>
	/// This class wraps a timer based on the windows threading timer
	/// </summary>
	public class SystemTimersTimer : ITimer, IDisposable
	{
		#region ITimer Members

		/// <see cref="ITimer.Interval"/>
		public TimeSpan Interval
		{
			get
			{
				return interval;
			}
			set
			{
				interval = value;
			}
		}

		/// <see cref="ITimer.Start"/>
		/// <exception cref="System.InvalidOperationException">
		/// Thrown if the timer has already been started
		/// </exception>
		public void Start()
		{
			lock (syncObject)
			{
				if (running)
					throw new InvalidOperationException();

				if (interval.TotalMilliseconds <= Int32.MaxValue)
				{
					timer.Interval = interval.TotalMilliseconds;
					largeIntervalRemaining = -1;
				}
				else
				{
					// workaround to allow the timer to have an interval greater than Int32.Max miliseconds
					largeIntervalRemaining = timer.Interval = Int32.MaxValue;
				}

				timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
				try
				{
					timer.Start();
					running = true;
				}
				catch (Exception)
				{
					timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed);
					throw;
				}
			}
		}

		/// <see cref="ITimer.Stop"/>
		/// <exception cref="System.InvalidOperationException">
		/// Thrown if the timer has not yet been started
		/// </exception>		
		public void Stop()
		{
			lock (syncObject)
			{
				if (!running)
					throw new InvalidOperationException();

				largeIntervalRemaining = 0;

				timer.Elapsed -= new ElapsedEventHandler(timer_Elapsed);
				timer.Stop();
				running = false;
			}
		}

		/// <see cref="ITimer.Elapsed"/>
		public event EventHandler Elapsed;

		#endregion

		#region IDisposable Members

		/// <see cref="System.IDisposable.Dispose"/>
		public void Dispose()
		{
			Dispose(true);

			// Take this object off the finalization queue and prevent
			// finalization code for this object from executing a second time.
			GC.SuppressFinalize(this);
		}

		#endregion

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
			lock (syncObject)
			{
				if (disposed == false)
				{
					if (disposing)
					{
						if (running)
							timer.Elapsed -= timer_Elapsed;

						// Dispose managed resources here.
						timer.Dispose();
						running = false;
					}

					disposed = true;
				}
			}
		}

		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			lock (syncObject)
			{
				// if we have a large interval we need to count down
				if (largeIntervalRemaining != -1)
				{
					// subtract the current interval from the total
					largeIntervalRemaining -= Interval.TotalMilliseconds;
					// if we're left with none, we're done...otherwise see how much time is left
					if (largeIntervalRemaining != 0)
					{
						// if we're left with some, reset the interval if the remainder
						// is less than the current interval
						if (largeIntervalRemaining <= Int32.MaxValue)
							timer.Interval = largeIntervalRemaining;

						return;
					}
					else
					{
						largeIntervalRemaining = -1;
					}
				}
			}

			EventHandler handler = Elapsed;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		/// <summary>
		/// True if the object has been disposed
		/// </summary>
		private bool disposed;
		/// <summary>
		/// The number of milliseconds remaining in the countdown 
		/// when the interval is large (> Int32 Max milliseconds)
		/// </summary>
		private double largeIntervalRemaining = -1;
		/// <summary>
		/// The underlying timer for which this object acts as a wrapper
		/// </summary>
		private Timer timer = new Timer();
		/// <summary>
		/// True if the timer is actively running
		/// </summary>
		private bool running;
		/// <summary>
		/// Object used for locking
		/// </summary>
		private readonly object syncObject = new object();
		/// <summary>
		/// The Timer interval reported to the user
		/// </summary>
		private TimeSpan interval;
	}
}
