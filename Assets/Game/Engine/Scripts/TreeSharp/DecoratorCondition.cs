using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeSharp
{
    /// <summary>
    /// Decorator that stops executing child if it fails even once
    /// Useful basically only in situations where decisions are final e.g.
    /// final berserking rush if damaged enemy can't get away from player
    /// </summary>
    class DecoratorCondition : Decorator
    {
        bool hasFailed = false;

        /// <summary>
        ///   Creates a new Wait decorator using the specified timeout, run delegate, and child composite.
        /// </summary>
        /// <param name = "timeoutSeconds"></param>
        /// <param name = "runFunc"></param>
        /// <param name = "child"></param>
        public DecoratorCondition(CanRunDecoratorDelegate runFunc, Composite child)
            : base(runFunc, child)
        {
        }

        /// <summary>
        ///   Creates a new Wait decorator with the specified timeout, and child composite.
        /// </summary>
        /// <param name = "timeoutSeconds"></param>
        /// <param name = "child"></param>
        public DecoratorCondition(Composite child)
            : base(child)
        {
        }

        public override void Start(object context)
        {
            if (Runner.Invoke(context))
                hasFailed = true;
            else
                hasFailed = false;
            base.Start(context);
        }

        public override void Stop(object context)
        {
            base.Stop(context);
        }

        public override IEnumerable<RunStatus> Execute(object context)
        {
            if (hasFailed)
            {
                yield return RunStatus.Failure;
                yield break;
            }

            DecoratedChild.Start(context);
            while (DecoratedChild.Tick(context) == RunStatus.Running)
            {
                yield return RunStatus.Running;
            }

            DecoratedChild.Stop(context);
            if (DecoratedChild.LastStatus == RunStatus.Failure)
            {
                hasFailed = true;
                yield return RunStatus.Failure;
                yield break;
            }

            yield return RunStatus.Success;
            yield break;
        }
    }
}
