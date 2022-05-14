using Spring.Objects.Factory.Attributes;

namespace DevWeek.Architecture.MessageQueuing
{
	public class QueueInfo
	{
		[Required]
		public uint ConsumerCount { get; set; }

		[Required]
		public uint MessageCount { get; set; }

		[Required]
		public string QueueName { get; set; }
	}
}