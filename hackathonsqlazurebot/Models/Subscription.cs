using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hackathonsqlazurebot.Models
{
    [Serializable]
    public class Subscription
    {
        public string DisplayName { get; internal set; }

        public string SubscriptionId { get; internal set; }
    }
}