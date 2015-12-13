using FileExplorer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Get an IEntryModel[] from ParameterDic[entryKey], 
        /// If it's an IEntryModel[], return directly.
        /// If it's an IEntryModel, convert to array and return.
        /// If it's a string, parse from profileKey, convert to IEntryModel and then IEntryModel[] and return.
        /// Throw exception if neither.
        /// </summary>
        /// <param name="paramDic"></param>
        /// <returns></returns>
        public static async Task<IEntryModel[]> GetValueAsEntryModelArrayAsync(this ParameterDic paramDic, string entryKey, string profileKey = null)
        {
            object value = paramDic.GetValue<object>(entryKey);
            if (value is IEnumerable)
                return (value as IEnumerable).Cast<IEntryModel>().ToArray();
            if (value is IEntryModel[])
                return (IEntryModel[])value;

            return new IEntryModel[] { await GetValueAsEntryModelAsync(paramDic, entryKey, profileKey) };
            throw new ArgumentException(entryKey);
        }

        /// <summary>
        /// Get an IEntryModel from ParameterDic[entryKey],         
        /// If it's an IEntryModel, convert to array and return.
        /// If it's a string, parse from profileKey, convert to IEntryModel and then IEntryModel[] and return.
        /// Throw exception if neither.
        /// </summary>
        /// <param name="paramDic"></param>
        /// <param name="entryKey"></param>
        /// <param name="profileKey"></param>
        /// <returns></returns>
        public static async Task<IEntryModel> GetValueAsEntryModelAsync(this ParameterDic paramDic, string entryKey, string profileKey = null)
        {
            object value = paramDic.GetValue<object>(entryKey);
            if (value is IEntryModel)
                return (IEntryModel)value;
            if (value is string)
            {
                string path = (string)value;
                if (profileKey != null)
                {
                    IProfile profile = paramDic.GetValue<IProfile>(profileKey);
                    if (profile == null)
                        throw new ArgumentException(profileKey);
                    return await profile.ParseAsync(path);
                }
            }
            throw new ArgumentException(entryKey);
        }

        public static string GetDescription(this IEntryModel[] entryModels)
        {
            if (entryModels.Count() == 1)
                return entryModels.First().Name;
            else return String.Format("{0} items", entryModels.Count());
        }

    }
}
