//  PlantUML Studio
//  Copyright 2013 Matthew Hamilton - matthamilton@live.com
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

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlantUmlStudio.Model.Snippets
{
	/// <summary>
	/// Responsible for loading and managing diagram code snippets.
	/// </summary>
	public class SnippetProvider
	{
		public SnippetProvider(ISnippetParser snippetParser)
		{
			_snippetParser = snippetParser;
		}

		/// <summary>
		/// The available snippets.
		/// </summary>
		public IEnumerable<CodeSnippet> Snippets => _snippets;

	    /// <summary>
		/// Loads snippets from the snippet location.
		/// If the snippet location is invalid, nothing is done.
		/// </summary>
		public void Load()
		{
			if (!SnippetLocation.Exists)
			{
				_snippets = Enumerable.Empty<CodeSnippet>();
				return;
			}

			_snippets = SnippetLocation.GetFiles("*.snip", SearchOption.AllDirectories)
				.Select(snippetFile => snippetFile.OpenRead())
				.Select(_snippetParser.Parse);
		}

		/// <summary>
		/// The directory to use for snippets.
		/// </summary>
		public DirectoryInfo SnippetLocation { get; set; }

		private IEnumerable<CodeSnippet> _snippets;
		private readonly ISnippetParser _snippetParser;
	}
}