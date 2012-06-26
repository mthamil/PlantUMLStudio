using System.Windows;
using System.Windows.Controls;

namespace PlantUmlEditor.Controls.Behaviors
{
	/// <summary>
	/// Attached property that provides a string that can be used in conjunction with the WatermarkTextBox style.
	/// </summary>
	public static class TextBoxWatermark
	{
		/// <summary>
		/// Gets the watermark string.
		/// </summary>
		[AttachedPropertyBrowsableForType(typeof(TextBox))]
		public static string GetWatermark(TextBox textBox)
		{
			return (string)textBox.GetValue(WatermarkProperty);
		}

		/// <summary>
		/// Sets the watermark string.
		/// </summary>
		public static void SetWatermark(TextBox textBox, string value)
		{
			textBox.SetValue(WatermarkProperty, value);
		}

		/// <summary>
		/// The watermark property.
		/// </summary>
		public static readonly DependencyProperty WatermarkProperty =
			DependencyProperty.RegisterAttached(
			"Watermark",
			typeof(string),
			typeof(TextBoxWatermark),
			new FrameworkPropertyMetadata(string.Empty));
	}
}