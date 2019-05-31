using CashCard.Service;
using Machine.Specifications;
using Messages.Commands;
using Moq;
using It = Machine.Specifications.It;

namespace CashCardExample.UnitTests.Scenarios.Application
{
    [Subject(typeof(RequestCashCard), "Command Handler")]
    public class when_handling_request_cash_card_command: HandlerSpec
    {
        protected static RequestCashCard RequestCashCardCommand;
        protected static RequestCashCardMessageHandler RequestCashCardMessageHandler;

        private Establish context = () =>
        {
            RequestCashCardMessageHandler = new RequestCashCardMessageHandler(Repository.Object);
            RequestCashCardCommand = new RequestCashCard(AuditInfo, CardNumber, "1");
        };
        Because of = () => RequestCashCardMessageHandler.Handle(RequestCashCardCommand, Context);
        It should_have_saved_cash_card = () => Repository.Verify(foo => foo.Save(Moq.It.IsAny<global::Domain.Aggregate.CashCard>(), false), Times.Exactly(1));
        private It should_have_saved_cash_card_with_no_balance = () => SavedCashCard.Balance.ShouldEqual(0);
    }
}
