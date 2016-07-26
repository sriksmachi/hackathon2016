//namespace hackathonsqlazurebot
//{
//    using System;
//    using System.Threading;
//    using System.Threading.Tasks;
//    using AuthBot;
//    using AuthBot.Dialogs;
//    using AuthBot.Models;
//    using Microsoft.Bot.Builder.Dialogs;
//    using Microsoft.Bot.Connector;
//    using System.Configuration;
//    using Microsoft.WindowsAzure.Management.Compute;
//    using Microsoft.WindowsAzure.Subscriptions;
//    using Microsoft.WindowsAzure;
//    using Microsoft.Bot.Builder.FormFlow;
//    using System.Reflection;
//    using System.IO;
//    using Newtonsoft.Json.Linq;
//    using Microsoft.Bot.Builder.FormFlow.Json;
//    using System.Collections.Generic;

//    [Serializable]
//    public class SQLAzureBotHelper
//    {
//        public static string Token { get; set; }

//        public static IForm<AzureModel> BuildForm()
//        {
//            OnCompletionAsyncDelegate<AzureModel> processResult = async (context, state) =>
//            {
//                bool showSource = false;
//                var responses = new Dictionary<string, string>();
//                // Iterate the responses and do what you like here
//                //foreach (var item in state)
//                //{
//                //    var commands = await ProcessChatMessage(context, item.ToString());
//                //    foreach (var command in commands)
//                //    {
//                //        responses.Add(item.Name, item.Value.ToString());
//                //    }
//                //    // Here we're only interested in the final answer to determine response
//                //    if (item.Name.Equals("Question10"))
//                //    {
//                //        // Display the repo url only if requested
//                //        if ((bool)item.Value == true)
//                //        {
//                //            showSource = true;
//                //        }
//                //    }
//                //}

//                var msg = context.MakeMessage();
//                if (showSource)
//                {
//                    string url = ConfigurationManager.AppSettings["repo_url"];
//                    msg.Text = "Great, hope you find this sample code useful: " + url;
//                }
//                else
//                {
//                    msg.Text = "Enjoy building your bot!";
//                }

//                await context.PostAsync(msg);

//                // Wang in your own telemetry here
//                //TelemetryClient telemetry = new TelemetryClient();
//                //telemetry.TrackEvent("BotSample", responses);
//            };
//            return new FormBuilder<AzureModel>()
//                .Message(new PromptAttribute("Hi, I'm a (MS Bot Framework) SQL Azure Bot - here to help you work with SQL Azure databases."))
//                .Field(new Microsoft.Bot.Builder.FormFlow.Advanced.FieldReflector<AzureModel>("Subscriptions")
//                    .SetDefine(async (state, field) =>
//                    { 
//                        foreach (string subscription in await ProcessChatMessage("azure"))
//                        {
//                            field
//                                .AddDescription("cookie", "Free cookie")
//                                .AddDescription("drink", "Free large drink");
//                        }
//                        return true;
//                    }))
//                .Message("Thanks for sticking with me, processing responses..")
//                .OnCompletion(processResult)
//                .Build();
//        }

//        private static Microsoft.Bot.Builder.FormFlow.Advanced.IField<JObject> GetFields()
//        {
//            return null;
//        }

//    }

//    public class AzureModel
//    {
//        public string Options { get; set; }
//        public string Subscriptions { get; set; }
//    }
//}