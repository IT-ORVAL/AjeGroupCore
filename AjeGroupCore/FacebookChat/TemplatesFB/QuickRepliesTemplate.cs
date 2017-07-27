using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookAPI.FacebookChat.TemplatesFB.QuickRepliesFB
{
    public class QuickRepliesTemplate
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
        public List<QuickReply> quick_replies { get; set; }
    }

    public class QuickReply
    {
        public string content_type { get; set; }
        public string title { get; set; }
        public string payload { get; set; }
    }


}


//messageData ={
//                        "text":"Elige uno de los siguientes tipo de Contenido:",
//                                "quick_replies":[
//                                {
//                                    "content_type":"text",
//                                    "title":"Texto",
//                                    "payload":"texto"
//                                },
//                                {
//                                    "content_type":"text",
//                                    "title":"Audio",
//                                    "payload":"Audio"
//                                },
//                                {
//                                    "content_type":"text",
//                                    "title":"Imagen",
//                                    "payload":"picture"
//                                },
//                                {
//                                    "content_type":"text",
//                                    "title":"Video",
//                                    "payload":"video"
//                                },
//                                {
//                                    "content_type":"text",
//                                    "title":"Archivo",
//                                    "payload":"archivo"
//                                },
//                                {
//                                    "content_type":"text",
//                                    "title":"Ubicacion",
//                                    "payload":"ubicacion"
//                                }
//                                ]

                              
//                };