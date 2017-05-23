using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace LogicAppTriggers.Controllers
{
    public class WebhookTriggerController : ApiController
    {
        public static List<string> subscriptions = new List<string>();

        /// <summary>
        /// Receive subscription to webhook.
        /// </summary>
        /// <param name="callbackUrl">The callback URL to get from the Logic Apps engine - @listCallbackUrl()</param>
        /// <returns></returns>
        [HttpPost, Route("api/webhooktrigger/subscribe")]
        public HttpResponseMessage Subscribe([FromBody] string callbackUrl)
        {
            subscriptions.Add(callbackUrl);
            return Request.CreateResponse();
        }

        /// <summary>
        /// Fire all triggers. To fire all subscribed triggers, perform a GET to this API.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/webhooktrigger/trigger")]
        public async Task<HttpResponseMessage> Get()
        {
            using (HttpClient client = new HttpClient())
            {
                foreach (string callbackUrl in subscriptions)
                    await client.PostAsync(callbackUrl, @"{""trigger"":""fired""}", new JsonMediaTypeFormatter(), "application/json");
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, String.Format("There are {0} subscriptions fired", subscriptions.Count));
        }
        /// <summary>
        /// Unsubscribe from webhook.
        /// </summary>
        /// <param name="callbackUrl"></param>
        /// <returns></returns>
        [HttpPost, Route("api/webhooktrigger/unsubscribe")]
        public HttpResponseMessage Unsubscribe([FromBody] string callbackUrl)
        {
            subscriptions.Remove(callbackUrl);
            return Request.CreateResponse();
        }
    }
}
