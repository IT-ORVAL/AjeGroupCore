using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookAPI.FacebookChat.TemplatesFB.TextFB
{

    public class TextTemplate
    {
        public Recipient recipient { get; set; }
        public Message message { get; set; }
    }

    public class Recipient
    {
        public string id { get; set; }
    }


    public class Message
    {
        public string text { get; set; }
    }
}