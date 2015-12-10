using System.Security.Principal;
using Moq;
using PlantUmlStudio.Core.Security;
using Xunit;

namespace Tests.Unit.PlantUmlStudio.Core.Security
{
	public class WindowsSecurityServiceTests
	{
		public WindowsSecurityServiceTests()
		{
			securityService = new WindowsSecurityService(() => principal.Object);
		}

		[Theory]
		[InlineData(true,  @"BUILTIN\Administrators")]
		[InlineData(false, @"BUILTIN\Users")]
		[InlineData(false, @"BUILTIN\Guests")]
		[InlineData(false, @"BUILTIN\Account Operators")]
		[InlineData(false, @"BUILTIN\Server Operators")]
		[InlineData(false, @"BUILTIN\Replicator")]
		public void Test_HasAdminPriviledges(bool expected, string role)
		{
			// Arrange.
			principal.Setup(p => p.IsInRole(role))
			         .Returns(true);

			// Act.
			bool actual = securityService.HasAdminPriviledges();

			// Assert.
			Assert.Equal(expected, actual);
		}

		private readonly WindowsSecurityService securityService;

		private readonly Mock<IPrincipal> principal = new Mock<IPrincipal>();
	}
}