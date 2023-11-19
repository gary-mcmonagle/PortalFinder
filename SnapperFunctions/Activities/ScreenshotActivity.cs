using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SnapperCore.Services;

namespace SnapperFunctions.Activities;

public class ScreenshotActivity
{
    private readonly IScreenShotService _screenShotService;

    public ScreenshotActivity(IScreenShotService screenShotService)
    {
        _screenShotService = screenShotService;
    }

    [FunctionName(nameof(ScreenShotActivity))]
    [StorageAccount("SNAP_STORAGE_ACCOUNT")]
    public async Task<byte[]> ScreenShotActivity([ActivityTrigger] ScreenShotActivityInput input,
    IBinder binder)
    {
        var (url, fileName) = input;
        var outboundBlob = new BlobAttribute($"snaps/{fileName}", FileAccess.Write);
        var bytes = await new ScreenShotService().TakeScreenShot(url);
        using var writer = binder.Bind<Stream>(outboundBlob);
        writer.Write(bytes);
        return bytes;
    }
}


public record ScreenShotActivityInput(string Url, string FileName);
