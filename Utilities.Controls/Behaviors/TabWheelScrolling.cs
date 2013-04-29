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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Utilities.Controls.Behaviors
{
	/// <summary>
	/// Behavior that adds support for scrolling with a mouse wheel in a tab control's tab strip.
	/// </summary>
	public class TabWheelScrolling : LoadDependentBehavior<TabControl>
	{
		/// <see cref="LoadDependentBehavior{T}.OnLoaded"/>
		protected override void OnLoaded()
		{
			_tabPanel = (
				from child in AssociatedObject.VisualChildren()
				from subChild in child.VisualChildren()
				select subChild).OfType<TabPanel>().Single();

			_tabPanel.MouseWheel += tabPanel_MouseWheel;
		}

		/// <see cref="Behavior.OnDetaching"/>
		protected override void OnDetaching()
		{
			_tabPanel.MouseWheel -= tabPanel_MouseWheel;
		}

		/// <summary>
		/// Whether to invert the tab scrolling direction. If true, scrolling down will move to the next tab and
		/// scrolling up will move to the previous tab. If false, scrolling down will move to the previous tab
		/// and scrolling up will move to the next tab.
		/// </summary>
		public bool InvertScrollDirection
		{
			get { return (bool)GetValue(InvertScrollDirectionProperty); }
			set { SetValue(InvertScrollDirectionProperty, value); }
		}

		/// <summary>
		/// The InvertScrollDirection dependency property.
		/// </summary>
		public static readonly DependencyProperty InvertScrollDirectionProperty =
			DependencyProperty.Register(
				"InvertScrollDirection",
				typeof(bool),
				typeof(TabWheelScrolling),
				new PropertyMetadata(false));

		/// <summary>
		/// Whether tab scrolling should wrap around the ends of the tab strip.
		/// </summary>
		public bool ScrollWrapsAround
		{
			get { return (bool)GetValue(ScrollWrapsAroundProperty); }
			set { SetValue(ScrollWrapsAroundProperty, value); }
		}

		/// <summary>
		/// The ScrollWrapsAround dependency property.
		/// </summary>
		public static readonly DependencyProperty ScrollWrapsAroundProperty =
			DependencyProperty.Register(
				"ScrollWrapsAround",
				typeof(bool),
				typeof(TabWheelScrolling),
				new PropertyMetadata(false));

		void tabPanel_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			bool nextTab = InvertScrollDirection ? e.Delta < 0 : e.Delta > 0;
			if (nextTab)
			{
				if (AssociatedObject.SelectedIndex < (AssociatedObject.Items.Count - 1))
					AssociatedObject.SelectedIndex++;
				else if (ScrollWrapsAround)
					AssociatedObject.SelectedIndex = 0;
			}
			else
			{
				if (AssociatedObject.SelectedIndex > 0)
					AssociatedObject.SelectedIndex--;
				else if (ScrollWrapsAround)
					AssociatedObject.SelectedIndex = AssociatedObject.Items.Count - 1;
			}
		}

		private TabPanel _tabPanel;
	}
}