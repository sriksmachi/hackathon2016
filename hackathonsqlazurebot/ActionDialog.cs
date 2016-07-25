﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AuthBot;
using AuthBot.Dialogs;
using AuthBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Configuration;

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
            //endpoint v2
            var accessToken = await context.GetAccessToken(AuthSettings.Scopes);
            if (string.IsNullOrEmpty(accessToken))
            {
                return;
            }
            await context.PostAsync($"Your access token is: {accessToken}");
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;
            //if (message.Text == "logon")
            //{
            //endpoint v2
            if (string.IsNullOrEmpty(await context.GetAccessToken(AuthSettings.Scopes)))
            {
                try
                {
                    await context.Forward(new AzureAuthDialog(AuthSettings.Scopes), this.ResumeAfterAuth, message, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                context.Wait(MessageReceivedAsync);
            }
            //}
            if (message.Text == "echo")
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
            else
            {
                context.Wait(MessageReceivedAsync);
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