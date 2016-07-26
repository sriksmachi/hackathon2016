

namespace hackathonsqlazurebot
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AuthBot;
    using AuthBot.Dialogs;
    using AuthBot.Models;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System.Configuration;
    using Microsoft.WindowsAzure.Management.Compute;
    using Microsoft.WindowsAzure.Subscriptions;
    using Microsoft.WindowsAzure;
    using Microsoft.Bot.Builder.FormFlow;
    using System.Reflection;
    using System.IO;
    using Newtonsoft.Json.Linq;
    using Microsoft.Bot.Builder.FormFlow.Json;
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Forms;
    using System.Linq;

    [LuisModel("6b3e425e-7bc0-4608-bf3b-4aeebca51fc9", "bafd6c991b194acc8d4f886f1f4f64df")]
    [Serializable]
    public class ActionDialog : LuisDialog<string>
    {
        private static Lazy<string> resourceId = new Lazy<string>(() => ConfigurationManager.AppSettings["ActiveDirectory.ResourceId"]);
        private bool serviceUrlSet = false;

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            string message = "Hello! You can use the Azure Bot to: \n";
            message += $"* List, Switch and Select an Azure subscription\n";
            message += $"* List, Start, Shutdown (power off your VM, still incurring compute charges), and Stop (deallocates your VM, no charges) your virtual machines\n";
            message += $"* List your automation accounts and your runbooks\n";
            message += $"* Start a runbook, get the description of a runbook, get the status and ouput of automation jobs\n";
            message += $"* Logout to sign out from Azure\n\n";
            message += $"Please type **login** to interact with me for the first time.";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        public async Task TokenSample(IDialogContext context)
        {
            //endpoint v1
            var accessToken = await context.GetAccessToken(ConfigurationManager.AppSettings["ActiveDirectory.ResourceId"]);
            if (string.IsNullOrEmpty(accessToken))
            {
                return;
            }
            await context.PostAsync($"Your access token is: {accessToken}");
            context.Wait(MessageReceived);
        }

        public virtual async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            try
            {
                var message = await item;
                if (!serviceUrlSet)
                {
                    context.PrivateConversationData.SetValue("ServiceUrl", message.ServiceUrl);
                    serviceUrlSet = true;
                }
                if (message.Text.ToLowerInvariant().Contains("help"))
                {
                    await base.MessageReceived(context, item);

                    return;
                }
                var token = await context.GetAccessToken(resourceId.Value);
                if (string.IsNullOrEmpty(token))
                {
                    if (message.Text.ToLowerInvariant().Contains("login"))
                    {
                        await context.Forward(new AzureAuthDialog(resourceId.Value), this.ResumeAfterAuth, message, CancellationToken.None);
                    }
                    else
                    {
                        await this.Help(context, new LuisResult());
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(context.GetSubscriptionId()))
                    {
                        await this.UseSubscriptionAsync(context, new LuisResult());
                    }
                    else
                    {
                        await base.MessageReceived(context, item);
                    }
                }
                    //if (string.IsNullOrEmpty(await context.GetAccessToken(ConfigurationManager.AppSettings["ActiveDirectory.ResourceId"])))
                    //{
                    //    await context.Forward(new AzureAuthDialog(ConfigurationManager.AppSettings["ActiveDirectory.ResourceId"]), 
                    //        this.ResumeAfterAuth, message, CancellationToken.None);
                    //}
                    //else
                    //{
                    //    context.Wait(MessageReceived);
                    //}
                //}
                //else if (message.Text == "echo")
                //{
                //    await context.PostAsync("echo");

                //    context.Wait(this.MessageReceived);
                //}
                //else if (message.Text == "token")
                //{
                //    await TokenSample(context);
                //}
                //else if (message.Text == "logout")
                //{
                //    await context.Logout();
                //    context.Wait(this.MessageReceived);
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task ResumeAfterAuth(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;
            await context.PostAsync(message);
            await this.UseSubscriptionAsync(context, new LuisResult());
        }

        [LuisIntent("UseSubscription")]
        public async Task UseSubscriptionAsync(IDialogContext context, LuisResult result)
        {
            EntityRecommendation subscriptionEntity;

            var accessToken = await context.GetAccessToken(resourceId.Value);
            if (string.IsNullOrEmpty(accessToken))
            {
                context.Wait(this.MessageReceived);
                return;
            }

            var availableSubscriptions = await new AzureManagement().ListAzureSubscriptionsAsync(accessToken);

            // check if the user specified a subscription name in the command
            if (result.TryFindEntity("Subscription", out subscriptionEntity))
            {
                // obtain the name specified by the user - text in LUIS result is different
                var subscriptionName = subscriptionEntity.GetEntityOriginalText(result.Query);

                // ensure that the subscription exists
                var selectedSubscription = availableSubscriptions.FirstOrDefault(p => p.DisplayName.Equals(subscriptionName, StringComparison.InvariantCultureIgnoreCase));
                if (selectedSubscription == null)
                {
                    await context.PostAsync($"The '{subscriptionName}' subscription was not found.");
                    context.Wait(this.MessageReceived);
                    return;
                }

                subscriptionEntity.Entity = selectedSubscription.DisplayName;
                subscriptionEntity.Type = "SubscriptionId";
            }

            var formState = new SubscriptionFormState(availableSubscriptions);
            if (availableSubscriptions.Count() == 1)
            {
                formState.SubscriptionId = availableSubscriptions.Single().SubscriptionId;
                formState.DisplayName = availableSubscriptions.Single().DisplayName;
            }

            var form = new FormDialog<SubscriptionFormState>(
                formState,
                EntityForms.BuildSubscriptionForm,
                FormOptions.PromptInStart,
                result.Entities);

            context.Call(form, this.UseSubscriptionFormComplete);
        }

        [LuisIntent("ListSqlServer")]
        public async Task ListSQLServer(IDialogContext context, LuisResult result)
        {
            var accessToken = await context.GetAccessToken(resourceId.Value);
            if (string.IsNullOrEmpty(accessToken))
            {
                return;
            }
            var subscriptionId = context.GetSubscriptionId();
            var azureManagementClient = new AzureManagement();
            var servers = await azureManagementClient.GetSQLServers(subscriptionId,accessToken);
            var formState = new SQLServerFormState(servers);
            if (servers.Count == 1)
            {
                formState.SubscriptionId = servers.Single().DisplayName;
                formState.DisplayName = servers.Single().DisplayName;
            }

            var form = new FormDialog<SQLServerFormState>(
                formState,
                EntityForms.BuildSQLServerForm,
                FormOptions.PromptInStart,
                result.Entities);

            context.Call(form, this.UseSQLServerFormComplete);
        }

        private async Task UseSubscriptionFormComplete(IDialogContext context, IAwaitable<SubscriptionFormState> result)
        {
            try
            {
                var subscriptionFormState = await result;
                if (!string.IsNullOrEmpty(subscriptionFormState.SubscriptionId))
                {
                    var selectedSubscription = subscriptionFormState.AvailableSubscriptions.Single(sub => sub.SubscriptionId == subscriptionFormState.SubscriptionId);
                    context.StoreSubscriptionId(subscriptionFormState.SubscriptionId);
                    await context.PostAsync($"Setting {selectedSubscription.DisplayName} as the current subscription. What would you like to do next?");
                    context.Wait(this.MessageReceived);
                }
                else
                {
                    PromptDialog.Confirm(
                        context,
                        this.OnLogoutRequested,
                        "Oops! You don't have any Azure subscriptions under the account you used to log in. To continue using the bot, log in with a different account. Do you want to log out and start over?",
                        "Didn't get that!",
                        promptStyle: PromptStyle.None);
                }
            }
            catch (FormCanceledException<SubscriptionFormState> e)
            {
                string reply;

                if (e.InnerException == null)
                {
                    reply = "You have canceled the operation. What would you like to do next?";
                }
                else
                {
                    reply = $"Oops! Something went wrong :(. Technical Details: {e.InnerException.Message}";
                }

                await context.PostAsync(reply);
                context.Wait(this.MessageReceived);
            }
        }

        private async Task UseSQLServerFormComplete(IDialogContext context, IAwaitable<SQLServerFormState> result)

        {
            try
            {
                var sqlServerFormState = await result;
                if (!string.IsNullOrEmpty(sqlServerFormState.DisplayName))
                {
                    var selectedSQLServer = sqlServerFormState.AvailableSQLServers.Single(sub => sub.DisplayName == 
                    sqlServerFormState.DisplayName);
                    //context.StoreSubscriptionId(sqlServerFormState.SubscriptionId);
                    await context.PostAsync($"Setting {selectedSQLServer.DisplayName} as the current subscription. What would you like to do next?");
                    context.Wait(this.MessageReceived);
                }
                else
                {
                    PromptDialog.Confirm(
                        context,
                        this.OnLogoutRequested,
                        "Oops! You don't have any SQL Azure under the account you used to log in. To continue using the bot, log in with a different account. Do you want to log out and start over?",
                        "Didn't get that!",
                        promptStyle: PromptStyle.None);
                }
            }
            catch (FormCanceledException<SQLServerFormState> e)
            {
                string reply;
                if (e.InnerException == null)
                {
                    reply = "You have canceled the operation. What would you like to do next?";
                }
                else
                {
                    reply = $"Oops! Something went wrong :(. Technical Details: {e.InnerException.Message}";
                }
                await context.PostAsync(reply);
                context.Wait(this.MessageReceived);
            }
        }

        private async Task OnLogoutRequested(IDialogContext context, IAwaitable<bool> confirmation)
        {
            var result = await confirmation;
            if (result)
            {
                context.Cleanup();
                await context.Logout();
            }
        }
    }
}