using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEDPaymentAPI.Models
{
    public class ThankYouModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerPhone { get; set; }
    }
}