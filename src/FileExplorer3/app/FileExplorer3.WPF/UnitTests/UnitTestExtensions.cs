using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Moq.Language;
using Moq.Language.Flow;

namespace FileExplorer.WPF.UnitTests
{
    public static class UnitTestExtensions
    {
        public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(
            this IReturns<TMock, Task<TResult>> setup, TResult value)
            where TMock : class
        {
            return setup.Returns(Task.FromResult(value));
        }

        public static ResultCompletionEventArgs ExecuteAndWait(this IResult action, CoroutineExecutionContext context, 
            System.Action executeAfterActionStarted = null)
        {
            ResultCompletionEventArgs retVal = null;
            var handle = new System.Threading.ManualResetEventSlim(false);
            action.Completed += (sender, args) =>
            {
                if (args == null)
                    throw new Exception("Args = null");
                retVal = args;
                handle.Set();
            };
            action.Execute(context);
            if (executeAfterActionStarted != null)
                executeAfterActionStarted();
            handle.Wait();

            if (retVal == null)
                throw new Exception("Completed not triggered");
            return retVal;
        }
    }
}
