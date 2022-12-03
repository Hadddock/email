using Microsoft.AspNetCore.Mvc;
using API.DTOs;
using Message;


namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<MessageController> _logger;

    public MessageController(ILogger<MessageController> logger)
    {
        _logger = logger;
    }

    [HttpPost(Name = "SendEmail")]
    public string Post(MessageDto messageDto)
    {
        if
        (
            messageDto.messageBody is not null &&
            messageDto.messageSubject is not null &&
            messageDto.recipientEmail is not null
        )

        {
            Message.Program.Main(new string[] { messageDto.recipientEmail, messageDto.messageSubject, messageDto.messageBody });
        }

        return "test";
    }
}
