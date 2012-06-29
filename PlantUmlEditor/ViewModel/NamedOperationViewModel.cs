using System.Windows.Input;

namespace PlantUmlEditor.ViewModel
{
	/// <summary>
	/// Represents a named operation that can be performed.
	/// </summary>
	public class NamedOperationViewModel
	{
		/// <summary>
		/// Initializes a new operation.
		/// </summary>
		/// <param name="name">The name of the operation</param>
		/// <param name="operation">The operation to perform</param>
		public NamedOperationViewModel(string name, ICommand operation)
		{
			Name = name;
			Operation = operation;
		}

		/// <summary>
		/// The name of the operation.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// The operation to perform.
		/// </summary>
		public ICommand Operation { get; private set; }
	}
}