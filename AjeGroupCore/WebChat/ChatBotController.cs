using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IBM.VCA.Watson.Watson;
using IBM.VCA.Watson.Watson.Model;
using static IBM.VCA.Watson.Watson.WatsonConversationService;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AjeGroupCore.WebChat
{
    [Route("api/[controller]")]
    public class ChatBotController : Controller
    {
        static Context context;

        private WatsonCredentials credentials = new WatsonCredentials()
        {
            workspaceID = "89fd0c87-c437-4573-9c87-c0d31e721cc8",
            username = "7ecc7e1d-b7a9-472b-9419-a7254411cdd5",
            password = "HQJwcbFZclYL"
        };

        [HttpPost]
        public JsonResult MessageChat(string msg)
        {
            MessageRequest result = Message(msg, context, credentials);

            context = result.Context;

            var json = JsonConvert.SerializeObject(result);

            return Json(json);
        }


    }
}
