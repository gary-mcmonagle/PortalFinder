namespace SnapperCore.Services;

public interface IChatGptService
{
    Task<string> AskAboutPortal(List<string> pageText);
    Task<string> Ask(string question);
}
