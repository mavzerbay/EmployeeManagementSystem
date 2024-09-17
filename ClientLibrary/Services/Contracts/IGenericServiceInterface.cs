using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts;

public interface IGenericServiceInterface<T>
{
    Task<List<T>> GetAll(string baseUrl);
    Task<T> GetById(int id, string baseUrl);
    Task<GeneralResponse> Create(T entity, string baseUrl);
    Task<GeneralResponse> Update(T entity, string baseUrl);
    Task<GeneralResponse> DeleteById(int id, string baseUrl);
}