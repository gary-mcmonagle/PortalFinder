
using Azure;
using Azure.Communication.Email;

namespace SnapperCore.Services;

public class EmailService : IEmailService
{
    private string _connectionString;
    private string _emailFrom;
    private string _emailTo;

    public EmailService(string connectionString, string emailFrom, string emailTo)
    {
        _connectionString = connectionString;
        _emailFrom = emailFrom;
        _emailTo = emailTo;
    }
    public async Task SendEmail(List<(string, string, string)> shopData)
    {
        var emailClient = new EmailClient(_connectionString);

        await emailClient.SendAsync(
            WaitUntil.Completed,
            new EmailMessage(
                _emailFrom,
                _emailTo,
                new EmailContent("PortalFinder") { Html = GetEmailBody(shopData) }),
            default);
    }


    private string GetEmailBody(List<(string, string, string)> shopData)
    {
        return @$"
        <html>
        <body>
        <h1>PortalFinder</h1>
        <p>Here are the results of your search:</p>
        <table>
        <tr>
        <th>Shop</th>
        <th>Result</th>
        </tr>
        {string.Join('\n', shopData.Select(x => $"<tr><td>{x.Item1}</td><td>{x.Item2}</td></tr>"))}
        </table>
        </body>
        </html>
        ";
    }
}
