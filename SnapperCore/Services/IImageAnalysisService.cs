namespace SnapperCore.Services;

public interface IImageAnalysisService
{
    Task<List<string>> Scan(string imageUrl);
}
