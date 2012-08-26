using System.Windows;
using System.Windows.Controls;

namespace Utilities.Controls
{
    /// <summary>
    /// Some funky progress bar control.
    /// </summary>
    public partial class ProgressBar : UserControl
    {
		/// <summary>
		/// Initializes a new progress bar.
		/// </summary>
		public ProgressBar()
		{
			InitializeComponent();
		}

		/// <summary>
		/// The current progress amount.
		/// </summary>
        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set 
			{  
				SetValue(ProgressProperty, value);
                UpdateLayout();
            }
        }

        /// <summary>
        /// The Progress amount dependency property.
        /// </summary>
        public static readonly DependencyProperty ProgressProperty = 
			DependencyProperty.Register("Progress", 
				typeof(int), 
				typeof(ProgressBar), 
				new UIPropertyMetadata(50));
    }
}
