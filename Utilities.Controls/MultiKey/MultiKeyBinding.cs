using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Utilities.Controls.MultiKey
{
	/// <summary>
	/// Provides a multi-key input binding.
	/// </summary>
	public class MultiKeyBinding : InputBinding
	{
		/// <see cref="InputBinding.Gesture"/>
		[TypeConverter(typeof(MultiKeyGestureConverter))]
		public override InputGesture Gesture
		{
			get { return base.Gesture as MultiKeyGesture; }
			set
			{
				if (!(value is MultiKeyGesture))
					throw new ArgumentException(@"Gesture does not support multiple keys.", "value");

				base.Gesture = value;
			}
		}
	}
}