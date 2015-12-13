using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FileExplorer.Utils
{
    public static class ExpressionUtils
    {
        public static T AbsoluteValue<T>(T a)
        {
            var mInfo = typeof(Math)
                .GetRuntimeMethods().First(m => m.Name == "Abs" && m.GetParameters().First().ParameterType == typeof(T));
            return (T)mInfo.Invoke(null, new object[] { a });
        }

        //http://www.yoda.arachsys.com/csharp/genericoperators.html
        public static T Add<T>(T a, T b)
        {
            // declare the parameters
            ParameterExpression paramA = System.Linq.Expressions.Expression.Parameter(typeof(T), "a"),
                paramB = System.Linq.Expressions.Expression.Parameter(typeof(T), "b");
            // add the parameters together
            BinaryExpression body;

            body = System.Linq.Expressions.Expression.Add(paramA, paramB);

            // compile it
            Func<T, T, T> add = System.Linq.Expressions.Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            return add(a, b);
        }

        public static T Subtract<T>(T a, T b)
        {
            // declare the parameters
            ParameterExpression paramA = System.Linq.Expressions.Expression.Parameter(typeof(T), "a"),
                paramB = System.Linq.Expressions.Expression.Parameter(typeof(T), "b");
            // add the parameters together
            BinaryExpression body = System.Linq.Expressions.Expression.Subtract(paramA, paramB);
            // compile it
            Func<T, T, T> subtract = System.Linq.Expressions.Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            return subtract(a, b);
        }

        public static T Multiply<T>(T a, T b)
        {
            // declare the parameters
            ParameterExpression paramA = System.Linq.Expressions.Expression.Parameter(typeof(T), "a"),
                paramB = System.Linq.Expressions.Expression.Parameter(typeof(T), "b");
            // add the parameters together
            BinaryExpression body = System.Linq.Expressions.Expression.Multiply(paramA, paramB);
            // compile it
            Func<T, T, T> multiply = System.Linq.Expressions.Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            return multiply(a, b);
        }

        public static T Divide<T>(T a, T b)
        {
            // declare the parameters
            ParameterExpression paramA = System.Linq.Expressions.Expression.Parameter(typeof(T), "a"),
                paramB = System.Linq.Expressions.Expression.Parameter(typeof(T), "b");
            // add the parameters together
            BinaryExpression body = System.Linq.Expressions.Expression.Divide(paramA, paramB);
            // compile it
            Func<T, T, T> divide = System.Linq.Expressions.Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            return divide(a, b);
        }

        public static T Modulo<T>(T a, T b)
        {
            // declare the parameters
            ParameterExpression paramA = System.Linq.Expressions.Expression.Parameter(typeof(T), "a"),
                paramB = System.Linq.Expressions.Expression.Parameter(typeof(T), "b");
            // add the parameters together
            BinaryExpression body = System.Linq.Expressions.Expression.Modulo(paramA, paramB);
            // compile it
            Func<T, T, T> modulo = System.Linq.Expressions.Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            return modulo(a, b);
        }

        public static string GetPropertyName<C, T>(Expression<Func<C, T>> expression)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }

        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }
    }
}
