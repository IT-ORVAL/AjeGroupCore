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
using AjeGroupCore.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using static AjeGroupCore.WebChat.Models.WebChatTemplates;
using AjeGroupCore.WebChat.Models;

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

        private readonly UserManager<ApplicationUser> _userManager;

        public ChatBotController(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _credentials = _session.GetObjectFromJson<WatsonCredentials>("Watson");

            var _credsTest = HttpContext?.Session?.GetObjectFromJson<WatsonCredentials>("Watson");

            _userManager = userManager;
        }

       
        [HttpPost]
        public async Task<JsonResult> MessageChatAsync(string msg, bool isInit, bool isValid)
        {
            string _attachment = null;

            if (isInit)
            {
                context = null;

            }

            if (context != null)
            {
                context.Valid = isValid;

                switch (context.Action)
                {
                    case "emailToValidate":
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
                            context.Email = msg;
                            context.Valid = true;

                            //_attachment = "<a class='btn btn-default' href=javascript:getGoogleUserInfo('" +
                            //  msg + "');>Ver datos</a>";
                        }

                        break;

                    case "secretToValidate":
                        var user = await _userManager.FindByEmailAsync(context.Email);


                        if (msg == user?.SecretResponse)
                        {
                            context.Valid = true;
                        }
                        else
                        {
                            context.Valid = false;
                        }

                        break;

                    case "confirmationToValidate":
                        if (context.Valid == true && context.Password != null)
                        {
                            string goog = RunPasswordReset(context.Email, context.Password);
                        }

                        break;

                    default:
                        break;
                }


            }


            MessageRequest result = Message(msg, context, _credentials);


            if (result.Intents != null)
            {
                string myIntent = result.Intents[0].IntentDescription;
                string myAction = context?.Action;

                switch (myIntent)
                {
                    case "clima":
                        string _forecast = await CallWeatherAsync(null, null);

                        if (!string.IsNullOrEmpty(_forecast))
                        {
                            result.Output.Text.Add(_forecast);
                        }

                        break;

                    case "menu":
                        ButtonListTemplate _menu = new ButtonListTemplate()
                        {
                            Buttons = new List<ButtonTemplate>()
                            {
                                new ButtonTemplate() { HrefLink = "", Text = "Crear Ticket" },
                                new ButtonTemplate() { HrefLink = "", Text = "Consultar Ticket" },
                            }
                        };



                        _attachment = ButtonListConstructor(_menu);

                        break;

                    default:
                        break;
                }
            }


            context = result.Context;

            switch (context.Action)
            {
                case "secretToValidate":
                    var user = await _userManager.FindByEmailAsync(context.Email);

                    if (user != null)
                    {
                        result.Output.Text.Add(user.SecretQuestion);
                        context.Valid = true;
                    }
                    else
                    {
                        result.Output = new OutputData()
                        {
                            Text = new List<string>()
                            {
                                "Debe registrarse primero en la aplicación"
                            }
                        };

                        //context.Valid = false;
                        context = null;
                    }



                    break;

                case "success":

                    //_attachment = "<a class='btn btn-default' href=javascript:getGoogleUserInfo('" +
                    // context.Email + "');>Ver datos</a>";
                    //_attachment = _attachment + "<br />";
                    //_attachment = _attachment + "<a class='btn btn-default' href=javascript:getGoogleTokens('" +
                    //       context.Email + "');>Generar Tokens</a>";

                    break;
                default:
                    break;
            }


            //if (context.Action == "success")
            //{
            //    _attachment = "<a class='btn btn-default' href=javascript:getGoogleUserInfo('" +
            //          context.Email + "');>Ver datos</a>";
            //    _attachment = _attachment + "<br />";
            //    _attachment = _attachment + "<a class='btn btn-default' href=javascript:getGoogleTokens('" +
            //           context.Email + "');>Generar Tokens</a>";
            //}

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
