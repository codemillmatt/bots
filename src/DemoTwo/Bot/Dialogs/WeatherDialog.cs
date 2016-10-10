using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using WeatherService;

namespace DemoTwo.Dialogs
{
    [Serializable]
    public class WeatherDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // Get the message out
            var message = await argument;
            var activityMessage = message as Activity;

            if (activityMessage.MentionsRecipient())
            {
                var geo = new GeoLocatorService.GeoService();
                var matches = await geo.FindCoordinates(activityMessage.RemoveRecipientMention());

                if (matches.Count > 1)
                {
                    await context.Forward(new LocationDialog(), LocationPicked, matches, new System.Threading.CancellationToken());
                }
                else if (matches.Count == 1)
                {
                    var weather = await DisplayWeather(matches[0]);
                    await context.PostAsync($"The current conditions for {activityMessage.RemoveRecipientMention()} are {weather.Summary} and {weather.CurrentTemp}");
                }
                else
                {
                    await context.PostAsync($"Could not find the weather for {activityMessage.RemoveRecipientMention()}");
                }

                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task LocationPicked(IDialogContext context, IAwaitable<string> argument)
        {
            var location = await argument;

            var geo = new GeoLocatorService.GeoService();
            var city = (await geo.FindCoordinates(location)).First();
            var weather = await DisplayWeather(city);

            await context.PostAsync($"The current conditions for {city} are {weather.Summary} and {weather.CurrentTemp}");
            context.Wait(MessageReceivedAsync);
        }

        async Task<WeatherInfo> DisplayWeather(GeoLocatorService.CoordinateInfo coord)
        {
            var weatherService = new WeatherService.WeatherService();
            var forecast = await weatherService.GetCurrentConditions(coord.Latitude, coord.Longitude);

            return forecast;
        }

        bool MentionsRecipient(IMessageActivity activity)
        {
            return activity.GetMentions().Any(a => a.Text.Equals(activity.Recipient.Name, StringComparison.OrdinalIgnoreCase));
        }

    }
}