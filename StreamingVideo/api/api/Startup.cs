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
            await History.Create("api", new History_API()
            {
                api_action = "API initializing",
                api_type = "Task -> API status",
                api_datetime = DateTime.Now
            });
            var disks = await Settings.Get.ToObject(Settings.Type.Disks);
            var settings = await Settings.Get.ToObject(Settings.Type.Settings);
            if(disks != null && settings != null)
            {
                Global.GlobalMovieDisksList = disks;
                Global.GlobalServerSettings = settings;
                await DatabaseMovieCheck();
                await History.Create("api", new History_API()
                {
                    api_action = "API started",
                    api_type = "Task -> API status",
                    api_datetime = DateTime.Now
                });
            }
            else
            {
                string[] txtError = new string[2];
                
                if(settings.Where(x => x.name == "APIKey").First().value == null || settings.Where(x => x.name == "APIKey").First().value == "")
                    txtError[0] += "no API key specified ";

                if (disks.Count == 0 ||disks == null)
                    txtError[1] = " no movie folder specified";

                if (txtError != null )
                {
                    await History.Create("api", new History_API()
                    {
                        api_action = "API-> " + String.Join(" | ",txtError) + " !",
                        api_type = "Task -> API error",
                        api_datetime = DateTime.Now
                    });
                }
                
            }
        }

        public async Task DatabaseMovieCheck()
        {
            Thread t1;
            await Task.Delay(0);
            try
            {
                t1 = new Thread(async () => await Database.Movie.Check.DatabaseThread())
                {
                    Priority = ThreadPriority.Normal
                };
                t1.Start();
            }
            catch(Exception ex)
            {
                await History.Create("api", new History_API()
                {
                    api_action = "Exception : Startup.cs-- > "+ ex.Message,
                    api_type = "Task -> API Exception",
                    api_datetime = DateTime.Now
                });
            }
            
           
        }
    }
}
