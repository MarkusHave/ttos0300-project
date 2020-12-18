using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EmployeeLog.ViewModels;

namespace EmployeeLog.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string loginId = txtLoginID.Text;
            string pwd = pwdPassword.Password;

            if (CheckLogin(loginId, pwd))
            {
                MainWindow.LoggedIn = true;
                MainWindow.currentUser = UserViewModel.employees.FirstOrDefault(x => x.LoginID == loginId);
                MessageBox.Show("Success!", "Message");
                this.Close();
            }
            else
            {
                MessageBox.Show("Authentication failed!", "Message");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!MainWindow.LoggedIn)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Application.Current.MainWindow.Visibility = Visibility.Visible;
                Application.Current.MainWindow.ShowInTaskbar = true;
            }
        }

        //kirjautumisen tarkastus
        public bool CheckLogin(string login, string pwd)
        {
            foreach (var item in UserViewModel.employees)
            {
                if (item.Password == pwd && item.LoginID == login)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
