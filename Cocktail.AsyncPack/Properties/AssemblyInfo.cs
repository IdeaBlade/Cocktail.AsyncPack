using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if SILVERLIGHT
[assembly: AssemblyTitle("Cocktail.AsyncPack.SL")]
#else
[assembly: AssemblyTitle("Cocktail.AsyncPack")]
#endif
[assembly: AssemblyDescription("Cocktail Async Pack for Visual Studio 2012")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("IdeaBlade")]
[assembly: AssemblyProduct("Cocktail")]
[assembly: AssemblyCopyright("Copyright © IdeaBlade 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguageAttribute("en")]

[assembly: XmlnsDefinition("http://cocktail.ideablade.com", "Cocktail")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
#if SILVERLIGHT
[assembly: Guid("C667B886-4989-45E6-A8AD-235B1EFEBE41")]
#else
[assembly: Guid("b9a73b6d-1f1e-46a7-bfef-c44aaaf97511")]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.1.0")]
