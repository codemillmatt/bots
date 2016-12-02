# I, For One, Welcome Our New Bot Overloads
Repo for talk on Microsoft's Bot Framework as given at Prairie.Code() on Oct 28, 2016 and CenDev UG on Dec 1, 2016.

## Demo 1 - Simple Weather Bot
 * Simple example of having a bot get the weather for a particular city.
 * The bot needs to be mentioned in order for it to "do its work"
 * It is possible that many results could be returned by the geo service
 * At first the bot takes the first result to get the weather
 * Then to prompt the user as to which city to get the weather for, a hero card is created and sent back as an attachment. However, it becomes difficult to handle all the requests (hero card and "first time" requests).
 
 ## Demo 2 - IDialogs and IDialogContexts
 * Extend the first example, but this time using IDialogs to show how a conversation and be suspended and then resumed.
 * More intelligent conversations can now occur because dialogs encapsulate smaller portions of an overall flow of a larger bot.
 
