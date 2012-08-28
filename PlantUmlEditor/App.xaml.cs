﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Autofac;
using PlantUmlEditor.Container;
using PlantUmlEditor.Core.Update;
using PlantUmlEditor.Properties;
using Utilities.Controls;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace PlantUmlEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		protected override void OnStartup(StartupEventArgs e)
		{
			if (!CheckGraphViz())
			{
				MessageBox.Show(
					"Either GraphViz is not installed or the environment variable\n" +
					"GRAPHVIZ_DOT that points to the dot.exe where GraphViz is\n" +
					"installed has not been created. Please create and re-run.",
					"GraphViz Environment variable not found",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);

				Shutdown(1);
				return;
			}

			base.OnStartup(e);
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

			var containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterModule<CoreModule>();
			containerBuilder.RegisterModule<PresentationModule>();
			_container = containerBuilder.Build();

			//var updater = _container.Resolve<IDependencyUpdateChecker>();
			//updater.HasUpdateAsync();
			//updater.DownloadLatestAsync(CancellationToken.None);
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			if (_container != null)
				_container.Dispose();
		}

		/// <summary>
		/// The application's IoC container.
		/// </summary>
		public static IContainer Container { get { return _container; } }
		private static IContainer _container;

		private static bool CheckGraphViz()
		{
			string graphVizPath = Settings.Default.GraphVizLocation;
			if (string.IsNullOrEmpty(graphVizPath))
				graphVizPath = Environment.GetEnvironmentVariable("GRAPHVIZ_DOT");

			if (string.IsNullOrEmpty(graphVizPath) || !File.Exists(graphVizPath))
			{
				var dialog = new OpenFileDialog
				{
					FileName = "dot.exe",
					DefaultExt = ".exe",
					Filter = "dot.exe|dot.exe"
				};

				// See if graphviz is there in environment PATH
				string envPath = Environment.GetEnvironmentVariable("PATH");
				string[] paths = envPath.Split(';');
				dialog.InitialDirectory = paths.FirstOrDefault(p => p.ToLower().Contains("graphviz"));
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					Settings.Default.GraphVizLocation = dialog.FileName;
					Settings.Default.Save();
					return true;
				}

				return false;
			}

			return true;
		}

		void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			e.Handled = true;
			uiDispatcher.Invoke(new Action(() => ShowMessageBox(e.Exception)));
		}

		void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
		{
			e.SetObserved();
			uiDispatcher.Invoke(new Action(() => ShowMessageBox(e.Exception)));
		}

		private static void ShowMessageBox(Exception exception)
		{
			var messageBox = new ScrollableMessageBox
			{
				Buttons = MessageBoxButton.YesNo,
				Title = "Application Error",
				Caption =
@"An application error occurred. If this error occurs again there may be a more serious malfunction in the application, and it should be closed.

Do you want to exit the application?
(Warning: If you click Yes the application will close, if you click No the application will continue)",

				Message = String.Format("{0}{1}{2}{3}{4}{5}",
				exception.GetType().Name,
				Environment.NewLine,
				exception.Message,
				Environment.NewLine,
				exception.StackTrace,
				exception.InnerException != null
										? String.Format("{0}{1}{2}{3}{4}",
											exception.InnerException.GetType().Name,
											Environment.NewLine,
											exception.InnerException.Message,
											Environment.NewLine,
											exception.InnerException.StackTrace)
										: string.Empty)
			};

			messageBox.ShowDialog();
			if (messageBox.DialogResult.HasValue && messageBox.DialogResult.Value)
			{
				Current.Shutdown();
			}
		}

		private readonly Dispatcher uiDispatcher = Dispatcher.CurrentDispatcher;
	}
}
