using System.Collections.ObjectModel;
using System.IO;
using PlantUmlEditor.Core;

namespace PlantUmlEditor.DesignTimeData
{
    public class DiagramFiles : ObservableCollection<Diagram>
    {
        public DiagramFiles()
        {
            this.Add(new Diagram()
            {
                Content =
@"@startuml class.png
package ""Sample Package""
  namespace N1 {
    class C1 {
      Method1()
    }
    enum E1 {
      Item1
      Item2
      Item3
    }
  }
end package
@enduml",
                File = new FileInfo("class.txt"),
                //ImageFilePath = "http://plantuml.sourceforge.net/img/sequence_img009.png"
            });

            this.Add(new Diagram()
            {
                Content = 
@"@startuml sequence.png
participant User
User -> A: DoWork
activate A
  A -> A: Internal call
  activate A
    A -> B: << createRequest >>
    activate B
      B --> A: RequestCreated
    deactivate B
  deactivate A
  A -> User: Done
deactivate A
@enduml",
                File = new FileInfo("sequence.txt"),
               // ImageFilePath = "http://plantuml.sourceforge.net/img/activity_img06.png"
            });
        }
    }
}
