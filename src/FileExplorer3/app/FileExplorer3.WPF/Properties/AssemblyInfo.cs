using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("FileExplorer.WPF")]
[assembly: AssemblyDescription("FileExplorer is a WPF control (not metro) that's emulate the Windows Explorer, it supports both FileSystem and non-FileSystem class, as long as they are in the folder-content structure. FileExplorer3 includes TabControl, Breadcrumb, FolderTree, FileList, Sidebar, Toolbar and Statusbar.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Leung Yat Chun Joseph")]
[assembly: AssemblyProduct("FileExplorer")]
[assembly: AssemblyCopyright("Copyright © Leung Yat Chun Joseph 2008 - 2013 under MIT license")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: XmlnsDefinition("http://www.quickzip.org/UserControls", "FileExplorer.WPF.UserControls")]
[assembly: XmlnsDefinition("http://www.quickzip.org/BaseControls", "FileExplorer.WPF.BaseControls")]
[assembly: XmlnsDefinition("http://www.quickzip.org/BaseControls", "FileExplorer.WPF.Defines")]
[assembly: XmlnsDefinition("http://www.quickzip.org/Converters", "QuickZip.Converters")]
[assembly: XmlnsDefinition("http://www.quickzip.org/Converters", "FileExplorer.WPF.Defines.Converters")]
[assembly: XmlnsDefinition("http://www.quickzip.org/Views", "FileExplorer.WPF.Views")]
[assembly: XmlnsDefinition("http://www.quickzip.org/Views/Explorer", "FileExplorer.WPF.Views.Explorer")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("cb36b0f2-ba83-4a07-950c-ae3e7521a7ce")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("3.0.24.0")]
[assembly: AssemblyVersion("3.0.24.0")]
[assembly: AssemblyFileVersion("3.0.24.0")]

#if !WINRT
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]
#endif
