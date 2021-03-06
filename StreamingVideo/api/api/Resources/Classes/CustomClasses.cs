﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Resources
{
    public class CustomClasses
    {
        public partial class JSONToMovieInfo
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
            public List<Random.values> genres { get; set; }
            public List<Random.values> production_countries { get; set; }
            public List<Random.values> production_companies { get; set; }
            public List<Random.values> spoken_languages { get; set; }
        }

        public class MovieSession
        {
            public Movie_Data movieData { get; set; }
            public Session_Guest sessionGuest { get; set; }
            public Session_Play sessionPlay { get; set; }

        }

        public class API
        {
            /// <summary>
            /// Administration all data 
            /// </summary>
            public class Data
            {
                public Users users { get; set; }
                public List<Settings> disks { get; set; }
                public List<Movie_Data> movies { get; set; }
                public List<Settings> settings { get; set; }
                public List<History_API> apiHistory { get; set; }
            }

            /// <summary>
            /// Administration all users and groups object
            /// </summary>
            public class Users
            {
                public List<User_Groups> groups { get; set; }
                public List<User_Info> users { get; set; }
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
            /// Movie/user/api edit object
            /// </summary>
            public class Edit
            {
                public Movie_Data movie { get; set; }
                public User_Info user { get; set; }
                public User_Groups groups { get; set; }
                public Data api { get; set; }
                public Auth.Auth.User auth { get; set; }
            }
        }
 
        public class Random
        {
            /// <summary>
            /// Part of MovieInfoJSON class
            /// </summary>
            public class values
            {
                public int id { get; set; }
                public string name { get; set; }
            }

            /// <summary>
            /// Used to parse JSON list to objects
            /// </summary>
            public class APIResults
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
}