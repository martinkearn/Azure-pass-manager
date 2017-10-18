namespace APM.Bot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Scorables;
    using Microsoft.Bot.Connector;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    [Serializable]
    public class RootDialog : DispatchDialog
    {
        // generic activity handler.
        [MethodBind]
        [ScorableGroup(0)]
        public async Task ActivityHandler(IDialogContext context, IActivity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    this.ContinueWithNextGroup();
                    break;
                case ActivityTypes.ConversationUpdate:
                    if (context.Activity.ChannelId.ToLower() != "webchat" && context.Activity.ChannelId.ToLower() != "bing")
                    {
                        IConversationUpdateActivity update = activity as IConversationUpdateActivity;
                        var client = new ConnectorClient(new Uri(activity.ServiceUrl), new MicrosoftAppCredentials());
                        if (update.MembersAdded != null && update.MembersAdded.Any())
                        {
                            foreach (var newMember in update.MembersAdded)
                            {
                                if (newMember.Id != activity.Recipient.Id)
                                {
                                    await this.Default(context, activity);
                                }
                            }
                        }
                    }
                    break;
                case ActivityTypes.ContactRelationUpdate:
                case ActivityTypes.Typing:
                case ActivityTypes.DeleteUserData:
                case ActivityTypes.Ping:
                default:
                    break;
            }
        }

        [RegexPattern("help")]
        [ScorableGroup(1)]
        public async Task Help(IDialogContext context, IActivity activity)
        {
            await this.Default(context, activity);
        }

        [MethodBind]
        [ScorableGroup(2)]
        public async Task Default(IDialogContext context, IActivity activity)
        {
            context.Call(new HelpDialog(), AfterDialog);
        }

        [RegexPattern("azure|code|pass|trial")]
        [RegexPattern("Azure trial code")]
        [ScorableGroup(1)]
        public async Task NewCode(IDialogContext context, IActivity activity)
        {
            context.Call(new APMDialog(), AfterDialog);
        }

        [RegexPattern("reset")]
        [RegexPattern("reset code")]
        [ScorableGroup(1)]
        public async Task ResetCode(IDialogContext context, IActivity activity)
        {
            PromptDialog.Confirm(context, AfterResetPrompt, "Are you sure you want to clear you Azure trial code?");
        }

        private async Task AfterResetPrompt(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                context.UserData.Clear();
                string message = "Bot state cleared.";
                await context.SayAsync(message, message);
            }
            context.Done<object>(null);
        }

        private async Task AfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(null);
        }
    }
}