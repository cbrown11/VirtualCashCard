using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using CreditCardApi;
using Machine.Specifications;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace CashCardExample.AcceptanceTests
{
    public abstract class CreditCardSpec
    {
        protected static HttpClient _httpClient;
        protected static HttpResponseMessage _response;
        private Establish context = () =>
        {
            var webhost = new WebHostBuilder()
                .UseUrls("http://*:8000")
                .UseStartup<Startup>();
            var server = new TestServer(webhost);
            _httpClient = server.CreateClient();
        };
        private Cleanup after = () => { };
    }


///[Subject(typeof(CreditCardApi))]
    public class when_request_cash_card_api : CreditCardSpec
    {
        private Because of = () =>
        {
       
            _response = _httpClient.PostAsJsonAsync("api/cashcard", 0).Result;
        };
        It should_return_successful_status = () => _response.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Ignore("Start todo a rest service")]
    public class when_anonymous_user_creates_cash_card : CreditCardSpec
    {
        private Because of = () =>
        {
            _response = _httpClient.PostAsJsonAsync("api/cashcard", 0).Result;
        };
        It should_return_unauthorized_status = () => _response.StatusCode.ShouldEqual(HttpStatusCode.Unauthorized);
    }


    public class when_authorise_user_creates_cash_card : CreditCardSpec
    {
        private static string result;

        private Because of = () =>
        {
            _response = _httpClient.PostAsJsonAsync("api/cashcard", 0).Result;
            result = _response.Content.ReadAsStringAsync().Result;
        };
        It should_return_card_number = () => result.ShouldNotBeEmpty();
        It and_should_return_card_number_in_the_format = () => Regex.IsMatch(result, @"^\d{16}?$").ShouldBeTrue();
        It and_not_return_default_number = () => result.ShouldNotEqual("0000000000000000");
    }

}