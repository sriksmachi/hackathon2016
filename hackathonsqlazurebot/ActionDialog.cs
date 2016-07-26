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

namespace hackathonsqlazurebot.Dialogs
{
    [Serializable]
    public class ActionDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
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

            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            try
            {
                var message = await item;

                if (message.Text == "logon")
                {
                    //endpoint v1
                    if (string.IsNullOrEmpty(await context.GetAccessToken(ConfigurationManager.AppSettings["ActiveDirectory.ResourceId"])))
                    {
                        await context.Forward(new AzureAuthDialog(ConfigurationManager.AppSettings["ActiveDirectory.ResourceId"]), this.ResumeAfterAuth, message, CancellationToken.None);
                    }
                    else
                    {
                        context.Wait(MessageReceivedAsync);
                    }
                }
                else if (message.Text == "echo")
                {
                    await context.PostAsync("echo");

                    context.Wait(this.MessageReceivedAsync);
                }
                else if (message.Text == "token")
                {
                    await TokenSample(context);
                }
                else if (message.Text == "logout")
                {
                    await context.Logout();
                    context.Wait(this.MessageReceivedAsync);
                }
                else if (message.Text == "azure")
                {
                    var azureManagementClient = new AzureManagement();
                    var token = await context.GetAccessToken(ConfigurationManager.AppSettings["ActiveDirectory.ResourceId"]);
                    var result = azureManagementClient.GetAzureSubscriptions(token);
                    await context.PostAsync(result);
                }
                else if (message.Text == "server")
                {
                    var azureManagementClient = new AzureManagement();
                    var token = await context.GetAccessToken(ConfigurationManager.AppSettings["ActiveDirectory.ResourceId"]);
                    var result = await azureManagementClient.GetSQLServers(token);
                    await context.PostAsync(result);
                }
                else
                {
                    context.Wait(MessageReceivedAsync);
                }
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
            context.Wait(MessageReceivedAsync);
        }
    }
}