namespace SnapperCore.Services;

public interface IScreenShotService
{
    public Task<byte[]> TakeScreenShot(string url);
}
