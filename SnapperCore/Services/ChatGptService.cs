using ChatGPT.Net;

namespace SnapperCore.Services;

public class ChatGptService : IChatGptService
{
    private readonly ChatGpt _bot;


    public ChatGptService(string apiKey)
    {
        _bot = new ChatGpt(apiKey);
    }

    public async Task<string> Ask(string question)
    {
        var response = await _bot.Ask(question);
        return response;
    }

    public async Task<string> AskAboutPortal(List<string> pageText)
    {
        var question = @$"
        given this as the extracted text of a webpage, would you expect the item to be available for purchase (in stock) ?
        please only answer with one of IN_STOCK, AVAILABLE, OUT_OF_STOCK, or UNKNOWN
        " + string.Join('\n', pageText);
        var response = await _bot.Ask(question);
        return response;
    }
}
