using System.Windows;

namespace EmployeesList
{
    public partial class Edit : Window
    {
        public Edit(EmployeeViewModel employee)
        {
            InitializeComponent();
            DataContext = new EditEmployeePageViewModel(employee, this);
        }
    }
}
