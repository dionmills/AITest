using System.Net;

namespace AITest.Helpers
{
    public static class ImageExtensions
    {
        public static ReadOnlyMemory<byte> ToReadOnlyMemory(this string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                byte[] imageBytes = httpClient.GetByteArrayAsync(url).Result;
                return new ReadOnlyMemory<byte>(imageBytes);
            }
        }
    }
}
