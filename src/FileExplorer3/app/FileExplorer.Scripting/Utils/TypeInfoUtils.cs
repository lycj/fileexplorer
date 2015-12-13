using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace FileExplorer.WPF.Utils
{
    public static class TypeInfoUtils
    {
        public static object GetPropertyOrMethod(object obj, params string[] pathSplit)
        {
            for (int i = 0; i < pathSplit.Length; i++)
                obj = GetPropertyOrMethod(obj, pathSplit[i]);
            return obj;
        }

        public static void SetProperty(object obj, string name, object value, bool throwIfNotFound = false)
        {
            var propertyInfo = obj.GetType().GetTypeInfo().GetPropertyInfoRecursive(name);
            if (propertyInfo == null)
            {
                if (throwIfNotFound)
                    throw new KeyNotFoundException(name);                
            }
            else propertyInfo.SetValue(obj, value);
        }

        public static object GetPropertyOrMethod(object obj, string name, bool throwIfNotFound = false)
        {
            var match = Regex.Match(name, RegexPatterns.ParseArrayCounterPattern);
            name = match.Groups["variable"].Value;
            int idx = match.Groups["counter"].Success ? Int32.Parse(match.Groups["counter"].Value) : -1;


            if (name.EndsWith("()"))
            {
                TypeInfo typInfo = obj.GetType().GetTypeInfo();
                string methodName = name.TrimEnd('(', ')');
                var methodInfo = typInfo.GetMethodInfoRecursive(methodName);
                if (methodInfo == null)
                {
                    Assembly assembly = obj is System.Collections.IEnumerable ? typeof(System.Linq.Enumerable).GetTypeInfo().Assembly :
                        typInfo.Assembly;
                    methodInfo = typInfo.GetMethodInfoFromExtension(mi => mi.Name.Equals(methodName), assembly).FirstOrDefault();
                    if (methodInfo != null)
                        return methodInfo.Invoke(null, new object[] { obj });
                    else if (throwIfNotFound)
                        throw new KeyNotFoundException(name);
                    else return null;
                }
                else
                {
                    return methodInfo.Invoke(obj, new object[] { });
                }
            }
            else
            {
                var propertyInfo = obj.GetType().GetTypeInfo().GetPropertyInfoRecursive(name);
                if (propertyInfo == null)
                {
                    if (throwIfNotFound)
                        throw new KeyNotFoundException(name);
                    else return null;
                }
                    
                else
                {
                    object retVal = propertyInfo.GetValue(obj);
                    if (retVal is Array && idx != -1)
                        return (retVal as Array).GetValue(idx);
                    else return retVal;
                }
            }
        }

        public static FieldInfo GetFieldInfoRecursive(this TypeInfo typeInfo, string fieldName)
        {
            var retVal = typeInfo.DeclaredFields.FirstOrDefault(pi => pi.Name.Equals(fieldName));
            if (retVal == null && typeInfo.BaseType != null)
            {
                return GetFieldInfoRecursive(typeInfo.BaseType.GetTypeInfo(), fieldName);
            }
            return retVal;
        }

        public static PropertyInfo GetPropertyInfoRecursive(this TypeInfo typeInfo, string propertyName)
        {
            var retVal = typeInfo.DeclaredProperties.FirstOrDefault(pi => pi.Name.Equals(propertyName));
            if (retVal == null && typeInfo.BaseType != null)
            {
                return GetPropertyInfoRecursive(typeInfo.BaseType.GetTypeInfo(), propertyName);
            }
            return retVal;
        }

        public static IEnumerable<PropertyInfo> EnumeratePropertyInfoRecursive(this TypeInfo typeInfo)
        {
            List<PropertyInfo> retVal = new List<PropertyInfo>();
            retVal.AddRange(typeInfo.DeclaredProperties);
            if (typeInfo.BaseType != null)
                retVal.AddRange(EnumeratePropertyInfoRecursive(typeInfo.BaseType.GetTypeInfo()));
            return retVal;
        }

        public static MethodInfo GetMethodInfoRecursive(this TypeInfo typeInfo, string methodName)
        {
            var retVal = typeInfo.DeclaredMethods.FirstOrDefault(pi => pi.Name.Equals(methodName));
            if (retVal == null && typeInfo.BaseType != null)
            {
                return GetMethodInfoRecursive(typeInfo.BaseType.GetTypeInfo(), methodName);
            }
            return retVal;
        }

        public static IEnumerable<MethodInfo> GetMethodInfoFromExtension(this TypeInfo typeInfo,
            Func<MethodInfo, bool> methodFilter, Assembly assembly)
        {
            IEnumerable<MethodInfo> retVal;

            if (typeInfo.GenericTypeArguments.FirstOrDefault() == null)
            {
                retVal = assembly.DefinedTypes
                  .Where(typ => typ.IsSealed && !typ.IsGenericType && !typ.IsNested)
                  .SelectMany(typ => typ.DeclaredMethods)
                  .Where(method => method.IsStatic &&
                      (method.GetParameters().FirstOrDefault() != null &&
                          method.GetParameters().FirstOrDefault().ParameterType.GetTypeInfo().Equals(typeInfo)))
                  .Where(method => methodFilter(method));
            }
            else
            {
                Type argumentType = typeInfo.GenericTypeArguments.FirstOrDefault();
                retVal = assembly.DefinedTypes
                .Where(typ => typ.IsSealed && !typ.IsNested)
                .SelectMany(typ => typ.DeclaredMethods)
                .Where(mi => mi.IsStatic && mi.IsGenericMethod)
                .Where(mi => mi.GetGenericArguments().Count() == 1) //Only one argument
                .Where(mi => mi.GetParameters().Count() == 1) //Only one parameter.
                .Select(mi => mi.MakeGenericMethod(argumentType)) //Make Generic
                .Where(mi => mi.GetParameters().FirstOrDefault().ParameterType.GetTypeInfo().IsAssignableFrom(typeInfo))
                .Where(mi => methodFilter(mi));
            }
            return retVal;
        }
    }
}
