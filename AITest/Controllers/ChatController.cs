using AITest.Helpers;
using AITest.Services;
using Azure.AI.Vision.ImageAnalysis;
using Azure.Identity;
using Hackathon.AI.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Net;

namespace AITest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly ChatHistoryService _chatHistoryService;
    private readonly IOptions<OpenAISettings> _settings;
    private readonly AzureVisionHelper _vision;

    public ChatController(ChatHistoryService chatHistoryService,IOptions<OpenAISettings> settings, AzureVisionHelper vision)
    {
        _settings = settings;
        _vision = vision;
        //DelegatingHandler handler = new ApiVersionHandler("2023-03-15-preview");
        //var client = new HttpClient();
        //IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
        //kernelBuilder.AddAzureOpenAIChatCompletion(
        //    deploymentName: "gpt-4o"
        //    , apiKey: _settings.Value.Key
        //    , endpoint: "https://hackathon20240402694327.openai.azure.com/"
        //    , modelId: "gpt-4o"
        //    , httpClient: client
        //    );

        //_kernel = kernelBuilder.Build();
        //_chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        //_chatHistoryService = chatHistoryService;
    }

    //[HttpPost]
    //public async Task<IActionResult> Post(string prompt = "describe this image in detail", string imageUri = "https://upload.wikimedia.org/wikipedia/commons/6/62/Panthera_tigris_sumatran_subspecies.jpg", int maxTokens = 50)
    //{
    //    string sessionId = "default-session";
    //    ChatHistory history = _chatHistoryService.GetOrCreateHistory(sessionId);

    //    _chatHistoryService.AddUserMessage(prompt, imageUri, sessionId);

    //    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings
    //    {
    //        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
    //        MaxTokens = maxTokens
    //    };

    //    ChatMessageContent result = await _chatCompletionService.GetChatMessageContentAsync(history, executionSettings: openAIPromptExecutionSettings, kernel: _kernel);

    //    history.AddAssistantMessage(result.Content ?? string.Empty);

    //    return new JsonResult(new { reply = result.Content });
    //}

    [HttpPost]
    public IActionResult Post(string imageUri = "https://upload.wikimedia.org/wikipedia/commons/6/62/Panthera_tigris_sumatran_subspecies.jpg")
    {
        ImageAnalysisResult result = _vision.AnalyseImage(imageUri);


        return Ok(result);
        //Console.WriteLine("Image analysis results:");
        //Console.WriteLine(" Caption:");
        //Console.WriteLine($"   '{result.Caption.Text}', Confidence {result.Caption.Confidence:F4}");

        //Console.WriteLine(" Read:");
        //foreach (DetectedTextBlock block in result.Read.Blocks)
        //    foreach (DetectedTextLine line in block.Lines)
        //    {
        //        Console.WriteLine($"   Line: '{line.Text}', Bounding Polygon: [{string.Join(" ", line.BoundingPolygon)}]");
        //        foreach (DetectedTextWord word in line.Words)
        //        {
        //            Console.WriteLine($"     Word: '{word.Text}', Confidence {word.Confidence.ToString("#.####")}, Bounding Polygon: [{string.Join(" ", word.BoundingPolygon)}]");
        //        }
        //    }
    }
}
