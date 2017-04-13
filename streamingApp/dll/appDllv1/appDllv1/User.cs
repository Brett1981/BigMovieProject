using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appDllv1
{
    public class User
    {
        public class Info
        {
            public int Id { get; set; }
            public string unique_id { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string profile_image { get; set; }
            public string display_name { get; set; }
            public DateTime? profile_created { get; set; }
            public DateTime? last_logon { get; set; }
            public DateTime? birthday { get; set; }
            public string email { get; set; }
            public int group { get; set; }

        }
        public class Groups
        {
            public int Id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string access { get; set; }
            public string desc { get; set; }
            public bool status { get; set; }
        }
}
}
