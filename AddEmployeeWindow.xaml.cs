using System.Collections.ObjectModel;
using System.Windows;

namespace EmployeesList
{
    public partial class AddEmployeeWindow : Window
    {
        public AddEmployeeWindow(ObservableCollection<EmployeeViewModel> employeesList)
        {
            InitializeComponent();
            DataContext = new AddEmployeePageViewModel(employeesList, this);
        }
    }
}
