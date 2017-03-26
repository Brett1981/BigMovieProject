using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Threading.Tasks;
using api.Resources;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using api.Resources.Global;
[assembly: OwinStartup(typeof(api.Startup))]

namespace api
{
    public partial class Startup
    {
        public async void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            await History.Set("api", new History_API()
            {
                api_action = "API initializing",
                api_type = "Task -> API status",
                api_datetime = DateTime.Now
            });
            var disks = JsonConvert.DeserializeObject<List<CustomClasses.API.Disks>>(api.Properties.Settings.Default.Disks);
            if(disks != null)
            {
                MovieGlobal.GlobalMovieDisksList = disks;
                await DatabaseMovieCheck();
                await History.Set("api", new History_API()
                {
                    api_action = "API started",
                    api_type = "Task -> API status",
                    api_datetime = DateTime.Now
                });
            }
            else
            {
                await History.Set("api", new History_API()
                {
                    api_action = "API -> no movie folder specified",
                    api_type = "Task -> API error",
                    api_datetime = DateTime.Now
                });
            }
        }

        public async Task DatabaseMovieCheck()
        {
            Thread t1;
            await Task.Delay(0);
            try
            {
                t1 = new Thread(async () => await Database.Movie.CheckDB())
                {
                    Priority = ThreadPriority.Normal
                };
                t1.Start();
            }
            catch(Exception ex)
            {
                await History.Set("api", new History_API()
                {
                    api_action = "Exception : Startup.cs-- > "+ ex.Message,
                    api_type = "Task -> API Exception",
                    api_datetime = DateTime.Now
                });
            }
            
           
        }
    }
}
