using Microsoft.EntityFrameworkCore;

namespace EmployeesList.DataBase
{
    public class EmployeesListDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionBuilder)
        {
            base.OnConfiguring(dbContextOptionBuilder);

            dbContextOptionBuilder.UseSqlite("Filename=Employees.db");
        }
    }
}
