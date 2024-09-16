using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class DepartmentRepository(AppDbContext context) : IGenericRepositoryInterface<Department>
    {

        public async Task<GeneralResponse> Create(Department entity)
        {
            if (await CheckName(entity.Name)) return new(false, "Sorry, the department name is already in use.");
            context.Departments.Add(entity);
            await CommitAsync();
            return Success();
        }

        public async Task<GeneralResponse> DeleteById(int id)
        {
            var dep = await context.Departments.FindAsync(id);
            if (dep is null) return NotFound();

            context.Departments.Remove(dep);
            await CommitAsync();
            return Success();
        }

        public async Task<List<Department>> GetAll() => await context.Departments.ToListAsync();

        public async Task<Department?> GetById(int id) => await context.Departments.FindAsync(id);

        public async Task<GeneralResponse> Update(Department entity)
        {
            var dep = await context.Departments.FindAsync(entity.Id);
            if (dep is null) return NotFound();

            dep.Name = entity.Name;
            context.Departments.Update(dep);
            await CommitAsync();
            return Success();
        }

        private static GeneralResponse NotFound() => new(false, "Sorry, the requested department was not found.");
        private static GeneralResponse Success() => new(true, "Process completed.");
        private async Task CommitAsync() => await context.SaveChangesAsync();
        private async Task<bool> CheckName(string name) => await context.Departments.AnyAsync(dep => dep.Name == name);
    }
}