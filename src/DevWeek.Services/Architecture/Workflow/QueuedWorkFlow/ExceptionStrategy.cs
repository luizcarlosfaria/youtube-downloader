namespace DevWeek.Architecture.Workflow.QueuedWorkFlow
{
	public enum ExceptionStrategy
	{
		SendToErrorQueue,
		SendToNextStepQueue,
		Requeue
	}
}