using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class TownRepository(AppDbContext context) : IGenericRepositoryInterface<Town>
    {
        public Task<GeneralResponse> Create(Town entity)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.Towns.FindAsync(id);
            if (dep is null) return NotFound();

            context.Towns.Remove(dep);
            await CommitAsync();
            return Success();
        }

        public async Task<List<Town>> GetAll() => await context.Towns.ToListAsync();

        public async Task<Town?> GetById(int id) => await context.Towns.FindAsync(id);

        public async Task<GeneralResponse> Update(Town entity)
        {
            var dep = await context.Towns.FindAsync(entity.Id);
            if (dep is null) return NotFound();

            dep.Name = entity.Name;
            context.Towns.Update(dep);
            await CommitAsync();
            return Success();
        }
        private static GeneralResponse NotFound() => new(false, "Sorry, the requested Town was not found.");
        private static GeneralResponse Success() => new(true, "Process completed.");
        private async Task CommitAsync() => await context.SaveChangesAsync();
    }
}