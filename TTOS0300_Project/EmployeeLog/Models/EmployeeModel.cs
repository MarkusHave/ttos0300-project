using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EmployeeLog.Models
{
    //employee olio malli
    public class Employee
    {
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public DateTime? HireDate { get; set; }
        public bool? Active { get; set; }
        public decimal? HoursBalance { get; set; }
        public decimal? Salary { get; set; }
        public Stopwatch workTimer = new Stopwatch();
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public decimal? DayLength { get; set; }

        public string Fullname
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public string UserType
        {
            get
            {
                if (UserTypeID == 1)
                {
                    return "Admin";
                }
                else if (UserTypeID == 2)
                {
                    return "Boss";
                }
                else
                {
                    return "Employee";
                }
            }
        }
    }
}
