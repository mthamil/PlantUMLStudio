using System;

namespace Utilities
{
	/// <summary>
	/// Callback that unsubscribes a handler from an event once its target has been garbage collected.
	/// </summary>
	public delegate void UnregisterCallback<E>(EventHandler<E> eventHandler) where E : EventArgs;

	/// <summary>
	/// Interface for a weak event handler.
	/// </summary>
	/// <typeparam name="E">The event args type</typeparam>
	public interface IWeakEventHandler<E> where E : EventArgs
	{
		/// <summary>
		/// The wrapped event handler.
		/// </summary>
		EventHandler<E> Handler { get; }
	}

	/// <summary>
	/// A weak event handler whose target can be garbage collected without event unsubscription
	/// once it becoems unreachable.
	/// </summary>
	/// <typeparam name="T">The handler declaring type</typeparam>
	/// <typeparam name="E">The event args type</typeparam>
	public class WeakEventHandler<T, E> : IWeakEventHandler<E>
		where T : class
		where E : EventArgs
	{
		/// <summary>
		/// Creates a new weak event handler.
		/// </summary>
		/// <param name="eventHandler">The event handler to wrap</param>
		/// <param name="unregister">A callback that will unsubscribe the handler once its target is garbage collected</param>
		public WeakEventHandler(EventHandler<E> eventHandler, UnregisterCallback<E> unregister)
		{
			_targetRef = new WeakReference(eventHandler.Target);
			_openHandler = (OpenEventHandler)Delegate.CreateDelegate(openEventHandlerType, null, eventHandler.Method);
			_handler = Invoke;
			_unregister = unregister;
		}

		/// <summary>
		/// Invokes the event handler.
		/// </summary>
		/// <param name="sender">The event sender</param>
		/// <param name="e">The event args</param>
		public void Invoke(object sender, E e)
		{
			T target = (T)_targetRef.Target;

			if (target != null)
				_openHandler.Invoke(target, sender, e);
			else if (_unregister != null)
			{
				_unregister(_handler);
				_unregister = null;
			}
		}

		#region Implementation of IWeakEventHandler<E>

		/// <see cref="IWeakEventHandler{E}.Handler"/>
		public EventHandler<E> Handler
		{
			get { return _handler; }
		}

		#endregion

		/// <summary>
		/// Converts a weak event handler to a regular event handler.
		/// </summary>
		public static implicit operator EventHandler<E>(WeakEventHandler<T, E> weakHandler)
		{
			return weakHandler._handler;
		}

		private delegate void OpenEventHandler(T @this, object sender, E e);

		private readonly WeakReference _targetRef;
		private readonly OpenEventHandler _openHandler;
		private readonly EventHandler<E> _handler;
		private UnregisterCallback<E> _unregister;

		private static readonly Type openEventHandlerType = typeof(OpenEventHandler);
	}

	/// <summary>
	/// Provides convenience utility methods for dealing with weak event handlers.
	/// </summary>
	public static class WeakEventHandlerExtensions
	{
		/// <summary>
		/// Wraps an event handler in a weak event handler.
		/// </summary>
		/// <typeparam name="E">The event args type</typeparam>
		/// <param name="eventHandler">The event handler to wrap</param>
		/// <param name="unregister">A callback that will unsubscribe the handler once its target is garbage collected</param>
		/// <returns>A weak event handler</returns>
		public static EventHandler<E> MakeWeak<E>(this EventHandler<E> eventHandler, UnregisterCallback<E> unregister)
			where E : EventArgs
		{
			if (eventHandler == null)
				throw new ArgumentNullException("eventHandler");

			if (eventHandler.Method.IsStatic || eventHandler.Target == null)
				throw new ArgumentException(@"Only instance methods are supported.", "eventHandler");

			var closedWeakHandlerType = openWeakEventHandlerType.MakeGenericType(eventHandler.Method.DeclaringType, typeof(E));
			var handlerConstructor = closedWeakHandlerType.GetConstructor(new[] { typeof(EventHandler<E>), typeof(UnregisterCallback<E>) });

			var weakEventHandler = (IWeakEventHandler<E>)handlerConstructor.Invoke(new object[] { eventHandler, unregister });

			return weakEventHandler.Handler;
		}

		private static readonly Type openWeakEventHandlerType = typeof(WeakEventHandler<,>);
	}
}