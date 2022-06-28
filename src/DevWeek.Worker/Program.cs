using Polly;
using System;
using System.Threading.Tasks;

namespace DevWeek;

internal class Program
{
    private static void Main(string[] args)
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


        Task.Run(() =>
        {
            while (true)
            {
                appContext.GetObject<Services.DataService>().RebuildCache();
                System.Threading.Thread.Sleep(TimeSpan.FromMinutes(2));
            }

        });

        AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
        {
            Console.WriteLine("Fim!");
        };

        var statemachine = appContext.GetObject<DevWeek.Architecture.Workflow.QueuedWorkFlow.QueuedStateMachine>("IngestionPipeline");
        statemachine.Start();


        //new CancellationTokenSource()


        while (true)
        {
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        Console.WriteLine("Hello World!");
    }





}
