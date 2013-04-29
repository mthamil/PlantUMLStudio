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
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Action that, in conjunction with the CollectionViewSource.Filter event, 
	/// provides a bindable CollectionViewSource filter.
	/// </summary>
	public class CollectionViewSourceFilter : TriggerAction<CollectionViewSource>
	{
		/// <see cref="System.Windows.Interactivity.TriggerAction.Invoke"/>
		protected override void Invoke(object parameter)
		{
			var e = (FilterEventArgs)parameter;
			if (Filter != null)
				e.Accepted = Filter(e.Item);
		}

		/// <summary>
		/// Gets or sets the collection filter.
		/// </summary>
		public Predicate<object> Filter
		{
			get { return (Predicate<object>)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}

		/// <summary>
		/// The Filter dependency property.
		/// </summary>
		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register(
				"Filter",
				typeof(Predicate<object>),
				typeof(CollectionViewSourceFilter),
				new UIPropertyMetadata(null));
	}
}