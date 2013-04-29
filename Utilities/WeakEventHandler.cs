//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
	/// <typeparam name="TArgs">The event args type</typeparam>
	public interface IWeakEventHandler<TArgs> where TArgs : EventArgs
	{
		/// <summary>
		/// The wrapped event handler.
		/// </summary>
		EventHandler<TArgs> Handler { get; }
	}

	/// <summary>
	/// A weak event handler whose target can be garbage collected without event unsubscription
	/// once it becoems unreachable.
	/// </summary>
	/// <typeparam name="TTarget">The handler declaring type</typeparam>
	/// <typeparam name="TArgs">The event args type</typeparam>
	public class WeakEventHandler<TTarget, TArgs> : IWeakEventHandler<TArgs>
		where TTarget : class
		where TArgs : EventArgs
	{
		/// <summary>
		/// Creates a new weak event handler.
		/// </summary>
		/// <param name="eventHandler">The event handler to wrap</param>
		/// <param name="unregister">A callback that will unsubscribe the handler once its target is garbage collected</param>
		public WeakEventHandler(EventHandler<TArgs> eventHandler, UnregisterCallback<TArgs> unregister)
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
		public void Invoke(object sender, TArgs e)
		{
			TTarget target = (TTarget)_targetRef.Target;

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
		public EventHandler<TArgs> Handler
		{
			get { return _handler; }
		}

		#endregion

		/// <summary>
		/// Converts a weak event handler to a regular event handler.
		/// </summary>
		public static implicit operator EventHandler<TArgs>(WeakEventHandler<TTarget, TArgs> weakHandler)
		{
			return weakHandler._handler;
		}

		private delegate void OpenEventHandler(TTarget @this, object sender, TArgs e);

		private readonly WeakReference _targetRef;
		private readonly OpenEventHandler _openHandler;
		private readonly EventHandler<TArgs> _handler;
		private UnregisterCallback<TArgs> _unregister;

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
		/// <typeparam name="TArgs">The event args type</typeparam>
		/// <param name="eventHandler">The event handler to wrap</param>
		/// <param name="unregister">A callback that will unsubscribe the handler once its target is garbage collected</param>
		/// <returns>A weak event handler</returns>
		public static EventHandler<TArgs> MakeWeak<TArgs>(this EventHandler<TArgs> eventHandler, UnregisterCallback<TArgs> unregister)
			where TArgs : EventArgs
		{
			if (eventHandler == null)
				throw new ArgumentNullException("eventHandler");

			if (eventHandler.Method.IsStatic || eventHandler.Target == null)
				throw new ArgumentException(@"Only instance methods are supported.", "eventHandler");

			var closedWeakHandlerType = openWeakEventHandlerType.MakeGenericType(eventHandler.Method.DeclaringType, typeof(TArgs));
			var handlerConstructor = closedWeakHandlerType.GetConstructor(new[] { typeof(EventHandler<TArgs>), typeof(UnregisterCallback<TArgs>) });

			var weakEventHandler = (IWeakEventHandler<TArgs>)handlerConstructor.Invoke(new object[] { eventHandler, unregister });

			return weakEventHandler.Handler;
		}

		private static readonly Type openWeakEventHandlerType = typeof(WeakEventHandler<,>);
	}
}