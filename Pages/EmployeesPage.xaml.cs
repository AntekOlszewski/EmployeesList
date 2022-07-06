using System.Windows.Controls;

namespace EmployeesList
{
    public partial class EmployeesPage : Page
    {
        public EmployeesPage()
        {
            InitializeComponent();

            DataContext = new EmployeesPageViewModel();

        }

    }
}
