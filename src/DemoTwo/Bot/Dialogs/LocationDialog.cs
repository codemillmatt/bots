using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using GeoLocatorService;

namespace DemoTwo.Dialogs
{
	[Serializable]
	public class LocationDialog : IDialog<string>
	{
		public async Task StartAsync(IDialogContext context)
		{
			context.Wait<List<CoordinateInfo>>(MessageReceived);
		}

		public async Task MessageReceived(IDialogContext context, IAwaitable<List<CoordinateInfo>> coordinates)
		{
			// Wait for the incoming
			var theCoords = await coordinates;

			var cityHero = new HeroCard();
			cityHero.Title = "Which city?";
			cityHero.Subtitle = $"Multiple cities found";
			cityHero.Text = "Select which city you want the weather for:";
			cityHero.Buttons = new List<CardAction>();

			foreach (var city in theCoords)
			{
				var cityAction = new CardAction();
				cityAction.Type = ActionTypes.PostBack;
				cityAction.Value = city.CityState;
				cityAction.Title = city.CityState;

				cityHero.Buttons.Add(cityAction);
			}

			var cityHeroReply = context.MakeMessage();
			cityHeroReply.Attachments = new List<Attachment>();
			cityHeroReply.Attachments.Add(cityHero.ToAttachment());

			await context.PostAsync(cityHeroReply);
			context.Wait<string>(PromptDone);
		}

		public async Task PromptDone(IDialogContext context, IAwaitable<string> cityName)
		{
			var city = await cityName;

			context.Done(city);
		}
	}
}