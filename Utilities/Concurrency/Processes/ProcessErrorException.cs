using System;
using System.Runtime.Serialization;

namespace Utilities.Concurrency.Processes
{
	/// <summary>
	/// Exception thrown when a process's error stream contains data.
	/// </summary>
	[Serializable]
	public class ProcessErrorException : Exception
	{
		/// <see cref="Exception()"/>
		public ProcessErrorException() { }

		/// <see cref="Exception(string)"/>
		public ProcessErrorException(string message) 
			: base(message) { }

		/// <see cref="Exception(string, Exception)"/>
		public ProcessErrorException(string message, Exception inner) 
			: base(message, inner) { }

		/// <see cref="Exception(SerializationInfo, StreamingContext)"/>
		protected ProcessErrorException(SerializationInfo info, StreamingContext context) 
			: base(info, context) { }
	}
}