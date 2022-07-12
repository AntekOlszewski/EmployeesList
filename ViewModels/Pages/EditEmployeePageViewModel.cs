using System.Linq;
using System.Windows.Input;

namespace EmployeesList
{
    public class EditEmployeePageViewModel
    {
        public EmployeeViewModel Employee { get; set; }
        public string Name { get; set; }
        public string Surename { get; set; }   
        public string Email { get; set; }
        public string Phonenumber { get; set; }
        public ICommand SaveCommand { get; set; }
        private EditEmployeeWindow editWindow { get; set; }
        public EditEmployeePageViewModel(EmployeeViewModel EmployeeToEdit, EditEmployeeWindow edit)
        {
            Employee = EmployeeToEdit;
            Name = EmployeeToEdit.Name;
            Surename = EmployeeToEdit.Surename;
            Email = EmployeeToEdit.Email;
            Phonenumber = EmployeeToEdit.Phonenumber; 
            SaveCommand = new RelayCommand(SaveData);
            editWindow = edit;
        }

        private void SaveData()
        {
            Employee.Name = Name;
            Employee.Surename = Surename;
            Employee.Email = Email;
            Employee.Phonenumber = Phonenumber;
            var e = DataBaseLocator.Database.Employees.ToList().FirstOrDefault(e => e.Id == Employee.Id);
            if(e != null)
            {
                e.Name = Name;
                e.Surename = Surename;
                e.Email = Email;
                e.Phonenumber = Phonenumber;
            }
            
            DataBaseLocator.Database.SaveChanges();
            editWindow.Close();
        }
    }
}
