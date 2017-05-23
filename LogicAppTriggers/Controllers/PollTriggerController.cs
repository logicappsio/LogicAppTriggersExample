using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace LogicAppTriggers.Controllers
{
    public class PollTriggerController : ApiController
    {
        /// <summary>
        /// Create a sample polling trigger that fires whenever the last time the trigger fired was more than 2 minutes ago.
        /// </summary>
        /// <param name="triggerState"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(string triggerState = "")
        {
            // If "triggerState" doesn't exist, then this iteration is the first poll.
            if(String.IsNullOrEmpty(triggerState))
            {
                /// Generate the "triggerState" value, commonly a "timestamp", 
                /// to identify which items caused the trigger to fire. 
                /// On each poll, check for any new items became available after the timestamp.
                triggerState = DateTime.UtcNow.ToString();
                return GenerateAsyncResponse(HttpStatusCode.Accepted, triggerState, "15");
            }
            // If "triggerState" exists, then the trigger was fired before 
            // and returned a "location" header from the previous "if" branch.
            else
            {
                // Check whether "triggerState" exists. 
                if(DateTime.Parse(triggerState) < DateTime.UtcNow)
                {
                    // If "triggerState exists, return a "200 OK" status. You can also update "triggerState".
                    // This example updates "triggerState" so that the trigger fires again in 2 minutes based on this fake logic.
                    triggerState = DateTime.UtcNow.AddMinutes(2).ToString();
                    return GenerateAsyncResponse(HttpStatusCode.OK, triggerState, "15");
                }
                else
                {
                    return GenerateAsyncResponse(HttpStatusCode.Accepted, triggerState, "15");
                }
            }
        }

        private HttpResponseMessage GenerateAsyncResponse(HttpStatusCode code, string triggerState, string retryAfter)
        {
            HttpResponseMessage responseMessage = Request.CreateResponse(code); // Return a "200 OK" status to fire the trigger.
            responseMessage.Headers.Add("location", String.Format("{0}://{1}/api/polltrigger?triggerState={2}", Request.RequestUri.Scheme, Request.RequestUri.Host, HttpUtility.UrlEncode(triggerState))); // The URL that the engine polls to check status
            responseMessage.Headers.Add("retry-after", retryAfter); // The number of seconds the engine should wait before polling again. If multiple files are found, you can return "0" seconds so that the engine immediately returns for more items.
            return responseMessage;
        }
    }
}
