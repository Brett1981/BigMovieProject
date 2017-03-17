using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDB_Windows_app.Resources
{
    public static class GlobalVar
    {
        static List<System.Windows.Forms.Button> _globalMovieButtonList;
        public static List<System.Windows.Forms.Button> GlobalMovieButtonList
        {
            get
            {
                return _globalMovieButtonList;

            }
            set
            {
                _globalMovieButtonList = value;
            }
        }

        static List<Movie_Data> _globalMovieData;
        public static List<Movie_Data> GlobalMovieData
        {
            get
            {
                return _globalMovieData;
            }
            set
            {
                _globalMovieData = value;
            }
        }

        static string _globalMovieid;
        public static string GlobalMovieId
        {
            get
            {
                return _globalMovieid;
            }
            set
            {
                _globalMovieid = value;
            }
        }

        static string _globalServerUrl;
        public static string GlobalServerUrl
        {
            get
            {
                return _globalServerUrl;
            }
            set
            {
                _globalServerUrl = value;
            }
        }

        static Movie_Data _globalMovieClick;
        public static Movie_Data GlobalMovieItemClick
        {
            get
            {
                return _globalMovieClick;
            }
            set
            {
                _globalMovieClick = value;
            }
        }

        static List<User_Info> _globalUserInfo;
        public static List<User_Info> GlobalUserInfo
        {
            get
            {
                return _globalUserInfo;
            }
            set
            {
                _globalUserInfo = value;
            }
        }

    }
}
