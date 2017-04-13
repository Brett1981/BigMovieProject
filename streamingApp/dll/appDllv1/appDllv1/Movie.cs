using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appDllv1
{
    public class Movie
    {
        public class Data
        {
            public int Id { get; set; }
            public string name { get; set; }
            public string ext { get; set; }
            public string guid { get; set; }
            public string folder { get; set; }
            public string dir { get; set; }
            public int? views { get; set; }
            public DateTime? added { get; set; }
            public bool enabled { get; set; }
            public Info Movie_Info { get; set; }
        }
        public class Info
        {
            public int id { get; set; }
            public bool? adult { get; set; }
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
    }
}
