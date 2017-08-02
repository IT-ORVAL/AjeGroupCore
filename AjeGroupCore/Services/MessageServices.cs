using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AjeGroupCore.Services
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("AJE Group", "vcaperuadmin@ajegroup.com"));
            emailMessage.To.Add(new MailboxAddress(email, email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("Html") { Text = message };

            using (var client = new SmtpClient())
            {
                var credentials = new NetworkCredential
                {
                    UserName = "vcaperuadmin@ajegroup.com", // replace with valid value
                    Password = "maximo01" // replace with valid value
                };

                client.LocalDomain = "gmail.com";
                // check your smtp server setting and amend accordingly:
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.Auto).ConfigureAwait(false);
                await client.AuthenticateAsync(credentials);
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }


        private SMSoptions Options = new SMSoptions()
        {
            SMSAccountIdentification = "AC3b8013df8b91cb174177c7fc53582e7b",
            SMSAccountPassword = "4c4f33d28a4d5c9360d80675075faef6",
            SMSAccountFrom = "+17372104316"
        };

        public Task SendSmsAsync(string number, string message)
        {
            try
            {
                var accountSid = Options.SMSAccountIdentification;
                var authToken = Options.SMSAccountPassword;

                TwilioClient.Init(accountSid, authToken);

                number = "+" + number;

                var msg = MessageResource.Create(
                  to: new PhoneNumber(number),
                  from: new PhoneNumber(Options.SMSAccountFrom),
                  body: message);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.InnerException);
            }

           

            return Task.FromResult(0);
        }
    }
}
