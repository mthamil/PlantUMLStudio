//  PlantUML Editor 2
//  Copyright 2012 Matthew Hamilton - matthamilton@live.com
//  Copyright 2010 Omar Al Zabir - http://omaralzabir.com/ (original author)
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
namespace PlantUmlEditor.ViewModel.Notifications
{
	/// <summary>
	/// Interface for a view model representing progress information for some task.
	/// </summary>
	public interface IProgressNotification
	{
		/// <summary>
		/// Whether there are currently actual discrete chunks of progress.
		/// </summary>
		bool HasDiscreteProgress { get; set; }

		/// <summary>
		/// Whether there is something currently reporting progress.
		/// </summary>
		bool InProgress { get; }

		/// <summary>
		/// The percentage of work completed.
		/// If null, the percentage is not applicable.
		/// </summary>
		int? PercentComplete { get; set; }

		/// <summary>
		/// The current progress message.
		/// </summary>
		string Message { get; }
	}
}