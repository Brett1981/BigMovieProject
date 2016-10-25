using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDB_Windows_app.Resources
{
    
    public class jsonMovieClass
    {
        public List<data> data { get; set; }
    }
    public class data
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string ServerLocation { get; set; }
        public List<int> DBgenres { get; set; }
        public string DBTitle { get; set; }
        public string DBid { get; set; }
        public string DBposter { get; set; }
    }
    public class jsonGenresClass
    {
        public List<genres> genres { get; set; }
    }
    public class genres
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
