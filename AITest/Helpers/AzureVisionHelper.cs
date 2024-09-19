using Azure.AI.Vision.ImageAnalysis;
using Azure;
using Microsoft.Extensions.Options;
using Hackathon.AI.Models.Settings;

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
                | VisualFeatures.Tags,
                new ImageAnalysisOptions { GenderNeutralCaption = true });

            return result;
        }
    }
}
