using System;
using System.IO;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;
using CreditCardValidator;
using CrossCutting.NServiceBus;
using DomainBase;
using Messages.Commands;
using Microsoft.Extensions.Configuration;
using NServiceBus.Serilog;
using StructureMap;


// Quick Emulator Screen 
namespace SimpleVirtualATMMachine
{
    class Program
    {
        private static ICurrentCardNumber _currentCardNumber;

        static async Task Main()
        {
            var endpoint = "SimpleVirtualATMMachine.Client";
            Console.Title = endpoint;
            var configuration = BuildConfigurationBuilder();
            var logger = LoggerConfiguration.CreateLogger(configuration, endpoint);
            LogManager.Use<SerilogFactory>().WithLogger(logger);
            var container = BuildContainer();
            _currentCardNumber = container.GetInstance<ICurrentCardNumber>();
            IEndpointInstance endpointInstance = await BusEndpointInstance.Learning(endpoint, container); ;
            OptionScreen1(endpointInstance);
            await endpointInstance.Stop().ConfigureAwait(false);
        }

        private static async void OptionScreen1(IEndpointInstance endpointInstance)
        {
            WriteChooseOption();
            Console.WriteLine("1.Request a virtual cash card");
            Console.WriteLine("2.Insert a virtual cash card");
            Console.WriteLine("Press Esc to exit");
            Console.WriteLine();
            while (true)
            {
                var auditInfo = new AuditInfo { By = "login User", Created = DateTime.UtcNow };
                var key = Console.ReadKey();
                Console.WriteLine();
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        break;
                    case ConsoleKey.D1:
                        _currentCardNumber.Number = String.Empty;
                        var message = new RequestCashCard(auditInfo,CreditCardFactory.RandomCardNumber(CardIssuer.MasterCard), "1");
                        Console.WriteLine($"card has been requested and will virtual be posted");
                        await endpointInstance.Send("CashCard.Service", message).ConfigureAwait(false);
                        Console.WriteLine($"waiting for card ...");
                        while (true)
                        {
                            if (_currentCardNumber.Number != message.CardNumber) continue;
                            Console.WriteLine($"received cash card {message.CardNumber} and inserting ...");
                            break;
                        }
                        InsertCard(endpointInstance, _currentCardNumber.Number);
                        break;
                    case ConsoleKey.D2:
                        //read Card
                        if (String.IsNullOrEmpty(_currentCardNumber.Number))
                        {
                            Console.WriteLine("Enter the card number");
                            _currentCardNumber.Number = Console.ReadLine();
                        }
                        InsertCard(endpointInstance,_currentCardNumber.Number);
                        break;
                    default:
                        Console.WriteLine("Please select correct option");
                        break;
                }
            }
        }

        private static async void InsertCard(IEndpointInstance endpointInstance,string cardNumber)
        {
            Console.WriteLine();
            Console.WriteLine($"Virtual cash card {cardNumber} has been inserted");
            Console.WriteLine();
            //read PIN
            Console.WriteLine("Enter the pin");
            var pin = int.Parse(Console.ReadLine());
            //TODO: validate cashcard and pin return token - Identity Server or something similar
            OptionScreen2(endpointInstance, cardNumber);
        }

        private static async void WriteOptionScreen2Options()
        {
            WriteChooseOption();
            Console.WriteLine("1.To withdraw money");
            Console.WriteLine("2.To deposite Money");
            Console.WriteLine("Press Esc to exit");
            Console.WriteLine();
        }

        private static async void OptionScreen2(IEndpointInstance endpointInstance, string cardNumber)
        {
            WriteOptionScreen2Options();

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();
                var auditInfo = new AuditInfo { By = cardNumber, Created = DateTime.UtcNow };
                double quantity;
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        break;
                    case ConsoleKey.D1:
                        Console.WriteLine("Enter the amount to withdraw");
                        {
                            quantity = double.Parse(Console.ReadLine());
                            var message = new WithdrawMoney(auditInfo, cardNumber, quantity);
                            await endpointInstance.Send("CashCard.Service", message).ConfigureAwait(false);
                            WriteOptionScreen2Options();
                        }
                        break;
                    case ConsoleKey.D2:
                        Console.WriteLine("Enter the amount to deposit");
                        {
                            quantity = double.Parse(Console.ReadLine());
                            var message = new DepositMoney(auditInfo, cardNumber, quantity);
                            await endpointInstance.Send("CashCard.Service", message).ConfigureAwait(false);
                            Console.WriteLine($"Checked and {quantity} has been deposited successfully..");
                            WriteOptionScreen2Options();
                        }
                        break;
                    default:
                        Console.WriteLine("Please select correct option");
                        break;
                }
            }
        }

        private static void WriteChooseOption()
        {
            Console.WriteLine();
            Console.WriteLine("*** PLEASE CHOOSE AN OPTION ***");
            Console.WriteLine();
        }

        private static IConfigurationRoot BuildConfigurationBuilder() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

        private static IContainer BuildContainer() => new Container(x => {
                x.For<ICurrentCardNumber>().Use<CurrentCardNumber>().Singleton();
            }
        );
    }
}