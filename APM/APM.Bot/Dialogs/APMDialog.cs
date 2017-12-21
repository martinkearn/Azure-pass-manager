namespace APM.Bot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Configuration;
    using System.Threading.Tasks;

    [Serializable]
    public class APMDialog : IDialog<object>
    {
        private const string AzureCodeKeyName = "AzurePromoCode";

        public async Task StartAsync(IDialogContext context)
        {
            if (context.UserData.TryGetValue(AzureCodeKeyName, out string existingCode))
            {
                // User already has a code - so show them it
                string response = $"You already have an Azure trial code.  Which is: {existingCode}";
                await context.SayAsync(response, response);
                context.Done<string>(null);
            }
            else
            {
                //PromptDialog.Text(context, ResumeAfterPromptDialog, "What is your event name?  Your Microsoft representative can give you this if you don't know.");
                string response = "What is your event name?  Your Microsoft representative can give you this if you don't know.";
                await context.SayAsync(response, response);
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var userResponse = await result;
            if (!string.IsNullOrWhiteSpace(userResponse.Text))
            {
                string response = "";
                var baseUri = new Uri(ConfigurationManager.AppSettings["APMAPI"]);
                var claimUri = new Uri(baseUri, $"claim?eventName={userResponse.Text}");
                var code = await APMHelper.GetAzurePassCode<Code>(claimUri);
                if (code != null)
                {
                    response = $"Your Azure trial code is: {code.PromoCode} which expires on {code.Expiry.ToLongDateString()}.  You can activate your code at https://www.microsoftazurepass.com";
                    context.UserData.SetValue(AzureCodeKeyName, code.PromoCode);
                    await context.SayAsync(response, response);

                    response = "Good luck with your project, you can now close this conversation.";
                    await context.PostAsync(response);
                    context.Done<string>(null);
                }
                else
                {
                    //PromptDialog.Text(context, ResumeAfterPromptDialog, "What is your event name?  Your Microsoft representative can give you this if you don't know.", "That's not a valid event.");
                    response = "What is your event name?  Your Microsoft representative can give you this if you don't know.";
                    await context.SayAsync(response, response);
                    context.Wait(MessageReceivedAsync);
                }
            }
        }
    }
}