using System;
using System.Collections.Generic;
using System.Text;

namespace shoesMic.data
{
    public class User
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
