using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;

namespace AjeGroupCore.WebChat.AjeGroup
{
    public class SOAPservice
    {

        private static WebRequest CreateSOAPWebRequest()
        {
            ////Making Web Request    
            //HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(@"http://172.36.0.141/asdkws/User.asmx");
            ////SOAPAction    
            //Req.Headers.Add(@"SOAPAction:http://tempuri.org/ListPerfiles");
            ////Content_type    
            //Req.ContentType = "text/xml;charset=\"utf-8\"";
            //Req.Accept = "text/xml";
            ////HTTP method    
            //Req.Method = "POST";
            ////return HttpWebRequest    
            //return Req;


            //Create request
            WebRequest request = WebRequest.Create(@"http://172.36.0.141/asdkws/User.asmx");

            request.Headers["SOAPAction"] = "http://tempuri.org/ListPerfiles";
            request.Method = "POST";
            request.ContentType = "text/xml;charset=\"utf-8\"";

            return request;
        }


        public static async System.Threading.Tasks.Task<List<string>> InvokeSOAPServiceAsync()
        {
            //Calling CreateSOAPWebRequest method    
            WebRequest request = CreateSOAPWebRequest();

            XmlDocument SOAPReqBody = new XmlDocument();
            //SOAP Body Request    
            SOAPReqBody.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                <soap:Body>
                    <ListPerfiles xmlns=""http://tempuri.org/"" />
                </soap:Body>
            </soap:Envelope>");



            using (Stream stream = await request.GetRequestStreamAsync())
            {
                SOAPReqBody.Save(stream);
            }

            //Geting response from request    
            using (WebResponse Serviceres = await request.GetResponseAsync())
            {
                using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                {
                    //reading stream    
                    var ServiceResult = rd.ReadToEnd();

                    //XmlSerializer serializer
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(ServiceResult);

                    XmlNodeList xnList = xml.GetElementsByTagName("User");

                    List<string> model = new List<string>();

                    string mylist = null;

                    foreach (XmlNode xn in xnList)
                    {
                        string firstName = xn["Uname"].InnerText;
                        string lastName = xn["UserActive"].InnerText;

                        mylist = mylist + string.Format("Perfil: {0} {1}", firstName, lastName) + "<br />";

                    }

                    model.Add(mylist);

                    return model;
                }



            }

        }
    }
}