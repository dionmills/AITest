
using AITest.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Hackathon.AI.Models.Settings;

namespace AITest;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IConfigurationSection config = builder.Configuration.GetSection("OpenAI");


        builder.Services.AddControllers();

        //builder.Services.AddAzureOpenAIChatCompletion(
        //            deploymentName: "AzureAITest",
        //            apiKey: config.GetSection("Key").Value!,
        //            endpoint: config.GetSection("Endpoint").Value!,
        //            modelId: "gpt-4");

        //builder.Services.AddTransient((serviceProvider) => new Kernel(serviceProvider));
        builder.Services.AddSingleton<ChatHistoryService>();
        builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
