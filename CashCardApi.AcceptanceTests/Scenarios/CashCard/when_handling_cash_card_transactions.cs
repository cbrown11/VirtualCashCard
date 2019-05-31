
using System;
using CashCard.Service;
using DomainBase;
using DomainBase.Interfaces;
using DomainBase.Repository;
using Machine.Specifications;
using Messages.Commands;
using NServiceBus.Testing;
using It = Machine.Specifications.It;

namespace CashCardExample.AcceptanceTests.Scenarios.CashCard
{

    public abstract class CashCardTransactionSpec
    {
        protected static IDomainRepository Repository;
        protected static DateTime DefaultDate = DateTime.UtcNow;
        protected static TestableMessageHandlerContext Context;
        protected static string CardNumber;
        protected static RequestCashCardMessageHandler RequestCashCardMessageHandler;
        protected static DepositMoneyMessageHandler DepositMoneyMessageHandler;
        protected static WithdrawMoneyMessageHandler WithdrawMoneyMessageHandler;

        protected static AuditInfo AuditInfo = new AuditInfo
        {
            Created = DefaultDate,
            By = "userName"
        };

        protected Establish context = () =>
        {
            CardNumber = "1111111111111111";
            Context = new TestableMessageHandlerContext();
            Repository = new InMemEventStoreDomainRespository("creditcard");
            RequestCashCardMessageHandler = new RequestCashCardMessageHandler(Repository);
            DepositMoneyMessageHandler = new DepositMoneyMessageHandler(Repository);
            WithdrawMoneyMessageHandler = new WithdrawMoneyMessageHandler(Repository);
        };
        private Cleanup after = () => { };
    }

    [Subject(typeof(DepositMoney), "Command Handler")]
    public class when_handling_cash_card_transactions : CashCardTransactionSpec
    {
        protected static Domain.Aggregate.CashCard CashCard;
        private Establish context = () =>
        {
            RequestCashCardMessageHandler.Handle(new RequestCashCard(AuditInfo, CardNumber, "1"), Context);
        };
        private Because of = () =>
        {
            DepositMoneyMessageHandler.Handle(new DepositMoney(AuditInfo, CardNumber, 250), Context);
            WithdrawMoneyMessageHandler.Handle(new WithdrawMoney(AuditInfo, CardNumber, 100), Context);
            DepositMoneyMessageHandler.Handle(new DepositMoney(AuditInfo, CardNumber, 300.40), Context);
            CashCard = Repository.GetById<Domain.Aggregate.CashCard>(CardNumber);
        };

        It should_have_balance = () => CashCard.Balance.ShouldEqual(450.40);
    }
}
