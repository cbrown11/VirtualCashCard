using System;
using DomainBase;
using DomainBase.Interfaces;
using Machine.Specifications;
using Moq;
using NServiceBus.Testing;

namespace CashCardExample.UnitTests.Scenarios.Application
{
    public abstract class HandlerSpec
    {
        protected static Mock<IDomainRepository> Repository;
        protected static DateTime DefaultDate = DateTime.UtcNow;
        protected static TestableMessageHandlerContext Context;
        protected static global::Domain.Aggregate.CashCard SavedCashCard;
        protected static string CardNumber;

   
        protected static AuditInfo AuditInfo = new AuditInfo
        {
            Created = DefaultDate,
            By = "userName"
        };

        protected Establish context = () =>
        {
            CardNumber = "1111111111111111";
            Context = new TestableMessageHandlerContext();
            Repository = new Mock<IDomainRepository>();
            Repository.Setup(c => c.Save(Moq.It.IsAny<global::Domain.Aggregate.CashCard>(), Moq.It.IsAny<bool>())).Callback<global::Domain.Aggregate.CashCard, bool>((obj, isInitial)
                => SavedCashCard =obj
            );

        };
        private Cleanup after = () => { };
    }

}