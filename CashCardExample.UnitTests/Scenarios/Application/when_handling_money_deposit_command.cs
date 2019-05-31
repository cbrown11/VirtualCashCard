using System;
using CashCard.Service;
using DomainBase.Exception;
using Machine.Specifications;
using Messages.Commands;
using Moq;
using It = Machine.Specifications.It;

namespace CashCardExample.UnitTests.Scenarios.Application
{



    [Subject(typeof(DepositMoney), "Command Handler")]
    public class when_handling_money_deposit_command : HandlerSpec
    {

        protected static DepositMoney DepositMoneyCommand;
        protected static DepositMoneyMessageHandler DepositMoneyMessageHandler;

        private Establish context = () =>
        {
            DepositMoneyMessageHandler = new DepositMoneyMessageHandler(Repository.Object);
            DepositMoneyCommand = new DepositMoney(AuditInfo, CardNumber, 100);
            Repository.Setup(c => c.GetById<global::Domain.Aggregate.CashCard>(CardNumber)).Returns(new global::Domain.Aggregate.CashCard { Id = CardNumber, ClientId = "1"});
            Repository.Setup(c => c.Exists<global::Domain.Aggregate.CashCard>(CardNumber)).Returns(true);
        };
        Because of = () => DepositMoneyMessageHandler.Handle(DepositMoneyCommand, Context);
        It should_have_saved_cash_card = () => Repository.Verify(foo => foo.Save(Moq.It.IsAny<global::Domain.Aggregate.CashCard>(), false), Times.Exactly(1));
        private It should_have_saved_cash_card_with_balance = () => SavedCashCard.Balance.ShouldEqual(100);
    }

    [Subject(typeof(DepositMoney), "Command Handler")]
    public class when_handling_money_deposit_command_where_no_card_exists : HandlerSpec
    {

        protected static DepositMoney DepositMoneyCommand;
        protected static DepositMoneyMessageHandler DepositMoneyMessageHandler;
        protected static Exception exception;

        private Establish context = () =>
        {
            DepositMoneyMessageHandler = new DepositMoneyMessageHandler(Repository.Object);
            DepositMoneyCommand = new DepositMoney(AuditInfo, CardNumber, 100);
            Repository.Setup(c => c.Exists<global::Domain.Aggregate.CashCard>(CardNumber)).Returns(false);
        };
        Because of = () => exception = Catch.Exception(() => DepositMoneyMessageHandler.Handle(DepositMoneyCommand, Context));
        It should_not_allow_deposit = () => exception.ShouldBeOfExactType<AggregateNotFoundException>();
        It should_report_the_reason = () => exception.Message.ShouldEqual($"Cash card {CardNumber} was not found");
        It should_have_not_saved_cash_card = () => Repository.Verify(foo => foo.Save(Moq.It.IsAny<global::Domain.Aggregate.CashCard>(), false), Times.Exactly(0));
     }

}
