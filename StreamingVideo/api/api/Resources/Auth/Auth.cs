using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Resources.Auth
{
    public class Auth
    {
        public class Login
        {
            public string username { get; set; }
            public string password { get; set; }
        }
        
        public class User
        {
            public int Id { get; set; }

            public string username { get; set; }
            public string password { get; set; }
            public string unique_id { get; set; }
            public string image_url { get; set; }
            public string display_name { get; set; }
            public Nullable<System.DateTime> profile_created { get; set; }
            public Nullable<System.DateTime> birthday { get; set; }
            public string email { get; set; }

        }

       

        public class AuthMovieEdit
        {
            public Auth.User User { get; set; }
            public Movie_Data Movie { get; set; }
        }

    }
}