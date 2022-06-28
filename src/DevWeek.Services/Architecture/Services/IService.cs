namespace DevWeek.Architecture.Services;

	public interface IService
	{
		string Name
		{
			get;
		}

		void Start();

		void Stop();
	}