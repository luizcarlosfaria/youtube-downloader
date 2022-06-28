using System;

namespace DevWeek.Architecture.MessageQueuing;

	public class DeserializationRejectionMessage
	{
		public string QueueName { get; set; }

		public DateTime Date { get; set; }

		public string SerializedException { get; set; }

		public string SerializedDataString { get; set; }

		public ReadOnlyMemory<byte> SerializedDataBinary { get; set; }
	}