using Microsoft.Extensions.Configuration;
using Spring.Context.Support;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevWeek
{
    public class ContextBuilder
    {
        
        public static AbstractApplicationContext BuildContext()
        {
            var configurationRoot = BuildConfigurationRoot();
            var configContext = CreateConfigContext(configurationRoot);
            var appContext = CreateAppContext(configContext);
            return appContext;
        }


        #region Support Methods

        private static IConfigurationRoot BuildConfigurationRoot()
        {
            var ASPNETCORE_ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
             .SetBasePath(System.IO.Directory.GetCurrentDirectory())
             .AddEnvironmentVariables()
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .AddJsonFile($"appsettings.{ASPNETCORE_ENVIRONMENT}.json", optional: true)
             .AddEnvironmentVariables();
            var configuration = builder.Build();
            return configuration;
        }

        private static AbstractApplicationContext CreateConfigContext(IConfigurationRoot configurationRoot)
        {
            StaticApplicationContext configContext = new StaticApplicationContext();
            foreach (var currentConfiguration in configurationRoot.AsEnumerable())
            {
                Spring.Objects.Factory.Support.GenericObjectDefinition objectDefinition = new Spring.Objects.Factory.Support.GenericObjectDefinition
                {
                    ObjectType = typeof(String),
                    FactoryMethodName = "Copy"
                };
                objectDefinition.ConstructorArgumentValues.AddIndexedArgumentValue(0, currentConfiguration.Value);
                //System.Diagnostics.Debug.WriteLine($"{currentConfiguration.Key} : {currentConfiguration.Value}");
                configContext.RegisterObjectDefinition($"CONFIG:{currentConfiguration.Key}", objectDefinition);
            }
            return configContext;
        }

        private static AbstractApplicationContext CreateAppContext(AbstractApplicationContext configurationContext)
        {
            var containerArgs = new XmlApplicationContextArgs()
            {
                CaseSensitive = true,
                Name = "root",
                ConfigurationLocations = new[] { "assembly://DevWeek.Worker/DevWeek/Container.Config.xml" },
                ParentContext = configurationContext
            };
            var appContext = new Spring.Context.Support.XmlApplicationContext(containerArgs);
            return appContext;
        }

        #endregion
    }
}
