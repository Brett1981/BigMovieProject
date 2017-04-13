using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace appDllv1
{
    public class Objects
    {
        public class Communication
        {
            /// <summary>
            /// Administration all data 
            /// </summary>
            public class Data
            {
                public Users users { get; set; }
                public List<Settings> disks { get; set; }
                public List<Movie.Data> movies { get; set; }
                public List<Settings> settings { get; set; }
                public List<History.API> apiHistory { get; set; }
            }

            /// <summary>
            /// Administration all users and groups object
            /// </summary>
            public class Users
            {
                public List<User.Groups> groups { get; set; }
                public List<User.Info> users { get; set; }
            }

            /// <summary>
            /// Disks init object
            /// </summary>
            public class Settings
            {
                public int id { get; set; }
                public string name { get; set; }
                public string value { get; set; }
            }

            /// <summary>
            /// Movie/user edit object
            /// </summary>
            public class Edit
            {
                public Data api { get; set; }
                public Movie.Data movie { get; set; }
                public User.Info user { get; set; }
                public User.Groups groups { get; set; }
                public API.Auth.User auth { get; set; }
            }
        }



        public class History
        {
            public class API
            {
                public int Id { get; set; }
                public DateTime? api_datetime { get; set; }
                public string api_action { get; set; }
                public string api_type { get; set; }
            }

            public class User
            {
                public int Id { get; set; }
                public string user_id { get; set; }
                public string user_action { get; set; }
                public string user_type { get; set; }
                public string user_movie { get; set; }
                public DateTime? user_datetime { get; set; }
            }
        }

        public class Other
        {
            public class values
            {
                public int id { get; set; }
                public string name { get; set; }
            }
        }
    }
}
