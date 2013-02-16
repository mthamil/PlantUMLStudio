using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Markup;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace Utilities.Controls.Behaviors.AvalonEdit
{
	/// <summary>
	/// Behavior that enables adding <see cref="IVisualLineTransformer"/>s to a <see cref="TextEditor"/>'s
	/// <see cref="TextView"/> through XAML markup.
	/// </summary>
	[ContentProperty("LineTransformers")]
	public class AddLineTransformers : Behavior<TextEditor>
	{
		/// <summary>
		/// When attachment occurs, the transformers are added.
		/// </summary>
		protected override void OnAttached()
		{
			foreach (var transformer in LineTransformers)
				AssociatedObject.TextArea.TextView.LineTransformers.Add(transformer);
		}

		/// <summary>
		/// The line transformers collection.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Collection<IVisualLineTransformer> LineTransformers
		{
			get { return (Collection<IVisualLineTransformer>)GetValue(LineTransformersProperty); }
			set { SetValue(LineTransformersProperty, value); }
		}

		/// <summary>
		/// The LineTransformers dependency property.
		/// </summary>
		public static readonly DependencyProperty LineTransformersProperty =
			DependencyProperty.Register(
				"LineTransformers", 
				typeof(Collection<IVisualLineTransformer>), 
				typeof(AddLineTransformers), 
				new PropertyMetadata(new Collection<IVisualLineTransformer>()));
	}
}