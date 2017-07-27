using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookAPI.FacebookChat.TemplatesFB.CarouselFBOld
{
    public class CarouselTemplate
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
        public Attachment attachment { get; set; }
    }


    public class Button
    {
        public string payload { get; set; }

        public string title { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public bool messenger_extensions { get; set; }
        public string webview_height_ratio { get; set; }
        public string fallback_url { get; set; }
    }

    public class Element
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public string image_url { get; set; }
        public List<Button> buttons { get; set; }

        public DefaultAction default_action { get; set; }
    }

    public class Payload
    {
        public string template_type { get; set; }
        public List<Element> elements { get; set; }
        public List<Button> buttons { get; set; }
    }

    public class Attachment
    {
        public string type { get; set; }
        public Payload payload { get; set; }
    }

    public class DefaultAction
    {
        public string type { get; set; }
        public string url { get; set; }
        public bool messenger_extensions { get; set; }
        public string webview_height_ratio { get; set; }
        public string fallback_url { get; set; }
    }

}