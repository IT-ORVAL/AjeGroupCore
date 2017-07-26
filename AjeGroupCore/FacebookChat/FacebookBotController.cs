using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using EntelMvcAPI.FacebookChat.TemplatesFB.TextFB;
using System.Net;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Collections.Generic;

namespace EntelMvcAPI.FacebookChat
{
    public class FacebookBotController : Controller
    {
        
        public ActionResult Receive()
        {
            var query = HttpContext.Request.Query;


            if (query["hub.mode"].ToString() == "subscribe" &&
                query["hub.verify_token"].ToString() == "gofurther")
            {
                var retVal = query["hub.challenge"].ToString();

                if (retVal == null)
                {
                    return NotFound();
                }

                return Json(int.Parse(retVal));
            }
            else
            {
                return NotFound();
            }
        }


        [ActionName("Receive")]
        [HttpPost]
        public ActionResult ReceivePost(BotRequest data)
        {

            if (data.entry.Count == 0)
            {
                return new StatusCodeResult(StatusCodes.Status204NoContent);
            }

            try
            {
                Task.Factory.StartNew(() =>
                {
                    foreach (var entry in data.entry)
                    {
                        foreach (var message in entry.messaging)
                        {
                            var text = message?.message?.text;
                            var payload = message?.postback?.payload;


                            string msg = null;

                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                msg = text;
                            }


                            if (string.IsNullOrWhiteSpace(msg))
                            {
                                continue;
                            }


                            PostJsonAsync(GetTextFB(message.sender.id, "Hola desde AjeGroup Core API para Facebook!"));
                        }

                    }
                });

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }


            return new StatusCodeResult(StatusCodes.Status200OK);
        }


        private async void PostJsonAsync(object data)
        {
            var access_token = "EAAEZBI9ILQHoBALxxwihl2FPHBmY5a5A8jJBKqzsuZBt6ulBcKZAcZCkOARFXlhsSGGjMkM698BO654gbkuK3euA4XxozlmzEXFKF7aopzRH0NoZCBOk5OxTTJcpxdHWphZAyZA55sjbcu1e4nLa0u01dcTMWq3LMO0lhDC2TbijgZDZD";
            string url = "https://graph.facebook.com/v2.6/me/messages?access_token=" + access_token;

            var json = JsonConvert.SerializeObject(data);


            //// MVC Legacy Code *************************************************************
            //var request = (HttpWebRequest)WebRequest.Create(url);

            //request.ContentType = "application/json";
            //request.Method = "POST";


            //using (var requestWriter = new System.IO.StreamWriter(request.GetRequestStream()))
            //{
            //    requestWriter.Write(json);
            //}

            //var response = (HttpWebResponse)request.GetResponse();

            //if (response == null)
            //    throw new InvalidOperationException("GetResponse returns null");

            //using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
            //{
            //    return sr.ReadToEnd();
            //}


            //// ASp Net Core Alternative 1 ***************************************
            //using (var client = new HttpClient())
            //{
            //    try
            //    {
            //        client.BaseAddress = new Uri(url);
            //        var response = await client.GetAsync(json);
            //        response.EnsureSuccessStatusCode(); // Throw in not success

            //    }
            //    catch (HttpRequestException e)
            //    {
            //        Console.WriteLine($"Request exception: {e.Message}");
            //    }
            //}


            //// ASp Net Core Alternative  ***************************************
            //encoder
            UTF8Encoding enc = new UTF8Encoding();

           
            //Create request
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            //Set data in request
            Stream dataStream = await request.GetRequestStreamAsync();
            dataStream.Write(enc.GetBytes(json), 0, json.Length);


            //Get the response
            WebResponse wr = await request.GetResponseAsync();
            Stream receiveStream = wr.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            string content = reader.ReadToEnd();


        }

        public TextTemplate GetTextFB(string senderId, string senderText)
        {

            TextTemplate model = new TextTemplate()
            {
                recipient = new TemplatesFB.TextFB.Recipient()
                {
                    id = senderId
                },
                message = new TemplatesFB.TextFB.Message()
                {
                    text = senderText
                }
            };

            return model;
        }


        public static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }


    }

}