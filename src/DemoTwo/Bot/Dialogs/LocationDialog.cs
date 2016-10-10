using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DemoTwo.Dialogs
{
    public class LocationDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait<List<GeoLocatorService.CoordinateInfo>>(MessageReceived);
        }

        public async Task MessageReceived(IDialogContext context, IAwaitable<List<GeoLocatorService.CoordinateInfo>> coordinates)
        {
            // Wait for the incoming
            var theCoords = await coordinates;

            var pickList = new List<string>();
            foreach (var coord in theCoords)
            {
                pickList.Add(coord.CityState);
            }
            PromptOptions<string> allOptions = new PromptOptions<string>("More than one city found, pick one:", null, null, pickList);

            PromptDialog.Choice<string>(context, PromptDone, allOptions);
        }

        public async Task PromptDone(IDialogContext context, IAwaitable<string> cityName)
        {
            var city = await cityName;

            context.Done(city);
        }
    }
}