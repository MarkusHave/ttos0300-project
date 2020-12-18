using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeLog.Models;
using System.Data;

namespace EmployeeLog.ViewModels
{
    public class UserViewModel
    {
        public static List<Employee> employees = new List<Employee>();

        //hakee datatablesta tiedot ja tekee niistä employee olioita
        public static void GetEmployees()
        {
            employees.Clear();
            DataTableReader reader = new DataTableReader(DB.userData);

            try
            {
                //ORM 
                while (reader.Read())
                {
                    Employee e = new Employee();
                    try
                    {
                        e.UserID = reader.GetInt32(0);
                        e.UserTypeID = reader.GetInt32(1);
                        e.LoginID = reader.GetString(2);
                        e.Password = reader.GetString(3);
                        e.FirstName = reader.GetString(4);
                        e.LastName = reader.GetString(5);
                        e.Title = reader.GetString(6);
                        e.Salary = reader.GetDecimal(7);
                        e.HoursBalance = reader.GetDecimal(8);
                        e.HireDate = reader.GetDateTime(9);
                        if (reader.GetValue(10).ToString() == "1")
                        {
                            e.Active = true;
                        }
                        else
                        {
                            e.Active = false;
                        }

                    }
                    catch
                    {
                    }
                    employees.Add(e);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
