using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluffinMuffin.DataTypes
{
    public class UserInfo
    {
        public double TotalMoney { get; set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public string Email { get; private set; }

        public string DisplayName { get; private set; }

        public UserInfo(string username, string password, string email, string displayname, double totalmoney)
        {
            DisplayName = displayname;
            Email = email;
            Password = password;
            TotalMoney = totalmoney;
            Username = username;
        }
    }
}
