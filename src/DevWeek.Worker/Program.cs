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
using System.Security.Policy;

namespace DevWeek
{
    class Program
    {
        static void Main(string[] args)
        {
            Oragon.Spring.Context.Support.AbstractApplicationContext appContext = null;

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

          
            Task.Run(() =>
            {
                while (true)
                {
                    appContext.GetObject<Services.DataService>().RebuildCache();
                    System.Threading.Thread.Sleep(TimeSpan.FromMinutes(2));
                }

            });

            appContext.GetObject<DevWeek.Architecture.Workflow.QueuedWorkFlow.QueuedStateMachine>("IngestionPipeline").Start();


            Console.WriteLine("Hello World!");
        }





    }
}
