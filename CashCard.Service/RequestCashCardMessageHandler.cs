using System;
using System.Threading.Tasks;
using DomainBase.Interfaces;
using Messages.Commands;
using NServiceBus;

namespace CashCard.Service
{
    public class RequestCashCardMessageHandler :
        IHandleMessages<RequestCashCard>
    {
        protected IDomainRepository _domainRepository;

        public RequestCashCardMessageHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public Task Handle(RequestCashCard message, IMessageHandlerContext context)
        {;
            var cashCard = Domain.Aggregate.CashCard.RequestCashCard(message.AuditInfo, message.CardNumber, message.ClientId, DateTime.UtcNow);
            _domainRepository.Save(cashCard);
            return Task.CompletedTask;
        }
    }
}
