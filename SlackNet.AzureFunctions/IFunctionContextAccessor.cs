using Microsoft.Azure.Functions.Worker;

namespace SlackNet.AzureFunctions;

public interface IFunctionContextAccessor
{
	FunctionContext? FunctionContext { get; set; }
}

public class FunctionContextAccessor : IFunctionContextAccessor
{
	private static readonly AsyncLocal<FunctionContextRedirect> CurrentContext = new();

	public virtual FunctionContext? FunctionContext
	{
		get => CurrentContext.Value?.HeldContext;
		set
		{
			var holder = CurrentContext.Value;
			if (holder != null)
			{
				// Clear current context trapped in the AsyncLocals, as its done.
				holder.HeldContext = null;
			}

			if (value != null)
			{
				// Use an object indirection to hold the context in the AsyncLocal,
				// so it can be cleared in all ExecutionContexts when its cleared.
				CurrentContext.Value = new FunctionContextRedirect { HeldContext = value };
			}
		}
	}

	private class FunctionContextRedirect
	{
		public FunctionContext? HeldContext;
	}
}