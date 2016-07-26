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

        public static void StoreSubscriptionId(this IBotContext context, string subscriptionId)
        {
            context.UserData.SetValue(ContextConstants.SubscriptionIdKey, subscriptionId);
        }
        public static void Cleanup(this IBotContext context)
        {
            context.UserData.RemoveValue(ContextConstants.SubscriptionIdKey);
        }

    }
}