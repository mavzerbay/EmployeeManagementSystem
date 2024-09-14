using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BaseLibrary.DTOs;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Helper
{
    public class CustomHttpHandler(LocalStorageService localStorageService, IUserAccountService accountService) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool login = request.RequestUri!.AbsoluteUri.Contains("login");
            bool register = request.RequestUri!.AbsoluteUri.Contains("register");
            bool refreshToken = request.RequestUri!.AbsoluteUri.Contains("refresh-token");

            if (login || register || refreshToken)
                return await base.SendAsync(request, cancellationToken);

            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                string? token = await localStorageService.GetToken();
                if (string.IsNullOrEmpty(token)) return response;

                token = request.Headers.Authorization?.Parameter ?? string.Empty;
                var deserializedToken = Serializations.DeserializeObj<UserSession>(token);
                if (deserializedToken is null) return response;

                if (string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", deserializedToken.Token);
                    return await base.SendAsync(request, cancellationToken);
                }

                var newJwtToken = await GetRefreshTokenAsync(deserializedToken.RefreshToken);
                if (string.IsNullOrEmpty(newJwtToken)) return response;

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newJwtToken);
                return await base.SendAsync(request, cancellationToken);

            }
            return response;
        }

        private async Task<string> GetRefreshTokenAsync(string? refreshToken)
        {
            var response = await accountService.RefreshTokenAsync(new RefreshToken { Token = refreshToken });
            string serializedToken = Serializations.SerializeObj(new UserSession { Token = response.Token, RefreshToken = response.RefreshToken });
            await localStorageService.SetToken(serializedToken);
            return response.Token;
        }
    }
}