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
            ArandaTicket model = new ArandaTicket();

            if (string.IsNullOrEmpty(_user.CodeUser))
            {
                return model;
            }

            string _openedDate = DateTime.UtcNow.ToUniversalTime().ToString("O");   //2017-08-07T14:48:43.413
            string _msgAddedTicket = "Favor de cambiar la clave (desde Chatbot Web)";
            string _comment = "Cambio de contraseña exitosa";
            string _codeUser = _user.CodeUser;

            XmlDocument xmlRequest = new XmlDocument();

            xmlRequest.LoadXml($@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:tem=""http://tempuri.org/"">
               <soap:Header/>
               <soap:Body>
                  <tem:Add>
                     <tem:item>

                        <tem:AuthorId>{_codeUser}</tem:AuthorId>
                        <tem:ClosureId>17</tem:ClosureId>
                        <tem:CategoryId>594</tem:CategoryId>
                        <tem:StatusId>7</tem:StatusId>
                        <tem:CustomerId>{_codeUser}</tem:CustomerId>
                        <tem:ReceptorId>4339</tem:ReceptorId>
                        <tem:ResponsibleId>4339</tem:ResponsibleId>
                        <tem:Commentary>{_comment}</tem:Commentary>
                        <tem:Description>{_msgAddedTicket}</tem:Description>

                        <tem:EstimatedCost>0.00</tem:EstimatedCost>

                        <tem:OpenedDate>{_openedDate}</tem:OpenedDate>

                        <tem:AttentionRealDate>{_openedDate}</tem:AttentionRealDate>
                        <tem:AttentionEstimatedDate>{_openedDate}</tem:AttentionEstimatedDate>
                        <tem:SolutionRealDate>{_openedDate}</tem:SolutionRealDate>
                        <tem:ClosedDate>{_openedDate}</tem:ClosedDate>
                        <tem:ExpiredDate>{_openedDate}</tem:ExpiredDate>

                        <tem:ModifierId>1</tem:ModifierId>
                        <tem:ProjectId>1</tem:ProjectId>
                        <tem:GroupId>4</tem:GroupId>
                        <tem:ImpactId>2</tem:ImpactId>

                        <tem:PriorityId>2</tem:PriorityId>
                        <tem:CurrentProgress>0</tem:CurrentProgress>

                        <tem:RegistryTypeId>2</tem:RegistryTypeId>
                        <tem:ServiceId>3</tem:ServiceId>
                        <tem:SlaId>36</tem:SlaId>
                        <tem:CurrentTime>5</tem:CurrentTime>
                        <tem:UrgencyId>3</tem:UrgencyId>
   
                        <tem:DescriptionNoHtml>{_msgAddedTicket}</tem:DescriptionNoHtml>
                        <tem:CommentaryNoHtml>{_comment}</tem:CommentaryNoHtml>
         
                     </tem:item>
                  </tem:Add>
               </soap:Body>
            </soap:Envelope>"
            );

            XmlDocument xmlResponse = await InvokeSOAPServiceAsync(xmlRequest, "http://172.36.0.141/asdkws/ServiceCall.asmx", "http://tempuri.org/Add");

            XmlNodeList xnList = xmlResponse.GetElementsByTagName("AddResponse");

            model.TicketNumber = xnList[0].InnerText;


            return model;
        }


        public static async Task<string> GetArandaProjectTicketAsync(string _ticket)
        {
            string model = string.Empty;

            if (string.IsNullOrEmpty(_ticket))
            {
                return model;
            }

            XmlDocument xmlRequest = new XmlDocument();

            xmlRequest.LoadXml($@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
              <soap:Body>
                <GetObject xmlns=""http://tempuri.org/"">
                  <id>{_ticket}</id>
                </GetObject>
              </soap:Body>
            </soap:Envelope>"
            );

            XmlDocument xmlResponse = await InvokeSOAPServiceAsync(xmlRequest, "http://172.36.0.141/asdkws/ServiceCall.asmx", "http://tempuri.org/GetObject");

            XmlNodeList xnList = xmlResponse.GetElementsByTagName("GetObjectResult");

  
           
            foreach (XmlNode item in xnList)
            {
                model = item["IdbyProject"].InnerText;
            }


            return model;
        }


        public static async Task<ArandaUser> GetArandaUserInfo(string _email)
        {

            XmlDocument xmlRequest = new XmlDocument();

            xmlRequest.LoadXml($@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
               <soapenv:Header/>
               <soapenv:Body>
                  <tem:ListSearch>
                     <!--Optional:-->
                      <tem:filter>usuarios.email like '%{_email}%' </tem:filter>
                  </tem:ListSearch>
               </soapenv:Body>
            </soapenv:Envelope>"
            );

            XmlDocument xmlResponse = await InvokeSOAPServiceAsync(xmlRequest, "http://172.36.0.141/asdkws/User.asmx", "http://tempuri.org/ListSearch");

            XmlNodeList xnList = xmlResponse.GetElementsByTagName("User");
            XmlNode xn = xnList[0];

            ArandaUser model = new ArandaUser();

            if (xn != null)
            {
                model.CodeUser = xn["CodeUser"].InnerText;
                model.Email = xn["Email"].InnerText;
                model.Uname = xn["Uname"].InnerText;
                model.UserName = xn["UserName"].InnerText;
            }


            return model;
        }



        public static async Task<ArandaTicket> UpdateArandaTicketAsync(ArandaUser _user)
        {
            ArandaTicket model = new ArandaTicket();

            if (string.IsNullOrEmpty(_user.CodeUser))
            {
                return model;
            }

            string _openedDate = DateTime.UtcNow.ToUniversalTime().ToString("O");   //2017-08-07T14:48:43.413
            string _msgAddedTicket = "Favor de cambiar la clave (desde Chatbot Web)";
            string _comment = "Cambio de contraseña exitosa";
            string _codeUser = _user.CodeUser;

            XmlDocument xmlRequest = new XmlDocument();

            xmlRequest.LoadXml($@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
            <soap:Body>
                <Update xmlns=""http://tempuri.org/"">
                   <item>
                    <AuthorId>{_codeUser}</AuthorId>
                    <ClosureId>17</ClosureId>
                    <CategoryId>594</CategoryId>
                    <StatusId>7</StatusId>
                    <CustomerId>{_codeUser}</CustomerId>
                    <ReceptorId>4339</ReceptorId>
                    <ResponsibleId>4339</ResponsibleId>
                    <Commentary>{_comment}</Commentary>
                    <Description>{_msgAddedTicket}</Description>

                    <EstimatedCost>0.00</EstimatedCost>
                    <OpenedDate>{_openedDate}</OpenedDate>
                    <AttentionRealDate>{_openedDate}</AttentionRealDate>
                    <AttentionEstimatedDate>{_openedDate}</AttentionEstimatedDate>
                    <SolutionRealDate>{_openedDate}</SolutionRealDate>
                    <ClosedDate>{_openedDate}</ClosedDate>
                    <ExpiredDate>{_openedDate}</ExpiredDate>

                    <ModifierId>1</ModifierId>
                    <ProjectId>1</ProjectId>
                    <GroupId>4</GroupId>
                    <ImpactId>2</ImpactId>

                    <PriorityId>2</PriorityId>
                    <CurrentProgress>0</CurrentProgress>

                    <RegistryTypeId>2</RegistryTypeId>
                    <ServiceId>3</ServiceId>
                    <SlaId>36</SlaId>

                    <CurrentTime>5</CurrentTime>
                    <UrgencyId>3</UrgencyId>

                    <DescriptionNoHtml>{_msgAddedTicket}</DescriptionNoHtml>
                    <CommentaryNoHtml>{_comment}</CommentaryNoHtml>
                  </item>
                </Update>
              </soap:Body>
            </soap:Envelope>"
            );

            XmlDocument xmlResponse = await InvokeSOAPServiceAsync(xmlRequest, "http://172.36.0.141/asdkws/ServiceCall.asmx", "http://tempuri.org/Update");

            XmlNodeList xnList = xmlResponse.GetElementsByTagName("UpdateResponse");

            model.Updated = Boolean.Parse(xnList[0].InnerText);


            return model;
        }



        public class ArandaTicket
        {
            public string TicketNumber { get; set; }
            public bool Updated { get; set; }
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