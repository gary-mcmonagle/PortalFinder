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
        // Set the viewport size if needed (optional)
        await page.SetViewportAsync(new ViewPortOptions
        {
            Width = 1920,
            Height = 1080
        });

        // Take the screenshot
        var bytes = await page.ScreenshotDataAsync();

        // Close the browser
        await browser.CloseAsync();
        return bytes;
    }
}
