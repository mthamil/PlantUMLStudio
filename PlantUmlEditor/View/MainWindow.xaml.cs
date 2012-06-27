﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using PlantUmlEditor.Model;
using PlantUmlEditor.Properties;
using PlantUmlEditor.ViewModel;
using Utilities;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

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

		private void DiagramFileListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			((DiagramsViewModel)DiagramFileListBox.DataContext).OpenDiagramCommand.Execute(DiagramFileListBox.DataContext);
		}

		/// <summary>
		/// </summary>
		/// <param name="diagram">
		/// </param>
		protected void DiagramViewControl_OnAfterSave(Diagram diagram)
		{
			StopProgress("Saved.");
		}

		/// <summary>
		/// </summary>
		/// <param name="diagram">
		/// </param>
		protected void DiagramViewControl_OnBeforeSave(Diagram diagram)
		{
			StartProgress("Saving and generating diagram...");
		}

		private void StartProgress(string message)
		{
			StatusMessage.Text = message;
			StatusProgressBar.IsIndeterminate = true;
			StatusProgressBar.Visibility = Visibility.Visible;
		}

		private void StartProgress(string message, int percentage)
		{
			StatusMessage.Text = message;

			StatusProgressBar.Use(p =>
			{
				p.IsIndeterminate = false;
				p.Visibility = Visibility.Visible;
				p.Minimum = 0;
				p.Maximum = 100;
				p.Value = percentage;
			});
		}

		private void StopProgress(string message)
		{
			StatusMessage.Text = message;
			StatusProgressBar.Use(p =>
			{
				p.Visibility = Visibility.Hidden;
				p.IsIndeterminate = false;
				p.Value = 0;
			});
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (!CheckGraphViz())
			{
				Close();
				return;
			}
		}

		private void NameHyperlink_Click(object sender, RoutedEventArgs e)
		{
			Process.Start((e.OriginalSource as Hyperlink).NavigateUri.ToString());
		}

		private bool CheckGraphViz()
		{
			string graphVizPath = Settings.Default.GraphVizLocation;
			if (string.IsNullOrEmpty(graphVizPath))
				graphVizPath = Environment.GetEnvironmentVariable("GRAPHVIZ_DOT");

			if (string.IsNullOrEmpty(graphVizPath) || !File.Exists(graphVizPath))
			{
				var dialog = new OpenFileDialog();
				dialog.FileName = "dot.exe";
				dialog.DefaultExt = ".exe";
				dialog.Filter = "dot.exe|dot.exe";

				// See if graphviz is there in environment PATH
				string envPath = Environment.GetEnvironmentVariable("PATH");
				string[] paths = envPath.Split(';');
				dialog.InitialDirectory = paths.Where(p => p.ToLower().Contains("graphviz")).FirstOrDefault();
				if (dialog.ShowDialog(Owner).Value)
				{
					Settings.Default.GraphVizLocation = dialog.FileName;
					Settings.Default.Save();
					return true;
				}
				else
				{
					return false;
				}

				//MessageBox.Show(Window.GetWindow(this),
				//    "You haven't either installed GraphViz or you haven't created " +
				//    Environment.NewLine + "the environment variable name GRAPHVIZ_DOT that points to the dot.exe" +
				//    Environment.NewLine + "where GraphViz is installed. Please create and re-run.",
				//    "GraphViz Environment variable not found",
				//    MessageBoxButton.OK,
				//    MessageBoxImage.Warning);

				//return false;
			}


			return true;
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