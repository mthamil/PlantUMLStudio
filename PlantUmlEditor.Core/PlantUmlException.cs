using System;
using System.Runtime.Serialization;

namespace PlantUmlEditor.Core
{
	/// <summary>
	/// Represents PlantUML errors.
	/// </summary>
	[Serializable]
	public class PlantUmlException : Exception
	{
		/// <see cref="Exception()"/>
		public PlantUmlException() { }

		/// <see cref="Exception(string)"/>
		public PlantUmlException(string message)
			: base(message) { }

		/// <see cref="Exception(string, Exception)"/>
		public PlantUmlException(string message, Exception inner) 
			: base(message, inner) { }

		/// <see cref="Exception(SerializationInfo, StreamingContext)"/>
		protected PlantUmlException(SerializationInfo info, StreamingContext context) 
			: base(info, context) { }
	}
}