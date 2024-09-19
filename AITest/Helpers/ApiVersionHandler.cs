using System.Web;

namespace AITest.Helpers
{
    public class ApiVersionHandler : DelegatingHandler
    {
        private const string _ApiVersionKey = "api-version";
        private string _NewApiVersion = "2023-12-01-preview";

        public ApiVersionHandler(string apiversion = "2023-03-15-preview") : base(new HttpClientHandler())
        {
            _NewApiVersion = apiversion;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var uriBuilder = new UriBuilder(request.RequestUri!);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (query[_ApiVersionKey] == null) return await base.SendAsync(request, cancellationToken);

            query[_ApiVersionKey] = _NewApiVersion;
            uriBuilder.Query = query.ToString();
            request.RequestUri = uriBuilder.Uri;

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
