using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CEDPaymentAPI.Models
{
    public class StripeRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Phone { get; set; }
        public string Payment_Method_Id { get; set; }
        public decimal Amount { get; set; }
        public string StripeId { get; set; }

    }
    public class StripeResponse
    {
        public bool requires_action { get; set; }
        public string payment_intent_client_secret { get; set; }
        public bool success { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class StripeKeys
    {
        public string PublicKeys { get; set; }
    }
}