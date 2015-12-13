//using FileExplorer.Script;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FileExplorer.Models.Bookmark
//{
//    public class BookmarkCommandProvider : ICommandProvider
//    {
//        private BookmarkProfile _profile;

//        public BookmarkCommandProvider(BookmarkProfile profile)
//        {
//            _profile = profile;
//        }        

//        public IEnumerable<ICommandModel> GetCommandModels()
//        {
//            yield return new CommandModelBase(ems => new NewBookmarkFolder(_profile, ems))
//                {
//                    Header = "New Folder", IsVisibleOnToolbar = true
//                };
//        }
//    }
   

//    public class NewBookmarkFolder : ScriptCommandBase
//    {
//        private BookmarkProfile _profile;
//        private IEntryModel[] _selectedModels;
//        public NewBookmarkFolder(BookmarkProfile profile, IEntryModel[] selectedModels)
//            : base("NewBookmarkFolder")
//        {
//            _profile = profile;
//            _selectedModels = selectedModels;
//        }

//        public override IScriptCommand Execute(ParameterDic pm)
//        {            
//            if (CanExecute(pm))
//            {
//                BookmarkModel bm = (_selectedModels[0] as BookmarkModel);
//                bm.AddFolder("New Folder");
//            }

//            return NextCommand;
//        }

//        public override bool CanExecute(ParameterDic pm)
//        {                        
//            return _selectedModels != null && _selectedModels.Length == 1 && 
//                _selectedModels[0] is BookmarkModel &&
//                ((_selectedModels[0] as BookmarkModel).Type == BookmarkModel.BookmarkEntryType.Directory ||
//                    (_selectedModels[0] as BookmarkModel).Type == BookmarkModel.BookmarkEntryType.Root);
//        }
//    }

    
    
//}
