using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Diagnostics;
//using QuickZip.IO.COFE.UserControls.ViewModel;
//using QuickZip.IO.COFE;
//using QuickZip.IO.COFE.UserControls.Model;
//using QuickZip.Dialogs;
using System.Reflection;
//using QuickZip.IO.COFE.UserControls;

namespace QuickZip.Translation
{
    public static class TranslationTools
    {
        public static TranslationDictionary LanguageContext = new TranslationDictionary();

        #region Translate Screen
        private static void translateMainScreen()
        {
            //TranslationTools.TranslateString("toolbarOrganize", "Header", ref OrganizeViewModel.strOrganize);
            //TranslationTools.TranslateString("flistName", "Header", ref FileListViewModel.strHeaderFile);
            //TranslationTools.TranslateString("flistTime", "Header", ref FileListViewModel.strHeaderTime);
            //TranslationTools.TranslateString("flistSize", "Header", ref FileListViewModel.strHeaderSize);
            //TranslationTools.TranslateString("flistType", "Header", ref FileListViewModel.strHeaderType);

            //TranslationTools.TranslateString("statusbarSelectingHeader", "Value", ref StatusbarViewModel.strSelectingHeader);
            //TranslationTools.TranslateString("statusbarSelectedHeader", "Value", ref StatusbarViewModel.strSelectedHeader);
            //TranslationTools.TranslateString("statusbarTotalHeader", "Value", ref StatusbarViewModel.strTotalHeader);

            //TranslationTools.TranslateString("statusbarFileSystem", "Value", ref StatusbarViewModel.strFileSystem);
            //TranslationTools.TranslateString("statusbarFreeSpace", "Value", ref StatusbarViewModel.strSpaceFree);
            //TranslationTools.TranslateString("statusbarTotalSpace", "Value", ref StatusbarViewModel.strTotalSize);
            //TranslationTools.TranslateString("statusbarSize", "Value", ref StatusbarViewModel.strSize);
            //TranslationTools.TranslateString("statusbarLastModified", "Value", ref StatusbarViewModel.strLastModified);

            //TranslationTools.TranslateString("iconsizeExtraLarge", "Header", ref ViewModeItemViewModel.strExtraLargeIcon);
            //TranslationTools.TranslateString("iconsizeLarge", "Header", ref ViewModeItemViewModel.strLargeIcon);
            //TranslationTools.TranslateString("iconsizeIcon", "Header", ref ViewModeItemViewModel.strIcon);
            //TranslationTools.TranslateString("iconsizeSmall", "Header", ref ViewModeItemViewModel.strSmallIcon);
            //TranslationTools.TranslateString("iconsizeList", "Header", ref ViewModeItemViewModel.strList);
            //TranslationTools.TranslateString("iconsizeDetails", "Header", ref ViewModeItemViewModel.strGrid);
            //TranslationTools.TranslateString("iconsizeTile", "Header", ref ViewModeItemViewModel.strTile);

            //TranslationTools.TranslateString("commandNewFolder", "Value", ref NewFolderViewModel.strFolder);
            //TranslationTools.TranslateString("commandNewWin", "Value", ref NewFolderViewModel.strWindow);

            //TranslationTools.TranslateString("statusbarTotalHeaderDesktop", "Value", ref WorkNotificationItemViewModel.strItems);



            //TranslationTools.TranslateString("toolbarOperation", "Value", ref ArchiveCommands.strOperationMask);
            //TranslationTools.TranslateString("toolbarArchive", "Value", ref ArchiveCommands.strArchive);
            //TranslationTools.TranslateString("toolbarArchiveCreate", "Value", ref ArchiveCommands.strAddTo);
            //TranslationTools.TranslateString("toolbarArchiveExtractTo", "Value", ref ArchiveCommands.strExtractTo);
            //TranslationTools.TranslateString("toolbarArchiveExtract", "Value", ref ArchiveCommands.strExtract);
            //TranslationTools.TranslateString("toolbarArchiveCheckout", "Value", ref ArchiveCommands.strCheckout);
            //TranslationTools.TranslateString("toolbarArchiveTest", "Value", ref ArchiveCommands.strTest);


            //TranslationTools.TranslateString("toolbarArchiveCreate", "ToolTip", ref ArchiveCommands.strAddToHint);
            //TranslationTools.TranslateString("toolbarArchiveExtract", "ToolTip", ref ArchiveCommands.strExtractHint);
            //TranslationTools.TranslateString("toolbarArchiveCheckout", "ToolTip", ref ArchiveCommands.strCheckoutHint);
            //TranslationTools.TranslateString("toolbarArchiveTest", "ToolTip", ref ArchiveCommands.strTestHint);

            //TranslationTools.TranslateString("strRemoveFromList", "Text", ref NotificationItemVIewModelBase.strRemoveFromList);
            //TranslationTools.TranslateString("strRemoveItemFromList", "Text", ref NotificationItemVIewModelBase.strRemoveItemFromList);
            //TranslationTools.TranslateString("toolbarOpen", "Value", ref WorkNotificationItemViewModel.strOpenItem);
            //TranslationTools.TranslateString("buttonCancel", "Content", ref COFECustomCommandProvider.strCancel);
            //TranslationTools.TranslateString("strCancelCurrentOperation", "Text", ref COFECustomCommandProvider.strCancelCurrentOperation);
            //TranslationTools.TranslateString("strRestart", "Text", ref COFECustomCommandProvider.strRestart);
            //TranslationTools.TranslateString("strRestartCurrentOperation", "Text", ref COFECustomCommandProvider.strRestartCurrentOperation);

            //TranslationTools.TranslateString("strSelectFileToPreview", "Text", ref BasicMediaPlayer.strSelectFileToPreview);
            //TranslationTools.TranslateString("strNoPreviewAvail", "Text", ref BasicMediaPlayer.strNoPreviewAvail);

            //ExAModel.dicEntryTypeNames.Clear();
            //ExAModel.dicEntryTypeNames.Add("Directory", TranslationTools.GetTranslateString("commandNewFolder", "Value", "Directory"));
            //ExAModel.dicEntryTypeNames.Add("Archive", TranslationTools.GetTranslateString("popupNewArchiveFolder", "Header", "Archive"));
        }
        private static void translateOverwriteDialog()
        {
            //TranslationTools.TranslateString("windowOverwrite", "Title", ref OverwriteDialog.strOverwrite);

            //TranslationTools.TranslateString("headerSize", "Text", ref OverwriteDialog.strSize);
            //TranslationTools.TranslateString("headerLastModified", "Text", ref OverwriteDialog.strLastModified);
            //TranslationTools.TranslateString("headerCRC", "Text", ref OverwriteDialog.strCRC);

            //TranslationTools.TranslateString("messageFileAlreadyExists", "Text", ref OverwriteDialog.strFileAlreadyExists);
            //TranslationTools.TranslateString("submessageKeepFile", "Text", ref OverwriteDialog.strSelectFileToKeep);
            //TranslationTools.TranslateString("headerReplace", "Text", ref OverwriteDialog.strCopyAndReplace);
            //TranslationTools.TranslateString("subheaderReplace", "Text", ref OverwriteDialog.strCopyAndReplaceDescription);
            //TranslationTools.TranslateString("headerKeep", "Text", ref OverwriteDialog.strKeep);
            //TranslationTools.TranslateString("subheaderKeep", "Text", ref OverwriteDialog.strKeepDescription);

            //TranslationTools.TranslateString("checkApplyAll", "Content", ref OverwriteDialog.strApplyAll);
            //TranslationTools.TranslateString("buttonCancel", "Content", ref OverwriteDialog.strCancel);
            //TranslationTools.TranslateString("buttonNo", "Content", ref OverwriteDialog.strNo);
            //TranslationTools.TranslateString("buttonYes", "Content", ref OverwriteDialog.strYes);
            //TranslationTools.TranslateString("messageFolderAlreadyExists", "Text", ref OverwriteDialog.strFolderAlreadyExists);
            //TranslationTools.TranslateString("submessageReplaceFolder", "Text", ref OverwriteDialog.strFolderAlreadyExistsDescription);
            //TranslationTools.TranslateString("messageUseFolder", "Text", ref OverwriteDialog.strUseThisFolder);
            //TranslationTools.TranslateString("messageReplaceFolder", "Text", ref OverwriteDialog.strReplaceThisFolder);
            ////TranslationTools.TranslateString("", "Text", ref OverwriteDialog.);        
        }
        private static void translateProgressDialog()
        {
            //TranslationTools.TranslateString("strRestart", "Text", ref ProgressDialog.strRestart);
            //TranslationTools.TranslateString("strResume", "Text", ref ProgressDialog.strResume);
            //TranslationTools.TranslateString("strFrom", "Text", ref ProgressDialog.strFrom);
            //TranslationTools.TranslateString("strTo", "Text", ref ProgressDialog.strTo);
            //TranslationTools.TranslateString("strRemainTime", "Text", ref ProgressDialog.strRemainTime);
            //TranslationTools.TranslateString("strRemainItem", "Text", ref ProgressDialog.strRemainItem);
            //TranslationTools.TranslateString("strMessage", "Text", ref ProgressDialog.strMessage);
            //TranslationTools.TranslateString("strMoreInformation", "Text", ref ProgressDialog.strMoreInformation);
            //TranslationTools.TranslateString("strLessInformation", "Text", ref ProgressDialog.strLessInformation);
            //TranslationTools.TranslateString("", "Text", ref ProgressDialog.);      
        }
        private static void translateCustomDirectory()
        {
            //Bookmark Directory        
            //TranslationTools.TranslateString("strBookmarks", "Text", ref BookmarkDirectoryLister.RootDirectoryName);
            //TranslationTools.TranslateString("strUnsupportedTypes", "Text", ref BookmarkDirectoryLister.strUnsupportedTypes);
            //TranslationTools.TranslateString("strNotFound", "Text", ref BookmarkDirectoryLister.strNotFound);
            //TranslationTools.TranslateString("strCreationFailed", "Text", ref BookmarkDirectoryLister.strCreationFailed);
            //TranslationTools.TranslateString("strBookmarksOperations", "Text", ref BookmarkCommands.strBookmarksOperations);

            //TranslationTools.TranslateString("strRemove", "Text", ref BookmarkCommands.strRemove);
            //TranslationTools.TranslateString("strRemoveBookmarkHint", "Text", ref BookmarkCommands.strRemoveBookmarkHint);
            //TranslationTools.TranslateString("strOpenInExplorer", "Text", ref BookmarkCommands.strOpenInExplorer);
            //TranslationTools.TranslateString("strOpenInExplorerHiint", "Text", ref BookmarkCommands.strOpenInExplorerHiint);
            //TranslationTools.TranslateString("strNewFolder", "Text", ref BookmarkCommands.strNewFolder);
            //TranslationTools.TranslateString("strNewFolderHint", "Text", ref BookmarkCommands.strNewFolderHint);
            //TranslationTools.TranslateString("strEnterFolderName", "Text", ref BookmarkCommands.strEnterFolderName);

        }

        private static void translateTexts()
        {
            foreach (FieldInfo fi in typeof(Texts).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                string value = fi.GetValue(null) as string;
                object[] translationKeyAttribute = fi.GetCustomAttributes(typeof(TranslationKeyAttribute), false);
                if (translationKeyAttribute != null && translationKeyAttribute.Length > 0)
                {
                    TranslationKeyAttribute attribute = translationKeyAttribute[0] as TranslationKeyAttribute;
                    TranslationTools.TranslateString(attribute.TranslationKey, attribute.PropertyKey, ref value);
                }
                else
                {
                    TranslationTools.TranslateString(fi.Name, "Text", ref value);                    
                }
                fi.SetValue(null, value);
            }
        }
        #endregion

        public static List<string> SearchPaths = new List<string>()
        {            
            Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "QuickZip\\5.1\\Languages"),

            Path.Combine(getProgramPath(), "Languages/")
        };

        #region Tools

        public static List<string> GetSupportedLanguages()
        {
            List<string> retVal = new List<string>();

            foreach (string searchPath in SearchPaths)
            {
                if (Directory.Exists(searchPath))
                    foreach (string languageFile in Directory.GetFiles(searchPath, "*.xml"))
                    {
                        string language = Path.GetFileNameWithoutExtension(languageFile);
                        if (!retVal.Contains(language))
                            retVal.Add(language);
                    }
            }

            return retVal;
        }

        public static List<TranslationDictionary> GetTranslationDictionaries()
        {
            List<TranslationDictionary> retVal = new List<TranslationDictionary>();

            foreach (string language in GetSupportedLanguages())
            {
                string languageFile = LookupFile(language + ".xml");
                if (languageFile != null)
                {
                    TranslationDictionary dic = new TranslationDictionary();
                    dic.Load(languageFile);
                    if (dic.Culture == language)
                        retVal.Add(dic);
                }

            }

            return retVal;
        }

        public static string LookupFile(string fileName)
        {
            foreach (string searchPath in SearchPaths)
            {
                string fullPath = Path.Combine(searchPath, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }

        private static void updateUserControlTranslations()
        {
            translateMainScreen();
            translateOverwriteDialog();
            translateProgressDialog();
            translateCustomDirectory();
            translateTexts();
            //COFECommands.Settings.Text = Texts.strSettings;
        }


        private static string getProgramPath()
        {
            return Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }
        #endregion

        #region TranslateString
        public static void TranslateString(string uid, string vid, ref string inputValue)
        {
            inputValue = GetTranslateString(uid, vid, inputValue);
        }

        public static string GetTranslateString(string uid, string vid, string defstring)
        {
            if (LanguageContext.DictionaryMode == DictionaryMode.Unknown)
                return defstring;

            try
            {
                return LanguageContext.Translate(uid, vid, defstring);
            }
            catch
            {
                Debug.WriteLine("ApplicationTranslatorServices.GetTranslateString - exception");
                return defstring;
            }
        }

        public static string GetTranslateString(string uid, string defstring)
        {
            if (LanguageContext.DictionaryMode == DictionaryMode.Unknown)
                return defstring;

            try
            {
                return LanguageContext.Translate(uid, "Value", defstring);
            }
            catch
            {
                Debug.WriteLine("ApplicationTranslatorServices.GetTranslateString - exception");
                return defstring;
            }
        }

        #endregion

        #region LoadLanguage
        private static void loadLanguage(string filename)
        {
            LanguageContext.Load(filename);
            updateUserControlTranslations();
        }



        public static void LoadLanguage(string languageName)
        {
            string languageFile = null;
            if (languageName.Length >= 3)
            {
                languageFile = LookupFile(languageName + ".xml");
            }
            else
                if (languageName.Length >= 2)
                {
                    languageFile = LookupFile(languageName.Substring(0, 2) + ".xml");
                }

            if (languageFile != null)
                loadLanguage(languageFile);
        }

        public static void LoadLanguage()
        {

            LoadLanguage(CultureInfo.CurrentCulture.Name);
            return;
        }

        #endregion
    }
}
