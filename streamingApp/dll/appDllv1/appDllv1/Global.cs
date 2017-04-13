using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using appDllv1;

namespace appDllv1
{
    public static class GlobalVar
    {
        static List<Button> _globalMovieButtonList;
        public static List<Button> GlobalMovieButtonList 
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

        static Objects.Communication.Data _globalData;
        public static Objects.Communication.Data GlobalData
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

        static API.Auth.User _globalAuthUser;
        public static API.Auth.User GlobalAuthUser
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

        static API.Auth _client;
        public static API.Auth client
        {
            get { return _client; }
            set { _client = value; }
        }
    }
}
