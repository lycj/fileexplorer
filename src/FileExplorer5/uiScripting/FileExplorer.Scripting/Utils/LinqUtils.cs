using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Utils
{
    public static class LinqUtils
    {
        //http://stackoverflow.com/questions/3453274/using-linq-to-get-the-last-n-elements-of-a-collection
        // Ex: collection.TakeLast(5);
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        //http://stackoverflow.com/questions/9801624/get-name-of-a-variable-or-parameter
        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }
    }
}
