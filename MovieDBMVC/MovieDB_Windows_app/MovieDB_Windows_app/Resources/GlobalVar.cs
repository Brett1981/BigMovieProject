using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDB_Windows_app.Resources
{
    public static class GlobalVar
    {

        static jsonMovieClass _globalMovieData;
        public static jsonMovieClass GlobalMovieData
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

        static jsonGenresClass _globalGenresData;
        public static jsonGenresClass GlobalGenresData
        {
            get
            {
                return _globalGenresData;
            }
            set
            {
                _globalGenresData = value;
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

    }
}
