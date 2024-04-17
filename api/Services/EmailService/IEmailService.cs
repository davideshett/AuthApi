using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Response;

namespace api.Services.EmailService
{
    public interface IEmailService
    {
       Task <GenericResponse> SendEmail(string toEmail, string subject, string message);
    }
}