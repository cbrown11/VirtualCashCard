using System.Collections.Generic;
using CreditCardValidator;
using Microsoft.AspNetCore.Mvc;


namespace CashCardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashCardController : ControllerBase
    {
        public CashCardController()
        {
          
        }

        // POST api/cashcards
        [HttpPost]
        public ActionResult<string> Post([FromBody] string customerId)
        {
            var cardNumber = CreditCardFactory.RandomCardNumber(CardIssuer.MasterCard);
            //var card = CashCardDomain.CashCard.CreateCashCard()

            return cardNumber;
        }
    }
}
