using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class GeneralDepartmentRepository(AppDbContext context) : IGenericRepositoryInterface<GeneralDepartment>
    {
        public async Task<GeneralResponse> Create(GeneralDepartment entity)
        {
            if (await CheckName(entity.Name)) return new(false, "Sorry, the department name is already in use.");
            context.GeneralDepartments.Add(entity);
            await CommitAsync();
            return Success();
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.GeneralDepartments.FindAsync(id);
            if (dep is null) return NotFound();

            context.GeneralDepartments.Remove(dep);
            await CommitAsync();
            return Success();
        }

        public async Task<List<GeneralDepartment>> GetAll() => await context.GeneralDepartments.ToListAsync();

        public async Task<GeneralDepartment?> GetById(int id) => await context.GeneralDepartments.FindAsync(id);

        public async Task<GeneralResponse> Update(GeneralDepartment entity)
        {
            var dep = await context.GeneralDepartments.FindAsync(entity.Id);
            if (dep is null) return NotFound();

            dep.Name = entity.Name;
            context.GeneralDepartments.Update(dep);
            await CommitAsync();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry, the requested department was not found.");
        private static GeneralResponse Success() => new(true, "Process completed.");
        private async Task CommitAsync() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name) => await context.GeneralDepartments.AnyAsync(dep => dep.Name == name);
    }
}