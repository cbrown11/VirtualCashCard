using System;
using Domain.Exception;
using DomainBase;
using Machine.Specifications;

namespace CashCardExample.UnitTests.Scenarios.Domains
{
    public abstract class CashCardSpec
    {
        protected static Domain.Aggregate.CashCard SUT;
        protected static DateTime DefaultDate = DateTime.UtcNow;
        protected static AuditInfo AuditInfo = new AuditInfo
        {
            Created = DefaultDate,
            By = "userName"
        };

        Establish context = () =>
        {
            SUT = new global::Domain.Aggregate.CashCard(AuditInfo, "1111111111111111", "1", DefaultDate);
        };
    }

    [Subject(typeof(Domain.Aggregate.CashCard))]
    public class when_request_cash_card : CashCardSpec
    {
        Because of = () => { };
        It should_create_a_card = () => SUT.Id.ShouldEqual("1111111111111111");
        It should_set_client = () => SUT.ClientId.ShouldEqual("1");
        It should_start_with_zero_balance = () => SUT.Balance.ShouldEqual(0);
    }


    [Subject(typeof(Domain.Aggregate.CashCard))]
    public class when_depositing_on_empty_balance_cash_card : CashCardSpec
    {
        Because of = () => { SUT.Deposit(AuditInfo,100.12, DefaultDate); };
        It should_have_balance = () => SUT.Balance.ShouldEqual(100.12);
    }

    [Subject(typeof(Domain.Aggregate.CashCard))]
    public class when_depositing_on_non_empty_balance_cash_card : CashCardSpec
    {
        Because of = () =>
        {
            SUT.Deposit(AuditInfo, 100.12, DefaultDate);
            SUT.Deposit(AuditInfo, 100, DefaultDate);
        };
        It should_have_balance = () => SUT.Balance.ShouldEqual(200.12);
    }

    [Subject(typeof(Domain.Aggregate.CashCard))]
    public class when_withdrawing_on_empty_balance_cash_card : CashCardSpec
    {
        protected static Exception exception;

        Because of = () => exception = Catch.Exception(() => SUT.Withdraw(AuditInfo, 100, DefaultDate));
        It should_not_allow_withdrawal = () => exception.ShouldBeOfExactType<WithDrawException>();
        It should_report_the_reason = () => exception.Message.ShouldEqual("Can not withdraw 100 as insufficent funds. Balance on the card is 0.");
        It should_balance_should_not_have_changed = () => SUT.Balance.ShouldEqual(0);
    }
}
