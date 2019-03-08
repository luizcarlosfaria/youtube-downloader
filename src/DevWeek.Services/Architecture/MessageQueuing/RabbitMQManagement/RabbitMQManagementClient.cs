//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using RestSharp;
//using Oragon.Spring.Objects.Factory.Attributes;

//namespace DevWeek.Architecture.MessageQueuing.RabbitMQManagement
//{
//	/// <summary>
//	/// Realiza as operações de validação no 
//	/// </summary>
//	public partial class RabbitMQManagementClient : IRabbitMQManagementClient
//	{
//		[Required]
//		public IRestClient RestClient { get; set; }

//		public RabbitMQManagementClient() { }

//		public List<DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetQueues.Model.Queue> GetQueues(string virtualHost = null)
//		{
//			if (string.IsNullOrWhiteSpace(virtualHost) || virtualHost == "/")
//				virtualHost = null;

//			var request = new RestSharp.RestRequest(virtualHost == null ? "queues" : "queues/" + virtualHost + "/");
//			IRestResponse<List<DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetQueues.Model.Queue>> response = this.RestClient.Execute<List<DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetQueues.Model.Queue>>(request);
//			var result = response.Data;
//			return result;
//		}

//		public List<DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetVirtualHosts.Model.VirtualHost> GetVirtualHosts()
//		{
//			var request = new RestSharp.RestRequest("vhosts");
//			IRestResponse<List<DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetVirtualHosts.Model.VirtualHost>> response = this.RestClient.Execute<List<DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetVirtualHosts.Model.VirtualHost>>(request);
//			var result = response.Data;
//			return result;
//		}

//	}
//}
