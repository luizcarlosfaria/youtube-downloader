using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetQueues.Model;
using RestSharp;
using Oragon.Spring.Objects.Factory.Attributes;


namespace DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetQueues.Model
{
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

	public class BackingQueueStatus
	{
		public int q1 { get; set; }
		public int q2 { get; set; }
		public List<object> delta { get; set; }
		public int q3 { get; set; }
		public int q4 { get; set; }
		public int len { get; set; }
		public int pending_acks { get; set; }
		public object target_ram_count { get; set; }
		public int ram_msg_count { get; set; }
		public int ram_ack_count { get; set; }
		public int next_seq_id { get; set; }
		public int persistent_count { get; set; }
		public double avg_ingress_rate { get; set; }
		public double avg_egress_rate { get; set; }
		public double avg_ack_ingress_rate { get; set; }
		public double avg_ack_egress_rate { get; set; }
	}

	public class Arguments
	{
	}

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

	public class PublishDetails
	{
		public double rate { get; set; }
	}

	public class GetDetails
	{
		public double rate { get; set; }
	}

	public class RedeliverDetails
	{
		public double rate { get; set; }
	}

	public class GetNoAckDetails
	{
		public double rate { get; set; }
	}

	public class MessageStats
	{
		public int ack { get; set; }
		public AckDetails ack_details { get; set; }
		public int deliver { get; set; }
		public DeliverDetails deliver_details { get; set; }
		public int deliver_get { get; set; }
		public DeliverGetDetails deliver_get_details { get; set; }
		public int publish { get; set; }
		public PublishDetails publish_details { get; set; }
		public int? get { get; set; }
		public GetDetails get_details { get; set; }
		public int? redeliver { get; set; }
		public RedeliverDetails redeliver_details { get; set; }
		public int? get_no_ack { get; set; }
		public GetNoAckDetails get_no_ack_details { get; set; }
	}

	public class Queue
	{
		public int memory { get; set; }
		public int messages { get; set; }
		public MessagesDetails messages_details { get; set; }
		public int messages_ready { get; set; }
		public MessagesReadyDetails messages_ready_details { get; set; }
		public int messages_unacknowledged { get; set; }
		public MessagesUnacknowledgedDetails messages_unacknowledged_details { get; set; }
		public string idle_since { get; set; }
		public string policy { get; set; }
		public string exclusive_consumer_tag { get; set; }
		public int consumers { get; set; }
		public BackingQueueStatus backing_queue_status { get; set; }
		public string status { get; set; }
		public string name { get; set; }
		public string vhost { get; set; }
		public bool durable { get; set; }
		public bool auto_delete { get; set; }
		public Arguments arguments { get; set; }
		public string node { get; set; }
		public MessageStats message_stats { get; set; }
	}

}