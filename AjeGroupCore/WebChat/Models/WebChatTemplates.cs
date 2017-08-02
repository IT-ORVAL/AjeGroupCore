using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AjeGroupCore.WebChat.Models
{
    public class WebChatTemplates
    {
        public class ButtonListTemplate
        {
            public List<ButtonTemplate> Buttons { get; set; }
        }

        public class ButtonTemplate
        {
            public string HrefLink { get; set; }
            public string Text { get; set; }
        }


        public static string ButtonListConstructor(ButtonListTemplate buttons)
        {
            string model = string.Empty;
            //int total = buttons.Buttons.Count;
            //int counter = 0;

            foreach (var item in buttons.Buttons)
            {
                model = model + "<a class='btn btn-default chatButton' href=" + item.HrefLink + ">" + item.Text + "</a>";

                //if (total > 1 && counter < total)
                //{
                //    model = model + "<br />";
                //}

                //counter = counter + 1;
            }


            return model;
        }


    }

}
