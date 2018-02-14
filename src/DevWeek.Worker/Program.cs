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

            Task.Run(async () => {
                while(true)
                {
                    await appContext.GetObject<Services.DataService>().RebuildCache();
                    System.Threading.Thread.Sleep(TimeSpan.FromMinutes(2));
                }
                
            });

            appContext.GetObject<DevWeek.Architecture.Workflow.QueuedWorkFlow.QueuedStateMachine>("IngestionPipeline").Start();


            Console.WriteLine("Hello World!");
        }





    }
}
