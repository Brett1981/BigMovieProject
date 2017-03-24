using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Resources.Global
{
    public static class MovieGlobal
    {
        private static List<CustomClasses.API.Disks>_globalMovieDisksList;
        public  static List<CustomClasses.API.Disks> GlobalMovieDisksList
        {
            get
            {
                return _globalMovieDisksList;
            }
            set
            {
                _globalMovieDisksList = value;
            }
        }
    }
}