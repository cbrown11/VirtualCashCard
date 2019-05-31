using System;
using System.Threading.Tasks;
using Domain.Events;
using Messages.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace SimpleVirtualATMMachine
{
    public class WithDrawReponseMessageHandler : IHandleMessages<WithDrawReponse>
    {
        static ILog log = LogManager.GetLogger<WithDrawReponseMessageHandler>();


        public WithDrawReponseMessageHandler()
        {
        }

        public Task Handle(WithDrawReponse message, IMessageHandlerContext context)
        {
            Console.WriteLine(message.WithDrawValid
                ? $"{message.Quantity} has been virtual dispensed. Please collect the money"
                : message.Message);

            return Task.CompletedTask;
        }
    }
}
