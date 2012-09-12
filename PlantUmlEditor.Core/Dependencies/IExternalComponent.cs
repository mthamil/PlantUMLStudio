using System.Threading.Tasks;
using PlantUmlEditor.Core.Dependencies.Update;

namespace PlantUmlEditor.Core.Dependencies
{
	/// <summary>
	/// Represents a software component dependency.
	/// </summary>
	public interface IExternalComponent : IComponentUpdateChecker
	{
		/// <summary>
		/// A dependency's name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Asynchronously retrieves a dependency's current version.
		/// </summary>
		/// <returns>A string representing the version</returns>
		Task<string> GetCurrentVersionAsync();
	}
}