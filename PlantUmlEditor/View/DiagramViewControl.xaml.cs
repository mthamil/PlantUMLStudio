using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PlantUmlEditor.Model;
using PlantUmlEditor.ViewModel;
using Utilities;

namespace PlantUmlEditor.View
{
    /// <summary>
    /// Takes a DiagramFile object into DataContext and renders the text editor and 
    /// shows the generated diagram
    /// </summary>
    public partial class DiagramViewControl : UserControl
    {
        private Weak<MenuItem> _lastMenuItemClicked = default(Weak<MenuItem>);

        public DiagramViewControl()
        {
            InitializeComponent();
        }

        public event Action<DiagramFile> OnBeforeSave;
        public event Action<DiagramFile> OnAfterSave;

        private DiagramFile CurrentDiagram
        {
            get
            {
				return ((DiagramEditorViewModel)DataContext).DiagramViewModel.Diagram;
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

			SnippetsMenu.DataContext = DataContext;

            if (_lastMenuItemClicked != default(Weak<MenuItem>))
            {
                _lastMenuItemClicked.Dispose();
                _lastMenuItemClicked = null;
            }
        }

        private void AddStuff_Click(object sender, RoutedEventArgs e)
        {
            // Trick: Open the context menu automatically whenever user
            // clicks the "Add" button
			SnippetsMenu.IsOpen = true;

            // If user last added a particular diagram items, say Use case
            // item, then auto open the usecase menu so that user does not
            // have to click on use case again. Saves time when you are adding
            // a lot of items for the same diagram
            if (_lastMenuItemClicked != default(Weak<MenuItem>))
            {
                MenuItem parentMenu = (_lastMenuItemClicked.Target.Parent as MenuItem);
                parentMenu.IsSubmenuOpen = true;
            }
        }

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {            
            Clipboard.SetImage(DiagramImage.Source as BitmapSource);
        }

        private void OpenInExplorer_Click(object sender, RoutedEventArgs e)
        {
            Process
                .Start("explorer.exe","/select," + CurrentDiagram.ImageFilePath)
                .Dispose();
        }

        private void CopyPath_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentDiagram.ImageFilePath);
        }
    }
}
