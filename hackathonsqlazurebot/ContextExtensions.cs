namespace hackathonsqlazurebot
{
    using Microsoft.Bot.Builder.Dialogs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public static class ContextExtensions
    {
        public static string GetSubscriptionId(this IBotContext context)
        {
            string subscriptionId;

            context.UserData.TryGetValue<string>(ContextConstants.SubscriptionIdKey, out subscriptionId);

            return subscriptionId;
        }

        public static string GetSQLServerName(this IBotContext context)
        {
            string sqlservername;

            context.UserData.TryGetValue<string>(ContextConstants.SQLServerKey, out sqlservername);

            return sqlservername;
        }

        public static void StoreSubscriptionId(this IBotContext context, string subscriptionId)
        {
            context.UserData.SetValue(ContextConstants.SubscriptionIdKey, subscriptionId);
        }

        public static void StoreSQLServer(this IBotContext context, string sqlServer)
        {
            context.UserData.SetValue(ContextConstants.SQLServerKey, sqlServer);
        }


        public static void Cleanup(this IBotContext context)
        {
            context.UserData.RemoveValue(ContextConstants.SubscriptionIdKey);
        }

    }
}