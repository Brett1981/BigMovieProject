//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace api
{
    using System;
    using System.Collections.Generic;
    
    public partial class Movie_Data
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string ext { get; set; }
        public string guid { get; set; }
        public string folder { get; set; }
        public string dir { get; set; }
        public Nullable<int> views { get; set; }
        public Nullable<System.DateTime> added { get; set; }
        public bool enabled { get; set; }
        public Nullable<System.DateTime> FileCreationDate { get; set; }
    
        public virtual Movie_Info Movie_Info { get; set; }
    }
}
