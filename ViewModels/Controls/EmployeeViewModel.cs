using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EmployeesList
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        private int id = 0;
        private string name = String.Empty;
        private string surename = String.Empty;
        private string email = String.Empty;
        private string phonenumber = String.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public EmployeeViewModel(int EmployeeId, string EmployeeName, string EmployeeSurename, string EmployeeEmail, string EmployeePhoneNumber)
        {
            id = EmployeeId;
            name = EmployeeName;
            surename = EmployeeSurename;
            email = EmployeeEmail;
            phonenumber = EmployeePhoneNumber;
        }

        public int Id
        {
            get { return id; }
            set
            {
                if (value != id)
                {
                    id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Surename
        {
            get { return surename; }
            set
            {
                if (value != surename)
                {
                    surename = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Email
        {
            get { return email; }
            set
            {
                if (value != email)
                {
                    email = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Phonenumber
        {
            get { return phonenumber; }
            set
            {
                if (value != phonenumber)
                {
                    phonenumber = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
