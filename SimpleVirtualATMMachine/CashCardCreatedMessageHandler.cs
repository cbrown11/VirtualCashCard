using System;
using System.Threading.Tasks;
using Domain.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace SimpleVirtualATMMachine
{
    public class CashCardCreatedMessageHandler : IHandleMessages<CashCardCreated>
    {
        static ILog log = LogManager.GetLogger<CashCardCreatedMessageHandler>();

        protected ICurrentCardNumber _currentCardNumber;

        public CashCardCreatedMessageHandler(ICurrentCardNumber currentCardNumber)
        {
            _currentCardNumber = currentCardNumber;
        }

        public Task Handle(CashCardCreated message, IMessageHandlerContext context)
        {
            var response = $"cash card {message.Id} has been sent out in the virtual post";
            log.Info(response);
            Console.WriteLine(response);
            _currentCardNumber.Number = message.Id;
            return Task.CompletedTask;
        }
    }
}
