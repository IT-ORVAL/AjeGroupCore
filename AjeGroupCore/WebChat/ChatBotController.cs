using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IBM.VCA.Watson.Watson.Model;
using static IBM.VCA.Watson.Watson.WatsonConversationService;
using Newtonsoft.Json;
using IBM.VCA.WebChat.Weather;
using static AjeGroupCore.WebChat.GoogleUser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using AjeGroupCore.Helpers;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AjeGroupCore.WebChat
{
    [Route("api/[controller]")]
    public class ChatBotController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        static Context context;
        public static WatsonCredentials _credentials;

        public ChatBotController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _credentials = _session.GetObjectFromJson<WatsonCredentials>("Watson");
        }

       
        [HttpPost]
        public async Task<JsonResult> MessageChatAsync(string msg, bool isInit, bool isValid)
        {
            string _forecast = null;
            string _attachment = null;

            if (isInit)
            {
                context = null;

                _forecast = await CallWeatherAsync(null, null);
            }

            if (context != null)
            {
                context.Valid = isValid;

                if (context.Action == "email")
                {
                    MyGoogleUserInfo _userinfo = GetGoogleUserInfo(msg);

                    //if (!GoogleUser.IsEmailRegistered(msg))
                    //{
                    //    context.Valid = false;
                    //}

                    if (_userinfo == null)
                    {
                        context.Valid = false;
                        //context = null;
                    }
                    else
                    {
                        context.UserName = _userinfo.GivenName;

                        _attachment = "<a class='btn btn-default' href=javascript:getGoogleUserInfo('" +
                          msg + "');>Ver datos</a>";
                    }
                }

                if (context.Action == "confirmation" && context.Valid == true && context.Password != null)
                {
                    string goog = RunPasswordReset(context.Email, context.Password);
                }

            }


            MessageRequest result = Message(msg, context, _credentials);

            context = result.Context;

            if (!string.IsNullOrEmpty(_forecast))
            {
                result.Output.Text.Add(_forecast);
            }


            if (context.Action == "success")
            {
                _attachment = "<a class='btn btn-default' href=javascript:getGoogleUserInfo('" +
                      context.Email + "');>Ver datos</a>";
                _attachment = _attachment + "<br />";
                _attachment = _attachment + "<a class='btn btn-default' href=javascript:getGoogleTokens('" +
                       context.Email + "');>Generar Tokens</a>";
            }

            if (!string.IsNullOrEmpty(_attachment))
            {
                result.Output.Text.Add(_attachment);
            }

            var json = JsonConvert.SerializeObject(result);

            return Json(json);
        }


        private async Task<string> CallWeatherAsync(string city, string date)
        {
            var result = await WeatherService.GetWeatherAsync(city, date);
            string _forecast = null;

            if (result != null)
            {

                string _temperature = result.main.temp.ToString();
                string _city = result.name;
                string _description = result.weather[0].description;
                string _urlIcon = string.Format("../images/icons/{0}.png", result.weather[0].icon);


                _forecast =
                    "<div id='weather_widget' class='weather_widget'>" +
                    "<div id= 'weather_widget_city_name' class='weather_widget_city_name'>Clima de " + city + "</div>" +
                    "<h3 id= 'weather_widget_temperature' class='weather_widget_temperature'>" +
                    "<img src='" + _urlIcon + "'> " + _temperature + "°C</h3>" +
                    "<div id='weather_widget_main' class='weather_widget_main'>" + _description + "</div>";


            }

            return _forecast;

        }

    }
}
