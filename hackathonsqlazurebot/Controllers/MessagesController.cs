using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.FormFlow;
using System.Configuration;
using System.Diagnostics;

namespace hackathonsqlazurebot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            try
            {
                if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
                {
                    switch (activity.GetActivityType())
                    {
                        case ActivityTypes.Message:
                            await Conversation.SendAsync(activity, () => new ActionDialog());
                            break;
                        default:
                            Trace.TraceError($"Azure Bot ignored an activity. Activity type received: {activity.GetActivityType()}");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }
    }
}