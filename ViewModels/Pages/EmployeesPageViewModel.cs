using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace EmployeesList
{
    public class EmployeesPageViewModel
    {
        public ObservableCollection<EmployeeViewModel> EmployeesList { get; set; } = new ObservableCollection<EmployeeViewModel>();
        public ICommand LoadCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public EmployeeViewModel? EmployeeSelected { get; set; }

        public EmployeesPageViewModel()
        {
            LoadCommand = new RelayCommand(LoadEmployeesFromCSV);
            EditCommand = new RelayCommand(EditEmployee);
            DeleteCommand = new RelayCommand(DeleteEmployees);

            foreach (var e in DataBaseLocator.Database.Employees.ToList())
            {
                EmployeesList.Add(new EmployeeViewModel(e.Id, e.Name, e.Surename, e.Email, e.Phonenumber));
            }
        }
        private void LoadEmployeesFromCSV()
        {
            var dialog = new OpenFileDialog { };
            dialog.Filter = "Csv files (*.csv)|*.csv";
            dialog.ShowDialog();

            string path = dialog.FileName;

            if(path != "") 
            {
                ReadCSV(path);
            }
        }
        public void ReadCSV(string path)
        {

            string[] lines = File.ReadAllLines(path);

            for(int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');

                var e = EmployeesList.FirstOrDefault(e => e.Id == Convert.ToInt32(data[0]));

                if(e == null)
                {
                    EmployeesList.Add(new EmployeeViewModel(Convert.ToInt32(data[0]), data[1], data[2], data[3], data[4]));
                    DataBaseLocator.Database.Employees.Add(new DataBase.Employee
                    {
                        Id = Convert.ToInt32(data[0]),
                        Name = data[1],
                        Surename = data[2],
                        Email = data[3],
                        Phonenumber = data[4]
                    });
                }
                
            }

            DataBaseLocator.Database.SaveChanges();
        }

        public void EditEmployee()
        {
            if (EmployeeSelected != null)
            {
                var EmployeeToEdit = EmployeesList.FirstOrDefault(e => e.Id == EmployeeSelected.Id);
                if (EmployeeToEdit != null)
                {
                    Edit edit = new Edit(EmployeeToEdit);
                    edit.Show();
                }
            }
        }

        public void DeleteEmployees()
        {
            foreach(EmployeeViewModel e in EmployeesList.Where(e => e.IsChecked == true).ToList())
            {
                EmployeesList.Remove(e);
                var foundEntity = DataBaseLocator.Database.Employees.FirstOrDefault(emp => emp.Id == e.Id);
                if(foundEntity != null)
                {
                    DataBaseLocator.Database.Employees.Remove(foundEntity);
                }
            }
            DataBaseLocator.Database.SaveChanges();
        }
    }
}
