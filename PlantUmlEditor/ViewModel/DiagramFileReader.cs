using System;
using System.IO;
using System.Text.RegularExpressions;
using PlantUmlEditor.Model;

namespace PlantUmlEditor.ViewModel
{
	public class DiagramFileReader : IDiagramReader
	{
		#region Implementation of IDiagramReader

		/// <see cref="IDiagramReader.Read"/>
		public DiagramFile Read(FileInfo file)
		{
			string content;
			using (var reader = new StreamReader(file.OpenRead()))
				content = reader.ReadToEnd();

			if (!String.IsNullOrWhiteSpace(content))
			{
				//string firstLine = content.Substring(0,500);
				Match match = Regex.Match(content, @"@startuml\s*(?:"")*([^\r\n""]*)",
				                          RegexOptions.IgnoreCase
				                          | RegexOptions.Multiline
				                          | RegexOptions.IgnorePatternWhitespace
				                          | RegexOptions.Compiled
					);

				if (match.Success && match.Groups.Count > 1)
				{
					string imageFileName = match.Groups[1].Value;
					var imageFilePath = Path.IsPathRooted(imageFileName)
						? Path.GetFullPath(imageFileName)
						: Path.GetFullPath(Path.Combine(file.DirectoryName, imageFileName));

					return new DiagramFile
					{
						Content = content,
						DiagramFilePath = file.FullName,
						ImageFilePath = imageFilePath
					};
				}
			}

			return null;
		}

		#endregion
	}
}