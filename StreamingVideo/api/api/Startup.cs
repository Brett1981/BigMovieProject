using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Threading.Tasks;
using api.Resources;
using System.Threading;
using System.Diagnostics;

[assembly: OwinStartup(typeof(api.Startup))]

namespace api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            DatabaseMovieCheck();
        }

        public async Task DatabaseMovieCheck()
        {
            Thread t1;
            try
            {
                t1= new Thread(async () => await Database.CheckDB());
                t1.Priority = ThreadPriority.Normal;
                t1.Start();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception --> {0} -- {1}", ex.Message, ex.InnerException.InnerException);
            }
            
           
        }
    }
}
