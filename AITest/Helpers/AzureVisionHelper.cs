using Azure.AI.Vision.ImageAnalysis;
using Azure;
using Microsoft.Extensions.Options;
using Hackathon.AI.Models.Settings;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using OpenAI.Files;
using System.IO;
using VideoFrameAnalyzer;
using System.Collections;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;

namespace AITest.Helpers
{
    public class AzureVisionHelper
    {
        private readonly OpenAISettings _settings;
        public AzureVisionHelper(IOptions<OpenAISettings> settings)
        {
            _settings = settings.Value;
        }
        public ImageAnalysisResult AnalyseImage(string imageUrl)
        {
            string endpoint = _settings.Endpoint;
            string key = _settings.Key;

            ImageAnalysisClient client = new ImageAnalysisClient(
                new Uri(endpoint),
                new AzureKeyCredential(key));

            ImageAnalysisResult result = client.Analyze(
                new Uri(imageUrl),
                VisualFeatures.Caption | VisualFeatures.Read,
                new ImageAnalysisOptions { GenderNeutralCaption = true });

            return result;
        }
        public ImageAnalysisResult AnalyseImage(FormFileCollection stream)
        {
            Stream FileStream = stream.First().OpenReadStream();
            BinaryData bin = BinaryData.FromStream(FileStream);
            string endpoint = _settings.Endpoint;
            string key = _settings.Key;

            ImageAnalysisClient client = new ImageAnalysisClient(
                new Uri(endpoint),
                new AzureKeyCredential(key));

            ImageAnalysisResult result = client.Analyze(
                bin,
                VisualFeatures.Caption
                | VisualFeatures.Read
                | VisualFeatures.Tags
                | VisualFeatures.DenseCaptions
                | VisualFeatures.Objects
                | VisualFeatures.People,
                new ImageAnalysisOptions { GenderNeutralCaption = true });

            return result;
        }


        public async Task<IEnumerable<ImageAnalysisResult>> AnalyseVideo(FormFileCollection video)
        {
            List<ImageAnalysisResult> results = new();
            Stream FileStream = video.First().OpenReadStream();
            BinaryData bin = BinaryData.FromStream(FileStream);
            string endpoint = _settings.Endpoint;
            string key = _settings.Key;

            // Create grabber.
            FrameGrabber<ImageAnalysisResult> grabber = new FrameGrabber<ImageAnalysisResult>();

            // Create Face Client.
            ImageAnalysisClient client = new ImageAnalysisClient(
                new Uri(endpoint),
                new AzureKeyCredential(key));

            // Set up a listener for when we acquire a new frame.
            grabber.NewFrameProvided += (s, e) =>
                {
                    Console.WriteLine($"New frame acquired at {e.Frame.Metadata.Timestamp}");
                };

            // Set up a Face API call.
            grabber.AnalysisFunction = async frame =>
            {
                Console.WriteLine($"Submitting frame acquired at {frame.Metadata.Timestamp}");
                // Encode image and submit to Face service.
                return (await client.AnalyzeAsync(BinaryData.FromStream(frame.Image.ToMemoryStream(".jpg")),
                VisualFeatures.Caption
                | VisualFeatures.Read
                | VisualFeatures.Tags
                | VisualFeatures.DenseCaptions
                | VisualFeatures.Objects
                | VisualFeatures.People,
                new ImageAnalysisOptions { GenderNeutralCaption = true }));
            };

            // Set up a listener for when we receive a new result from an API call.
            grabber.NewResultAvailable += (s, e) =>
            {
                if (e.TimedOut)
                    Console.WriteLine("API call timed out.");
                else if (e.Exception != null)
                    Console.WriteLine("API call threw an exception.");
                else
                {
                    results.Add(e.Analysis);
                    Console.WriteLine($"New result received for frame acquired at {e.Frame.Metadata.Timestamp}. {e.Analysis.Objects.Values.Count} objects detected");
                }
            };

            // Tell grabber when to call the API.
            // See also TriggerAnalysisOnPredicate
            grabber.TriggerAnalysisOnInterval(TimeSpan.FromMilliseconds(100));

            // Start running in the background.
            await grabber.StartProcessingCameraAsync();

            // Wait for key press to stop.
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            //    // Stop, blocking until done.
            //    await grabber.StopProcessingAsync();
            return results;
        }
    }
}
