using DevWeek.Architecture.Extensions;
using RabbitMQ.Client;
using Spring.Objects.Factory.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevWeek.Architecture.MessageQueuing;

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

        if (this.PoolSize == 0) this.PoolSize = 1;

        this.EnsurePoolSize();
    }

    public IConnection GetConnection(int depth = 0)
    {
        this.EnsurePoolSize();

        IConnection connection = this.Connections.First(c => c.IsOpen);

        try
        {
            using IModel model = connection.CreateModel();

            model.Close();
            
        }
        catch (Exception)
        {
            Console.WriteLine($"{nameof(RabbitMQConnectionPool)}.{nameof(GetConnection)} | Removendo conexão");
            this.Connections.Remove(connection);

            connection = this.GetConnection(depth++);
        }

        return connection;
    }

    private void EnsurePoolSize()
    {
        this.Connections.RemoveAll(c => c.IsOpen == false);

        int newConnectionsNeeded = (this.PoolSize - this.Connections.Count).ToInt();

        for (int i = 0; i < newConnectionsNeeded; i++)
        {
            Console.WriteLine($"{nameof(RabbitMQConnectionPool)}.{nameof(EnsurePoolSize)} | Adicionando conexão");
            this.Connections.Add(this.ConnectionFactory.CreateConnection());
        }
    }

    public void Dispose()
    {
        System.Diagnostics.Debug.WriteLine($"{nameof(RabbitMQConnectionPool)}.{nameof(Dispose)} | Encerrando tudo");

        this.Connections.RemoveAll(c => c.IsOpen == false);

        this.Connections.Where(c => c.IsOpen).ForEach(c => c.Close());
    }
}