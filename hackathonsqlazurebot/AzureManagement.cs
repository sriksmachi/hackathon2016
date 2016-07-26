using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Sql;
using Microsoft.WindowsAzure.Subscriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace hackathonsqlazurebot
{
    public class AzureManagement
    {


        public string GetAzureSubscriptions(string token)
        {
            var credentials = new TokenCloudCredentials(token);
            SubscriptionClient subscriptionClient = null;
            using (subscriptionClient = new SubscriptionClient(credentials))
            {
                var subscriptions = subscriptionClient.Subscriptions.List();
                string subscriptionNames = string.Empty;
                foreach (var subscription in subscriptions)
                {
                    subscriptionNames += subscription.SubscriptionName + "\n";
                }
                return subscriptionNames;
            }
        }

        public async Task<string> GetSQLServers(string token)
        {
            var credentials = new TokenCloudCredentials("090fcc3a-ed78-4e98-a932-974261d033e2", token);

            using (SqlManagementClient sqlManagementClient = 
                new SqlManagementClient(credentials))
            {
                var servers = await sqlManagementClient.Servers.ListAsync();
                string serverNames = string.Empty;
                foreach (var server in servers)
                {
                    serverNames += server.Name + "\n";
                }
                return await Task.FromResult<string>(serverNames);
            }
        }
    }
}