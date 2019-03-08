using DevWeek.Architecture.Extensions;
using RabbitMQ.Client;
using Oragon.Spring.Objects.Factory.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevWeek.Architecture.MessageQueuing
{
	public class RabbitMQConnectionPool : IDisposable
	{
		[Required]
		private ConnectionFactory ConnectionFactory { get; set; }

		private List<IConnection> Connections { get; set; }

		private uint PoolSize { get; set; }

		public RabbitMQConnectionPool(ConnectionFactory connectionFactory)
		{
			this.ConnectionFactory = connectionFactory;

			this.Connections = new List<IConnection>();

			if (this.PoolSize == 0)
			{
				this.PoolSize = 1;
			}

			this.EnsurePoolSize();
		}

		public IConnection GetConnection()
		{
			this.EnsurePoolSize();

			IConnection connection = this.EnsureConnectionOpen(this.Connections.First(c => c.IsOpen));

			return connection;
		}

		private IConnection EnsureConnectionOpen(IConnection connection)
		{
			IConnection ensuredConnection;

			try
			{
				IModel model = connection.CreateModel();

				model.Dispose();

				ensuredConnection = connection;
			}
			catch (Exception)
			{
				this.Connections.Remove(connection);

				ensuredConnection = this.GetConnection();
			}
			
			return ensuredConnection;
		}

		private void EnsurePoolSize()
		{
			this.Connections.RemoveAll(c => c.IsOpen == false);

			int newConnectionsNeeded = (this.PoolSize - this.Connections.Count).ToInt();

			for (int i = 0; i < newConnectionsNeeded; i++)
			{
				this.Connections.Add(this.ConnectionFactory.CreateConnection());
			}
		}

		public void Dispose()
		{
			this.Connections.RemoveAll(c => c.IsOpen == false);

			this.Connections.Where(c => c.IsOpen).ForEach(c => c.Close());
		}
	}
}