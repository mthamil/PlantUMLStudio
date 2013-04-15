//  PlantUML Editor
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

using System;
using System.Security.Principal;

namespace PlantUmlEditor.Core.Security
{
	/// <summary>
	/// Provides functionality related to Windows security and permissions.
	/// </summary>
	public class WindowsSecurityService : ISecurityService
	{
		/// <summary>
		/// Initializes a <see cref="WindowsSecurityService"/>.
		/// </summary>
		/// <param name="principalFactory">Retrieves the current <see cref="IPrincipal"/></param>
		public WindowsSecurityService(Func<IPrincipal> principalFactory)
		{
			_principalFactory = principalFactory;
		}

		/// <see cref="ISecurityService.HasAdminPriviledges"/>
		public bool HasAdminPriviledges()
		{
			var principal = _principalFactory();
			return principal.IsInRole(AdministratorRole);
		}

		private readonly Func<IPrincipal> _principalFactory;

		private const string AdministratorRole = @"BUILTIN\Administrators";
	}
}