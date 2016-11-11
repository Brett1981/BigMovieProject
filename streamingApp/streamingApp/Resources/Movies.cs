using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace streamingApp.Resources
{
    class MovieData
    {
        public int Id { get; set; }
        public string movie_name { get; set; }
        public string movie_ext { get; set; }
        public string movie_guid { get; set; }
        public string movie_folder { get; set; }
        public Nullable<int> movie_views { get; set; }
        public MovieInfo MovieInfo { get; set; }
    }
    public class MovieInfo
    {
        public Nullable<int> id { get; set; }
        public int id_movie { get; set; }
        public Nullable<bool> adult { get; set; }
        public string backdrop_path { get; set; }
        public string budget { get; set; }
        public string homepage { get; set; }
        public string imdb_id { get; set; }
        public string original_title { get; set; }
        public string overview { get; set; }
        public string popularity { get; set; }
        public string poster_path { get; set; }
        public Nullable<System.DateTime> release_date { get; set; }
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
