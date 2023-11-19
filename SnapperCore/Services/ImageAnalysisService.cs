using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace SnapperCore.Services;

public class ImageAnalysisService : IImageAnalysisService
{

    private readonly string _endpoint;
    private readonly string _key;

    public ImageAnalysisService(string endpoint, string key)
    {
        _endpoint = endpoint;
        _key = key;
    }

    public async Task<List<string>> Scan(string imageUrl)
    {
        var client = Authenticate();
        Console.WriteLine(client.Endpoint);
        // Read text from URL
        var textHeaders = await client.ReadAsync(imageUrl);
        // After the request, get the operation location (operation ID)
        string operationLocation = textHeaders.OperationLocation;
        Thread.Sleep(5000);

        // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
        // We only need the ID and not the full URL
        const int numberOfCharsInOperationId = 36;
        string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

        // Extract the text
        ReadOperationResult results;
        Console.WriteLine($"Extracting text from URL file {Path.GetFileName(imageUrl)}...");
        Console.WriteLine();
        do
        {
            results = await client.GetReadResultAsync(Guid.Parse(operationId));
        }
        while ((results.Status == OperationStatusCodes.Running ||
            results.Status == OperationStatusCodes.NotStarted));

        var textUrlFileResults = results.AnalyzeResult.ReadResults;
        var lines = textUrlFileResults.Select(x => x.Lines);
        var texts = lines.SelectMany(x => x).Select(x => x.Text).ToList();
        Console.WriteLine();
        return texts ?? new List<string>();
    }
    private ComputerVisionClient Authenticate()
    {
        ComputerVisionClient client =
          new ComputerVisionClient(new ApiKeyServiceClientCredentials(_key))
          { Endpoint = _endpoint };
        return client;
    }
}
