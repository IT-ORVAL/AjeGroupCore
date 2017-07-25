using IBM.VCA.Watson.Watson.Http;
using IBM.VCA.Watson.Watson.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Helpers;

namespace IBM.VCA.Watson.Watson
{
    public class CloudantNoSqlService
    {
        //ChatBot repository
        static string _user = "b76537c4-5a25-4057-8a63-c03473631626-bluemix";
        static string _psw = "cf8e60ee5270ca460fa9a720b634ecada60aa44e45e6f61ae68f612dfc8fa958";
        static string _db = "chatbot-vca";

        //Sensor repository
        static string _user2 = "957f6715-e2c8-45f3-a9a3-77ea2304bd05-bluemix";
        static string _psw2 = "89db651afc6ebc2f39c8d6a8ef00b492e3123f332fa300dd4a23c312ef1a0998";
        static string _db2 = "sensores";
        //static string _all = "_all_docs?limit=1&include_docs=true";
        static string _find = "_find";

        #region CreateHttpClient
        private static HttpClient CreateHttpClient(string user, string psw, string database)
        {
             HttpClientHandler handler = new HttpClientHandler { Credentials = new NetworkCredential(user, psw) };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(string.Format("https://{0}.cloudant.com/{1}/", user, database))
            };
        }
        #endregion CreateHttpClient


        #region CreateHttpClient Filtered
        private static HttpClient CreateHttpClientFiltered(string user, string psw, string database, string filter)
        {
            HttpClientHandler handler = new HttpClientHandler { Credentials = new NetworkCredential(user, psw) };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(string.Format("https://{0}.cloudant.com/{1}/{2}", user, database, filter))
            };
        }
        #endregion CreateHttpClient

        #region Create
        public static HttpResponseMessage Create(object doc)
        {

            using (var client = CreateHttpClient(_user, _psw, _db))
            {
                var json = JsonConvert.SerializeObject(doc, Formatting.None);
                return client.PostAsync("", new StringContent(json, Encoding.UTF8, "application/json")).Result;
            }
        }
        #endregion Create

        #region Read
        public static HttpResponseMessage Read(HttpClient client, string id)
        {
            return client.GetAsync(id).Result;
        }
        #endregion Read

        #region Read Sensor
        public static async System.Threading.Tasks.Task<MessageRequest> ReadSensor(string msg, Context ctx)
        {
            HttpResponseMessage sensor = new HttpResponseMessage();
            HttpResponseMessage docs = new HttpResponseMessage();

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


            //using (var client = CreateHttpClient(_user2, _psw2, _db2))
            //{
            //    docs = client.GetAsync(_all).Result;
            //}
            
            //CloudantModel model = await docs.Content.ReadAsAsync<CloudantModel>();

            string strEscape = "{\"selector\": {\"_id\": {\"$gt\": 0}},\"fields\": [\"_id\",\"_rev\",\"payload\"],\"sort\": [{\"_id\": \"desc\"}],\"limit\": 1}";

            var selector = new StringContent(strEscape, Encoding.UTF8, "application/json");

            using (var client = CreateHttpClient(_user2, _psw2, _db2))
            {
                sensor = client.PostAsync(_find, selector).Result;
            }

            CloudantFiltered2 model2 = await sensor.Content.ReadAsAsync<CloudantFiltered2>();


            //string temperature = model.rows[0].doc.payload.d.temperatura.ToString();
            //string humidity = model.rows[0].doc.payload.d.humedad.ToString();

            string temperature = model2.docs[0].payload.d.temperatura.ToString();
            string humidity = model2.docs[0].payload.d.humedad.ToString();

            string forecast = string.Format("El sensor indica una temperatura de {0}°C y una humedad de {1}%.", temperature, humidity);

            List<string> list = new List<string>();

            list.Add(forecast);

            OutputData output = new OutputData
            {
                Text = list
            };


            messageRequest.Output = output;

            return messageRequest;
        }
        #endregion Read

        #region Update
        public static HttpResponseMessage Update(HttpClient client, string id, object doc)
        {
            var json = JsonConvert.SerializeObject(doc, Formatting.None);
            return client.PutAsync(id, new StringContent(json, Encoding.UTF8, "application/json")).Result;
        }
        #endregion Update

        #region Delete
        public static HttpResponseMessage Delete(HttpClient client, string id, string rev)
        {
            return client.DeleteAsync(id + "?rev=" + rev).Result;
        }
        #endregion Delete
    }
}