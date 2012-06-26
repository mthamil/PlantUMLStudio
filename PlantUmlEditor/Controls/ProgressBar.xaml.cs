using System.Windows;
using System.Windows.Controls;

namespace PlantUmlEditor.Controls
{
    /// <summary>
    /// Some funky progress bar
    /// </summary>
    public partial class ProgressBar : UserControl
    {
		public ProgressBar()
		{
			InitializeComponent();
		}

        public int Progress
        {
            get { return (int)GetValue(ProgressProperty); }
            set { 
                SetValue(ProgressProperty, value);
                this.UpdateLayout();
            }
        }

        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressProperty = 
			DependencyProperty.Register("Progress", 
				typeof(int), 
				typeof(ProgressBar), 
				new UIPropertyMetadata(50));
    }
}
