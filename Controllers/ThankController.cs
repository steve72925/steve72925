using CEDPaymentAPI.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CEDPaymentAPI.Controllers
{
    public class ThankController : ApiController
    {

        [HttpPost]
        public IHttpActionResult ThankYou([FromBody] Models.ThankYouModel thankyou)
        {
            int result = SendContactToActiveCampaign(thankyou);
            return Json(new { error = result.ToString() });
        }
        private int SendContactToActiveCampaign(Models.ThankYouModel thankyouModel)
        {
            int result = 0;
            string activeCampaignAddContact = ConfigurationManager.AppSettings["ActiveCampaignAddContact"];
            string apiToken = ConfigurationManager.AppSettings["ApiToken"];
            string cookie = ConfigurationManager.AppSettings["Cookie"];
            var client = new RestClient(activeCampaignAddContact);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Api-Token", apiToken);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Cookie", cookie);
            var body = @"{" +
            @"         ""contact"": {" +
            @"         ""email"": ""[CONTACT_EMAIL]""," +
            @"         ""firstName"": ""[CONTACT_FIRSTNAME]""," +
            @"         ""lastName"": ""[CONTACT_LASTNAME]""," +
            @"         ""phone"": ""[CONTACT_PHONE]"" " +
            @"    }" +
            @"}";

            body = body.Replace("[CONTACT_EMAIL]", thankyouModel.CustomerEmail);
            body = body.Replace("[CONTACT_FIRSTNAME]", thankyouModel.FirstName);
            body = body.Replace("[CONTACT_LASTNAME]", thankyouModel.LastName);
            body = body.Replace("[CONTACT_PHONE]", thankyouModel.CustomerPhone);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            if (response != null && response.Content != "")
            {
                result = AddContactInDeal(response.Content);
            }
            else
            {
                result = -2;
            }
            return result;
        }
        private int AddContactInDeal(string jsonString, string amount = "")
        {
            int result = 0;
            ActiveCampaignModel myDeserializedClass = JsonConvert.DeserializeObject<ActiveCampaignModel>(jsonString);
            string contactId = myDeserializedClass.contact.id;
            string activeCampaignDeal = ConfigurationManager.AppSettings["ActiveCampaignDeal"];
            string apiToken = ConfigurationManager.AppSettings["ApiToken"];
            string cookie = ConfigurationManager.AppSettings["Cookie"];
            string groupId = ConfigurationManager.AppSettings["GroupId"];
            string accountId = ConfigurationManager.AppSettings["AccountId"];
            string ownerId = ConfigurationManager.AppSettings["OwnerId"];
            string stageId = ConfigurationManager.AppSettings["StageId"];
            string dealName = ConfigurationManager.AppSettings["DealName"];

            var client = new RestClient(activeCampaignDeal);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Api-Token", apiToken);
            request.AddHeader("Content-Type", "text/plain");
            request.AddHeader("Cookie", cookie);
            var body = @"{
" + "\n" +
            @"  ""deal"": {
" + "\n" +
            @"    ""contact"": ""[CONTACT_ID]"",
" + "\n" +
            @"    ""account"": ""[ACCOUNT_ID]"",
" + "\n" +
            @"    ""description"": ""This deal is an important deal"",
" + "\n" +
            @"    ""currency"": ""usd"",
" + "\n" +
            @"    ""group"": ""[GROUP_ID]"",
" + "\n" +
            @"    ""owner"": ""[OWNER_ID]"",
" + "\n" +
            @"    ""percent"": null,
" + "\n" +
            @"    ""stage"": ""[STAGE_ID]"",
" + "\n" +
            @"    ""status"": 0,
" + "\n" +
            @"    ""title"": ""[DEAL_NAME]"",
" + "\n" +
            @"    ""value"": 0
" + "\n" +
            @"
" + "\n" +
            @"  }
" + "\n" +
            @"}";

            body = body.Replace("[CONTACT_ID]", contactId);
            body = body.Replace("[ACCOUNT_ID]", accountId);
            body = body.Replace("[GROUP_ID]", groupId);
            body = body.Replace("[OWNER_ID]", ownerId);
            body = body.Replace("[STAGE_ID]", stageId);
            body = body.Replace("[DEAL_NAME]", dealName);
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            try
            {
                IRestResponse response = client.Execute(request);
                result = 1;
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }
    }
}
