//  PlantUML Studio
//  Copyright 2014 Matthew Hamilton - matthamilton@live.com
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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Autofac;
using PlantUmlStudio.Configuration;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace PlantUmlStudio
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
            _errorHandler = new UnhandledErrorHandler(Current, Dispatcher);

			var containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterAssemblyModules(GetType().Assembly);
			_container = containerBuilder.Build();
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
		    _container?.Dispose();
		}

        /// <summary>
		/// The application's IoC container.
		/// </summary>
		public static IContainer Container => _container;
        private static IContainer _container;

		private static bool CheckGraphViz()
		{
		    var graphVizPath = new Func<string>[]
		                       {
		                           () => Settings.Default.GraphVizLocation,
		                           () => Environment.GetEnvironmentVariable("GRAPHVIZ_DOT"),
		                           () => $@"{Environment.GetEnvironmentVariable("PATH")?
                                                        .Split(';')
                                                        .FirstOrDefault(p => p.ToLower().Contains("graphviz"))}\dot.exe",
		                           CheckInstallDir
		                       }
                               .Select(c => c())
		                       .FirstOrDefault(p => !String.IsNullOrEmpty(p) && File.Exists(p));

		    if (graphVizPath != null)
                return true;

		    using (var dialog = new OpenFileDialog
		                        {
		                            FileName = "dot.exe",
		                            DefaultExt = ".exe",
		                            Filter = "dot.exe|dot.exe"
		                        })
		    {
		        if (dialog.ShowDialog() == DialogResult.OK)
		        {
		            Settings.Default.GraphVizLocation = dialog.FileName;
		            Settings.Default.Save();
		            return true;
		        }
		    }

		    return false;
		}

        private static string CheckInstallDir()
        {
            var installDir = Directory.GetDirectories(@"C:\Program Files (x86)\", "Graphviz*").FirstOrDefault();
            if (installDir != null)
            {
                var executable = Directory.GetFiles(installDir + @"\bin\", "dot.exe").FirstOrDefault();
                if (executable != null && File.Exists(executable))
                {
                    Settings.Default.GraphVizLocation = executable;
                    Settings.Default.Save();
                    return executable;
                }
            }

            return null;
        }

        private static UnhandledErrorHandler _errorHandler;
	}
}
