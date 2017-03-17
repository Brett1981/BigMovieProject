using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDB_Windows_app.Resources
{
    
    public class Movie_Data
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string ext { get; set; }
        public string guid { get; set; }
        public string folder { get; set; }
        public string dir { get; set; }
        public Nullable<int> views { get; set; }
        public Nullable<System.DateTime> added { get; set; }
        public bool enabled { get; set; }
        public Movie_Info Movie_Info { get; set; }
    }
    public class Movie_Info
    {
        public int id { get; set; }
        public Nullable<bool> adult { get; set; }
        public string backdrop_path { get; set; }
        public string budget { get; set; }
        public string homepage { get; set; }
        public string imdb_id { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public string popularity { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
        public string revenue { get; set; }
        public string status { get; set; }
        public string tagline { get; set; }
        public string title { get; set; }
        public string vote_average { get; set; }
        public string vote_count { get; set; }
        public string genres { get; set; }
        public string production_countries { get; set; }
        public string production_companies { get; set; }
        public string spoken_languages { get; set; }
    }
    public class values
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class User_Info
    {
        public int Id { get; set; }
        public string unique_id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string profile_image { get; set; }
        public string display_name { get; set; }
        public Nullable<System.DateTime> profile_created { get; set; }
        public Nullable<System.DateTime> last_logon { get; set; }
        public Nullable<System.DateTime> birthday { get; set; }
        public string email { get; set; }
        public int groupId { get; set; }
        public User_Groups User_Group { get; set; }

    }
    public class User_Groups
    {
        public int Id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string access { get; set; }
        public string desc { get; set; }
        public bool status { get; set; }
    }
}
