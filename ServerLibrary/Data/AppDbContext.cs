using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;

namespace ServerLibrary.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; }

        #region Department related entities
        public DbSet<GeneralDepartment> GeneralDepartments { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Branch> Branches { get; set; }
        #endregion
       
        #region Location related entities
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Town> Towns { get; set; }
        #endregion
      
        #region Auth related entities
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<SystemRole> SystemRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshTokenInfo> RefreshTokenInfos { get; set; }
        #endregion
  
        public DbSet<Vacation> Vacations { get; set; }
        public DbSet<VacationType> VacationTypes { get; set; }

        public DbSet<Overtime> Overtimes { get; set; }
        public DbSet<OvertimeType> OvertimeTypes { get; set; }

        public DbSet<Sanction> Sanctions { get; set; }
        public DbSet<SanctionType> SanctionTypes { get; set; }

        public DbSet<Doctor> Doctors { get; set; }
    }
}