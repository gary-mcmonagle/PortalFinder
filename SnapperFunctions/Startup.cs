
using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SnapperCore.Services;

[assembly: FunctionsStartup(typeof(SnapperFunctions.Startup))]

namespace SnapperFunctions;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddTransient<IChatGptService>(x => new ChatGptService(Environment.GetEnvironmentVariable("CHAT_GPT_API_KEY")));
        builder.Services.AddTransient<IImageAnalysisService>(x => new ImageAnalysisService(
            Environment.GetEnvironmentVariable("COMPUTER_VISION_ENDPOINT"),
            Environment.GetEnvironmentVariable("COMPUTER_VISION_API_KEY")));
        builder.Services.AddTransient<IScreenShotService, ScreenShotService>();
        builder.Services.AddTransient<IEmailService>(x => new EmailService(
            Environment.GetEnvironmentVariable("EMAIL_CONNECTION_STRING"),
            Environment.GetEnvironmentVariable("EMAIL_FROM"),
            Environment.GetEnvironmentVariable("EMAIL_TO")));
    }
}
