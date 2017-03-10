using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TypedHttpClient
{
    public interface ITypedHttpClient : IDisposable
    {
        Task<T> GetObjectAsync<T>(string uri);
        Task<T> GetObjectAsync<T>(string uri, T schema);
        Task<HttpResponseMessage> PostObjectAsync<T>(string uri, T obj);
        Task<HttpResponseMessage> PutObjectAsync<T>(string uri, T obj);
        Task<HttpResponseMessage> PatchObjectAsync<T>(string uri, JsonPatchDocument<T> obj) where T : class;

        Task<HttpResponseMessage> GetAsync(string requestUri);
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content);
        Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content);
        Task<HttpResponseMessage> DeleteAsync(string requestUri);
    }
}