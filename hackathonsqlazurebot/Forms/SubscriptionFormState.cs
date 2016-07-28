using hackathonsqlazurebot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hackathonsqlazurebot.Forms
{
    [Serializable]
    public class SubscriptionFormState
    {
        public SubscriptionFormState(IEnumerable<Subscription> availableSubscriptions)
        {
            this.AvailableSubscriptions = availableSubscriptions;
        }

        public string DisplayName { get; set; }

        public string SubscriptionId { get; set; }

        public IEnumerable<Subscription> AvailableSubscriptions { get; private set; }
    }

    [Serializable]
    public class SQLServerFormState
    {
        public SQLServerFormState(IEnumerable<SQLServer> sqlServers)
        {
            this.AvailableSQLServers = sqlServers;
        }

        public string DisplayName { get; set; }

        public string SubscriptionId { get; set; }

        public IEnumerable<SQLServer> AvailableSQLServers { get; private set; }
    }


    [Serializable]
    public class SQLServerInfoFormState
    {
        public SQLServerInfoFormState(List<string> sqlServersInfo)
        {
            this.Info = sqlServersInfo;
        }

        public List<string> Info { get; set; }
    }

}