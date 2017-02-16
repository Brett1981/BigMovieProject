using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Models
{
    public class DatabaseUserModels
    {
        public string user_id { get; set; }
        public string movie_id { get; set; }
    }
    public class CustomUserModel
    {
        public int Id { get; set; }

        public string username { get; set; }
        public string password { get; set; }
        public string unique_id { get; set; }
        public string image_url { get; set; }
        public string user_display_name { get; set; }
        public Nullable<System.DateTime> profile_created { get; set; }
        public Nullable<System.DateTime> user_birthday { get; set; }
        public string user_email { get; set; }

    }
}