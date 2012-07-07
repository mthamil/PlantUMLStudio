using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using PlantUmlEditor.Properties;

namespace PlantUmlEditor.View
{
	/// <summary>
	/// MainWindow that hosts the diagram list box and the editing environment.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
		}

		private void NameHyperlink_Click(object sender, RoutedEventArgs e)
		{
			Process.Start((e.OriginalSource as Hyperlink).NavigateUri.ToString());
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (Directory.Exists(DiagramLocationTextBox.Text))
			{
				Settings.Default.LastPath = DiagramLocationTextBox.Text;
				Settings.Default.Save();
			}
		}
	}
}