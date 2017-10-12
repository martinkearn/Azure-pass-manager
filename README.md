# Azure Pass Manager
Azure promotional pass codes can be used to evaluate and use Azure services. The codes are often distributed at hackathons and similar events.

This repository contains a web application and associated back-end services used to manage the distribution of Azure trial pass codes. 

This solution will not help you get the codes, there is an existing process for that which is available to Microsoft employees via https://requests.microsoftazurepass.com.

You can use the site that is built via this repository at https://aka.ms/azurepassmanager (built on the master branch).

## Project History and Credits
This repository is built with thanks and gratitude on the work of a small team within the CSE group at Microsoft who worked on a similar project at an internal hackathon in September 2017. The internal project was not open source but much of the code and architecture in this project was taken (with permission) from the hackathon output, the team was
* [Dariusz Porowski](https://twitter.com/DariuszPorowski)
* [Denis Cepun](https://twitter.com/DenisCepun)
* [Francesca Longoni](https://www.linkedin.com/in/francesca-longoni-wehq/)
* [Christine Matheney](https://twitter.com/Matheneyc)
* [Sudhir Rawat](https://twitter.com/rawatsudhir)
* [Martin Kearn](https://twitter.com/MartinKearn)

Since forking the original solution, the work has been carried out with thanks to:
* [Martin Kearn](https://twitter.com/MartinKearn) - API, Back-end, Web interface
* [Jamie Dalton](https://twitter.com/daltskin) - Bots
* [Lee Stott](https://twitter.com/lee_stott) - Testing

## Issues, feedback, feature requests etc
Please use the [GitHub issues](https://github.com/martinkearn/Azure-pass-manager/issues) section to log all feedback.

## Usage - Administrators (the people with the codes)
If you work for Microsoft and handle Azure trial codes, this section is for you.

### Getting your codes
You'll need some Azure codes first. This is not handled by the Azure Pass Manager system, but it is easy to request in another system.
1. Order codes at https://requests.microsoftazurepass.com.
1. 'Passes for Field - {region} FY18' is generally the most appropriate choice as it is managed centrally, but it is up to you
1. If you request is approved, you'll receive an email with your codes as a CSV file and the expiration date.

### Upload your codes and create an event
Follow these steps to get up and running
1. Go to https://aka.ms/azurepassmanager and sign in with your corporate credentials
1. Go to `Upload Codes` and set a name for your event (make it simple and easy to remember), expiration date for the codes (you'll have been told this in the email that contained your CSV) and upload the CSV
1. Your event is live and codes are available to use. You can view/manage your events at `Your Events`. View your event to see the URL you should direct users to in order to claim their code
1. (optional) You may want to use http://aka.ms to setup a short URL for your event; your call.
1. (optional) You may want to configure a bot in Slack, Teams or Skype as an alterntive way for delegates to claim their code. Links are on the `Event Details` page

### Instructing delegates
You'll need to tell delegates at your event how to get their codes. There are a few choices:
* Give them a direct URL for your event's `Code Claim` page. You can get the URL on the event details page. You can get a vanity URL at http://aka.ms if you'd like to (recommended).
* Give them the event name and direct them to the http://aka.ms/AzurePassManager where they will be prompted to find the event by entering the event name. Spaces should be included and it is not case sensitive.
* Configure a bot in Slack, Teams or Skype as an alterntive way for delegates to claim their code. Links are on the `Event Details` page

If delegates attempt to claim more than one code, they may receive an error that says something like _"Cheeky! ... You have already requested a code for the EVENTNAMEGOESHERE on this machine and it was CODEGOESHERE. Contact your Microsoft representative if you really need a second code."_. If you would like to for them to get a second code, tell them to either use another device or to clear their browser cookies.

### Getting data about your event
When your event is completed, you may want to see data about how many codes were claimed, to do this
1. Go to https://aka.ms/azurepassmanager and sign in with your corporate credentials
1. View/manage your events at `Your Events`
1. You can view data about used/un-used codes and optionally export them as a CSV (they might be useful at a future event?) 

### Removing your event and its codes
If you want to clear your event, consider exporting unused codes first, then follow these steps.
1. Go to https://aka.ms/azurepassmanager and sign in with your corporate credentials
1. View/manage your events at `Your Events`
1. Simply `Delete event and codes` to remove your event and all codes from the system.

## Contact me
Contact Martin.Kearn@Microsoft.com with comments and feedback that you don't want to log as a [GitHub issue](https://github.com/martinkearn/Azure-pass-manager/issues).
