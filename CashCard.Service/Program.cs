using System;
using System.IO;
using System.Threading.Tasks;
using CrossCutting.NserviceBus.PubSub;
using CrossCutting.NServiceBus;
using DomainBase.Interfaces;
using DomainBase.Repository;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using StructureMap;


namespace CashCard.Service
{
    static class Program
    {
        private static ITransientDomainEventPublisher _transientDomainEventPublisher;

        static async Task Main()
        {
            var endpoint = "CashCard.Service";
            Console.Title = endpoint;
            var configuration = BuildConfigurationBuilder();
            var logger = LoggerConfiguration.CreateLogger(configuration, endpoint);
            LogManager.Use<SerilogFactory>().WithLogger(logger);
            var container = BuildContainer(configuration);
            IEndpointInstance endpointInstance = await BusEndpointInstance.Learning(endpoint, container);
            endpointInstance = ConfigEndpointInstance(container, endpointInstance);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpointInstance.Stop().ConfigureAwait(false);
           
        }

 
        private static IConfigurationRoot BuildConfigurationBuilder() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

        private static IEndpointInstance ConfigEndpointInstance(IContainer container, IEndpointInstance endpointInstance)
        {
            container.Configure(x => x.For<IEndpointInstance>().Use(endpointInstance).Singleton());
            //using (var nested = container.GetNestedContainer())
            //{
            //    nested.Configure(x =>
            //    {
            //        x.For<ITransientDomainEventPublisher>().Use<TransientDomainEventPublisher>();
            //    });
            //    var r = nested.GetInstance<TransientDomainEventPublisher>();
            //}       
            return endpointInstance;
        }

        private static IContainer BuildContainer(IConfigurationRoot configuration) => new Container(x => {
                x.For<ITransientDomainEventPublisher>().Use<TransientDomainEventPublisher>();
                //TODO: Change to Greg Young Event Stor 
                x.For<IDomainRepository>().Use<InMemEventStoreDomainRespository>().Ctor<string>("catergory").Is("CashCardExmaple").Singleton();
                //
                x.Scan(y =>
                {
                    y.TheCallingAssembly();
                    y.WithDefaultConventions();
                });

 
        }
        );

    }
}