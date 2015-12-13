using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Utils
{
    public interface IFileNameGenerator
    {
        string Generate();
    }

    public static class FileNameGenerator
    {
        public static IFileNameGenerator FromNameGenerationMode(NameGenerationMode mode, string fileName)
        {
            switch (mode)
            {
                case NameGenerationMode.NoRename: return NoRename(fileName);
                case NameGenerationMode.Rename: return Rename(fileName);
                default: throw new NotSupportedException(mode.ToString());
            }
        }

        public static IFileNameGenerator NoRename(string fileName)
        {
            return new OneNameFileNameGenerator(fileName);
        }

        public static IFileNameGenerator Rename(string fileName)
        {
            return new IterateFileNameGenerator(fileName);
        }
    }

    public class OneNameFileNameGenerator : IFileNameGenerator
    {
        private string _fileName;
        public OneNameFileNameGenerator(string fileName)
        {
            _fileName = fileName;
        }

        public string Generate()
        {
            string retVal = _fileName;
            _fileName = null;
            return retVal;
        }
    }

    public class IterateFileNameGenerator : IFileNameGenerator
    {
        private string _fileName, _ext;
        private int _id = 0;
        private string _syntax;
        public IterateFileNameGenerator(string baseFileName, string syntax = "{0} ({2}){1}")
        {
            _fileName = System.IO.Path.GetFileNameWithoutExtension(baseFileName);
            _ext = System.IO.Path.GetExtension(baseFileName);
            _syntax = syntax;
        }

        public string Generate()
        {
            string retVal;
            if (_id == 0)
                retVal = _fileName + _ext;
            else
                retVal = String.Format(_syntax, _fileName, _ext, _id);
            _id++;

            return retVal;
        }
    }
}
