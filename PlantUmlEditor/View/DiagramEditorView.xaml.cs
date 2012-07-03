using System;
using System.Windows.Controls;

namespace PlantUmlEditor.View
{
    public partial class DiagramEditorView : UserControl
    {
        public DiagramEditorView()
        {
            InitializeComponent();

			ContentEditor.IsEnabledChanged += ContentEditor_IsEnabledChanged;
        }

		void ContentEditor_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			// Maintain focus on the text editor.
			if ((bool)e.NewValue)
				Dispatcher.BeginInvoke(new Action(() => ContentEditor.Focus()));
		}
    }
}
