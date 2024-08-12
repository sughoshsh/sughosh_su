// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Linq;
using System;
using EchoBot.Models;
using System.Diagnostics.Eventing.Reader;

namespace Microsoft.BotBuilderSamples.EchoBots
{
    public class MyBot : ActivityHandler
    {
        FinacleSqldbContext context;
        public FinacleSqldbContext Context { get { return context; } }

        public MyBot()
        {
            context = new FinacleSqldbContext();
        }

        public Regions FetchDisplayName(string str)
        {
            Regions region;
            try
            {
                region = (from r in Context.Regions
                          where r.Name == str
                          select r).FirstOrDefault();//Query for region details with name
            }
            catch (Exception)
            {
                throw;
            }
            return region;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //var replyText = $"Echo: {turnContext.Activity.Text}";
            //await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);

            var genText = turnContext.Activity.Text;
            Regions region = FetchDisplayName(genText);
            if (region == null)
            {
                var otherText = $"echo: {genText}";
                await turnContext.SendActivityAsync(MessageFactory.Text(otherText, otherText), cancellationToken);
            }
             else {
                var replyText = region.DisplayName + ": " + region.Name;
                await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }
            
        }


        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome! I have connected this bot to Azure DB."; //welcome message
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}

