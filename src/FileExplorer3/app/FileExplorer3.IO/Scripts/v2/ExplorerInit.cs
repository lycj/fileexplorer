//using FileExplorer.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FileExplorer.Script
//{

//       //ScriptRunner.RunScriptAsync(new ParameterDic() { 
//       //             //Required
//       //             { "Profiles", profiles },
//       //             { "OnModelCreated", onModelCreated },
//       //             { "OnViewAttached", UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot()},                    
//       //             { "RootDirectories", RootModels.ToArray() },	                    
//       //             //Optional
//       //             { "StartupPath", OpenPath },
//       //             { "Events", _events },
//       //             { "WindowManager", _windowManager },
//       //             { "EnableDrag", _enableDrag }, 
//       //             { "EnableDrop", _enableDrop },                     
//       //             { "EnableMultiSelect", _enableMultiSelect}, 
//       //         }, UIScriptCommands.ExplorerShow());

//    public class ExplorerInit : ScriptCommandBase
//    {
//        public static IScriptCommand Default_OnModelCreated =
//                ScriptCommands.RunCommandsInSequence(null,
//                    IOScriptCommands.ExplorerDefault(),
//                    IOScriptCommands.ExplorerDefaultToolbarCommands(),
//                    UIScriptCommands.ExplorerAssignScriptParameters("{Explorer}",
//                            "{OnViewAttached},{OnModelCreated},{EnableDrag},{EnableDrop},{EnableMultiSelect}")
//                    );
//        public static IScriptCommand Default_OnViewAttached =
//            UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot();

//        public string ExplorerKey { get; set; }

//        public IProfile[] Profiles { get; set; }
//        public IScriptCommand OnModelCreated { get; set; }
//        public IScriptCommand OnViewAttached { get; set; }
//        public IEntryModel[] RootDirectories { get; set; }
//        public string StartupPath { get; set; }
//        public bool EnableDrag { get; set; }
//        public bool EnableDrop { get; set; }
//        public bool EnableMultiSelect { get; set; }


//        public ExplorerInit()
//            : base("ExplorerInit")
//        {
//            ExplorerKey = "{Explorer}";
//            Profiles = new IProfile[] { new FileSystemInfoExProfile( };
//            OnModelCreated = Default_OnModelCreated;
//            OnViewAttached = Default_OnViewAttached;
//            RootDirectories = new IEntryModel[] { };
//        }

//        public override IScriptCommand Execute(ParameterDic pm)
//        {


//            return NextCommand;
//        }

//    }
//}
