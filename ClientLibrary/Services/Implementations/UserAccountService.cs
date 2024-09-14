using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using ClientLibrary.Helper;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations
{
    public class UserAccountService(GetHttpClient getHttpClient) : IUserAccountService
    {
        public const string AuthUrl = "api/authentication";
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            var client = getHttpClient.GetPublicHttpClient();
            var response = await client.PostAsJsonAsync($"{AuthUrl}/register", user);
            if (!response.IsSuccessStatusCode) return new GeneralResponse(false, "Failed to create user");

            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? new GeneralResponse(false, "Failed to create user");
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            var client = getHttpClient.GetPublicHttpClient();
            var response = await client.PostAsJsonAsync($"{AuthUrl}/login", user);
            if (!response.IsSuccessStatusCode) return new LoginResponse(false, "Failed to login");

            return await response.Content.ReadFromJsonAsync<LoginResponse>() ?? new LoginResponse(false, "Failed to login");
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            var client = getHttpClient.GetPublicHttpClient();
            var response = await client.PostAsJsonAsync($"{AuthUrl}/refresh-token", token);
            if (!response.IsSuccessStatusCode) return new LoginResponse(false, "Failed to refresh token");

            return await response.Content.ReadFromJsonAsync<LoginResponse>() ?? new LoginResponse(false, "Failed to refresh token");
        }
    }
}