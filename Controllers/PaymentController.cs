using CEDPaymentAPI.Models;
using Newtonsoft.Json;
using RestSharp;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace CEDPaymentAPI.Controllers
{
    public class PaymentController : ApiController
    {
        public StripeKeys GetPublicKey()
        {
            StripeKeys keyModel = new StripeKeys();
            bool isSandBoxMode = false;
            isSandBoxMode = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSandboxMode"]);
            string stripePublicKey = "";
            if (isSandBoxMode)
            {
                stripePublicKey = ConfigurationManager.AppSettings["SandboxStripePublicKey"];
            }
            else
            {
                stripePublicKey = ConfigurationManager.AppSettings["StripePublicKey"];
            }
            keyModel.PublicKeys = stripePublicKey;
            return keyModel;
        }
        [HttpPost]
        public IHttpActionResult CreateStripeTransaction([FromBody] Models.StripeRequest model)
        {
            Models.StripeResponse resposne = new Models.StripeResponse();
            string customerName = model.FirstName + " " + model.LastName;
            PaymentIntent paymentIntent = null;
            ConfirmPaymentRequest request = new ConfirmPaymentRequest();
            request.PaymentMethodId = model.Payment_Method_Id;
            if (string.IsNullOrEmpty(model.Payment_Method_Id))
            {
                return Json(new { error = "Payment_Method_Id is not provided!" });
            }
            else if (string.IsNullOrEmpty(customerName))
            {
                return Json(new { error = "Provide customer name!" });
            }
            else if (model.Amount <= 0)
            {
                return Json(new { error = "Amount should be greater than 0!" });
            }
            int chargeAmount = 0;
            string currency = "usd", description = "";
            string connectAccountId = "";
            string stripeSecretKey = "";

            bool isSandBoxMode = false;
            isSandBoxMode = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSandboxMode"]);
            if (isSandBoxMode)
            {
                stripeSecretKey = ConfigurationManager.AppSettings["SandboxStripeSecretKey"];
                connectAccountId = ConfigurationManager.AppSettings["SandboxConnectAccountId"];
            }
            else
            {
                stripeSecretKey = ConfigurationManager.AppSettings["StripeSecretKey"];
                connectAccountId = ConfigurationManager.AppSettings["ConnectAccountId"];
            }
            currency = ConfigurationManager.AppSettings["StripeCurrency"];
            StripeConfiguration.ApiKey = stripeSecretKey;
            string stripeCustomerId = "";
            stripeCustomerId = CreateCustomer(model);
            if (stripeCustomerId == "-1")
            {
                return Json(new { error = "Customer is not created in Stripe!" });
            }
            try
            {
                if (model.Amount > 0)
                {
                    description = "Donation -- $" + Convert.ToString(model.Amount / 100) + " -- " + customerName + " -- " + model.Email;
                }
            }catch(Exception xxxxxx)
            {
                description = "Donation -- $ -- " + customerName + " -- " + model.Email;
            }

            //   chargeAmount = Convert.ToInt32(model.Amount * 100);
            chargeAmount = Convert.ToInt32(model.Amount);
            if (string.IsNullOrEmpty(connectAccountId))
            {
                return Json(new { error = "No Connect Account User Credentials exist!" });
            }
            string fullName = customerName;
            if (fullName.Length >= 18)
                fullName = fullName.Substring(0, 18);
            var paymentIntentService = new PaymentIntentService();
            try
            {
                if (request.PaymentMethodId != null)
                {
                    // Create the PaymentIntent
                    var createOptions = new PaymentIntentCreateOptions
                    {
                        PaymentMethod = request.PaymentMethodId,
                        Amount = chargeAmount,
                        Description = description,
                        Metadata = new Dictionary<string, string>
                                {
                          { "customer", description },
                                },
                        Currency = currency,
                        ConfirmationMethod = "manual",
                        PaymentMethodTypes = new List<string> { "card" },
                        Customer = stripeCustomerId,
                        StatementDescriptorSuffix = fullName,
                        TransferData = new PaymentIntentTransferDataOptions
                        {
                            //Amount = fee,
                            Destination = connectAccountId,
                        },
                        Confirm = true,
                    };
                    paymentIntent = paymentIntentService.Create(createOptions);
                }
                else
                {
                    return Json(new { error = "Payment_Method_Id is not provided!" });
                }

            }
            catch (StripeException e)
            {
                //resposne.ErrorCode = "0";
                //resposne.ErrorMessage = e.StripeError.Message;
                //return resposne;
                return Json(new { error = e.StripeError.Message });
            }

            return generatePaymentResponse(paymentIntent);
        }
        private IHttpActionResult generatePaymentResponse(PaymentIntent intent)
        {
            // Note that if your API version is before 2019-02-11, 'requires_action'
            // appears as 'requires_source_action'.
            if (intent.Status == "requires_action" &&
                intent.NextAction.Type == "use_stripe_sdk")
            {
                // Tell the client to handle the action
                return Json(new
                {
                    requires_action = true,
                    payment_intent_client_secret = intent.ClientSecret
                });
            }
            else if (intent.Status == "succeeded")
            {
                // The payment didn’t need any additional actions and completed!
                // Handle post-payment fulfillment
                //TempData["ChargeId"] = paymentIntent.Id;
                return Json(new { success = true });
            }
            else
            {
                // Invalid status
                return Json(new { success = false });
                //return StatusCode(500, new { error = "Invalid PaymentIntent status" });
            }
        }

        public IHttpActionResult ConfirmPayment(String payment_intent_id)
        {

            ConfirmPaymentRequest request = new ConfirmPaymentRequest();
            request.PaymentIntentId = payment_intent_id;

            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent = null;
            try
            {
                if (request.PaymentIntentId != null)
                {
                    var confirmOptions = new PaymentIntentConfirmOptions { };
                    paymentIntent = paymentIntentService.Confirm(
                        request.PaymentIntentId,
                        confirmOptions
                    );

                }
            }
            catch (StripeException e)
            {
                return Json(new { error = e.StripeError.Message });
            }
            return generatePaymentResponse(paymentIntent);

        }

        [NonAction]
        private string CreateCustomer(Models.StripeRequest model)
        {
            string stripeUserId = string.Empty;
            try
            {
                var customerservice = new CustomerService();
                // To check customer exist or not

                var options = new CustomerListOptions
                {
                    Limit = 1,
                    Email = model.Email,
                };
                StripeList<Customer> customers = customerservice.List(options);
                if (customers != null && customers.Data.Count > 0)
                {
                    stripeUserId = customers.Data[0].Id;
                    //Customer customer = customers.ge
                }
                else
                {
                    var customerOption = new CustomerCreateOptions();
                    customerOption.Email = model.Email;
                    customerOption.Name = model.FirstName + " " + model.LastName;
                    customerOption.Description = model.FirstName + " " + model.LastName + " (" + model.Email + ")";
                    customerOption.Metadata = new Dictionary<string, string>
                    {
                        {"customer",model.FirstName+ " " + model.LastName },
                    };
                    //
                    Customer customer = customerservice.Create(customerOption);
                    stripeUserId = customer.Id;
                }
            }
            catch (Exception ex)
            {
                stripeUserId = "-1";
            }
            return stripeUserId;
        }


    }

    public class ConfirmPaymentRequest
    {
        [JsonProperty("payment_method_id")]
        public string PaymentMethodId { get; set; }

        [JsonProperty("payment_intent_id")]
        public string PaymentIntentId { get; set; }
    }
}
