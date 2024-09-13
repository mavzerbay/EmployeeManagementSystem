using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BaseLibrary.DTOs;

namespace ClientLibrary.Helper
{
    public class GetHttpClient(IHttpClientFactory httpClientFactory, LocalStorageService localStorageService)
    {
        private const string HeaderKey = "Authorization";

        public async Task<HttpClient> GetPrivateHttpClient(){
            var client = httpClientFactory.CreateClient("SystemApiClient");
            var token = await localStorageService.GetToken();
            if(string.IsNullOrEmpty(token)) return client;

            var deserializeToken = Serializations.DeserializeObj<UserSession>(token);
            if(deserializeToken is null) return client;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", deserializeToken.Token);
            return client;
        }

        public HttpClient GetPublicHttpClient()
        {
            var client = httpClientFactory.CreateClient("SystemApiClient");
            client.DefaultRequestHeaders.Remove(HeaderKey);

            return client;
        }
    }
}