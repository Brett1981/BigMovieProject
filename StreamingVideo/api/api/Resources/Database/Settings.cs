using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using api.Resources;
using Newtonsoft.Json;
using System.Data.Entity;
using System.IO;

namespace api.Resources
{
    public class Settings
    {
        public enum Type
        {
            Disks,
            Settings
        }
        private static MovieDatabaseEntities db = new MovieDatabaseEntities();

        public static class Get
        {
            /// <summary>
            /// Return list of items as objects
            /// </summary>
            /// <param name="item">Type</param>
            /// <returns>List<CustomClasses.API.Settings></returns>
            public static async Task<List<CustomClasses.API.Settings>> ToObject(Type item)
            {
                return await FromDatabaseToObject(item);
            }
            /// <summary>
            /// Return list of items as Json string
            /// </summary>
            /// <param name="item">Type</param>
            /// <returns>String</returns>
            public static async Task<string> ToString(Type item)
            {
                return await FromDatabaseToString(item);
            }

            private static async Task<List<CustomClasses.API.Settings>> FromDatabaseToObject(Type item)
            {
                var setting = await db.API_Settings
                    .Where(x => x.name == item.ToString())
                    .FirstOrDefaultAsync();
                    
                return JsonConvert.DeserializeObject<List<CustomClasses.API.Settings>>(setting.value);
            }
            private static async Task<string> FromDatabaseToString(Type item)
            {
                return JsonConvert.SerializeObject(await db.API_Settings.Where(x => x.name == item.ToString()).FirstOrDefaultAsync());
            }
        }

       

        public class Set
        {
            /// <summary>
            /// Write data to database using type and string to as json to input data to db
            /// </summary>
            /// <param name="item">Type</param>
            /// <param name="data">String</param>
            /// <returns>List<CustomClasses.API.Settings></returns>
            public static async Task<List<CustomClasses.API.Settings>> Data(Type item, string data)
            {
                return await ToDatabase(item, data);
            }
            /// <summary>
            /// Writes data to db
            /// </summary>
            /// <param name="item">Type</param>
            /// <param name="data">String</param>
            /// <returns></returns>
            private static async Task<List<CustomClasses.API.Settings>> ToDatabase(Type item, string data)
            {
                var setting = await db.API_Settings.Where(x => x.name == item.ToString()).FirstOrDefaultAsync();
                if(setting != null)
                {
                    try
                    {
                        if(JsonConvert.DeserializeObject<List<CustomClasses.API.Settings>>(data) != null)
                        {
                            setting.value = data;
                            await db.SaveChangesAsync();

                            return JsonConvert.DeserializeObject<List<CustomClasses.API.Settings>>(
                                JsonConvert.SerializeObject(
                                    await db.API_Settings.Where(x => x.name == item.ToString()).ToListAsync())
                                    );
                        }
                        
                    }
                    catch(Exception ex)
                    {
                        await History.Create(History.Type.API, new History_API()
                        {
                            api_action = "Exception caught on APISettings.Set.ToDatabase -> "+ex.Message,
                            api_type = "Task -> Exception thrown"
                        });
                    }
                    
                }
                return null;
            }
        }

        public class Edit
        {
            public static async Task<bool> All(CustomClasses.API.Data data)
            {
                //edit api settings
                var settings = JsonConvert.DeserializeObject<List<CustomClasses.API.Settings>>(Properties.Resources.ResourceManager.GetString("Settings"));
                var disks = JsonConvert.DeserializeObject<List<CustomClasses.API.Settings>>(Properties.Resources.ResourceManager.GetString("Disks"));
                if (data.disks != disks)
                {
                    await History.Create(History.Type.API, new History_API()
                    {
                        api_action = "Task -> new settings submited for API -> Checking new movie directory validation ...",
                        api_type = "Task -> Checking new movie directory location",
                        api_datetime = DateTime.Now
                    });
                    //disks are edited
                    foreach (var disk in data.disks)
                    {
                        if (!Directory.Exists(disk.value))
                        {
                            await History.Create(History.Type.API, new History_API()
                            {
                                api_action = "Task-> location does not exist, " + disk.value,
                                api_type = "Task -> Checking directory destination failed",
                                api_datetime = DateTime.Now
                            });
                            return false;
                        }
                    }
                    await History.Create(History.Type.API, new History_API()
                    {
                        api_action = "Task -> Completed movie directory check",
                        api_type = "Task -> Completed",
                        api_datetime = DateTime.Now
                    });
                    await Set.Data(Type.Disks, JsonConvert.SerializeObject(data.disks));
                    

                }
                if (data.settings != settings)
                {
                    //settings are edited
                    await History.Create(History.Type.API, new History_API()
                    {
                        api_action = "Task -> checking new submited settings",
                        api_type = "Task -> Checking submited settings",
                        api_datetime = DateTime.Now
                    });
                    foreach (var s in data.settings)
                    {
                        if (s.value == "")
                        {
                            await History.Create(History.Type.API, new History_API()
                            {
                                api_action = "Task -> Settings " + s.name + " -> value is empty",
                                api_type = "Task -> Setting string is null",
                                api_datetime = DateTime.Now
                            });
                            return false;
                        }

                    }
                    await History.Create(History.Type.API, new History_API()
                    {
                        api_action = "Task -> Settings are valid",
                        api_type = "Task -> Completed",
                        api_datetime = DateTime.Now
                    });
                    await Set.Data(Type.Settings, JsonConvert.SerializeObject(data.settings)); 
    
                }

                return true;
            }
        }
    }
}