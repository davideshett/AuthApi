using api.Services.EmailService;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public  async Task<IActionResult> SendEmail([FromBody] EmailObject model)
    {
        var message = "Hello, this is a test email";
        var data = await emailService.SendEmail(model.ToEmail, "TEST", message);
        return Ok(data);
    }

    public class EmailObject
    {
        public string ToEmail { get; set; }
    }

}
