using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SnapperCore.Services;

namespace SnapperFunctions.Activities;

public class ChatGptActivity
{

    private readonly IChatGptService _chatGptService;

    public ChatGptActivity(IChatGptService chatGptService)
    {
        _chatGptService = chatGptService;
    }

    [FunctionName(nameof(GenerateChat))]
    public async Task<string> GenerateChat([ActivityTrigger] ChatGptActivityInput input)
    {
        return await _chatGptService.AskAboutPortal(input.PageText);
    }
}

public record ChatGptActivityInput(List<string> PageText);