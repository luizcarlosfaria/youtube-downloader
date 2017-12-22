using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Minio;
using DevWeek.Services.Downloader;
using Polly;

namespace DevWeek
{
    class Program
    {
        static void Main(string[] args)
        {
            Spring.Context.Support.AbstractApplicationContext appContext = null;

            var retryOnStartupPolicy = Policy
               //.HandleInner<StackExchange.Redis.RedisConnectionException>()
               .Handle<Exception>()
               .WaitAndRetry(9, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );
            //2^9=512 segundos (8,53333 minutos)
            retryOnStartupPolicy.Execute(() =>
            {
                appContext = ContextBuilder.BuildContext();
            });


#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                var rabbitMQ = appContext.GetObject<RabbitMQ.Client.IModel>();
                string queue = appContext.GetObject<string>("CONFIG:DevWeek:RabbitMQ:DownloadPipeline:Queue");
                while (true)
                {
                    RabbitMQ.Client.BasicGetResult result = rabbitMQ.BasicGet(queue, false);
                    if (result != null)
                    {
                        try
                        {
                            string messageBody = System.Text.Encoding.UTF8.GetString(result.Body);

                            Services.Downloader.Download messageObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Services.Downloader.Download>(messageBody);

                            var pipeline = appContext.GetObject<DevWeek.Services.Downloader.DownloadPipeline>();

                            var context = new DownloadContext()
                            {
                                MediaUrl = messageObject.OriginalMediaUrl,
                                OutputFileName = $"{Guid.NewGuid().ToString("N")}.mp4",
                                Download = messageObject
                            };

                            await pipeline.Run(context);

                            rabbitMQ.BasicAck(result.DeliveryTag, false);
                        }
                        catch (InvalidOperationException ex) when (ex.Message.Contains("#invalidUrl"))
                        {
                            rabbitMQ.BasicAck(result.DeliveryTag, false);
                        }
                        catch (Exception ex)
                        {
                            rabbitMQ.BasicNack(result.DeliveryTag, false, true);
                        }
                    }
                    else
                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                }

            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed




            Console.WriteLine("Hello World!");
        }





    }
}
