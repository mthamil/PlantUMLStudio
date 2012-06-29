using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Utilities;

namespace PlantUmlEditor.View
{
    /// <summary>
    /// Takes a DiagramFile object into DataContext and renders the text editor and 
    /// shows the generated diagram
    /// </summary>
    public partial class DiagramViewControl : UserControl
    {
        public DiagramViewControl()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

			SnippetsMenu.DataContext = DataContext;
        }

        private void AddStuff_Click(object sender, RoutedEventArgs e)
        {
            // Trick: Open the context menu automatically whenever user
            // clicks the "Add" button
			SnippetsMenu.IsOpen = true;
        }
    }
}
