using PuppeteerSharp;

namespace SnapperCore.Services;

public class ScreenShotService : IScreenShotService
{
    public async Task<byte[]> TakeScreenShot(string url)
    {
        await new BrowserFetcher().DownloadAsync();
        // Launch the browser
        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        // Navigate to the URL
        using var page = await browser.NewPageAsync();

        await page.GoToAsync(url);

        // Take the screenshot
        var bytes = await page.ScreenshotDataAsync(new ScreenshotOptions { CaptureBeyondViewport = true, FullPage = true });

        // Close the browser
        await browser.CloseAsync();
        return bytes;
    }
}
