using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEDPaymentAPI.Models
{
    // ActiveCampaignModel myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ActiveCampaignModel
    {
        public Contact contact { get; set; }
    }

    public class Links
    {
        public string bounceLogs { get; set; }
        public string contactAutomations { get; set; }
        public string contactData { get; set; }
        public string contactGoals { get; set; }
        public string contactLists { get; set; }
        public string contactLogs { get; set; }
        public string contactTags { get; set; }
        public string contactDeals { get; set; }
        public string deals { get; set; }
        public string fieldValues { get; set; }
        public string geoIps { get; set; }
        public string notes { get; set; }
        public string organization { get; set; }
        public string plusAppend { get; set; }
        public string trackingLogs { get; set; }
        public string scoreValues { get; set; }
        public string accountContacts { get; set; }
        public string automationEntryCounts { get; set; }
    }

    public class Contact
    {
        public string email { get; set; }
        public string phone { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool email_empty { get; set; }
        public DateTime cdate { get; set; }
        public DateTime udate { get; set; }
        public string orgid { get; set; }
        public string orgname { get; set; }
        public Links links { get; set; }
        public string hash { get; set; }
        public string id { get; set; }
        public string organization { get; set; }
    }

 

}