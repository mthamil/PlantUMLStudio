using System.Collections.Generic;

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
			Snippets = new List<SnippetCategory>
				{
					new SnippetCategory("Use Case",
						new List<SnippetCategory>
						{
							new Snippet("Actor", 
@"  actor :Last actor: as Men << Human >>"),
							new Snippet("Usecase", 
@"  (Use the application) as (Use) << Main >>"),
							new Snippet("Actor to Usecase", 
@"  Men -> (Start) : Some Label"),
							new Snippet("Actor to Usecase (long arrow)", 
@"  Men --> (Use the application) : A small label"),
							new Snippet("Actor Extends Actor", 
@"  User <|-- Admin"),
							new Snippet("Usecase extends Usecase", 
@"  (Start) <|-- (Use)"),
							new Snippet("Note beside usecase", 
@"  note right of (Use)\r
    A note can also\r
    be on several lines\r
  end note")
						}),

					new SnippetCategory("Sequence",
						new List<SnippetCategory>
						{
							new Snippet("Actor", 
@"actor Bob"),
							new Snippet("Participant", 
@"participant ""Multiline\nTitle"" as P1 <<stereotype>>"),
							new Snippet("Synchronous Request", 
@"Alice -> Bob: Authentication Request"),
							new Snippet("Asynchronous Request", 
@"Alice ->> Bob: Authentication Request"),
							new Snippet("Return Message", 
@"Bob --> Alice: Authentication Response"),
							new Snippet("Self Signal", 
@"Alice -> Alice: This is a signal to self.\nIt also demonstrates\nmultiline \ntext"),
							new Snippet("Activation Block", 
@"User -> A: DoWork\r
activate A\r
A -> B: << createRequest >>\r
A --> User: Return response\r                  
deactivate A"),
							new Snippet("Destroy", 
@"activate C\r
\r
destroy C"),
							new Snippet("Alternate Block", 
@"Alice -> Bob: Authentication Request\r
alt successful case\r
Bob -> Alice: Authentication Accepted\r
else some kind of failure\r
Bob -> Alice: Authentication Failure\r
end"),
							new Snippet("Optional", 
@"opt\r
loop 1000 times\r
Alice -> Bob: DNS Attack\r
end\r
end"),
							new Snippet("Loop", 
@"loop 1000 times\r
Alice -> Bob: DNS Attack\r
end"),
							new Snippet("Note", 
@"note left\r
a note\r
can also be defined\r
on several lines\r
end note"),
							new Snippet("Partition", 
@"== Initialisation =="),
							new Snippet("Autonumber", 
@"autonumber 10 ""<b>[000]"""),
							new Snippet("New page", 
@"newpage"),
							new Snippet("Diagram Title", 
@"title
<u>Simple</u> communication example
on <i>several</i> lines and using <font color=red>html</font>
This is hosted by <img src=sourceforge.jpg>
end title")
						}),

				new SnippetCategory("Activity",
					new List<SnippetCategory>
					{
						new Snippet("Start", 
@"(*) --> ""First Activity"""),
						new Snippet("End", 
@"""First Activity"" --> (*)"),
						new Snippet("Next activity", 
@"--> ""Second Activity"" : You can put also labels"),
						new Snippet("Side Activity", 
@"""Third Activity"" <- ""Second Activity"""),
						new Snippet("Long jump", 
@"""First Activity"" ---> Last"),
						new Snippet("Condition", 
@"--> <> B1\r
--> [true] ""Some Activity""\r
--> ""Another activity""\r
-> [false] ""Something else""\r"),
						new Snippet("Synchronization Bar", 
@"--> ===B1==="),
						new Snippet("Sync Bar to Activity", 
@"===B1=== --> ""Parallel Activity 1"""),
						new Snippet("Parallel Activities", 
@"(*) --> ===B1===\r
--> ""Parallel Activity 1""\r
--> ===B2===\r
\r
===B1=== --> ""Parallel Activity 2""\r
--> ===B2===\r
--> (*)"),
						new Snippet("HTML Activity", 
@"(*) --> ""this <font size=20>activity</font>\r
is <b>very</b> <font color=red>long</font>\r
and defined on several lines\r
that contains many <i>text</i>"" as A1\r
--> ""Another activity\n on several lines""\r
A1 --> ""Short activity"""),
						new Snippet("Note to activity", 
@"(*) --> ""Some Activity""\r
note right: This activity has to be defined\r"),
						new Snippet("Partition", 
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
end partition")
					}),

			new SnippetCategory("Component",
				new List<SnippetCategory>
				{
					new Snippet("Component", 
@"component [Some\ncomponent] as SomeComp"),
					new Snippet("Interface", 
@"interface ""Some\ninterface"" as SomeInterface"),
					new Snippet("Component offer interface", 
@"SomeInterface - [Some Component]"),
					new Snippet("Component use interface", 
@"[First Component] ..> HTTP : use"),
					new Snippet("Note", 
@"note right of [First Component]\r
A note can also\r
be on several lines\r
end note\r")
				}),

			new SnippetCategory("State",
				new List<SnippetCategory>
				{
					new Snippet("Init to State", 
@"CDATA[[*] --> State1"),
					new Snippet("State to End", 
@"State1 --> [*]"),
					new Snippet("A state", 
@"state ""Some new state"" as NewState"),
					new Snippet("Content inside state", 
@"State1 : this is a string\r
State1 : this is another string"),
					new Snippet("Composite State", 
@"state NotShooting {\r
[*] --> Idle\r
Idle --> Configuring : EvConfig\r
Configuring --> Idle : EvConfig\r
}"),
					new Snippet("Active State", 
@"--\r
[*] -> CapsLockOff\r
CapsLockOff --> CapsLockOn : EvCapsLockPressed\r
CapsLockOn --> CapsLockOff : EvCapsLockPressed\r"),
					new Snippet("Note", 
@"note right of InactiveState\r
A note can also\r
be defined on\r
several lines\r
end note"),
				}),

			new SnippetCategory("Class",
				new List<SnippetCategory>
				{
					new Snippet("Sample Class", 
@"class Dummy {
-field1
#field2
~method1()
+method2()
}"),
					new Snippet("Generalization", 
@"Human <|-- Student"),
					new Snippet("Composition", 
@"Class ""1"" *-- ""many"" Students : contains"),
					new Snippet("Aggregation", 
@"School o-- ""1...N"" Class : aggregates"),
					new Snippet("Association", 
@"Student ""1"" -- ""1...N"" Course : associates"),
					new Snippet("Class Method", 
@"Course : take()"),
					new Snippet("Class Property", 
@"Course : Subject[] subjects"),
					new Snippet("Abstract Class", 
@"abstract Class AbstractClass"),
					new Snippet("Interface", 
@"Interface SomeInterface"),
					new Snippet("Enum", 
@"enum TimeUnit\r
TimeUnit : DAYS\r
TimeUnit : HOURS\r
TimeUnit : MINUTES\r"),
					new Snippet("Package", 
@"package ""Classic Collections"" #DDDDDD\r
Object <|-- ArrayList\r
end package\r"),
					new Snippet("Package Link", 
@"package foo1.foo2\r
end package\r
package foo1.foo2.foo3\r
class Object\r
end package\r
\r
foo1.foo2 +-- foo1.foo2.foo3\r"),
					new Snippet("Association Class", 
@"Student ""0..*"" - ""1..*"" Course
(Student, Course) .. Enrollment"),
				})

			};
		}

		public IEnumerable<SnippetCategory> Snippets { get; private set; }
	}
}