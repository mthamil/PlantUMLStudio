using System;
using System.IO;
using System.Runtime.Serialization;

namespace PlantUmlEditor.Core.InputOutput
{
	/// <summary>
	/// Exception thrown when a file does not contain a valid diagram.
	/// </summary>
	[Serializable]
	public class InvalidDiagramFileException : Exception
	{
		/// <summary>
		/// Initializes a new exception.
		/// </summary>
		public InvalidDiagramFileException(FileInfo file)
			: base(String.Format(MESSAGE_FORMAT, file.Name))
		{
		}

		/// <summary>
		/// Initializes a new exception.
		/// </summary>
		public InvalidDiagramFileException(FileInfo file, Exception inner)
			: base(String.Format(MESSAGE_FORMAT, file.Name), inner)
		{
		}

		/// <summary>
		/// Initializes a new exception.
		/// </summary>
		protected InvalidDiagramFileException()
		{
		}

		/// <summary>
		/// Initializes a new exception.
		/// </summary>
		protected InvalidDiagramFileException(SerializationInfo info, StreamingContext context) 
			: base(info, context)
		{
		}

		private const string MESSAGE_FORMAT = "'{0}' is not a valid diagram file.";
	}
}