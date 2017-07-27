using System;
using IBM.VCA.Watson.Watson.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBM.VCA.WebChat.Weather;

namespace IBM.VCA.Watson.Watson
{
    public class WatsonConversationService
    {
        public static ConversationService _conversation;
        public static Context _ctx;

        public class WatsonCredentials
        {
            public string workspaceID = "89fd0c87-c437-4573-9c87-c0d31e721cc8";
            public string username = "7ecc7e1d-b7a9-472b-9419-a7254411cdd5";
            public string password = "HQJwcbFZclYL";
        }

        public static MessageRequest Message(string text, Context ctx, WatsonCredentials creds)
        {

            MessageRequest messageRequest = new MessageRequest();

            if (text == null)
            {
                List<Intent> myIntents = new List<Intent>();

                myIntents.Add(new Intent()
                {
                    Confidence = 1,
                    IntentDescription = "algomas"
                });

                messageRequest.Intents = myIntents;
            }
            else
            {
                messageRequest.Input = new InputData()
                {
                    Text = text
                };
            }


            if (ctx != null && ctx.System != null)
            {
                messageRequest.Context = ctx;
            }

            _conversation = new ConversationService(creds.username, creds.password);


            var result = _conversation.Message(creds.workspaceID, messageRequest);

            if (result != null)
            {
                if (result.Intents.Count > 0)
                {
                    messageRequest.Intents = result.Intents;
                }
                else
                {
                    Console.WriteLine("Intents vacíos");
                }

                if (result.Output != null)
                {
                    messageRequest.Context = result.Context.ToObject<Context>();
                    messageRequest.Output = result.Output.ToObject<OutputData>();

                    return messageRequest;
                }
                else
                {
                    return messageRequest;
                }
            }
            else
            {

                return messageRequest;
            }
        }


        public static async Task<MessageRequest> Weather(string msg, Context ctx)
        {

            MessageRequest messageRequest = new MessageRequest()
            {
                Input = new InputData()
                {
                    Text = msg
                }
            };

            if (ctx != null && ctx.System != null)
            {
                messageRequest.Context = ctx;
            }



            var result = await WeatherService.GetWeatherAsync(ctx.City, null);


            if (result != null)
            {

                string temperature = result.main.temp.ToString();
                string city = result.name;
                string description = result.weather[0].description;
                string urlIcon = string.Format("../Watson/Content/weather/{0}.png", result.weather[0].icon);

                messageRequest.Context.City = city;
                messageRequest.Context.Temperature = temperature;
                //messageRequest.Context.Description = description;

                string forecast =
                    "<div id='weather_widget' class='weather_widget'>" +
                    "<div id= 'weather_widget_city_name' class='weather_widget_city_name'>Clima de " + city + "</div>" +
                    "<h3 id= 'weather_widget_temperature' class='weather_widget_temperature'>" +
                    "<img src='" + urlIcon + "'> " + temperature + "°C</h3>" +
                    "<div id='weather_widget_main' class='weather_widget_main'>" + description + "</div>";

                List<string> list = new List<string>();

                list.Add(forecast);


                OutputData output = new OutputData
                {
                    Text = list
                };



                messageRequest.Output = output;

            }

            return messageRequest;
        }

    }
}
