using PlantUmlStudio.Core;
using SharpEssentials.Controls.Mvvm;

namespace PlantUmlStudio.ViewModel
{
    public class DiagramErrorViewModel : ViewModelBase
    {
        public DiagramErrorViewModel(DiagramError error)
        {
            Line = error.LineNumber;
            Message = error.Message;
        }

        public int Line { get; }

        public string Message { get; }
    }
}