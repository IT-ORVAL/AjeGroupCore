﻿using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using FacebookAPI.FacebookChat.TemplatesFB.TextFB;
using System.Net;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Collections.Generic;
using IBM.VCA.Watson.Watson.Model;
using static IBM.VCA.Watson.Watson.WatsonConversationService;

namespace FacebookAPI.FacebookChat
{
    [Route("api/[controller]")]
    public class FacebookBotController : Controller
    {
        static Context context;

        private WatsonCredentials credentials = new WatsonCredentials()
        {
            workspaceID = "89fd0c87-c437-4573-9c87-c0d31e721cc8",
            username = "7ecc7e1d-b7a9-472b-9419-a7254411cdd5",
            password = "HQJwcbFZclYL"
        };


        [HttpGet]
        public ActionResult WebHook()
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


        [HttpPost]
        public ActionResult WebHook([FromBody] BotRequest data)
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


                            MessageRequest result = Message(msg, context, credentials);

                            context = result.Context;

                            foreach (string item in result.Output.Text)
                            {
                                PostJson(GetTextFB(message.sender.id, item));
                            }
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


        private async void PostJson(object data)
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
            ////encoder
            //UTF8Encoding enc = new UTF8Encoding();

           
            //Create request
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            //Set data in request
            Stream dataStream = await request.GetRequestStreamAsync();

            //dataStream.Write(enc.GetBytes(json), 0, json.Length);

            using (var requestWriter = new StreamWriter(dataStream))
            {
                requestWriter.Write(json);
            }

            //Get the response
            WebResponse wr = await request.GetResponseAsync();
            Stream receiveStream = wr.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
            string content = reader.ReadToEnd();


        }

        private TextTemplate GetTextFB(string senderId, string senderText)
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


        private static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }


    }

}