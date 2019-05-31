using System;
using CashCard.Service;
using DomainBase.Exception;
using Machine.Specifications;
using Messages.Commands;
using Messages.Messages;
using Moq;
using It = Machine.Specifications.It;

namespace CashCardExample.UnitTests.Scenarios.Application
{
    [Subject(typeof(WithdrawMoney), "Command Handler")]
    public class when_handling_money_withdraw_command : HandlerSpec
    {
        protected static WithdrawMoney WithdrawMoneyCommand;
        protected static WithdrawMoneyMessageHandler WithdrawMoneyMessageHandler;

        private Establish context = () =>
        {
            WithdrawMoneyMessageHandler = new WithdrawMoneyMessageHandler(Repository.Object);
            WithdrawMoneyCommand = new WithdrawMoney(AuditInfo, CardNumber, 100);
            Repository.Setup(c => c.GetById<global::Domain.Aggregate.CashCard>(CardNumber)).Returns(new global::Domain.Aggregate.CashCard { Id = CardNumber, ClientId = "1", Balance = 200});
            Repository.Setup(c => c.Exists<global::Domain.Aggregate.CashCard>(CardNumber)).Returns(true);
        };
        Because of = () => WithdrawMoneyMessageHandler.Handle(WithdrawMoneyCommand, Context);
        It should_have_saved_cash_card = () => Repository.Verify(foo => foo.Save(Moq.It.IsAny<global::Domain.Aggregate.CashCard>(), false), Times.Exactly(1));
        It should_have_saved_cash_card_with_balance = () => SavedCashCard.Balance.ShouldEqual(100);
        It should_send_valid_withdraw_reponse = () => ((WithDrawReponse)Context.RepliedMessages[0].Message).WithDrawValid.ShouldBeTrue();
    }

    public class when_handling_money_withdraw_command_that_has_zero_balance : HandlerSpec
    {
        protected static WithdrawMoney WithdrawMoneyCommand;
        protected static WithdrawMoneyMessageHandler WithdrawMoneyMessageHandler;

        private Establish context = () =>
        {
            WithdrawMoneyMessageHandler = new WithdrawMoneyMessageHandler(Repository.Object);
            WithdrawMoneyCommand = new WithdrawMoney(AuditInfo, CardNumber, 100);
            Repository.Setup(c => c.GetById<Domain.Aggregate.CashCard>(CardNumber)).Returns(new global::Domain.Aggregate.CashCard { Id = CardNumber, ClientId = "1", Balance = 0 });
            Repository.Setup(c => c.Exists<Domain.Aggregate.CashCard>(CardNumber)).Returns(true);
        };
        Because of = () => WithdrawMoneyMessageHandler.Handle(WithdrawMoneyCommand, Context);
        It should_have_not_saved_cash_card_changes = () => Repository.Verify(foo => foo.Save(Moq.It.IsAny<global::Domain.Aggregate.CashCard>(), false), Times.Exactly(0));
        It should_send_validwith_draw_reponse = () => ((WithDrawReponse)Context.RepliedMessages[0].Message).WithDrawValid.ShouldBeFalse();
        It should_send_validwith_draw_reponse_with_the_reason = () => ((WithDrawReponse)Context.RepliedMessages[0].Message).Message.ShouldEqual("Can not withdraw 100 as insufficent funds. Balance on the card is 0.");

    }


    [Subject(typeof(WithdrawMoney), "Command Handler")]
    public class when_handling_money_withdraw_command_where_no_card_exists : HandlerSpec
    {
        protected static WithdrawMoney WithdrawMoneyCommand;
        protected static WithdrawMoneyMessageHandler WithdrawMoneyMessageHandler;
        protected static Exception exception;

        private Establish context = () =>
        {
            WithdrawMoneyMessageHandler = new WithdrawMoneyMessageHandler(Repository.Object);
            WithdrawMoneyCommand = new WithdrawMoney(AuditInfo, CardNumber, 100);
            Repository.Setup(c => c.Exists<global::Domain.Aggregate.CashCard>(CardNumber)).Returns(false);
        };
        Because of = () => exception = Catch.Exception(() => WithdrawMoneyMessageHandler.Handle(WithdrawMoneyCommand, Context));
        It should_not_allow_deposit = () => exception.ShouldBeOfExactType<AggregateNotFoundException>();
        It should_report_the_reason = () => exception.Message.ShouldEqual($"Cash card {CardNumber} was not found");
        It should_have_not_saved_cash_card = () => Repository.Verify(foo => foo.Save(Moq.It.IsAny<global::Domain.Aggregate.CashCard>(), false), Times.Exactly(0));
     }

}
