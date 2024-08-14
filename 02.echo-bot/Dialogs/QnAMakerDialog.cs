using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.AI.QnA.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Dialogs
{
    public class QnAMakerDialog : CancelAndHelpDialog
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _endpointKey;
        private readonly string _hostName;
        private readonly string _knwledgeBaseId;
        private readonly string _DefaultWelcome = "Hello There";


        public QnAMakerDialog(IConfiguration configuration, IHttpClientFactory httpClientFactory)
            : base(nameof(QnAMakerDialog))
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

            _hostName = configuration["LanguageEndpointHostName"];
            _endpointKey = configuration["LanguageEndpointKey"];
            _knwledgeBaseId = configuration["ProjectName"];

            var welcomeMsg = configuration["DefaultWelcomeMessage"];
            if (!string.IsNullOrWhiteSpace(welcomeMsg))
            {
                _DefaultWelcome = welcomeMsg;
            }

        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter question.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Question"] = (string)stepContext.Result;

            // Call QnA Maker service
            using var httpClient = _httpClientFactory.CreateClient();
            
            var qnaMaker = new QnAMaker(new QnAMakerEndpoint
            {
                
                KnowledgeBaseId = _configuration["ProjectName"],
                EndpointKey = _configuration["LanguageEndpointKey"],
                Host = _configuration["LanguageEndpointHostName"],
                QnAServiceType = ServiceType.Language

            },
            null,
            httpClient);

            var options = new QnAMakerOptions { Top = 1 };

            // The actual call to the QnA Maker service.
            var response = await qnaMaker.GetAnswersAsync(stepContext.Context, options);
            if (response != null && response.Length > 0)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("No QnA Maker answers were found."), cancellationToken);
            }

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Would you like to ask more questions?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                return await stepContext.ReplaceDialogAsync(InitialDialogId, null, cancellationToken);
            }
            else
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
        }
    }
}