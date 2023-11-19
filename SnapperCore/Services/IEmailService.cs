namespace SnapperCore.Services;

public interface IEmailService
{
    Task SendEmail(List<(string, string, string)> shopData);
}
