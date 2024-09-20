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

namespace Hackathon.AI.OpenAI
{
    public class VideoRetrievalService
    {
        OpenAISettings _settings;
        public VideoRetrievalService(IOptions<OpenAISettings> settings) 
        { 
            _settings = settings.Value;
        }
        public async Task<IEnumerable<IngestionIndexModel>> ListIndexes()
        {

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(_settings.Endpoint)
            };
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.Key);
            var result = await client.GetFromJsonAsync<ApiResponse<IEnumerable<IngestionIndexModel>>>
                ($"/computervision/retrieval/indexes?api-version={_settings.ApiVersion}");
            return result.Value;
        }

        public async Task<KeyValuePair<VideoMetadata, VideoIndexStateModel>> AddVideoToIndex(IngestionIndexModel index, string videoUrl, string filename = "unknown", string injestion = "my-injestion")
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(_settings.Endpoint)
            };
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.Key);
            VideoMetadata videoMetadata = new VideoMetadata
            {
                CameraId = $"1",
                Timestamp = DateTime.Now,
                DocumentId = filename
            };

            VideoRequestModel model =
                new VideoRequestModel
                {
                    Videos = new List<VideoModel>
                {
                    new VideoModel
                    {
                         DocumentId = filename,
                         DocumentUrl = videoUrl,
                          Metadata = videoMetadata
                    }
                }
                };
            StringContent content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.SendAsync(
                new HttpRequestMessage(HttpMethod.Put,
                $"/computervision/retrieval/indexes/{index.Name}/ingestions/{injestion}?api-version={_settings.ApiVersion}")
                { Content = content });
            var ret = await result.Content.ReadFromJsonAsync<VideoIndexStateModel>();
            return new KeyValuePair<VideoMetadata, VideoIndexStateModel>(videoMetadata, ret);
        }


        public async Task<IngestionIndexModel> CreateIndex(string indexName)
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(_settings.Endpoint)
            };
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.Key);
            

            CreateIndexModel model =
                new CreateIndexModel
                {
                     MetadataSchema = new MetadataSchemaModel
                     {
                          Fields = new List<MetadataSchemaFieldModel>
                          {
                              new MetadataSchemaFieldModel
                              {
                                    Name = "cameraId",
                                    Searchable = false,
                                     Filterable = true,
                                     Type = "string"
                              },
                              new MetadataSchemaFieldModel
                              {
                                  Name = "timestamp",
                                  Searchable = false,
                                  Filterable = true,
                                  Type = "datetime"
                              }, 
                              new MetadataSchemaFieldModel
                              {
                                  Name = "documentId",
                                  Searchable = false,
                                  Filterable = true,
                                  Type = "string"
                              }
                          }
                     }
                };
            StringContent content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.SendAsync(
                new HttpRequestMessage(HttpMethod.Put,
                $"/computervision/retrieval/indexes/{indexName}?api-version={_settings.ApiVersion}")
                { Content = content });
            var ret = await result.Content.ReadFromJsonAsync<IngestionIndexModel>();
            return ret;
        }

        public async Task<IEnumerable<VideoIndexStateModel>> WaitForInjestionToComplete(IngestionIndexModel index, string injestion)
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

        public async Task<IEnumerable<VisionQueryResponseModel>> SearchWithVisionFeature(string queryText, string indexName, string documentId)
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
                            //new VisionStringFilter
                            //{
                            //     FieldName = "cameraid",
                            //     Values = new List<string>{cameraId}
                            //},
                            new VisionStringFilter
                            {
                                 FieldName = "documentid",
                                 Values = new List<string>{documentId}
                            }
                        }
                  },
                   QueryText = queryText
                };
            string stringBody = JsonSerializer.Serialize(model);
            StringContent content = new StringContent(stringBody, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.SendAsync(
                new HttpRequestMessage(HttpMethod.Post,
                $"/computervision/retrieval/indexes/{indexName}:queryByText?api-version={_settings.ApiVersion}")
                {
                    Content = content
                });
            try
            {
                result.EnsureSuccessStatusCode();
            } catch (Exception ex) {
                throw new Exception($"\"error\": {await result.Content.ReadAsStringAsync()}  \"Body\": {stringBody}");
            }

            var ret = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<VisionQueryResponseModel>>>();
            return ret.Value;
        }
    }
}
