using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace dg.test.infrastructure
{
    public static class HttpClientExtensions
    {

        public static StringContent BuildRequestContent(object o)
        {
            var json = JsonConvert.SerializeObject(o);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return content;
        }

        public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, string uri, object o)
        {
            var stringContent = BuildRequestContent(o);
            return await client.PostAsync(uri, stringContent);
        }

        public static async Task<HttpResponseMessage> PutAsync(this HttpClient client, string uri, object o)
        {
            var stringContent = BuildRequestContent(o);
            return await client.PostAsync(uri, stringContent);
        }
    }
}
