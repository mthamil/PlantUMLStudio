using System.Collections.Generic;
using PlantUmlEditor.ViewModel;

namespace PlantUmlEditor.Model
{
	/// <summary>
	/// Creates the defaultr code snippets.
	/// TODO: use a more flexible format than storing in code.
	/// </summary>
	public class DefaultSnippets
	{
		public DefaultSnippets()
		{
			var snippets = new List<Snippet>
			{
				new Snippet("Actor", "Use Case",
@"  actor :Last actor: as Men << Human >>"),
				new Snippet("Usecase", "Use Case",
@"  (Use the application) as (Use) << Main >>"),
				new Snippet("Actor to Usecase", "Use Case",
@"  Men -> (Start) : Some Label"),
				new Snippet("Actor to Usecase (long arrow)", "Use Case",
@"  Men --> (Use the application) : A small label"),
				new Snippet("Actor Extends Actor", "Use Case",
@"  User <|-- Admin"),
				new Snippet("Usecase extends Usecase", "Use Case",
@"  (Start) <|-- (Use)"),
				new Snippet("Note beside usecase", "Use Case",
@"  note right of (Use)\r
    A note can also\r
    be on several lines\r
  end note"),

				new Snippet("Actor", "Sequence",
@"actor Bob"),
				new Snippet("Participant", "Sequence",
@"participant ""Multiline\nTitle"" as P1 <<stereotype>>"),
				new Snippet("Synchronous Request", "Sequence",
@"Alice -> Bob: Authentication Request"),
				new Snippet("Asynchronous Request", "Sequence",
@"Alice ->> Bob: Authentication Request"),
				new Snippet("Return Message", "Sequence",
@"Bob --> Alice: Authentication Response"),
				new Snippet("Self Signal", "Sequence",
@"Alice -> Alice: This is a signal to self.\nIt also demonstrates\nmultiline \ntext"),
				new Snippet("Activation Block", "Sequence",
@"User -> A: DoWork\r
activate A\r
A -> B: << createRequest >>\r
A --> User: Return response\r                  
deactivate A"),
				new Snippet("Destroy", "Sequence",
@"activate C\r
\r
destroy C"),
				new Snippet("Alternate Block", "Sequence",
@"Alice -> Bob: Authentication Request\r
alt successful case\r
Bob -> Alice: Authentication Accepted\r
else some kind of failure\r
Bob -> Alice: Authentication Failure\r
end"),
				new Snippet("Optional", "Sequence",
@"opt\r
loop 1000 times\r
Alice -> Bob: DNS Attack\r
end\r
end"),
				new Snippet("Loop", "Sequence",
@"loop 1000 times\r
Alice -> Bob: DNS Attack\r
end"),
				new Snippet("Note", "Sequence",
@"note left\r
a note\r
can also be defined\r
on several lines\r
end note"),
				new Snippet("Partition", "Sequence",
@"== Initialisation =="),
				new Snippet("Autonumber", "Sequence",
@"autonumber 10 ""<b>[000]"""),
				new Snippet("New page", "Sequence",
@"newpage"),
				new Snippet("Diagram Title", "Sequence",
@"title
<u>Simple</u> communication example
on <i>several</i> lines and using <font color=red>html</font>
This is hosted by <img src=sourceforge.jpg>
end title"),

				new Snippet("Start", "Activity",
@"(*) --> ""First Activity"""),
				new Snippet("End", "Activity",
@"""First Activity"" --> (*)"),
				new Snippet("Next activity", "Activity",
@"--> ""Second Activity"" : You can put also labels"),
				new Snippet("Side Activity", "Activity",
@"""Third Activity"" <- ""Second Activity"""),
				new Snippet("Long jump", "Activity",
@"""First Activity"" ---> Last"),
				new Snippet("Condition", "Activity",
@"--> <> B1\r
--> [true] ""Some Activity""\r
--> ""Another activity""\r
-> [false] ""Something else""\r"),
				new Snippet("Synchronization Bar", "Activity",
@"--> ===B1==="),
				new Snippet("Sync Bar to Activity", "Activity",
@"===B1=== --> ""Parallel Activity 1"""),
				new Snippet("Parallel Activities", "Activity",
@"(*) --> ===B1===\r
--> ""Parallel Activity 1""\r
--> ===B2===\r
\r
===B1=== --> ""Parallel Activity 2""\r
--> ===B2===\r
--> (*)"),
				new Snippet("HTML Activity", "Activity",
@"(*) --> ""this <font size=20>activity</font>\r
is <b>very</b> <font color=red>long</font>\r
and defined on several lines\r
that contains many <i>text</i>"" as A1\r
--> ""Another activity\n on several lines""\r
A1 --> ""Short activity"""),
				new Snippet("Note to activity", "Activity",
@"(*) --> ""Some Activity""\r
note right: This activity has to be defined\r"),
				new Snippet("Partition", "Activity",
@"partition Conductor
(*) --> ""Climbs on Platform""
--> === S1 ===
--> Bows
end partition
partition Audience LightSkyBlue
=== S1 === --> Applauds
end partition
partition Conductor
Bows --> === S2 ===
--> WavesArmes
Applauds --> === S2 ===
end partition"),

				new Snippet("Component", "Component",
@"component [Some\ncomponent] as SomeComp"),
				new Snippet("Interface", "Component",
@"interface ""Some\ninterface"" as SomeInterface"),
				new Snippet("Component offer interface", "Component",
@"SomeInterface - [Some Component]"),
				new Snippet("Component use interface", "Component",
@"[First Component] ..> HTTP : use"),
				new Snippet("Note", "Component",
@"note right of [First Component]\r
A note can also\r
be on several lines\r
end note\r"),

				new Snippet("Init to State", "State",
@"CDATA[[*] --> State1"),
				new Snippet("State to End", "State",
@"State1 --> [*]"),
				new Snippet("A state", "State",
@"state ""Some new state"" as NewState"),
				new Snippet("Content inside state", "State",
@"State1 : this is a string\r
State1 : this is another string"),
				new Snippet("Composite State", "State",
@"state NotShooting {\r
[*] --> Idle\r
Idle --> Configuring : EvConfig\r
Configuring --> Idle : EvConfig\r
}"),
				new Snippet("Active State", "State",
@"--\r
[*] -> CapsLockOff\r
CapsLockOff --> CapsLockOn : EvCapsLockPressed\r
CapsLockOn --> CapsLockOff : EvCapsLockPressed\r"),
				new Snippet("Note", "State",
@"note right of InactiveState\r
A note can also\r
be defined on\r
several lines\r
end note"),
				new Snippet("Sample Class", "Class",
@"class Dummy {
-field1
#field2
~method1()
+method2()
}"),
				new Snippet("Generalization", "Class",
@"Human <|-- Student"),
				new Snippet("Composition", "Class",
@"Class ""1"" *-- ""many"" Students : contains"),
				new Snippet("Aggregation", "Class",
@"School o-- ""1...N"" Class : aggregates"),
				new Snippet("Association", "Class",
@"Student ""1"" -- ""1...N"" Course : associates"),
				new Snippet("Class Method", "Class",
@"Course : take()"),
				new Snippet("Class Property", "Class",
@"Course : Subject[] subjects"),
				new Snippet("Abstract Class", "Class",
@"abstract Class AbstractClass"),
				new Snippet("Interface", "Class",
@"Interface SomeInterface"),
				new Snippet("Enum", "Class",
@"enum TimeUnit\r
TimeUnit : DAYS\r
TimeUnit : HOURS\r
TimeUnit : MINUTES\r"),
				new Snippet("Package", "Class",
@"package ""Classic Collections"" #DDDDDD\r
Object <|-- ArrayList\r
end package\r"),
					new Snippet("Package Link", "Class",
@"package foo1.foo2\r
end package\r
package foo1.foo2.foo3\r
class Object\r
end package\r
\r
foo1.foo2 +-- foo1.foo2.foo3\r"),
				new Snippet("Association Class", "Class",
@"Student ""0..*"" - ""1..*"" Course
(Student, Course) .. Enrollment")

			};

			var categories = new SortedDictionary<string, SnippetCategoryViewModel>();
			foreach (var snippet in snippets)
			{
				SnippetCategoryViewModel category;
				if (!categories.TryGetValue(snippet.Category, out category))
				{
					category = new SnippetCategoryViewModel(snippet.Category);
					categories[category.Name] = category;
				}

				category.Snippets.Add(new SnippetViewModel(snippet));
			}

			SnippetCategories = categories.Values;
		}

		public IEnumerable<SnippetCategoryViewModel> SnippetCategories { get; private set; }
	}
}