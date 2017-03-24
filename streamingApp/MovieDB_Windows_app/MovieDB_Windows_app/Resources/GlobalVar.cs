using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MovieDB_Windows_app.API;

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

        static Movie.Data _globalMovieClick;
        public static Movie.Data GlobalMovieItemClick
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

        static APIObjects.Data _globalData;
        public static APIObjects.Data GlobalData
        {
            get
            {
                return _globalData;
            }
            set
            {
                _globalData = value;
            }
        }

        static User.Info _globalCurrentUserInfo;
        public static User.Info GlobalCurrentUserInfo
        {
            get
            {
                return _globalCurrentUserInfo;
            }
            set
            {
                _globalCurrentUserInfo = value;
            }
        }

        static Auth.User _globalAuthUser;
        public static Auth.User GlobalAuthUser
        {
            get
            {
                return _globalAuthUser;
            }
            set
            {
                _globalAuthUser = value;
            }
        }


    }
}
