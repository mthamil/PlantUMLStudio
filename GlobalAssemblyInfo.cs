using System.Reflection;

[assembly: AssemblyProduct("PlantUML Editor")]
[assembly: AssemblyCopyright("Copyright © 2012")]

[assembly: AssemblyVersion("2.0.0.*")]
[assembly: AssemblyFileVersion("2.0.0.*")]
[assembly: AssemblyInformationalVersion("2.0")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif