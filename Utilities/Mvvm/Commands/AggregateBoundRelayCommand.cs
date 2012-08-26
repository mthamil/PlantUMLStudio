using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Utilities.Mvvm.Commands
{
	/// <summary>
	/// A command whose sole purpose is to relay its functionality to other
	/// objects by invoking delegates. CanExecute and CanExecuteChanged are bound to 
	/// each value of a given property of a collection of items.
	/// </summary>
	public class AggregateBoundRelayCommand<TCollectionSource, TPropertySource, TCollection> : ChildPropertyBoundCommandBase<TCollectionSource, TPropertySource, TCollection>
		where TPropertySource : INotifyPropertyChanged
		where TCollection : IEnumerable<TPropertySource>
	{
		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="collectionExpression">The collection whose items trigger a change in whether the command can execute</param>
		/// <param name="propertyAggregationExpression">
		/// A predicate that aggregates the values of a property from each child item.
		/// This predicate MUST make use of a boolean property of the child type.
		/// </param>
		/// <param name="parent">The object that declares the collection property whose items trigger a change in whether the command can execute</param>
		public AggregateBoundRelayCommand(Action<object> execute, Expression<Func<TCollectionSource, TCollection>> collectionExpression,
										  Expression<Func<TCollection, bool>> propertyAggregationExpression, TCollectionSource parent)
			: base(execute, collectionExpression, GetChildProperty(propertyAggregationExpression), parent)
		{
			if (propertyAggregationExpression == null) 
				throw new ArgumentNullException("propertyAggregationExpression");

			var propertyAggregationFunc = propertyAggregationExpression.Compile();
			CanExecutePredicate = () => propertyAggregationFunc(GetCollection());
		}

		private static Expression<Func<TPropertySource, bool>> GetChildProperty(Expression<Func<TCollection, bool>> propertyAggregationExpression)
		{
			var methodCall = (MethodCallExpression)propertyAggregationExpression.Body;
			var childPropertyExpression = methodCall.Arguments.OfType<Expression<Func<TPropertySource, bool>>>().Single();
			return childPropertyExpression;
		}
	}
}