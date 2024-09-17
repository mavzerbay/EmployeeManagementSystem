using System.Net.Http.Json;
using BaseLibrary.Responses;
using ClientLibrary.Helper;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Services.Implementations;

public class GenericServiceImplementation<T>(GetHttpClient getHttpClient) : IGenericServiceInterface<T>
{
    public async Task<List<T>> GetAll(string baseUrl)
    {
        var httpClient = await getHttpClient.GetPrivateHttpClient();
        var results = await httpClient.GetFromJsonAsync<List<T>>($"{baseUrl}/all");
        return results ?? new List<T>();
    }

    public async Task<T> GetById(int id, string baseUrl)
    {
        var httpClient = await getHttpClient.GetPrivateHttpClient();
        var result = await httpClient.GetFromJsonAsync<T>($"{baseUrl}/{id}");
        return result ?? Activator.CreateInstance<T>();
    }

    public async Task<GeneralResponse> Create(T entity, string baseUrl)
    {
        var httpClient = await getHttpClient.GetPrivateHttpClient();
        var response = await httpClient.PostAsJsonAsync($"{baseUrl}", entity);
        var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result ?? new(false, "An error occurred");
    }

    public async Task<GeneralResponse> Update(T entity, string baseUrl)
    {
        var httpClient = await getHttpClient.GetPrivateHttpClient();
        var response = await httpClient.PutAsJsonAsync($"{baseUrl}", entity);
        var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result ?? new(false, "An error occurred");
    }

    public async Task<GeneralResponse> DeleteById(int id, string baseUrl)
    {
        var httpClient = await getHttpClient.GetPrivateHttpClient();
        var response = await httpClient.DeleteAsync($"{baseUrl}/{id}");
        var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
        return result ?? new(false, "An error occurred");
    }
}