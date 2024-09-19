using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hackathon.AI.Models.Api;
using Hackathon.AI.Models.Settings;
using Microsoft.Extensions.Options;
using OpenCvSharp;

namespace Hackathon.AI.OpenAI
{
    public class VideoRetrievalService
    {
        OpenAISettings _settings;
        public VideoRetrievalService(IOptions<OpenAISettings> settings) 
        { 
            _settings = settings.Value;
        }
        public async Task<IEnumerable<GetIngestionIndexResponseModel>> ListIndexes()
        {

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(_settings.Endpoint)
            };
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.Key);
            var result = await client.GetFromJsonAsync<ApiResponse<IEnumerable<GetIngestionIndexResponseModel>>>
                ($"/computervision/retrieval/indexes?api-version={_settings.ApiVersion}");
            return result.Value;
        }
        public async Task<IEnumerable<GetIngestionIndexResponseModel>> CreateIndex(string indexName)
        {
            // TODO: not finished
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(_settings.Endpoint)
            };
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.Key);
            var result = await client.GetFromJsonAsync<ApiResponse<IEnumerable<GetIngestionIndexResponseModel>>>
                ($"/retrieval/indexes/{indexName}?api-version={_settings.ApiVersion}");
            return result.Value;
        }

        public async Task<VideoIndexStateModel> AddVideoToIndex(GetIngestionIndexResponseModel index, string videoUrl, string filename = "unknown", string injestion = "my-injestion")
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(_settings.Endpoint)
            };
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.Key);
            VideoRequestModel model =
                new VideoRequestModel
                {
                    Videos = new List<VideoModel>
                {
                    new VideoModel
                    {
                         DocumentId = filename,
                         DocumentUrl = videoUrl,
                          Metadata = new VideoMetadata
                          {
                               CameraId = "1",
                                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                          }
                    }
                }
                };
            StringContent content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.SendAsync(
                new HttpRequestMessage(HttpMethod.Put,
                $"/computervision/retrieval/indexes/{index.Name}/ingestions/{injestion}?api-version={_settings.ApiVersion}")
                { Content = content });
            var ret = await result.Content.ReadFromJsonAsync<VideoIndexStateModel>();
            return ret;
        }

        public async Task<IEnumerable<VideoIndexStateModel>> WaitForInjestionToComplete(GetIngestionIndexResponseModel index, string injestion)
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(_settings.Endpoint)
            };
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.Key);
            var result = await client.GetFromJsonAsync<ApiResponse<IEnumerable<VideoIndexStateModel>>>
                ($"/computervision/retrieval/indexes/{index.Name}/ingestions?api-version={_settings.ApiVersion}&$top=20");
            return result.Value;
        }

        public async Task<IEnumerable<VisionQueryResponseModel>> SearchWithVisionFeature(string indexName, string queryText)
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(_settings.Endpoint)
            };
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.Key);
            VisionQueryModel model =
                new VisionQueryModel
                {
                  Filters  = new VisionFilters
                  {
                       FeatureFilters = new List<string> { "vision" },
                        StringFilters = new List<VisionStringFilter>
                        {
                            new VisionStringFilter
                            {
                                 FieldName = "cameraId",
                                 Values = new List<string>{"1"}
                            }
                        }
                  },
                   QueryText = queryText
                };
            StringContent content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.SendAsync(
                new HttpRequestMessage(HttpMethod.Post,
                $"/computervision/retrieval/indexes/{indexName}:queryByText?api-version={_settings.ApiVersion}")
                { Content = content });
            var ret = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<VisionQueryResponseModel>>>();
            return ret.Value;
        }
    }
}
