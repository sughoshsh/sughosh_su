// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using AdaptiveCards;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.AI.QnA.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using EchoBot.Dialogs;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
namespace EchoBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {

        protected readonly ILogger Logger;

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(ILogger<MainDialog> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
            : base(nameof(MainDialog))
        {

            Logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new AzureSQLDBDialog());
            AddDialog(new QnAMakerDialog(configuration, httpClientFactory));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Please choose an option to proceed further."), cancellationToken);
            List<string> operationList = new List<string> { "Use Azure SQL DB", "Use QnA Maker" };
            // Create card
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {
                // Use LINQ to turn the choices into submit actions
                Actions = operationList.Select(choice => new AdaptiveSubmitAction
                {
                    Title = choice,
                    Data = choice,  // This will be a string
                }).ToList<AdaptiveAction>(),
            };
            // Prompt
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    // Convert the AdaptiveCard to a JObject
                    Content = JObject.FromObject(card),
                }),
                Choices = ChoiceFactory.ToChoices(operationList),
                // Don't render the choices outside the card
                Style = ListStyle.None,
            },
                cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["UserOperation"] = ((FoundChoice)stepContext.Result).Value;

            string operation = (string)stepContext.Values["UserOperation"];
            if (operation.Equals("Use Azure SQL DB"))
            {
                return await stepContext.BeginDialogAsync(nameof(AzureSQLDBDialog), null, cancellationToken);
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(QnAMakerDialog), null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            // Restart the main dialog with a different message the second time around
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
      
        
    }
}
