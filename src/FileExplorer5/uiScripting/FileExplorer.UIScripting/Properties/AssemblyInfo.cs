using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("FileExplorer.Scripting.UIEventHub")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("FileExplorer.Scripting.UIEventHub")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("189372b5-b3cd-41c0-ab97-7cf3dcf81d7b")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("3.0.21.0")]
[assembly: AssemblyVersion("3.0.21.0")]
[assembly: AssemblyFileVersion("3.0.21.0")]


[assembly: XmlnsDefinition("http://www.quickzip.org/UIEventHub", "FileExplorer.WPF.BaseControls")]
[assembly: XmlnsDefinition("http://www.quickzip.org/UIEventHub", "FileExplorer.Defines")]
[assembly: XmlnsDefinition("http://www.quickzip.org/UIEventHub", "FileExplorer.UIEventHub.Converters")]

[assembly: XmlnsDefinition("http://www.quickzip.org/BaseControls", "FileExplorer.WPF.BaseControls")]
[assembly: XmlnsDefinition("http://www.quickzip.org/BaseControls", "FileExplorer.Defines")]
[assembly: XmlnsDefinition("http://www.quickzip.org/Defines", "FileExplorer.Defines")]
[assembly: XmlnsDefinition("http://www.quickzip.org/Converters", "FileExplorer.UIEventHub.Converters")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]