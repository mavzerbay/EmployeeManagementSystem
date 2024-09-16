using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class CountryRepository(AppDbContext context) : IGenericRepositoryInterface<Country>
    {
        public Task<GeneralResponse> Create(Country entity)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.Countries.FindAsync(id);
            if (dep is null) return NotFound();

            context.Countries.Remove(dep);
            await CommitAsync();
            return Success();
        }

        public async Task<List<Country>> GetAll() => await context.Countries.ToListAsync();

        public async Task<Country?> GetById(int id) => await context.Countries.FindAsync(id);

        public async Task<GeneralResponse> Update(Country entity)
        {
            var dep = await context.Countries.FindAsync(entity.Id);
            if (dep is null) return NotFound();

            dep.Name = entity.Name;
            context.Countries.Update(dep);
            await CommitAsync();
            return Success();
        }
        private static GeneralResponse NotFound() => new(false, "Sorry, the requested Country was not found.");
        private static GeneralResponse Success() => new(true, "Process completed.");
        private async Task CommitAsync() => await context.SaveChangesAsync();
    }
}