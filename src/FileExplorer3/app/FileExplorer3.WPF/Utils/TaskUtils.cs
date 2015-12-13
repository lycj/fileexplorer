using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace FileExplorer.WPF.ViewModels
{
    public static class TaskUtils
    {
        /// <summary>
        /// Make sure continueTask runs even if prevTask is completed / faulted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prevTask"></param>
        /// <param name="continueTask"></param>
        public static  void ContinueWithCheck<T>(this Task<T> prevTask, Action<Task<T>> continueTask)
        {
            if (prevTask.Status == TaskStatus.Running)
                prevTask.ContinueWith(continueTask);
            else continueTask(prevTask);
        }

        //public static void Execute(this IEnumerable<IResult> actions, ActionExecutionContext context = null)
        //{
        //    new SequentialResult(actions.GetEnumerator()).Execute(context);
        //}

        //public static async Task ExecuteAsync(this IEnumerable<IResult> actions, ActionExecutionContext context = null)
        //{
        //    await new SequentialResult(actions.GetEnumerator()).ExecuteAsync(context);
        //}

        public static IEnumerable<IResult> Append(this IEnumerable<IResult> actions, params IResult[] appendActions)
        {
            foreach (var a in actions.ToList())
                yield return a;
            foreach (var a in appendActions)
                yield return a;   
        }
    }
}
