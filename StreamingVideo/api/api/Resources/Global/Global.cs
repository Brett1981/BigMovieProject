using System;
using System.Collections.Generic;
using System.Linq;

namespace api.Resources.Global
{
    public static class Global
    {
        private static List<CustomClasses.API.Settings>_globalMovieDisksList;
        public  static List<CustomClasses.API.Settings> GlobalMovieDisksList
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

        private static List<CustomClasses.API.Settings> _globalServerSettings;
        public static List<CustomClasses.API.Settings> GlobalServerSettings
        {
            get
            {
                return _globalServerSettings;
            }
            set
            {
                _globalServerSettings = value;
            }
        }
    }
}