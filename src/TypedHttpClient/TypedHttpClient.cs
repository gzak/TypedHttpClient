using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TypedHttpClient
{
    public class TypedHttpClient : HttpClient, ITypedHttpClient
    {
        private static readonly HttpMethod _patch = new HttpMethod("PATCH");

        public TypedHttpClient() { }
        public TypedHttpClient(HttpMessageHandler handler) : base(handler) { }
        public TypedHttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler) { }

        public async Task<T> GetObjectAsync<T>(string uri)
        {
            var result = await GetAsync(uri);
            if (result.IsSuccessStatusCode)
            {
                var reader = new JsonSerializer();
                using (var stream = new StreamReader(await result.Content.ReadAsStreamAsync()))
                using (var text = new JsonTextReader(stream))
                    return reader.Deserialize<T>(text);
            }
            else
                throw new HttpServerException(result.StatusCode, $"Downstream {(int)result.StatusCode}: GET {uri}");
        }

        public async Task<T> GetObjectAsync<T>(string uri, T schema)
        {
            var result = await GetAsync(uri);
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeAnonymousType(await result.Content.ReadAsStringAsync(), schema);
            else
                throw new HttpServerException(result.StatusCode, $"Downstream {(int)result.StatusCode}: GET {uri}");
        }

        public async Task<HttpResponseMessage> PostObjectAsync<T>(string uri, T obj)
        {
            var content = obj as HttpContent;
            if (content != null) return await PostAsync(uri, content);

            return await PostAsync(uri, Serialize(obj));
        }

        public async Task<HttpResponseMessage> PutObjectAsync<T>(string uri, T obj)
        {
            var content = obj as HttpContent;
            if (content != null) return await PutAsync(uri, content);

            return await PutAsync(uri, Serialize(obj));
        }

        public async Task<HttpResponseMessage> PatchObjectAsync<T>(string uri, JsonPatchDocument<T> obj) where T : class
        {
            return await PatchAsync(uri, Serialize(obj));
        }

        public async Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content)
        {
            return await SendAsync(new HttpRequestMessage(_patch, requestUri)
            {
                Content = content
            });
        }

        //TODO: Remove the below methods once the real version works with .NET Core 1.0
        //Previously found in Microsoft.AspNet.WebApi.Client
        private HttpContent Serialize<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return content;
        }
    }
}