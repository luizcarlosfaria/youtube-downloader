namespace DevWeek.Architecture.MessageQueuing.RabbitMQManagement;

internal interface IRabbitMQManagementClient
{
    System.Collections.Generic.List<DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetQueues.Model.Queue> GetQueues(string virtualHost = null);
    System.Collections.Generic.List<DevWeek.Architecture.MessageQueuing.RabbitMQManagement.GetVirtualHosts.Model.VirtualHost> GetVirtualHosts();
}
