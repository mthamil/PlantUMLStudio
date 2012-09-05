using System.Threading.Tasks;

namespace PlantUmlEditor.Core
{
	/// <summary>
	/// The API for PlantUML.
	/// </summary>
	public interface IPlantUml : IDiagramCompiler
	{
		/// <summary>
		/// Gets the current PlantUML version.
		/// </summary>
		/// <returns>The version string</returns>
		Task<string> GetCurrentVersion();
	}
}