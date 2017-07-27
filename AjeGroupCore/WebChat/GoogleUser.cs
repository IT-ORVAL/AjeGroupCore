using AjeGroupCore.Controllers;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AjeGroupCore.WebChat
{
    public class GoogleUser
    {
        static readonly string[] Scopes = { DirectoryService.Scope.AdminDirectoryUser };
        private const string ApplicationName = "AJE Group ChatBot";
        private const string ClientSecretJsonFile = "AJE_Client_Secret.json";
        private const string GoogleFolder = "Google";


        public static string RunPasswordReset(string userEmailString, string userPassword)
        {
            string msg;
            //var webRootInfo = _wwwRoot.WebRootPath;


            try
            {
                ////Set location for Google Token to be locally stored
                //var googleTokenLocation = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), GoogleFolder);

                var googleTokenLocation = Path.Combine(HomeController._wwwRoot.WebRootPath, GoogleFolder);


                //Load the Client Configuration in JSON Format as a stream which is used for API Calls
                var fileStream = new FileStream(ClientSecretJsonFile, FileMode.Open, FileAccess.Read);

                //This will create a Token Response User File on the GoogleFolder indicated on your Application
                var credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(fileStream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                  new FileDataStore(googleTokenLocation)).Result;

                //Create Directory API service.
                var directoryService = new DirectoryService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credentials,
                    ApplicationName = ApplicationName
                });

                //Email is considered the Primary on Google Accoutns
                var userkey = userEmailString;

                //Set User attributes, in this example the password.
                var userBody = new User
                {
                    Password = userPassword
                };

                //Prepares the update request
                var updateRequest = directoryService.Users.Update(userBody, userkey);

                //Executes the update request
                updateRequest.Execute();

                msg = "Clave modificada!";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                //Add you exception here
            }

            return msg;
        }



        public static bool IsEmailRegistered(string userEmailString)
        {

            //var webRootInfo = _wwwRoot.WebRootPath;


            try
            {
                ////Set location for Google Token to be locally stored
                //var googleTokenLocation = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), GoogleFolder);


                var googleTokenLocation = Path.Combine(HomeController._wwwRoot.WebRootPath, GoogleFolder);

                //Load the Client Configuration in JSON Format as a stream which is used for API Calls
                var fileStream = new FileStream(ClientSecretJsonFile, FileMode.Open, FileAccess.Read);

                //This will create a Token Response User File on the GoogleFolder indicated on your Application
                var credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(fileStream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                  new FileDataStore(googleTokenLocation)).Result;

                //Create Directory API service.
                var directoryService = new DirectoryService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credentials,
                    ApplicationName = ApplicationName
                });

                //Email is considered the Primary on Google Accoutns
                var userkey = userEmailString;

                UsersResource.GetRequest getUser = directoryService.Users.Get(userkey);
                User _user = getUser.Execute();

                if (_user == null)
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
                //Add you exception here
            }

            return true;
        }


        public static string GenerateVerificationCode(string userKey)
        {
            var googleTokenLocation = Path.Combine(HomeController._wwwRoot.WebRootPath, GoogleFolder);
            var fileStream = new FileStream(ClientSecretJsonFile, FileMode.Open, FileAccess.Read);

            var credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(fileStream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
              new FileDataStore(googleTokenLocation)).Result;

            var _service = new DirectoryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = ApplicationName,
            });

            var generateVerificationCodesRequest = _service.VerificationCodes.Generate(userKey);
            generateVerificationCodesRequest.Execute();

            var verificationCodesRequest = _service.VerificationCodes.List(userKey);
            var verificationCodes = verificationCodesRequest.Execute();

            var verificationCode = verificationCodes.Items[0].VerificationCodeValue;

            return verificationCode;
        }

    }
}
