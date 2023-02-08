namespace Universe.Stores.Runtime
{
	public enum AsyncState
	{
		INVALID = -1,
		WAITING = 0,
		STARTED = 1,
		SUCCEED = 2,
		CANCELLED = 3,
		FAILED = 4
	}
}
