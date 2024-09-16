using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class BranchRepository(AppDbContext context) : IGenericRepositoryInterface<Branch>
    {
        public Task<GeneralResponse> Create(Branch entity)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.Branches.FindAsync(id);
            if (dep is null) return NotFound();

            context.Branches.Remove(dep);
            await CommitAsync();
            return Success();
        }

        public async Task<List<Branch>> GetAll() => await context.Branches.ToListAsync();

        public async Task<Branch?> GetById(int id) => await context.Branches.FindAsync(id);

        public async Task<GeneralResponse> Update(Branch entity)
        {
            var dep = await context.Branches.FindAsync(entity.Id);
            if (dep is null) return NotFound();

            dep.Name = entity.Name;
            context.Branches.Update(dep);
            await CommitAsync();
            return Success();
        }
        private static GeneralResponse NotFound() => new(false, "Sorry, the requested branch was not found.");
        private static GeneralResponse Success() => new(true, "Process completed.");
        private async Task CommitAsync() => await context.SaveChangesAsync();
    }
}