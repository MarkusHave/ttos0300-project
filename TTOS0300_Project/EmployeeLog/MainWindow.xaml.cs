using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using EmployeeLog.ViewModels;
using System.Windows.Threading;
using System.Threading;

namespace EmployeeLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool LoggedIn = false;
        private static List<TextBox> textBoxes;
        public static Models.Employee currentUser;
        private static DispatcherTimer Timer;
        

        public MainWindow()
        {
            InitializeComponent();
            InitStuff();
        }

        //tehdään joka kerta kun joku kirjautuu sisään
        private void Window_Activated(object sender, EventArgs e)
        {
            txbLogin.Text = currentUser.LoginID;
            txbFullName.Text = currentUser.Fullname;
            txbTitle.Text = currentUser.Title;
            txbHours.Text = currentUser.HoursBalance.ToString();
            txbHiredate.Text = currentUser.HireDate.ToString();
            UpdateDataGrids();

            if (currentUser.UserTypeID == 3)
            {
                tbcMain.SelectedIndex = 0;
                tabData.Visibility = Visibility.Hidden;
                tabSettings.Visibility = Visibility.Hidden;
            }
            else if (currentUser.UserTypeID == 2)
            {
                tabData.Visibility = Visibility.Visible;
                tabSettings.Visibility = Visibility.Hidden;
            }
            else
            {
                tabData.Visibility = Visibility.Visible;
                tabSettings.Visibility = Visibility.Visible;
            }
        }

        //alustus metodi
        private void InitStuff()
        {

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += new EventHandler(Timer_tick);
            Timer.Start();

            try
            {
                DB.GetData();
                UpdateDataGrids();
                Views.LoginView LoginWindow = new Views.LoginView();
                LoginWindow.ShowDialog();

                textBoxes = new List<TextBox>
                {
                    txtUserType,
                    txtLoginID,
                    txtPassword,
                    txtFirstname,
                    txtLastname,
                    txtTitle,
                    txtSalary,
                    txtHoursBalance,
                    txtHireDate,
                    txtActive
                };

                txtServer.Text = Properties.Settings.Default.mysqlServer;
                txtDataBase.Text = Properties.Settings.Default.database;
                txtUser.Text = Properties.Settings.Default.user;
                txtServerPwd.Text = Properties.Settings.Default.password;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nShutting down.", "Error!");
                App.Current.Shutdown();
            }

        }

        //kello
        private void Timer_tick(object sender, EventArgs e)
        {
            txbTime.Text = DateTime.Now.ToLongTimeString();
        }

        //päivittää datagridit
        private void UpdateDataGrids()
        {
            UserViewModel.GetEmployees();
            //users datagridissä näyteään vain tarvittavat tiedot
            dgUsers.ItemsSource = UserViewModel.employees.Select(x => new
            {
                x.UserType,
                x.Fullname,
                x.LoginID,
                x.Password,
                x.HoursBalance,
                x.Title,
                x.HireDate,
                x.Salary,
                x.Active
            });

            dgLog.ItemsSource = DB.logData.DefaultView;
        }

        //uloskirjautuminen
        private void BtnLogOut_Click(object sender, RoutedEventArgs e)
        {
            LoggedIn = false;
            currentUser = null;
            this.Visibility = Visibility.Hidden;
            this.ShowInTaskbar = false;
            Views.LoginView LoginWindow = new Views.LoginView();
            LoginWindow.ShowDialog();
        }

        //datatablen rivin päivitys
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = DB.userData.Select($"LoginID = '{txtLoginID.Text}'");
                var dataRow = result[0];

                //päivitetään tiedot datatableen (toimii pyhällä hengellä) ÄLÄ KOSKE!!!
                //============================================
                for (int i = 0; i < textBoxes.Count; i++)
                {
                    dataRow[i + 1] = textBoxes[i].Text;
                }
                //============================================
                UpdateDataGrids();
                DB.UpdateUser(txtLoginID.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        //rivin poisto datatablesta
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var remove = MessageBox.Show($"Remove {txtLoginID.Text}?",
                "Remove employee", MessageBoxButton.YesNo);

                if (remove == MessageBoxResult.Yes)
                {
                    var result = DB.userData.Select($"LoginID = '{txtLoginID.Text}'");
                    DB.userData.Rows.Remove(result[0]);
                    UpdateDataGrids();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
            
        }

        //valitun rivin tiedot viedään textboxeihin josta voi muokata
        private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem != null)
            {

                try
                {
                    // "rautalanka" ratkaisu
                    var item = dgUsers.SelectedItem.ToString().Split(',');
                    var loginID = item[2].Substring(11);
                    var result = DB.userData.Select($"LoginID = '{loginID}'");
                    var dataRow = result[0];

                    //tuodaan tiedot textboxeihin
                    for (int i = 0; i < textBoxes.Count; i++)
                    {
                        textBoxes[i].Text = dataRow[i + 1].ToString();
                    }

                    tbcTables.SelectedIndex = 2;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!");
                }
            }
        }

        //työpäivän aloitus/lopetus
        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnStart.Content.ToString() == "Start")
                {
                    currentUser.workTimer.Start();
                    currentUser.LoginTime = DateTime.Now;
                    btnStart.Content = "Stop";
                    btnBreak.IsEnabled = true;
                }
                else
                {
                    currentUser.workTimer.Stop();
                    btnStart.Content = "Start";
                    currentUser.LogoutTime = DateTime.Now;
                    currentUser.DayLength = decimal.Parse((currentUser.LogoutTime - currentUser.LoginTime).Value.TotalHours.ToString("0.##"));
                    DataRow row = DB.logData.NewRow();
                    row.SetField<int>("UserID", currentUser.UserID);
                    row.SetField<DateTime?>("LoginTime", currentUser.LoginTime);
                    row.SetField<DateTime?>("LogoutTime", currentUser.LogoutTime);
                    row.SetField<decimal?>("DayLenght", currentUser.DayLength);
                    DB.logData.Rows.Add(row);
                    UpdateDataGrids();
                    DB.UpdateLog(currentUser.UserID, currentUser.LoginTime, currentUser.LogoutTime, currentUser.DayLength);
                    btnBreak.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error!");
            }
        }

        //tauon aloitus/lopetus [WIP]
        private void BtnBreak_Click(object sender, RoutedEventArgs e)
        {
           
            if (btnBreak.Content.ToString() == "Start break")
            {
                currentUser.workTimer.Stop();
                btnBreak.Content = "Stop break";

            }
            else
            {
                currentUser.workTimer.Start();
                btnBreak.Content = "Start break";
            }
        }

        //lisätään uusi työntekijä datatableen ja tietokantaan
        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            if (DB.userData.Select($"LoginID = '{txtLoginID.Text}'").Length == 0)
            {
                try
                {
                    DataRow dataRow = DB.userData.NewRow();

                    dataRow[1] = int.Parse(txtUserType.Text);
                    dataRow[2] = txtLoginID.Text;
                    dataRow[3] = txtPassword.Text;
                    dataRow[4] = txtFirstname.Text;
                    dataRow[5] = txtLastname.Text;
                    dataRow[6] = txtTitle.Text;
                    dataRow[7] = decimal.Parse(txtSalary.Text);
                    dataRow[8] = decimal.Parse(txtHoursBalance.Text);
                    dataRow[9] = DateTime.Parse(txtHireDate.Text);
                    dataRow[10] = int.Parse(txtActive.Text);

                    DB.userData.Rows.Add(dataRow);
                    DB.NewUser(txtLoginID.Text);
                    UpdateDataGrids();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!");
                }
            }
            else
            {
                MessageBox.Show("Non-unique LoginID", "Error");
            }
            
        }

        //asetusten tallennus [WIP]
        private void BtnUpdateSettings_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.mysqlServer = txtServer.Text;
            Properties.Settings.Default.database = txtDataBase.Text;
            Properties.Settings.Default.user = txtUser.Text;
            Properties.Settings.Default.password = txtServerPwd.Text;
        }
    }
}
