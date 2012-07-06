using System.Collections.Generic;
using System.IO;

namespace Utilities.InputOutput
{
	/// <summary>
	/// Contains extensions for the Stream class.
	/// </summary>
	public static class StreamExtensions
	{
		/// <summary>
		/// Returns a stream as a sequence of lines.
		/// </summary>
		/// <param name="stream">The stream to iterate over</param>
		/// <returns>A sequence of lines from the stream</returns>
		public static IEnumerable<string> Lines(this Stream stream)
		{
			using (var reader = new StreamReader(stream))
			{
				while (!reader.EndOfStream)
					yield return reader.ReadLine();
			}
		}
	}
}