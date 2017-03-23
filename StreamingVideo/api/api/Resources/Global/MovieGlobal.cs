using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Resources.Global
{
    public static class MovieGlobal
    {
        private static List<CustomClasses.Disks>_globalMovieDisksList;
        public  static List<CustomClasses.Disks> GlobalMovieDisksList
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