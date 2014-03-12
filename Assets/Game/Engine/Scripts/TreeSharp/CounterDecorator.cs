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
		
		AlienAiState myState;
		
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
			myState = 0;
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
			myState = 0;
		}
		
		public CounterDecorator(int count, AlienAiState stateToCheck, Composite child)
			: base(child)
		{
			counterMax = count;
			counter = -1;
			
			myState = stateToCheck;
		}
		
		public override void Start(object context)
		{
			if (context != null)
			{
				AiLookupTable bb = context as AiLookupTable;
				
				if (bb != null && myState != 0)
				{
					if (bb.LatestPathType != myState)
						counter = -1;
				}
			}
			
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