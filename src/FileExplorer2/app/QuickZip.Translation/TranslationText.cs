using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace QuickZip.Translation
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TranslationKeyAttribute : Attribute
    {
        public TranslationKeyAttribute(string translationKey, string propertyKey)
        {
            TranslationKey = translationKey;
            PropertyKey = propertyKey;
        }

        public TranslationKeyAttribute(string translationKey)
            : this(translationKey, "Text")
        {
         
        }

        public string TranslationKey { get; private set; }
        public string PropertyKey { get; private set; }
    }


    public static class Texts
    {
        //Toolbar
        public static string strBrowseMedia = "Browse Media";
        public static string strSubmitBugs = "Submit bugs";
        public static string strSubmitSuggestions = "Submit suggestions";

        //Statusbar
        public static string strViewProgressLog = "View Progress log";

        //Settings
        public static string strRestartRequired = "Restart required";
        public static string strUseSevenZip = "Use 7z.dll to handle archives.";

        #region Dialogs

        //Progress Dialog

        public static string strClose = ApplicationCommands.Close.Text;
        //public static string strCancel = "Cancel";
        public static string strRestart = "Restart";
        public static string strPause = MediaCommands.Pause.Text;
        public static string strResume = "Resume";
        public static string strFrom = "From";
        public static string strTo = "To";
        public static string strRemainTime = "Remain Time";
        public static string strRemainItem = "Remain Item";
        public static string strMessage = "Message";
        public static string strMoreInformation = "More Information";
        public static string strLessInformation = "Less Information";


        //OverwriteDialog             
        [TranslationKey("windowOverwrite", "Title")]
        public static string strOverwrite = "Overwrite"; //windowOverwrite
        //public static string strSize = "Size : "; //headerSize
        //public static string strLastModified = "LastModified : "; //headerLastModified
        [TranslationKey("headerCRC", "Text")]
        public static string strCRC = "CRC : "; //headerCRC
        [TranslationKey("messageFileAlreadyExists", "Text")]
        public static string strFileAlreadyExists = "A file with same name already exists."; //messageFileAlreadyExists
        [TranslationKey("submessageKeepFile", "Text")]
        public static string strSelectFileToKeep = "Please select a file to keep."; //submessageKeepFile
        [TranslationKey("headerReplace", "Text")]
        public static string strCopyAndReplace = "Copy and replace"; //headerReplace
        [TranslationKey("subheaderReplace", "Text")]
        public static string strCopyAndReplaceDescription = "Use current file to replace the existing file :"; //subheaderReplace
        [TranslationKey("headerKeep", "Text")]
        public static string strKeep = "Do not overwrite"; //headerKeep
        [TranslationKey("subheaderKeep", "Text")]
        public static string strKeepDescription = "Keep existing file, discard changes :"; //subheaderKeep
        [TranslationKey("checkApplyAll", "Content")]
        public static string strApplyAll = "Apply To all"; //checkApplyAll
        [TranslationKey("buttonCancel", "Content")]
        public static string strCancel = "Cancel"; //buttonCancel
        [TranslationKey("buttonNo", "Content")]
        public static string strNo = "No"; //buttonNo
        [TranslationKey("buttonYes", "Content")]
        public static string strYes = "Yes"; //buttonYes
        [TranslationKey("messageFolderAlreadyExists", "Text")]
        public static string strFolderAlreadyExists = "Destation already contained a folder named"; //messageFolderAlreadyExists
        [TranslationKey("submessageReplaceFolder", "Text")]
        public static string strFolderAlreadyExistsDescription = "If there's any file with same name inside this folder, it will [b]be replaced[/b]."; //submessageReplaceFolder
        [TranslationKey("messageUseFolder", "Text")]
        public static string strUseThisFolder = "Do You want to use this folder"; //messageUseFolder
        [TranslationKey("messageReplaceFolder", "Text")]
        public static string strReplaceThisFolder = "to replace the following folder?"; //messageReplaceFolder



        #endregion

        #region COFE
        //COFE Archive Commands
        [TranslationKey("toolbarOperation", "Value")]
        public static string strOperationMask = "{0} Operations";
        [TranslationKey("toolbarArchive", "Value")]
        public static string strArchive = "Archive";
        [TranslationKey("toolbarArchiveCreate", "Value")]
        public static string strAddTo = "Add to";
        [TranslationKey("toolbarArchiveExtractTo", "Value")]
        public static string strExtractTo = "Extract to";
        [TranslationKey("toolbarArchiveExtract", "Value")]
        public static string strExtract = "Extract";
        [TranslationKey("toolbarArchiveCheckout", "Value")]
        public static string strCheckout = "Checkout";
        [TranslationKey("toolbarArchiveTest", "Value")]
        public static string strTest = "Test";

        [TranslationKey("toolbarArchiveCreate", "ToolTip")]
        public static string strAddToHint = "Add selected files to {0}";
        [TranslationKey("toolbarArchiveExtract", "ToolTip")]
        public static string strExtractHint = "Extract selected archive(s) to {0}";
        [TranslationKey("toolbarArchiveCheckout", "ToolTip")]
        public static string strCheckoutHint = "Extract selected items to temporary folder..";
        [TranslationKey("toolbarArchiveTest", "ToolTip")]
        public static string strTestHint = "Test the integrity of selected archive(s).";

        //COFE Bookmark Directory

        public static string strBookmarks = "Bookmarks";
        public static string strUnsupportedTypes = "Unsupported types.";
        public static string strNotFound = "Not found ({0}).";
        public static string strCreationFailed = "Creation failed ({0}).";

        public static string strBookmarksOperations = "Bookmarks Operations";
        //public static string strRemove = "Remove";
        public static string strRemoveBookmarkHint = "Remove this bookmark";
        public static string strOpenInExplorer = "Open in Explorer";
        public static string strOpenInExplorerHiint = "Open contained folder in Explorer";
        public static string strNewFolder = "New Folder";
        public static string strNewFolderHint = "Add a new bookmark folder here.";
        public static string strEnterFolderName = "Enter folder name :";
        #endregion

        //NatvigationBar commands
        public static string strRemoveFromList = "Remove from list";
        public static string strRemoveItemFromList = "Remove item from list";

        //Navigation Bars
        [TranslationKey("statusbarTotalHeaderDesktop", "Value")]
        public static string strItems = "{0} items"; //statusbarTotalHeaderDesktop
        public static string strOpenItem = "Open {0}";

        //Delete
        public static string strConfirmDelete = "Are you sure want to permanently remove these {0} item{1}?"; //Untranslated

        //Toolbar

        [TranslationKey("commandNewFolder", "Value")]
        public static string strFolder = "Folder";
        [TranslationKey("commandNewWin", "Value")]
        public static string strWindow = "Window";

        [TranslationKey("toolbarOrganize", "Header")]
        public static string strOrganize = "Organize";
        public static string strOpen = ApplicationCommands.Open.Text; //"Open"; //Untranslated
        public static string strChooseDefaultProgram = "Choose default program...";
        [TranslationKey("iconsizeExtraLarge", "Header")]
        public static string strExtraLargeIcon = "Extra Large Icon";
        [TranslationKey("iconsizeLarge", "Header")]
        public static string strLargeIcon = "Large Icon";
        [TranslationKey("iconsizeIcon", "Header")]
        public static string strIcon = "Icon";
        [TranslationKey("iconsizeSmall", "Header")]
        public static string strSmallIcon = "Small Icon";
        [TranslationKey("iconsizeList", "Header")]
        public static string strList = "List";
        [TranslationKey("iconsizeDetails", "Header")]
        public static string strGrid = "Grid";
        [TranslationKey("iconsizeTile", "Header")]
        public static string strTile = "Tile";

        //Statusbar

        [TranslationKey("statusbarSelectingHeader", "Value")]
        public static string strSelectingHeader = "Selecting {0} items.";
        [TranslationKey("statusbarSelectedHeader", "Value")]
        public static string strSelectedHeader = "{1} folders and {2} files selected. ({0} in total).";
        [TranslationKey("statusbarTotalHeader", "Value")]
        public static string strTotalHeader = "{1} folders and {2} files. ({0} in total)";
        [TranslationKey("statusbarFileSystem", "Value")]
        public static string strFileSystem = "File system";
        [TranslationKey("statusbarSpaceFree", "Value")]
        public static string strSpaceFree = "Space free";
        [TranslationKey("statusbarTotalSize", "Value")]
        public static string strTotalSize = "Total size";
        [TranslationKey("statusbarSize", "Value")]
        public static string strSize = "Size";
        [TranslationKey("statusbarLastModified", "Value")]
        public static string strLastModified = "Date modified";

        //Settings
        public static string strSettings = "Settings";
        public static string strGeneral = "General";
        public static string strTranslations = "Translations";
        public static string strExternalApps = "External Apps";
        public static string strAbout = "About";

        //Translations
        public static string strTranslationsHint = "Translation is applied automatically.";
        public static string strCode = "Code";
        public static string strEmail = "Email";
        public static string strTranslator = "Translator";
        public static string strLanguageName = "Language Name";
        public static string strAutoTranslate = "Auto Translate";
        public static string strAutoTranslateHelp = "If your language is not listed here, you can generate/update one using";

        //General
        public static string strAutoUpdate = "Auto Update";
        public static string strPromptUpdate = "Prompt for update when available.";
        public static string strUserInterface = "User Interface";
        public static string strEnableAero = "Enable Aero Windows";
        public static string strUIScale = "UIScale";
        public static string strCache = "Cache";
        public static string strClearCache = "Clear Now";
        public static string strKeepCache = "Keep cache";
        public static string strClearCacheWhenExit = "Clear cache when exit";

        //ExternalApps
        public static string strNew = "New";
        public static string strRemove = "Remove";
        public static string strExternalEditorHint = "External editor can be used to open a file in folder or archive.";
        public static string strSupportedExtensions = "Supported Extensions";
        public static string strOpenCommand = "Open Command";
        public static string strDescription = "Description";

        //About
        public static string strLaunchAboutDialog = "Launch About dialog";

        [TranslationKey("flistName", "Header")]
        public static string strHeaderFile = "File";
        [TranslationKey("flistType", "Header")]
        public static string strHeaderType = "Type";
        [TranslationKey("flistTime", "Header")]
        public static string strHeaderTime = "Time";
        [TranslationKey("flistSize", "Header")]

        public static string strHeaderSize = "Size";

        //FileList

    }
}
