using EchoBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoBot.Dialogs
{
    public class AzureSQLDBDialog : CancelAndHelpDialog
    {
        FinacleSqldbContext context;
        public FinacleSqldbContext Context { get { return context; } }
        public AzureSQLDBDialog()
            : base(nameof(AzureSQLDBDialog))
        {
            context = new FinacleSqldbContext();

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
        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter the Region Name.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Name"] = (string)stepContext.Result;

            var genText = (string)stepContext.Values["Name"];
            Regions region = FetchDisplayName(genText);
            if (region == null)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Region with Name {genText} not found."), cancellationToken);
            }
            else
            {
                var replyText = region.DisplayName + ": " + region.Name;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }

           
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Would you like to search for more Region Names?")
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