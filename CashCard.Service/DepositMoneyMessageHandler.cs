using System;
using System.Threading.Tasks;
using DomainBase.Exception;
using DomainBase.Interfaces;
using Messages.Commands;
using NServiceBus;
using NServiceBus.Logging;

namespace CashCard.Service
{
    public class DepositMoneyMessageHandler : IHandleMessages<DepositMoney>
    {
        protected IDomainRepository _domainRepository;

        static ILog log = LogManager.GetLogger<DepositMoneyMessageHandler>();

        public DepositMoneyMessageHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public Task Handle(DepositMoney message, IMessageHandlerContext context)
        {

            if (!_domainRepository.Exists<Domain.Aggregate.CashCard>(message.CardNumber))
                throw new AggregateNotFoundException($"Cash card {message.CardNumber} was not found");
            var cashCard = _domainRepository.GetById<Domain.Aggregate.CashCard>(message.CardNumber);
            cashCard.Deposit(message.AuditInfo,message.Quantity,DateTime.UtcNow);
            _domainRepository.Save(cashCard);
            log.Info($"card {cashCard.Id} balance is now {cashCard.Balance}");
            return Task.CompletedTask;
        }
    }
}
