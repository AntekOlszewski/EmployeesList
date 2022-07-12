using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EmployeesList
{
    internal class AddEmployeePageViewModel
    {
        private AddEmployeeWindow addEmployeeWindow;
        private ObservableCollection<EmployeeViewModel> employeesList;
        public string Name { get; set; } = string.Empty;
        public  string Surename { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phonenumber { get; set; } = string.Empty;
        public ICommand SaveCommand { get; set; }

        public AddEmployeePageViewModel(ObservableCollection<EmployeeViewModel> employeesList, AddEmployeeWindow addEmployeeWindow)
        {
            this.addEmployeeWindow = addEmployeeWindow;
            this.employeesList = employeesList;
            SaveCommand = new RelayCommand(SaveData);
        }

        private void SaveData()
        {
            var employee = new DataBase.Employee
            {
                Name = this.Name,
                Surename = this.Surename,
                Email = this.Surename,
                Phonenumber = this.Phonenumber
            };
            DataBaseLocator.Database.Employees.Add(employee);
            DataBaseLocator.Database.SaveChanges();
            employeesList.Add(new EmployeeViewModel(employee.Id, this.Name, this.Surename, this.Surename, this.Phonenumber));
            addEmployeeWindow.Close();
        }
    }
}
