using AITest.Helpers;
using AITest.Services;
using Azure.AI.Vision.ImageAnalysis;
using Azure.Identity;
using Azure.Storage.Blobs.Models;
using Hackathon.AI.Models;
using Hackathon.AI.Models.Api;
using Hackathon.AI.Models.Settings;
using Hackathon.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Text.Json;

namespace AITest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisionController : ControllerBase
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly ChatHistoryService _chatHistoryService;
    private readonly IOptions<OpenAISettings> _settings;
    private readonly AzureVisionHelper _vision;
    private readonly VideoRetrievalService _videoRetrievalService;

    public VisionController(ChatHistoryService chatHistoryService,IOptions<OpenAISettings> settings, AzureVisionHelper vision, VideoRetrievalService videoRetrievalService)
    {
        _settings = settings;
        _vision = vision;
        _videoRetrievalService = videoRetrievalService;
    }


    [HttpPost("Image")]
    public IActionResult Image([FromForm] FormFileCollection image)
    {
        ImageAnalysisResult result = _vision.AnalyseImage(image);
        return Ok(result);
    }
    [HttpPost("Video")]
    public async Task<IActionResult> Video(string videoUrl, string indexName)
    {
        string injestion = Guid.NewGuid().ToString("N");
        IEnumerable<Hackathon.AI.Models.Api.IngestionIndexModel> indexes = (await _videoRetrievalService.ListIndexes())?? new List<IngestionIndexModel>();
        IngestionIndexModel? index = indexes.FirstOrDefault(i => i.Name == indexName);
        if (index == null)
        {
            return NotFound($"Index does not exist: {indexName}");
        }

        KeyValuePair<VideoMetadata, VideoIndexStateModel> result = await _videoRetrievalService.AddVideoToIndex(index, videoUrl, injestion, injestion);

        await Task.Delay(1000);
        bool finished = false;
        while (!finished)
        {
            IEnumerable<VideoIndexStateModel> status = await _videoRetrievalService.WaitForInjestionToComplete(index, injestion);
            var state = status.FirstOrDefault(s => s.Name == injestion);
            if (state != null)
            {
                if (state.State == "Completed")
                {
                    return Ok(new 
                    { 
                        Index = index.Name,
                        State = state,
                        MetaData = result.Key
                    });
                } else if (state.State == "Failed")
                {
                    return Problem(JsonSerializer.Serialize(state));
                }
            } else
            {
                return NotFound($"Injestion {injestion} was not found");
            }
            await Task.Delay(3000);
        }
        return Problem("Something went wrong");
    }

    [HttpGet("InjestionStatus")]
    public async Task<IActionResult> InjestionStatus()
    {
        string injestion = Guid.NewGuid().ToString("N");
        IEnumerable<Hackathon.AI.Models.Api.IngestionIndexModel> indexes = await _videoRetrievalService.ListIndexes();
        IngestionIndexModel index = indexes.First();
        return Ok(await _videoRetrievalService.WaitForInjestionToComplete(index, injestion));
    }
    [HttpGet("IndexList")]
    public async Task<IActionResult> GetIndexList()
    {
        string injestion = Guid.NewGuid().ToString("N");
        return Ok(await _videoRetrievalService.ListIndexes());
    }

    [HttpGet]
    public async Task<IActionResult> QueryVideo(string queryText, string indexName, string documentId )
    {
        try
        {
            IEnumerable<VisionQueryResponseModel> result = await _videoRetrievalService.SearchWithVisionFeature(queryText, indexName, documentId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("Index")]
    public async Task<IActionResult> CreateIndex(string indexName)
    {
        return Ok(await _videoRetrievalService.CreateIndex(indexName));

    }

    [HttpGet("VideoConcerns")]
    public async Task<IActionResult> AnalyseVideo(string documentId)
    {
        Dictionary<string, double> tests = new Dictionary<string, double>
        {
            { "forklift crashes and causes objects to fall quickly to the ground",39},
            { "2 people using a forklift truck",39 },
            { "objects fall and break apart",39 },
            { "a person strikes another person",39 },
            { "a person placing an object in their pocket",39 },
            { "person not sitting in forklift but in contact with forklift",39 },
            { "smoke or fire or explosion",21 }
        };

        List<VideoTestModel> outcomes = new List<VideoTestModel>();

        foreach (KeyValuePair<string,double> test in tests)
        {
            IEnumerable<VisionQueryResponseModel> query = await _videoRetrievalService.SearchWithVisionFeature(test.Key, "aisle-entry-index", documentId);
            double confidence = query.Max(q => q.Relevance)*100;
            outcomes.Add(new VideoTestModel { Confidence = confidence, Query = test.Key, MinimumConfidence = test.Value });
            await Task.Delay(5000);
        }
        return Ok(outcomes.Where(o => o.Confidence > o.MinimumConfidence).Select(o => o.Query));
    }
}
