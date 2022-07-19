using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
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
            LoadCommand = new RelayCommand(LoadEmployeesFromCSV);
            EditCommand = new RelayCommand(EditEmployee);
            DeleteCommand = new RelayCommand(DeleteEmployees);
            AddCommand = new RelayCommand(AddEmployee);
            ExportCommand = new RelayCommand(ExportData);
            CheckAllCommand = new RelayCommand(CheckAll);
            SortCommand = new RelayCommand(parameter => SortByColumn((string)parameter));

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
                    EditEmployeeWindow edit = new EditEmployeeWindow(EmployeeToEdit);
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
                File.WriteAllText(saveFileDialog.FileName, csv.ToString());
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
