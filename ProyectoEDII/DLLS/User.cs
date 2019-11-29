using System;
using System.Collections.Generic;
using System.Text;

namespace DLLS
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public enum UserRole
        {
            NORMAL,
            ADMIN
        }
    }
}
