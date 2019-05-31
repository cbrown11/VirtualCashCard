using System;
using System.Threading.Tasks;
using Domain.Exception;
using DomainBase.Exception;
using DomainBase.Interfaces;
using Messages.Commands;
using Messages.Messages;
using NServiceBus;
using NServiceBus.Logging;

namespace CashCard.Service
{
    public class WithdrawMoneyMessageHandler: IHandleMessages<WithdrawMoney>
    {
        protected IDomainRepository _domainRepository;
        static ILog log = LogManager.GetLogger<DepositMoneyMessageHandler>();

        public WithdrawMoneyMessageHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public Task Handle(WithdrawMoney message, IMessageHandlerContext context)
        {
            var replyMessage = new WithDrawReponse(message.AuditInfo, message.CardNumber, message.Quantity, true); ;
            try
            {
                if (!_domainRepository.Exists<Domain.Aggregate.CashCard>(message.CardNumber))
                    throw new AggregateNotFoundException($"Cash card {message.CardNumber} was not found");
                var cashCard = _domainRepository.GetById<Domain.Aggregate.CashCard>(message.CardNumber);
                cashCard.Withdraw(message.AuditInfo, message.Quantity, DateTime.UtcNow);
                _domainRepository.Save(cashCard);
                log.Info($"card {cashCard.Id} balance is now {cashCard.Balance}");
                return context.Reply(replyMessage);
            }
            catch (WithDrawException ex)
            {
                replyMessage.Message = ex.Message;
                replyMessage.WithDrawValid = false;
                return context.Reply(replyMessage);
            }
     
        }
    }
}
