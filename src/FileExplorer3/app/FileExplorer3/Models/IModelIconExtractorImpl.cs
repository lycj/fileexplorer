using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;
using System.Collections.Concurrent;

namespace FileExplorer.Models
{
    public class ModelIconExtractor<T> : IModelIconExtractor<T>
    {
        Func<T, Task<byte[]>> _retTask = null;
        Func<T, byte[]> _retFunc = null;
        bool _isStatic = false;
        byte[] _cache = null;

        public static IModelIconExtractor<T> FromTaskFunc(Func<T, Task<byte[]>> task)
        {
            return new ModelIconExtractor<T>() { _retTask = task };
        }

        public static IModelIconExtractor<T> FromTaskFunc(Func<Task<byte[]>> task)
        {
            var retVal = FromTaskFunc(t => task()) as ModelIconExtractor<T>;
            retVal.IsStatic = true;
            return retVal;
        }

        public static IModelIconExtractor<T> FromFunc(Func<T, byte[]> func)
        {
            return new ModelIconExtractor<T>() { _retFunc = func };
        }

        public static IModelIconExtractor<T> FromFunc(Func<byte[]> func)
        {
            var retVal = FromFunc(t => func()) as ModelIconExtractor<T>;
            retVal.IsStatic = true;
            return retVal;
        }

        public static IModelIconExtractor<T> FromBytes(byte[] bytes)
        {
            return FromTaskFunc(() => Task<byte[]>.FromResult(bytes));
        }

        public static IModelIconExtractor<T> FromStream(Stream stream)
        {
            return FromBytes(stream.ToByteArray());
        }

        public async Task<byte[]> GetIconBytesForModelAsync(T model, CancellationToken ct)
        {
            if (_retTask == null)
            {
                return _retFunc(model);
            }

            if (_isStatic)
                if (_cache != null)
                    return _cache;
                else
                    return _cache = await _retTask(model);
            else
                return await _retTask(model);
        }

        public bool IsStatic { get { return _isStatic; } set { _isStatic = value; } }

        #region Cachable support
        private static ConcurrentDictionary<string, byte[]> _cacheDic
            = new ConcurrentDictionary<string, byte[]>();

        public static IModelIconExtractor<T> FromTaskFuncCachable(
            Func<T, string> keyFunc,
            Func<T, Task<byte[]>> task)
        {
            return ModelIconExtractor<T>
                .FromTaskFunc(
                    async t =>
                    {
                        string key = keyFunc(t);
                        if (key == null)
                            return await task(t);

                        if (!_cacheDic.ContainsKey(key))
                            return _cacheDic[key] = await task(t);
                        return _cacheDic[key] ?? new byte[] { };
                    }
                );
        }

        public static IModelIconExtractor<T> FromTaskFuncCachable(string key, Func<Task<byte[]>> task)
        {
            return ModelIconExtractor<T>
               .FromTaskFunc(
                   async () =>
                   {
                       if (key == null)
                           return await task();

                       if (!_cacheDic.ContainsKey(key))
                           return _cacheDic[key] = await task();
                       return _cacheDic[key] ?? new byte[] {};
                   }
               );
        }

        public static IModelIconExtractor<T> FromFuncCachable(
            Func<T, string> keyFunc, Func<T, byte[]> func)
        {
            return ModelIconExtractor<T>
               .FromFunc(
                   t =>
                   {
                       string key = keyFunc(t);
                       if (key == null)
                           return func(t);

                       if (!_cacheDic.ContainsKey(key))
                       {
                           byte[] val = func(t);
                           if (val != null && val.Length > 0)
                               _cacheDic[key] = val;
                           return val;                           
                       }
                       return _cacheDic[key] ?? new byte[] {};
                   }
               );
        }

        public static IModelIconExtractor<T> FromFuncCachable(string key, Func<byte[]> func)
        {
            return ModelIconExtractor<T>
              .FromFunc(
                  () =>
                  {
                      if (key == null)
                          return func();

                      if (!_cacheDic.ContainsKey(key))
                          return _cacheDic[key] = func() ?? new byte[] {};
                      return _cacheDic[key] ?? new byte[] {};
                  }
              );
        }
        #endregion

    }
}
