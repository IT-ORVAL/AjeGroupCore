using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace AjeGroupCore.WebChat.AjeGroup
{
    public class SOAPservice
    {

        //private static WebRequest CreateSOAPWebRequest()
        //{
           
        //    //Create request
        //    WebRequest request = WebRequest.Create(@"http://172.36.0.141/asdkws/User.asmx");

        //    request.Headers["SOAPAction"] = "http://tempuri.org/ListPerfiles";
        //    request.Method = "POST";
        //    request.ContentType = "text/xml;charset=\"utf-8\"";

        //    return request;
        //}


        public static async Task<XmlDocument> InvokeSOAPServiceAsync(XmlDocument xmlRequest, string endPoint, string action)
        {
            //Create request
            WebRequest request = WebRequest.Create(@endPoint);

            request.Headers["SOAPAction"] = action;
            request.Method = "POST";
            request.ContentType = "text/xml;charset=\"utf-8\"";


            using (Stream stream = await request.GetRequestStreamAsync())
            {
                xmlRequest.Save(stream);
            }

            //Geting response from request    
            using (WebResponse Serviceres = await request.GetResponseAsync())
            {
                using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                {
                    //reading stream    
                    var ServiceResult = rd.ReadToEnd();

                    //XmlSerializer serializer
                    XmlDocument xmlResponse = new XmlDocument();
                    xmlResponse.LoadXml(ServiceResult);



                    return xmlResponse;
                }



            }

        }


        public static async Task<List<string>> GetListPerfilesAsync()
        {
            XmlDocument xmlRequest = new XmlDocument();
           
            xmlRequest.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                <soap:Body>
                    <ListPerfiles xmlns=""http://tempuri.org/"" />
                </soap:Body>
            </soap:Envelope>");

            XmlDocument xmlResponse = await InvokeSOAPServiceAsync(xmlRequest, "http://172.36.0.141/asdkws/User.asmx" , "http://tempuri.org/ListPerfiles");


            XmlNodeList xnList = xmlResponse.GetElementsByTagName("User");

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


        public static async Task<ArandaTicket> SetArandaNewTicketAsync(ArandaUser _user)
        {
            string _openedDate = DateTime.UtcNow.ToUniversalTime().ToString("O");   //2017-08-07T14:48:43.413
            string _msgAddedTicket = "Password changed from Chatbot Web application";

            XmlDocument xmlRequest = new XmlDocument();

            xmlRequest.LoadXml($@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:tem=""http://tempuri.org/"">
               <soap:Header/>
               <soap:Body>
                  <tem:Add>
                     <tem:item>

                        <tem:AuthorId>4338</tem:AuthorId>
                        <tem:CategoryId>594</tem:CategoryId>
                        <tem:StatusId>13</tem:StatusId>
                        <tem:CustomerId>4338</tem:CustomerId>
                        <tem:ReceptorId>2068</tem:ReceptorId>
                        <tem:ResponsibleId>2068</tem:ResponsibleId>
                        <tem:Description>{_msgAddedTicket}</tem:Description>

                        <tem:EstimatedCost>0</tem:EstimatedCost>
                        <tem:OpenedDate>{_openedDate}</tem:OpenedDate>
                        <tem:AttentionEstimatedDate>{_openedDate}</tem:AttentionEstimatedDate>

                        <tem:ModifierId>1</tem:ModifierId>
                        <tem:ProjectId>1</tem:ProjectId>
                        <tem:GroupId>2</tem:GroupId>
                        <tem:ImpactId>2</tem:ImpactId>

                        <tem:PriorityId>1</tem:PriorityId>
                        <tem:CurrentProgress>27</tem:CurrentProgress>

                        <tem:RegistryTypeId>2</tem:RegistryTypeId>
                        <tem:ServiceId>3</tem:ServiceId>
                        <tem:SlaId>35</tem:SlaId>
                        <tem:CurrentTime>3144</tem:CurrentTime>
                        <tem:UrgencyId>2</tem:UrgencyId>
   
                        <tem:DescriptionNoHtml>{_msgAddedTicket}</tem:DescriptionNoHtml>
         
                     </tem:item>
                  </tem:Add>
               </soap:Body>
            </soap:Envelope>"
            );

            XmlDocument xmlResponse = await InvokeSOAPServiceAsync(xmlRequest, "http://172.36.0.141/asdkws/ServiceCall.asmx", "http://tempuri.org/Add");

            XmlNodeList xnList = xmlResponse.GetElementsByTagName("AddResponse");


            ArandaTicket model = new ArandaTicket()
            {
                TicketNumber = xnList[0].InnerText
            };


            return model;
        }


        public static async Task<ArandaUser> GetArandaUserInfo(string _email)
        {
            ArandaUser model = new ArandaUser();

            XmlDocument xmlRequest = new XmlDocument();

            xmlRequest.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
              <soapenv:Header/>
               <soapenv:Body>
                  <tem:ListUserByProject>
                     <tem:projectId>1</tem:projectId>
                  </tem:ListUserByProject>
               </soapenv:Body>
            </soapenv:Envelope>"
            );

            XmlDocument xmlResponse = await InvokeSOAPServiceAsync(xmlRequest, "http://172.36.0.141/asdkws/User.asmx", "http://tempuri.org/ListUserByProject");

            XmlNodeList xnList = xmlResponse.GetElementsByTagName("User");


 

            foreach (XmlNode xn in xnList)
            {
                if (xn["Email"].InnerText == _email)
                {
                    model.CodeUser = xn["CodeUser"].InnerText;
                    model.Email = xn["Email"].InnerText;
                    model.Uname = xn["Uname"].InnerText;
                    model.UserName = xn["UserName"].InnerText;

                    break;
                }

            }


            return model;
        }



        public class ArandaTicket
        {
            public string TicketNumber { get; set; }
        }


        public class ArandaUser
        {
            public string CodeUser { get; set; }
            public string Email { get; set; }
            public string Uname { get; set; }
            public string UserName { get; set; }

            //public ArandaUser()
            //{
            //    Email = "vcaperuuser@ajegroup.com";
            //}
        }
    }
}