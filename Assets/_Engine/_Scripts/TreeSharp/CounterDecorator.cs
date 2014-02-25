using System;
using System.Collections.Generic;

namespace TreeSharp
{
    /// <summary>
    ///   Implements a 'wait' composite. This composite will return Running until some condition is met, or it has
    ///   exceeded its alloted wait time.
    /// </summary>
    /// <remarks>
    ///   Created 1/13/2011.
    /// </remarks>
    public class CounterDecorator : Decorator
    {
        readonly int counterMax;
        int counter;

        /// <summary>
        ///   Creates a new Wait decorator using the specified timeout, run delegate, and child composite.
        /// </summary>
        /// <param name = "timeoutSeconds"></param>
        /// <param name = "runFunc"></param>
        /// <param name = "child"></param>
        public CounterDecorator(int count, CanRunDecoratorDelegate runFunc, Composite child)
            : base(runFunc, child)
        {
            counterMax = count;
            counter = -1;
        }

        /// <summary>
        ///   Creates a new Wait decorator with the specified timeout, and child composite.
        /// </summary>
        /// <param name = "timeoutSeconds"></param>
        /// <param name = "child"></param>
        public CounterDecorator(int count, Composite child)
            : base(child)
        {
            counterMax = count;
            counter = -1;
        }

        public override void Start(object context)
        {

            base.Start(context);
        }

        public override void Stop(object context)
        {
            base.Stop(context);
        }

        public override IEnumerable<RunStatus> Execute(object context)
        {
            while (counter > 0)
            {
                counter--;
                yield return RunStatus.Success;
            }

            counter = counterMax;

            DecoratedChild.Start(context);
            while (DecoratedChild.Tick(context) == RunStatus.Running)
            {
                yield return RunStatus.Running;
            }

            DecoratedChild.Stop(context);
            if (DecoratedChild.LastStatus == RunStatus.Failure)
            {
                counter = -1;
                yield return RunStatus.Failure;
                yield break;
            }

            yield return RunStatus.Success;
            yield break;
        }
    }
}