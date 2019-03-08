using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetVirtualHosts.Model;
using RestSharp;
using Oragon.Spring.Objects.Factory.Attributes;


namespace DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetVirtualHosts.Model
{
	public class AckDetails
	{
		public double rate { get; set; }
	}

	public class DeliverDetails
	{
		public double rate { get; set; }
	}

	public class DeliverGetDetails
	{
		public double rate { get; set; }
	}

	public class GetDetails
	{
		public double rate { get; set; }
	}

	public class PublishDetails
	{
		public double rate { get; set; }
	}

	public class RedeliverDetails
	{
		public double rate { get; set; }
	}

	public class ConfirmDetails
	{
		public double rate { get; set; }
	}

	public class GetNoAckDetails
	{
		public double rate { get; set; }
	}

	public class MessageStats
	{
		public long ack { get; set; }
		public AckDetails ack_details { get; set; }
		public long deliver { get; set; }
		public DeliverDetails deliver_details { get; set; }
		public long deliver_get { get; set; }
		public DeliverGetDetails deliver_get_details { get; set; }
		public long get { get; set; }
		public GetDetails get_details { get; set; }
		public long publish { get; set; }
		public PublishDetails publish_details { get; set; }
		public long redeliver { get; set; }
		public RedeliverDetails redeliver_details { get; set; }
		public long? confirm { get; set; }
		public ConfirmDetails confirm_details { get; set; }
		public long? get_no_ack { get; set; }
		public GetNoAckDetails get_no_ack_details { get; set; }
	}

	public class MessagesDetails
	{
		public double rate { get; set; }
	}

	public class MessagesReadyDetails
	{
		public double rate { get; set; }
	}

	public class MessagesUnacknowledgedDetails
	{
		public double rate { get; set; }
	}

	public class RecvOctDetails
	{
		public double rate { get; set; }
	}

	public class SendOctDetails
	{
		public double rate { get; set; }
	}

	public class VirtualHost
	{
		public string name { get; set; }
		public bool tracing { get; set; }
		public MessageStats message_stats { get; set; }
		public long? messages { get; set; }
		public MessagesDetails messages_details { get; set; }
		public long? messages_ready { get; set; }
		public MessagesReadyDetails messages_ready_details { get; set; }
		public long? messages_unacknowledged { get; set; }
		public MessagesUnacknowledgedDetails messages_unacknowledged_details { get; set; }
		public long? recv_oct { get; set; }
		public RecvOctDetails recv_oct_details { get; set; }
		public long? send_oct { get; set; }
		public SendOctDetails send_oct_details { get; set; }
	}

}
