using System;

namespace Utilities
{
	/// <summary>
	/// Provides a safe method for handling objects where it is expected that a value may not exist.
	/// This is similar to a Nullable type, but it is also usable with reference types using the Null
	/// Object Pattern. It is intended to serve the same function that F#'s Options module does, but 
	/// implemented in C#.
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
		/// the value is mapped. If the Option is a None, a None of the 
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
		public abstract Option<TResult> SelectMany<TResult>(Func<T, Option<TResult>> optionSelector);

		/// <summary>
		/// Applies a mapping function to an Option.
		/// </summary>
		public abstract Option<TResult> SelectMany<TIntermediate, TResult>(Func<T, Option<TIntermediate>> optionSelector, Func<T, TIntermediate, TResult> resultSelector);
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
		/// Returns a None of the result type.
		/// </summary>
		public override Option<TResult> Select<TResult>(Func<T, TResult> selector)
		{
			return Option<TResult>.None();
		}

		/// <summary>
		/// Returns a None of the result type.
		/// </summary>
		public override Option<TResult> SelectMany<TResult>(Func<T, Option<TResult>> optionSelector)
		{
			return Option<TResult>.None();
		}

		/// <summary>
		/// Returns a None of the result type.
		/// </summary>
		public override Option<TResult> SelectMany<TIntermediate, TResult>(Func<T, Option<TIntermediate>> optionSelector, Func<T, TIntermediate, TResult> resultSelector)
		{
			return Option<TResult>.None(); 
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

		private readonly T _value;

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
	}
}