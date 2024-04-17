using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helper;
using api.Response;
using api.Settings;
using Microsoft.Extensions.Options;
using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace api.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<SendGridSettings> config;

        public EmailService(IOptions<SendGridSettings> config)
        {
            this.config = config;
        }


        public async Task <GenericResponse> SendEmail(string toEmail, string subject, string message)
        {
                var client = new SendGridClient(config.Value.ApiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("collins.okafor@eChithub.com", subject),
                    Subject = subject,
                    PlainTextContent = message,
                    HtmlContent = message,
                };
                msg.AddTo(new EmailAddress(toEmail));

                msg.SetClickTracking(false, false);

                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    return new GenericResponse
                    {
                        Message = "Email sent",
                        StatusCode = 200,
                        IsSuccessful = true
                    };
                }

                return new GenericResponse
                {
                    Message = "Something went wrong. Email not delivered",
                    StatusCode = 401,
                    IsSuccessful = false
                };

            }
        }
    }