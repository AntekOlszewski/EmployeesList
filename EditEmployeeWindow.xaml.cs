using System.Windows;

namespace EmployeesList
{
    public partial class EditEmployeeWindow : Window
    {
        public EditEmployeeWindow(EmployeeViewModel employee)
        {
            InitializeComponent();
            DataContext = new EditEmployeePageViewModel(employee, this);
        }
    }
}
