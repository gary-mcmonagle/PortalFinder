using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SnapperCore.Services;

namespace SnapperFunctions.Activities;

public class EmailActivity
{

    private readonly IEmailService _emailService;

    public EmailActivity(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [FunctionName(nameof(SendEmail))]
    public async Task SendEmail([ActivityTrigger] List<(string, string, string)> shopData)
    {
        await _emailService.SendEmail(shopData);
    }
}