using api.Services.EmailService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class TestEmailController : BaseController
{
    private readonly IEmailService emailService;

    public TestEmailController(IEmailService emailService)
    {
        this.emailService = emailService;
    }

   

    [HttpPost("sendemail")]

    public  IActionResult SendEmail([FromBody] EmailObject model)
    {
        var rrr = "<!DOCTYPE html><html><p>Hello Dave,<br>CODE:<body><main><h3> 123456</h3></main></body></html>";
        var data = emailService.SendEmail(model.ToEmail, "AUTH OTP", rrr);
        return Ok();
    }

    public class EmailObject
    {
        public string ToEmail { get; set; }
    }

}
