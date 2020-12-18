using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.ComponentModel;

namespace EmployeeLog
{
    //tietokanta luokka
    public static class DB
    {
        public static DataTable userData = new DataTable("User");
        public static DataTable logData = new DataTable("Log");
        private static readonly string connStr = string.Format("server={0};database={1};user={2};password={3};",
            Properties.Settings.Default.mysqlServer, Properties.Settings.Default.database, 
            Properties.Settings.Default.user, Properties.Settings.Default.password);


        //hakee tarvittavan datan tietoknnasta
        public static void GetData()
        {
            string sql = $"SELECT * FROM User;";
            string sql2 = $"SELECT * FROM Log;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    //haetaan työntekijöiden tiedot tietokannasta
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    adapter.SelectCommand = new MySqlCommand(sql, conn);
                    conn.Open();
                    adapter.Fill(userData);
                    adapter.Dispose();

                    //haetaan loki tiedot
                    adapter.SelectCommand = new MySqlCommand(sql2, conn);
                    adapter.Fill(logData);
                    adapter.Dispose();
                }
            }
            catch
            {
                throw;
            }
        }

        //suorittaa nonquery sql komennon
        private static void NonQuery(string sql)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
        }

        //päivittää yhden rivin databaseen user taulu
        public static void UpdateUser(string loginID)
        {
            DataRow[] rows = userData.Select($"LoginID = '{loginID}'");
            DataRow row = rows[0];
            string sqlUpdateUser = string.Format("UPDATE User SET UserTypeID = {0}, Password = '{1}', " +
                "FirstName = '{2}', LastName = '{3}', Title = '{4}', Salary = {5}, HoursBalance = {6}, Active = {7} " +
                "WHERE UserID = {8}", row[1], row[3], row[4], row[5], row[6], row[7].ToString().Replace(',', '.'), 
                row[8].ToString().Replace(',', '.'), row[10], row[0]);

            try
            {
                NonQuery(sqlUpdateUser);
            }
            catch
            {

                throw;
            }
        }

        //lisää uuden rivin user tauluun
        public static void NewUser(string loginID)
        {
            DataRow[] rows = userData.Select($"LoginID = '{loginID}'");
            DataRow row = rows[0];
            var date = (DateTime)row[2];

            string sqlInsert = string.Format("INSERT INTO User " +
                "(UserTypeID, LoginID, Password, FirstName, LastName, Title, Salary, HoursBalance, HireDate, Active)" +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
                row[1], row[2], row[3], row[4], row[5], row[6], row[7].ToString().Replace(',', '.'),
                row[8].ToString().Replace(',', '.'), date.ToString("yyyy-MM-dd"), row[10]);

            try
            {
                NonQuery(sqlInsert);
            }
            catch
            {

                throw;
            }
        }

        //lisää uuden rivin log tauluun 
        public static void UpdateLog(int userID, DateTime? login, DateTime? logout, decimal? daylen)
        {
            string sqlUpdateLog = string.Format("INSERT INTO Log (UserID, LoginTime, LogoutTime, DayLenght)" +
                "VALUES ({0}, '{1}', '{2}', {3})",
                userID, login?.ToString("yyyy-MM-dd HH:mm:ss"), logout?.ToString("yyyy-MM-dd HH:mm:ss"),
                daylen.ToString().Replace(',', '.'));

            try
            {
                NonQuery(sqlUpdateLog);
            }
            catch
            {

                throw;
            }
        }
    }
}
