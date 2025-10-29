using Microsoft.Extensions.Configuration;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System.Threading.Tasks;

namespace LowYurt.Services
{
    public class PayPalService
    {
        private readonly IConfiguration _configuration;

        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private PayPalEnvironment GetEnvironment()
        {
            var mode = _configuration["PayPal:Mode"];
            var clientId = _configuration["PayPal:ClientId"];
            var clientSecret = _configuration["PayPal:ClientSecret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                throw new Exception("❌ PayPal ClientId o ClientSecret no configurados correctamente.");

            return mode == "live"
                ? new LiveEnvironment(clientId, clientSecret)
                : new SandboxEnvironment(clientId, clientSecret);
        }

        private PayPalHttpClient GetClient() => new PayPalHttpClient(GetEnvironment());

        public async Task<string> CreateOrderAsync(decimal total, string returnUrl, string cancelUrl)
        {
            var orderRequest = new OrderRequest()
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "USD",
                            Value = total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                        }
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            };

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(orderRequest);

            var response = await GetClient().Execute(request);
            var result = response.Result<Order>();

            return result.Links.First(x => x.Rel == "approve").Href;
        }

        public async Task CaptureOrderAsync(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());
            await GetClient().Execute(request);
        }
    }
}
