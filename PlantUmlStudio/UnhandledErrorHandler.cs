//  PlantUML Studio
//  Copyright 2014 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - omaralzabir@gmail.com (original author)
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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Utilities.Controls;

namespace PlantUmlStudio
{
    /// <summary>
    /// Handles unhandled application exceptions.
    /// </summary>
    internal class UnhandledErrorHandler
    {
        public UnhandledErrorHandler(Application application, Dispatcher dispatcher)
        {
            _application = application;
            _dispatcher = dispatcher;
            application.DispatcherUnhandledException += Application_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            _dispatcher.Invoke(() => ShowMessageBox(e.Exception));
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            _dispatcher.BeginInvoke(new Action(() => ShowMessageBox(e.Exception)));
        }

        private void ShowMessageBox(Exception exception)
        {
            var message = new StringBuilder();
            FormatMessage(message, exception);
            var messageBox = new ScrollableMessageBox
            {
                Buttons = MessageBoxButton.YesNo,
                Title = "Application Error",
                Caption =
@"An application error occurred. If this error occurs again there may be a more serious malfunction in the application, and it should be closed.

Do you want to exit the application?
(Warning: If you click Yes the application will close, if you click No the application will continue)",
                Message = message.ToString()
            };

            messageBox.ShowDialog();
            if (messageBox.DialogResult.HasValue && messageBox.DialogResult.Value)
                _application.Shutdown();
        }

        private static void FormatMessage(StringBuilder message, Exception exception)
        {
            message.AppendFormat("{1}:{0}{2}{0}{3}",
                                 Environment.NewLine,
                                 exception.GetType().Name,
                                 exception.Message,
                                 exception.StackTrace);

            if (exception.InnerException != null)
            {
                message.AppendLine();
                FormatMessage(message, exception.InnerException);
            }
        }

        private readonly Application _application;
        private readonly Dispatcher _dispatcher;
    }
}