using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xaml;
using Utilities.Mvvm.Commands;
using Expressions = System.Linq.Expressions;

namespace Utilities.Controls.Markup
{
	/// <summary>
	/// Markup extension that defines a command.
	/// </summary>
	[MarkupExtensionReturnType(typeof(ICommand))]
	public class CommandExtension : MarkupExtension
	{
		/// <summary>
		/// The path of the method to invoke.
		/// </summary>
		public PropertyPath Execute { get; set; }

		/// <summary>
		/// The path of a property that determines whether a command can be invoked.
		/// </summary>
		public PropertyPath CanExecute { get; set; }

		#region Overrides of MarkupExtension

		/// <see cref="MarkupExtension.ProvideValue"/>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Execute == null)
				throw new InvalidOperationException("A method must be provided.");

			var target = serviceProvider.GetService<IProvideValueTarget>();
			var rootProvider = serviceProvider.GetService<IRootObjectProvider>();

			var targetObj = target.TargetObject as DependencyObject;
			if (targetObj == null)
				throw new InvalidOperationException("The target object must be a DependencyObject");

			object dataContext = RetrieveDataContext(targetObj, rootProvider);
			var methodCall = GetMethod(dataContext, Execute);
			var commandParameter = methodCall.Method.GetParameters().FirstOrDefault();

			Type delegateType;
			Type commandType;
			IEnumerable<ParameterExpression> commandParameters; 
			if (commandParameter == null)
			{
				delegateType = typeof(Action);
				commandType = typeof(RelayCommand);
				commandParameters = Enumerable.Empty<ParameterExpression>();
			}
			else
			{
				delegateType = typeof(Action<>).MakeGenericType(commandParameter.ParameterType);
				commandType = typeof(RelayCommand<>).MakeGenericType(commandParameter.ParameterType);
				commandParameters = new [] { Expressions.Expression.Parameter(commandParameter.ParameterType, commandParameter.Name) };
			}

			var lambda = Expressions.Expression.Lambda(delegateType, methodCall, commandParameters);
			var action = lambda.Compile();

			Delegate canExecute = null;
			if (CanExecute != null)
			{
				var canExecuteDelegateType = commandParameter == null 
					? typeof(Func<bool>) 
					: typeof(Predicate<>).MakeGenericType(commandParameter.ParameterType);

				var canExecuteParameters = commandParameter == null 
					? Enumerable.Empty<ParameterExpression>() 
					: new[] { Expressions.Expression.Parameter(commandParameter.ParameterType, commandParameter.Name) };

				var canExecuteExpression = GetMember(dataContext, CanExecute);
				var canExecuteLambda = Expressions.Expression.Lambda(canExecuteDelegateType, canExecuteExpression, canExecuteParameters);
				canExecute = canExecuteLambda.Compile();
			}

			var command = Activator.CreateInstance(commandType, action, canExecute);
			return command;
		}

		#endregion

		private static object RetrieveDataContext(DependencyObject target, IRootObjectProvider rootProvider)
		{
			object dataContext = target.GetValue(FrameworkElement.DataContextProperty)
			                  ?? target.GetValue(FrameworkContentElement.DataContextProperty);

			if (dataContext == null)
			{
				var root = rootProvider.RootObject as DependencyObject;
				if (root != null)
				{
					dataContext = root.GetValue(FrameworkElement.DataContextProperty)
							   ?? root.GetValue(FrameworkContentElement.DataContextProperty);

					if (dataContext == null)
						throw new InvalidOperationException("DataContext could not be found.");
				}
			}

			return dataContext;
		}

		private static MethodCallExpression GetMethod(object root, PropertyPath path)
		{
			var components = path.Path.Split('.');
			Expressions.Expression current = Expressions.Expression.Constant(root);
			foreach (var component in components)
			{
				var property = current.Type.GetProperty(component);
				if (property != null)
				{
					current = Expressions.Expression.MakeMemberAccess(current, property);
				}
				else
				{
					return Expressions.Expression.Call(current, current.Type.GetMethod(component));
				}
			}

			throw new InvalidOperationException(String.Format("Method path '{0}' invalid", path.Path));
		}

		private static Expressions.Expression GetMember(object root, PropertyPath path)
		{
			var components = path.Path.Split('.');
			Expressions.Expression current = Expressions.Expression.Constant(root);
			foreach (var component in components)
			{
				var property = current.Type.GetProperty(component);
				if (property != null)
					current = Expressions.Expression.MakeMemberAccess(current, property);
			}

			return current;
		}
	}
}