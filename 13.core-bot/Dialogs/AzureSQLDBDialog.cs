using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using CoreBot.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Dialogs
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

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter the region Code.")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Name"] = (string)stepContext.Result;

            // Do some query on SQL Database
            var RegName = (string)stepContext.Values["Name"];
            Region region = FetchRegionName(RegName);

            if (region == null)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Region with id {RegName} not found."), cancellationToken);
            }
            else
            {
                var replyText = region.Name + ": " + region.DisplayName;
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
            }


            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Would you like to search for more region names?")
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

        public Region FetchRegionName(string str)
        {
            Region region;
            try
            {
                region = (from e in Context.Regions
                            where e.Name == str
                            select e).FirstOrDefault();//Query for employee details with id
            }
            catch (Exception)
            {
                throw;
            }
            return region;
        }
    }
}