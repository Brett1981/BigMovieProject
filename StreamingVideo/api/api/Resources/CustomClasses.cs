using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Resources
{
    public class CustomClasses
    {
        /// <summary>
        /// stringify movie data to json with this class
        /// </summary>
        public partial class MovieInfoToJSON
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
            public List<values> genres { get; set; }
            public List<values> production_countries { get; set; }
            public List<values> production_companies { get; set; }
            public List<values> spoken_languages { get; set; }
        }
        /// <summary>
        /// Part of MovieInfoJSON class
        /// </summary>
        public class values
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        /// <summary>
        /// Used to parse JSON to objects
        /// </summary>
        public class DataAPI
        {
            public List<results> results { get; set; }
        }
        /// <summary>
        /// Part of DataAPI
        /// </summary>
        public class results
        {
            public int id { get; set; }
            public string title { get; set; }
            public string release_date { get; set; }
            public List<int> genre_ids { get; set; }
            public string poster_path { get; set; }
        }
    }
}