using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.IServices;

namespace Talabat.APIs.Controllers
{
    public class PaymentsController : BaseAPIController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        private const string endpointSecret = "whsec_217760af72866773a4d5ccbf9f191ee4fa4f723640c265482da5c32199bdca5b";

        public PaymentsController(IPaymentService paymentService,
            ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }
        [Authorize]
        [ProducesResponseType(typeof(CustomerBasket), 200)]
        [ProducesResponseType(typeof(APIsResponse), 400)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket is null)
                return BadRequest(new APIsResponse(400, "An error with your basket"));
            return Ok(basket);
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Event stripeEvent;
            var signatureHeader = Request.Headers["Stripe-Signature"];
            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            Order order;
            switch (stripeEvent.Type)
            {
                case "payment.intent.succeeded":
                    order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, true);
                    _logger.LogInformation("Payment is succeeded", paymentIntent.Id);
                    break;
                case "payment.intent.payment.failed":
                    order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, false);
                    _logger.LogInformation("Payment is failed", paymentIntent.Id);
                    break;
            }
            return Ok();


        }
    }
}
