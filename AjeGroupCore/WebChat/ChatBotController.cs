﻿using AjeGroupCore.Helpers;
using AjeGroupCore.Models;
using AjeGroupCore.WebChat.AjeGroup;
using IBM.VCA.Watson.Watson.Model;
using IBM.VCA.WebChat.Weather;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static AjeGroupCore.WebChat.GoogleUser;
using static AjeGroupCore.WebChat.Models.WebChatTemplates;
using static IBM.VCA.Watson.Watson.WatsonConversationService;


namespace AjeGroupCore.WebChat
{
	[Route("api/[controller]")]
	public class ChatBotController : Controller
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private ISession _session => _httpContextAccessor.HttpContext.Session;

		static Context context;
		public static WatsonCredentials _credentials;

		public static string _keyEncode = "E546C8DF278CD5931069B522E695D4F2";

		private readonly UserManager<ApplicationUser> _userManager;

		public ChatBotController(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_credentials = _session.GetObjectFromJson<WatsonCredentials>("Watson");

			var _credsTest = HttpContext?.Session?.GetObjectFromJson<WatsonCredentials>("Watson");

			_userManager = userManager;
		}

	   
		[HttpPost]
		public async Task<JsonResult> MessageChatAsync(string msg, bool isInit, bool isValid, string actionPayload)
		{
			try
			{
				string _attachment = null;

				if (_credentials == null)
				{
					//IT-ORVAL=>Test-aje-jg->Aje group Paris
					_credentials = new WatsonCredentials()
					{
						workspaceID = "ac6889fe-7f09-4f71-ac0e-4d8850b72d2f",
						username = "7ecc7e1d-b7a9-472b-9419-a7254411cdd5",
						password = "HQJwcbFZclYL"
					};
				}

				if (isInit)
				{
					context = new Context();

				}

				if (!string.IsNullOrEmpty(actionPayload))
				{
					if (context == null)
					{
						context = new Context();
					}

					context.Action = actionPayload;
				}

				if (context != null)
				{
					context.Valid = isValid;

					switch (context.Action)
					{
						case "emailToValidate":

							if (!context.Valid)
							{
								break;
							}

							var userEmailToValidate = await _userManager.FindByEmailAsync(msg);

							if (userEmailToValidate?.EmailConfirmed == false || userEmailToValidate == null)
							{
								context.Valid = false;
								break;
							}

							MyGoogleUserInfo _userinfo = GetGoogleUserInfo(msg);

							//if (!GoogleUser.IsEmailRegistered(msg))
							//{
							//    context.Valid = false;
							//}

							if (_userinfo == null || _userinfo.Suspended == true)
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

							if (string.IsNullOrEmpty(context.Email))
							{
								break;
							}

							var user = await _userManager.FindByEmailAsync(context.Email);
							var _decrypt = Helpers.Helpers.DecryptString(user?.SecretResponse, _keyEncode);

							//Default if not encrypted secret response previously
							if (string.IsNullOrEmpty(_decrypt))
							{
								_decrypt = user?.SecretResponse;
							}

							if (msg == _decrypt)
							{
								context.Valid = true;
								//context.Action = null;
							}
							else
							{
								context.Valid = false;
								//context.Action = "emailToValidate";
							}

							break;

						case "confirmationToValidate":
							if (context.Valid == true && context.Password != null)
							{
								string goog = RunPasswordReset(context.Email, context.Password);

								if (goog.Contains("Error") || goog.Contains("forbidden"))
								{

									context.Action = null;
									context.Valid = false;
									context.Password = string.Empty;


									if (goog.Contains("forbidden"))
									{
										context.Error = "No tengo los privilegios suficientes para modificar la contraseña de un Administrador";
									}
									else
									{
										context.Error = "Ha sucedido un error al momento de intentar actualizar la contraseña";
									}


									//if (goog.Contains("forbidden"))
									//{
									//	goog = "No tengo los privilegios suficientes para modificar la contraseña de un Administrador";
									//}
									//else
									//{
									//	goog = "Ha sucedido un error al momento de intentar actualizar la contraseña";
									//}

									//MessageRequest _msgGoog = new MessageRequest()
									//{
									//	Output = new OutputData()
									//	{
									//		Text = new List<string>()
									//		{
									//			goog
									//		}
									//	},
									//	Context = context
									//};

									//return Json(JsonConvert.SerializeObject(_msgGoog));
								}
							}

							break;

						case "ListPerfiles":
							List<string> _listPerfiles = await SOAPservice.GetListPerfilesAsync();
							context.Action = null;

							MessageRequest _message = new MessageRequest()
							{

								Output = new OutputData()
								{
									Text = _listPerfiles
								},
								Context = context
							};

							var obj = JsonConvert.SerializeObject(_message);

							return Json(obj);

						case "AddServiceCall":

							//Test
							//if (string.IsNullOrEmpty(context.Email))
							//{
							//    context.Email = "vcaperuadmin@ajegroup.com";
							//}

							if (string.IsNullOrEmpty(context.Email))
							{
								context.Email = "vcaperuuser@ajegroup.com";
							}


							SOAPservice.ArandaUser _user = await SOAPservice.GetArandaUserInfo(context.Email);

							SOAPservice.ArandaTicket ticket = await SOAPservice.SetArandaNewTicketAsync(_user);

							string msgTicket;

							if (string.IsNullOrEmpty(ticket.TicketNumber))
							{
								msgTicket = "Correo electrónico no registrado en Aranda. No se pudo crear ticket.";
							}
							else
							{
								msgTicket = string.Format("El ticket {0} ha sido creado con exito!", ticket.TicketNumber);
							}

							context.Action = null;

							MessageRequest _msgAddTicket = new MessageRequest()
							{
								Output = new OutputData()
								{
									Text = new List<string>()
								{
									msgTicket
								}
								},
								Context = context
							};

							var objAdTicket = JsonConvert.SerializeObject(_msgAddTicket);

							return Json(objAdTicket);

						default:
							break;
					}


				}


				MessageRequest result = Message(msg, context, _credentials);

				context.Error = null;

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
								new ButtonTemplate() { HrefLink = "javascript:sendRequest(false,'ListPerfiles',true);", Text = "Listado de Perfiles" },
								new ButtonTemplate() { HrefLink = "javascript:sendRequest(false,'AddServiceCall',true);", Text = "Crear Ticket" },
							}
							};

							_attachment = ButtonListConstructor(_menu);

							break;

						case "productos":

							_attachment = CarouselConstructor(GetCarouselList());

							break;
						default:
							break;
					}
				}


				context = result.Context;

				switch (context.Action)
				{
					case "secretToValidate":

						context.Valid = false;

						if (string.IsNullOrEmpty(context.Email) || !IsValidEmail(context.Email))
						{
						  
							break;
						}

						var user = await _userManager.FindByEmailAsync(context.Email);
						var _decrypt = Helpers.Helpers.DecryptString(user?.SecretResponse, _keyEncode);

						//Default if not encrypted secret response previously
						if (string.IsNullOrEmpty(_decrypt))
						{
							_decrypt = user?.SecretResponse;
						}

						if (msg == _decrypt)
						{
							context.Valid = true;
							break;
						}

						if (user != null)
						{
							result.Output.Text.Add(user.SecretQuestion);
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
							context = new Context();
						}



						break;

					case "success":

						//_attachment = "<a class='btn btn-default' href=javascript:getGoogleUserInfo('" +
						// context.Email + "');>Ver datos</a>";
						//_attachment = _attachment + "<br />";
						//_attachment = _attachment + "<a class='btn btn-default' href=javascript:getGoogleTokens('" +
						//       context.Email + "');>Generar Tokens</a>";

						SOAPservice.ArandaUser _user = await SOAPservice.GetArandaUserInfo(context.Email);

						SOAPservice.ArandaTicket ticket = await SOAPservice.SetArandaNewTicketAsync(_user);

						string msgTicket;

						if (string.IsNullOrEmpty(ticket.TicketNumber))
						{
							msgTicket = "Correo electrónico no registrado en Aranda. No se pudo crear ticket.";
						}
						else
						{
							string _IdbyProject = await SOAPservice.GetArandaProjectTicketAsync(ticket.TicketNumber);

							msgTicket = string.Format("El ticket {0} ha sido creado con exito!", _IdbyProject);
						}

						context = new Context();
						result.Output.Text.Add(msgTicket);

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
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);

				return null;
			}


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
					//"<div id= 'weather_widget_city_name' class='weather_widget_city_name'>Clima de " + city + "</div>" +
					"<h3 id= 'weather_widget_temperature' class='weather_widget_temperature'>" +
					"<img src='" + _urlIcon + "'> " + _temperature + "°C</h3>" +
					"<div id='weather_widget_main' class='weather_widget_main'>" + _description + "</div>";


			}

			return _forecast;

		}

		private CarouselTemplate GetCarouselList()
		{
			CarouselTemplate _carousel = new CarouselTemplate()
			{
				CarouselName = "CarouselProductos",
				Elements = new List<ElementTemplate>()
							{
								new ElementTemplate()
								{
									Img_Url = "images/products/cool_tea.png",
									Title = "Cool Tea",
									Buttons = new List<ButtonTemplate>()
									{
										new ButtonTemplate() { Text = "Ver más", HrefLink = "javascript:void();" }
									}
								},
								new ElementTemplate()
								{
									Img_Url = "images/products/volt.jpg",
									Title = "Volt",
									Buttons = new List<ButtonTemplate>()
									{
										new ButtonTemplate() { Text = "Ver más", HrefLink = "javascript:void();" }
									}
								},
								new ElementTemplate()
								{
									Img_Url = "images/products/sporade.jpg",
									Title = "Sporade",
									Buttons = new List<ButtonTemplate>()
									{
										new ButtonTemplate() { Text = "Ver más", HrefLink = "javascript:void();" }
									}
								},
								new ElementTemplate()
								{
									Img_Url = "images/products/cifrut.png",
									Title = "Cifrut",
									Buttons = new List<ButtonTemplate>()
									{
										new ButtonTemplate() { Text = "Ver más", HrefLink = "javascript:void();" }
									}
								},
								new ElementTemplate()
								{
									Img_Url = "images/products/big_cola.jpg",
									Title = "Big Cola",
									Buttons = new List<ButtonTemplate>()
									{
										new ButtonTemplate() { Text = "Ver más", HrefLink = "javascript:void();" }
									}
								},
								new ElementTemplate()
								{
									Img_Url = "images/products/agua_cielo.png",
									Title = "Cielo",
									Buttons = new List<ButtonTemplate>()
									{
										new ButtonTemplate() { Text = "Ver más", HrefLink = "javascript:void();" }
									}
								},
								new ElementTemplate()
								{
									Img_Url = "images/products/pulp.png",
									Title = "Pulp",
									Buttons = new List<ButtonTemplate>()
									{
										new ButtonTemplate() { Text = "Ver más", HrefLink = "javascript:void();" }
									}
								}
							}
			};

			return _carousel;

		}


		public bool IsValidEmail(string _email)
		{
			if (String.IsNullOrEmpty(_email))
				return false;

			// Use IdnMapping class to convert Unicode domain names.
			try
			{
				_email = Regex.Replace(_email, @"(@)(.+)$", this.DomainMapper,
									  RegexOptions.None, TimeSpan.FromMilliseconds(200));
			}
			catch (RegexMatchTimeoutException)
			{
				return false;
			}

			
			// Return true if strIn is in valid e-mail format.
			try
			{
				return Regex.IsMatch(_email,
					  @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
					  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
					  RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
			}
			catch (RegexMatchTimeoutException)
			{
				return false;
			}
		}

		private string DomainMapper(Match match)
		{
			// IdnMapping class with default property values.
			IdnMapping idn = new IdnMapping();

			string domainName = match.Groups[2].Value;
			try
			{
				domainName = idn.GetAscii(domainName);
			}
			catch (ArgumentException)
			{
				return "true";
			}
			return match.Groups[1].Value + domainName;
		}
	}


}
