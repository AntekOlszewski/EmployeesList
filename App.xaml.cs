using System.Windows;

namespace EmployeesList
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var database = new DataBase.EmployeesListDbContext();

            database.Database.EnsureCreated();

            DataBaseLocator.Database = database;
        }
    }
}
