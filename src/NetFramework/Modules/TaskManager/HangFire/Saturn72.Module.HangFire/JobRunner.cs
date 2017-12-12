#region

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Saturn72.Core;
using Saturn72.Core.Services.Impl.Tasks;
using Saturn72.Core.Tasks;
using Saturn72.Extensions;

#endregion

namespace Saturn72.Module.HangFire
{
    public class JobRunner
    {
        public static void Run(string typeName)
        {
            var instance = CommonHelper.CreateInstance<ITask>(typeName);
            Guard.NotNull(instance);

            Run(instance.Execute);
        }

        public static void Run(string typeName, IDictionary<string, object> parameters)
        {
            var instance = CommonHelper.CreateInstance<BackgroundTaskBase>(typeName);
            Guard.NotNull(instance);
            instance.Parameters = parameters;

            Run(instance.Execute);
        }


        public static void Run(Action action)
        {
            action();
        }

        public static void RunExpression(Expression<Action> exp)
        {
            exp.Compile()();
        }
    }
}