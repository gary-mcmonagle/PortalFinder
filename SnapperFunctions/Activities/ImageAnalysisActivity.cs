using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SnapperCore.Services;

namespace SnapperFunctions.Activities;

public class ImageAnalysisActivity
{

    private readonly IImageAnalysisService _imageAnalysisService;

    public ImageAnalysisActivity(IImageAnalysisService imageAnalysisService)
    {
        _imageAnalysisService = imageAnalysisService;
    }

    [FunctionName(nameof(AnalyzeImage))]
    public async Task<List<string>> AnalyzeImage([ActivityTrigger] ImageAnalysisActivityInput input)
    {
        return await _imageAnalysisService.Scan(input.fileUrl);
    }
}


public record ImageAnalysisActivityInput(string fileUrl);
