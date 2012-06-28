namespace Utilities.Concurrency
{
	/// <summary>
	/// Defines a provider for progress updates.
	/// </summary>
	/// <typeparam name="T">The type of progress update value</typeparam>
	/// <remarks>This class is a copy of the .NET v4.5 IProgress interface and becomes obsolete on that framework</remarks>
	public interface IProgress<in T>
	{
		/// <summary>
		/// Reports a progress update.
		/// </summary>
		/// <param name="value">The value of the updated progress</param>
		void Report(T value);
	}
}