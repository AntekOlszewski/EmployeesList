using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace EmployeesList
{
    public class EmployeesPageViewModel
    {
        public ObservableCollection<EmployeeViewModel> EmployeesList { get; set; } = new ObservableCollection<EmployeeViewModel>();
        public ICommand LoadCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand CheckAllCommand { get; set; }
        public ICommand SortCommand { get; set; }

        public EmployeeViewModel? EmployeeSelected { get; set; }
        public bool IsCheckedAll { get; set; }
        private enum Column { ID, Name, Surename, Email, Phonenumber}
        private Column sortBy = Column.ID;
        private bool sortAscending = true;
        public EmployeesPageViewModel()
        {
            LoadCommand = new RelayCommand(LoadEmployeesFromCSVAsync);
            EditCommand = new RelayCommand(EditEmployee);
            DeleteCommand = new RelayCommand(DeleteEmployeesAsync);
            AddCommand = new RelayCommand(AddEmployee);
            ExportCommand = new RelayCommand(ExportData);
            CheckAllCommand = new RelayCommand(CheckAll);
            SortCommand = new RelayCommand(parameter => SortByColumn((string)parameter));

            foreach (var e in DataBaseLocator.Database.Employees.ToList())
            {
                EmployeeViewModel Employee = new EmployeeViewModel(e.Id, e.Name, e.Surename, e.Email, e.Phonenumber);
                BindingOperations.EnableCollectionSynchronization(EmployeesList, Employee);
                Task.Run(() => EmployeesList.Add(Employee));
            }
        }
        private async void LoadEmployeesFromCSVAsync()
        {
            var dialog = new OpenFileDialog { };
            dialog.Filter = "Csv files (*.csv)|*.csv";
            dialog.ShowDialog();

            string path = dialog.FileName;

            if(path != "") 
            {
                await ReadCSVAsync(path);
            }
        }
        public async Task ReadCSVAsync(string path)
        {

            string[] lines = await File.ReadAllLinesAsync(path);
            List<Task> tasks = new List<Task>();

            foreach (string line in lines.Skip(1))
            {
                string[] data = line.Split(',');

                var e = EmployeesList.FirstOrDefault(e => e.Id == Convert.ToInt32(data[0]));

                if(e == null)
                {
                    EmployeeViewModel newEmployee = new EmployeeViewModel(Convert.ToInt32(data[0]), data[1], data[2], data[3], data[4]);
                    BindingOperations.EnableCollectionSynchronization(EmployeesList, newEmployee);

                    tasks.Add(Task.Run(() => EmployeesList.Add(newEmployee)));
                    
                    await DataBaseLocator.Database.Employees.AddAsync(new DataBase.Employee
                    {
                        Id = newEmployee.Id,
                        Name = newEmployee.Name,
                        Surename = newEmployee.Surename,
                        Email = newEmployee.Email,
                        Phonenumber = newEmployee.Phonenumber
                    });
                }
                await Task.WhenAll(tasks);
                
            }

            await Task.Run(() => DataBaseLocator.Database.SaveChanges());
        }

        public void EditEmployee()
        {
            if (EmployeeSelected != null)
            {
                var EmployeeToEdit = EmployeesList.FirstOrDefault(e => e.Id == EmployeeSelected.Id);
                if (EmployeeToEdit != null)
                {
                    EditEmployeeWindow edit = new EditEmployeeWindow(EmployeeToEdit);
                    edit.Show();
                }
            }
        }

        public async void DeleteEmployeesAsync()
        {
            List<int> idsToDelete = new List<int>();

            foreach(EmployeeViewModel e in EmployeesList.Where(e => e.IsChecked == true))
            {      
                idsToDelete.Add(e.Id);
            }

            List<EmployeeViewModel> employeesListTmp = EmployeesList.ToList();
            await Task.Run(() => employeesListTmp.RemoveAll(e => idsToDelete.Contains(e.Id)));

            EmployeesList.Clear();
            foreach(EmployeeViewModel e in employeesListTmp)
            {
                EmployeesList.Add(e);
            }

            foreach(int id in idsToDelete)
            {
                var foundEntity = DataBaseLocator.Database.Employees.FirstOrDefault(emp => emp.Id == id);
                if (foundEntity != null)
                {
                    await Task.Run(() => DataBaseLocator.Database.Employees.Remove(foundEntity));
                }
            }
            await Task.Run(() => DataBaseLocator.Database.SaveChanges());
        }
        public void AddEmployee()
        {
            AddEmployeeWindow add = new AddEmployeeWindow(EmployeesList);
            add.Show();
        }

        public void ExportData()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Csv file (*.csv)|*.csv";
            StringBuilder csv = new StringBuilder();
            csv.Append("id,name,surename,email,phone\n");

            if (saveFileDialog.ShowDialog() == true)
            {
                ObservableCollection<EmployeeViewModel> exportedEmployees = new ObservableCollection<EmployeeViewModel>();
                if(sortBy != Column.ID || !sortAscending)
                {
                    exportedEmployees = new ObservableCollection<EmployeeViewModel>(EmployeesList.OrderBy(e => e.Id));
                }
                else
                {
                    exportedEmployees = EmployeesList;
                }
                foreach (EmployeeViewModel e in exportedEmployees)
                {
                    csv.Append(Convert.ToString(e.Id) + ',' + e.Name + ',' + e.Surename + ',' + e.Email + ',' + e.Phonenumber + '\n');
                }
                File.WriteAllTextAsync(saveFileDialog.FileName, csv.ToString());
            }
                
        }

        private void CheckAll()
        {
            foreach(EmployeeViewModel e in EmployeesList)
            {
                e.IsChecked = IsCheckedAll;
            }
        }

        private void SortByColumn(string name)
        {
            EmployeeSelected = null;
            ObservableCollection<EmployeeViewModel> temp = new ObservableCollection<EmployeeViewModel>();
            switch (name)
            {
                case "ID":
                    if(sortBy == Column.ID)
                    {
                        sortAscending = !sortAscending;
                    }
                    else
                    {
                        sortAscending = true;
                    }
                    sortBy = Column.ID;
                    if (sortAscending)
                    {
                        temp = new ObservableCollection<EmployeeViewModel>(EmployeesList.OrderBy(e => e.Id));
                    }
                    else
                    {
                        temp = new ObservableCollection<EmployeeViewModel>(EmployeesList.OrderByDescending(e => e.Id));
                    }
                    break;
                case "Name":
                    if (sortBy == Column.Name)
                    {
                        sortAscending = !sortAscending;
                    }
                    else
                    {
                        sortAscending = true;
                    }
                    sortBy = Column.Name;
                    if (sortAscending)
                    {
                        temp = new ObservableCollection<EmployeeViewModel>(EmployeesList.OrderBy(e => e.Name));
                    }
                    else
                    {
                        temp = new ObservableCollection<EmployeeViewModel>(EmployeesList.OrderByDescending(e => e.Name));
                    }
                    break;
                case "Surename":
                    if (sortBy == Column.Surename)
                    {
                        sortAscending = !sortAscending;
                    }
                    else
                    {
                        sortAscending = true;
                    }
                    sortBy = Column.Surename;
                    if (sortAscending)
                    {
                        temp = new ObservableCollection<EmployeeViewModel>(EmployeesList.OrderBy(e => e.Surename));
                    }
                    else
                    {
                        temp = new ObservableCollection<EmployeeViewModel>(EmployeesList.OrderByDescending(e => e.Surename));
                    }
                    break;
                case "Email":
                    if (sortBy == Column.Email)
                    {
                        sortAscending = !sortAscending;
                    }
                    else
                    {
                        sortAscending = true;
                    }
                    sortBy = Column.Email;
                    if (sortAscending)
                    {
                        temp = new ObservableCollection<EmployeeViewModel>(EmployeesList.OrderBy(e => e.Email));
                    }
                    else
                    {
                        temp = new ObservableCollection<EmployeeViewModel>(EmployeesList.OrderByDescending(e => e.Email));
                    }
                    break;
                case "Phonenumber":
                    if (sortBy == Column.Phonenumber)
                    {
                        sortAscending = !sortAscending;
                    }
                    else
                    {
                        sortAscending = true;
                    }
                    sortBy = Column.Phonenumber;
                    if (sortAscending)
                    {
                        temp = new ObservableCollection<EmployeeViewModel>(EmployeesList.OrderBy(e => e.Phonenumber));
                    }
                    else
                    {
                        temp = new ObservableCollection<EmployeeViewModel>(EmployeesList.OrderByDescending(e => e.Phonenumber));
                    }
                    break;
            }
            EmployeesList.Clear();
            foreach (EmployeeViewModel e in temp)
            {
                EmployeesList.Add(e);
            } 
        }
    }
}
