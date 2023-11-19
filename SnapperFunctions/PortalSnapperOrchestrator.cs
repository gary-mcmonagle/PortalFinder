using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SnapperCore.Services;
using SnapperFunctions.Activities;

namespace PortalSnapper
{
    public class PortalSnapperOrchestrator
    {
        [FunctionName("PortalSnapperOrchestrator")]
        public async Task<List<ShopSubOrchestratorOutput>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var shops = new List<ShopSubOrchestratorInput>
            {
                new("https://direct.playstation.com/en-gb/buy-accessories/playstation-portal-remote-player", "Sony"),
                new("https://store.ee.co.uk/products/sony-playstation-portal--remote-player-711719580782-HFDK.html", "EE"),
                new("https://www.currys.co.uk/products/sony-playstation-portal-remote-player-10257483.html", "Currys"),
                new ("https://www.very.co.uk/playstation-5-portaltrade-remote-player-for-ps5supregsup-console/1600929640.prd", "very"),
                new ("https://www.game.co.uk/en/playstation-portal-2924759", "Game"),
                new ("https://www.currys.ie/products/sony-playstation-portal-remote-player-10257483.html", "Currys Ireland")
            };
            var results = new List<ShopSubOrchestratorOutput>();
            foreach (var shop in shops)
            {
                var result = await context.CallSubOrchestratorAsync<ShopSubOrchestratorOutput>("ShopSubOrchestrator", shop);
                results.Add(result);
            }
            await context.CallActivityAsync(nameof(EmailActivity.SendEmail), results.Select(x => (x.name, x.result, x.imageUrl)).ToList());
            return results;
        }

        [FunctionName("ShopSubOrchestrator")]
        public async Task<ShopSubOrchestratorOutput> RunShopSubOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var (url, name) = context.GetInput<ShopSubOrchestratorInput>();
            var fileName = $"{name}{context.InstanceId}.png";

            try
            {
                await context.CallActivityAsync<byte[]>(nameof(ScreenshotActivity.ScreenShotActivity),
                new ScreenShotActivityInput(url, fileName));
                var fileUrl = $"{Environment.GetEnvironmentVariable("SNAP_STORAGE_URL")}{fileName}";
                var websiteContent = await context.CallActivityAsync<List<string>>(nameof(ImageAnalysisActivity.AnalyzeImage),
                    new ImageAnalysisActivityInput(fileUrl));

                var result = await context.CallActivityWithRetryAsync<string>(nameof(ChatGptActivity.GenerateChat), new RetryOptions(TimeSpan.FromSeconds(5), 5),
                    new ChatGptActivityInput(websiteContent));
                return new ShopSubOrchestratorOutput(fileUrl, result, name);
            }
            catch
            {
                return new ShopSubOrchestratorOutput("error", "error", name);
            }
        }

        [FunctionName("PortalSnapperOrchestrator_HttpStart")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("PortalSnapperOrchestrator", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("PortalOrchestrator_TimerStart")]
        public async Task TimerStart(
            [TimerTrigger("*/5 */6,12,15,20 * * *")] TimerInfo myTimer,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("PortalSnapperOrchestrator", null);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
        }
    }
}

public record ShopSubOrchestratorInput(string url, string name);
public record ShopSubOrchestratorOutput(string imageUrl, string result, string name);