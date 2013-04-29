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
	/// Provides a safe method for handling objects where it is expected that a value may not exist.
	/// This is similar to a Nullable type, but it is also usable with reference types using the 
	/// Option/Maybe monad pattern. It is intended to serve the same function that F#'s Options module 
	/// does, but implemented in C#.
	/// </summary>
	public abstract class Option<T>
	{
		/// <summary>
		/// This constructor is internal to disallow further subclassing.
		/// </summary>
		internal Option() { }

		/// <summary>
		/// Returns a None Option of a given type.
		/// </summary>
		public static Option<T> None()
		{
			return none;
		}
		private static readonly Option<T> none = new None<T>();	// this can be a singleton for each type because None has no variable properties

		/// <summary>
		/// Creates a Some Option with the given value.
		/// </summary>
		public static Option<T> Some(T value)
		{
			return new Some<T>(value);
		}

		/// <summary>
		/// Creates an Option from a value that may be null.
		/// None will be returned if the value is null, otherwise
		/// Some.
		/// </summary>
		public static Option<T> From(T value)
		{
			return value == null ? None() : Some(value);
		}

		/// <summary>
		/// Converts a value to an Option type.
		/// </summary>
		public static implicit operator Option<T>(T value)
		{
			return From(value);
		}

		/// <summary>
		/// True if a value exists.
		/// </summary>
		public abstract bool HasValue { get; }

		/// <summary>
		/// The value if it exists.
		/// </summary>
		public abstract T Value { get; }

		/// <summary>
		/// Applies a mapping function to an Option. If the Option is a Some, 
		/// its value is mapped. If the Option is a None, a None of the 
		/// destination type is returned.
		/// </summary>
		/// <typeparam name="TResult">The type to map to</typeparam>
		/// <param name="selector">The mapping function</param>
		/// <returns>A Some with the mapped value or a None</returns>
		public abstract Option<TResult> Select<TResult>(Func<T, TResult> selector);

		/// <summary>
		/// Applies a mapping function to an Option.
		/// </summary>
		/// <typeparam name="TResult">The type of Option to map to</typeparam>
		/// <param name="optionSelector">The mapping function</param>
		/// <remarks>This method corresponds to the monad pattern's Bind.</remarks>
		public abstract Option<TResult> SelectMany<TResult>(Func<T, Option<TResult>> optionSelector);

		/// <summary>
		/// Applies a mapping function to an Option.
		/// </summary>
		public abstract Option<TResult> SelectMany<TIntermediate, TResult>(Func<T, Option<TIntermediate>> optionSelector, Func<T, TIntermediate, TResult> resultSelector);

		/// <summary>
		/// Returns this Option if it is Some and its value matches the given predicate.
		/// Otherwise, None is returned.
		/// </summary>
		/// <param name="predicate">The condition to be met</param>
		public abstract Option<T> Where(Func<T, bool> predicate);

		/// <summary>
		/// Performs an action on an Option's value if it is Some,
		/// otherwise no action is performed.
		/// </summary>
		/// <param name="action">The action to perform</param>
		public abstract void Apply(Action<T> action);

		/// <summary>
		/// Returns Option.Some if an Option is Some, otherwise if None,
		/// the given function is executed to return an alternative. 
		/// </summary>
		/// <param name="fallbackAction">The alternative function</param>
		public abstract Option<T> OrElse(Func<Option<T>> fallbackAction);

		/// <summary>
		/// Retrieves an Option's value if it is Some.  Otherwise if None, the given function
		/// is executed to return an alternative value.
		/// </summary>
		/// <param name="fallbackAction">The alternative function</param>
		public abstract T GetOrElse(Func<T> fallbackAction);
	}

	/// <summary>
	/// Represents an Option with no value.
	/// </summary>
	internal class None<T> : Option<T>
	{
		/// <summary>
		/// Always returns false.
		/// </summary>
		public override bool HasValue
		{
			get { return false; }
		}

		/// <summary>
		/// Throws an exception because no value exists.
		/// </summary>
		public override T Value
		{
			get { throw new InvalidOperationException(); }
		}

		/// <summary>
		/// Returns None of the result type.
		/// </summary>
		public override Option<TResult> Select<TResult>(Func<T, TResult> selector)
		{
			return Option<TResult>.None();
		}

		/// <summary>
		/// Returns None of the result type.
		/// </summary>
		public override Option<TResult> SelectMany<TResult>(Func<T, Option<TResult>> optionSelector)
		{
			return Option<TResult>.None();
		}

		/// <summary>
		/// Returns None of the result type.
		/// </summary>
		public override Option<TResult> SelectMany<TIntermediate, TResult>(Func<T, Option<TIntermediate>> optionSelector, Func<T, TIntermediate, TResult> resultSelector)
		{
			return Option<TResult>.None(); 
		}

		/// <summary>
		/// Returns None.
		/// </summary>
		public override Option<T> Where(Func<T, bool> predicate)
		{
			return None(); 
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		public override void Apply(Action<T> action)
		{
			// Do nothing.
		}

		/// <summary>
		/// Executes an alternative function.
		/// </summary>
		public override Option<T> OrElse(Func<Option<T>> fallbackAction)
		{
			return fallbackAction();
		}

		/// <summary>
		/// Executes an alternative function.
		/// </summary>
		public override T GetOrElse(Func<T> fallbackAction)
		{
			return fallbackAction();
		}
	}

	/// <summary>
	/// Represents an Option that has a value.
	/// </summary>
	internal class Some<T> : Option<T>
	{
		/// <summary>
		/// Creates a new Some Option with the given value.
		/// </summary>
		/// <param name="value">The value of the Option</param>
		public Some(T value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			_value = value;
		}

		/// <summary>
		/// Always returns true.
		/// </summary>
		public override bool HasValue
		{
			get { return true; }
		}

		/// <summary>
		/// The value of the Option.
		/// </summary>
		public override T Value
		{
			get { return _value; }
		}

		/// <summary>
		/// Applies a mapping function to a Some's value.
		/// </summary>
		public override Option<TResult> Select<TResult>(Func<T, TResult> selector)
		{
			return Option<TResult>.From(selector(Value));
		}

		/// <summary>
		/// Applies a mapping function to a Some's value.
		/// </summary>
		public override Option<TResult> SelectMany<TResult>(Func<T, Option<TResult>> optionSelector)
		{
			return optionSelector(Value);
		}

		/// <summary>
		/// Applies a mapping function to a Some's value.
		/// </summary>
		public override Option<TResult> SelectMany<TIntermediate, TResult>(Func<T, Option<TIntermediate>> optionSelector, Func<T, TIntermediate, TResult> resultSelector)
		{
			var intermediate = optionSelector(Value);
			if (intermediate.HasValue)
				return Option<TResult>.From(resultSelector(Value, intermediate.Value));

			return Option<TResult>.None();
		}

		/// <summary>
		/// Returns this Option if its value meets the given condition.
		/// </summary>
		public override Option<T> Where(Func<T, bool> predicate)
		{
			return predicate(Value) ? this : None();
		}

		/// <summary>
		/// Performs an action on the Option's value.
		/// </summary>
		public override void Apply(Action<T> action)
		{
			action(Value);
		}

		/// <summary>
		/// Returns this.
		/// </summary>
		public override Option<T> OrElse(Func<Option<T>> fallbackAction)
		{
			return this;
		}

		/// <summary>
		/// Returns this Option's value.
		/// </summary>
		public override T GetOrElse(Func<T> fallbackAction)
		{
			return Value;
		}

		/// <summary>
		/// Whether a Some Option is equal to another.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			Some<T> other = obj as Some<T>;
			if (other == null)
				return false;

			return Value.Equals(other.Value);
		}

		/// <summary>
		/// Gets the hash code for a Some Option.
		/// </summary>
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		private readonly T _value;
	}
}