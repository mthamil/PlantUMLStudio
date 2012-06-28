using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using PlantUmlEditor.Container;
using PlantUmlEditor.Controls;

namespace PlantUmlEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		private void Application_Startup(object sender, StartupEventArgs e)
		{
			Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
			TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

			var containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterModule<CoreModule>();
			containerBuilder.RegisterModule<PresentationModule>();
			_container = containerBuilder.Build();
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			_container.Dispose();
		}

		/// <summary>
		/// The application's IoC container.
		/// </summary>
		public static IContainer Container { get { return _container; } }
		private static IContainer _container;

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
