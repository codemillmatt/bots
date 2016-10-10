using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DemoOne
{
	[BotAuthentication]
	public class MessagesController : ApiController
	{
		/// <summary>
		/// POST: api/Messages
		/// Receive a message from a user and reply to it
		/// </summary>
		public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
		{
			if (activity.Type == ActivityTypes.Message)
			{
				if (activity.MentionsRecipient())
				{
					ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

					// Show typing while performing lookups
					var typingActivity = activity.CreateReply();
					typingActivity.Type = ActivityTypes.Typing;
					await connector.Conversations.ReplyToActivityAsync(typingActivity);

					// Remove the mention from the activity text
					var cityName = activity.RemoveRecipientMention();

					var locatorService = new GeoLocatorService.GeoService();
					var matches = await locatorService.FindCoordinates(cityName);

					if (matches.Count > 0)
					{
						var weather = new WeatherService.WeatherService();
						var current = await weather.GetCurrentConditions(matches[0].Latitude, matches[0].Longitude);
						
						var hero = new HeroCard("I am a hero!");
						hero.Subtitle = "Subtitle";
						hero.Text = "text";
						var action = new CardAction();
						action.Type = ActionTypes.PostBack;
						action.Value = "hello @bot1";
						action.Title = "press me";
						hero.Buttons = new List<CardAction>();
						hero.Buttons.Add(action);

						var heroReply = activity.CreateReply();
						heroReply.Attachments = new List<Attachment>();
						heroReply.Attachments.Add(hero.ToAttachment());
						heroReply.TopicName = "hero reply";
						await connector.Conversations.ReplyToActivityAsync(heroReply);

						var reply = activity.CreateReply($"The current conditions for {matches[0].CityState} are {current.Summary} and {current.CurrentTemp}.");

						await connector.Conversations.ReplyToActivityAsync(reply);
					}
					else
					{
						await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("Couldn't find any weather!"));
					}
				}
			}
			else
			{
				HandleSystemMessage(activity);
			}
			var response = Request.CreateResponse(HttpStatusCode.OK);
			return response;
		}

		private Activity HandleSystemMessage(Activity message)
		{
			if (message.Type == ActivityTypes.DeleteUserData)
			{
				// Implement user deletion here
				// If we handle user deletion, return a real message
			}
			else if (message.Type == ActivityTypes.ConversationUpdate)
			{
				// Handle conversation state changes, like members being added and removed
				// Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
				// Not available in all channels
			}
			else if (message.Type == ActivityTypes.ContactRelationUpdate)
			{
				// Handle add/remove from contact lists
				// Activity.From + Activity.Action represent what happened
			}
			else if (message.Type == ActivityTypes.Typing)
			{
				// Handle knowing tha the user is typing
			}
			else if (message.Type == ActivityTypes.Ping)
			{
			}

			return null;
		}
	}
}