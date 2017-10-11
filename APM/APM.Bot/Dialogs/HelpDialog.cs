namespace APM.Bot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading.Tasks;

    [Serializable]
    public class HelpDialog : IDialog<object>
    {
        private const string GetPromoCode = "Azure trial code";

        public async Task StartAsync(IDialogContext context)
        {
            var message = context.MakeMessage();
            message.Speak = "Azure Pass Manager";
            message.InputHint = InputHints.AcceptingInput;

            message.Attachments = new List<Attachment>
            {
                new ThumbnailCard(title: "Azure Pass Manager", subtitle: "Get your Azure trial code here")
                {
                    Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.ImBack, "Get Azure trial code", value: GetPromoCode),
                    },
                    Images = new List<CardImage>
                    {
                        new CardImage(url: ConfigurationManager.AppSettings["ImagePath"] + "AzureLogo96x96.png")
                    }
                }.ToAttachment()
            };

            await context.PostAsync(message);
            context.Done<object>(null);
        }
    }
}