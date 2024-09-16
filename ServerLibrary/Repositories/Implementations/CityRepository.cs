using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class CityRepository(AppDbContext context) : IGenericRepositoryInterface<City>
    {
        public Task<GeneralResponse> Create(City entity)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.Cities.FindAsync(id);
            if (dep is null) return NotFound();

            context.Cities.Remove(dep);
            await CommitAsync();
            return Success();
        }

        public async Task<List<City>> GetAll() => await context.Cities.ToListAsync();

        public async Task<City?> GetById(int id) => await context.Cities.FindAsync(id);

        public async Task<GeneralResponse> Update(City entity)
        {
            var dep = await context.Cities.FindAsync(entity.Id);
            if (dep is null) return NotFound();

            dep.Name = entity.Name;
            context.Cities.Update(dep);
            await CommitAsync();
            return Success();
        }
        private static GeneralResponse NotFound() => new(false, "Sorry, the requested City was not found.");
        private static GeneralResponse Success() => new(true, "Process completed.");
        private async Task CommitAsync() => await context.SaveChangesAsync();
    }
}