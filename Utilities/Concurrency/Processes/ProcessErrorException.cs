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
		/// <see cref="ProcessErrorException()"/>
		public ProcessErrorException() { }

		/// <see cref="ProcessErrorException(string)"/>
		public ProcessErrorException(string message) 
			: base(message) { }

		/// <see cref="ProcessErrorException(string, Exception)"/>
		public ProcessErrorException(string message, Exception inner) 
			: base(message, inner) { }

		/// <see cref="ProcessErrorException(SerializationInfo, StreamingContext)"/>
		protected ProcessErrorException(SerializationInfo info, StreamingContext context) 
			: base(info, context) { }
	}
}