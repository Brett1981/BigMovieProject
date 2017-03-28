using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace api.Resources
{

    public  class History
    {
        private static MDBSQLEntities db = new MDBSQLEntities();
        public static class Set 
        {
            public static async Task API(History_API api)
            {
                try
                {
                    //create a text log of api history
                    Debug.WriteLine("Api -> | Action - " + api.api_action + " | Type - " + api.api_type);
                    db.History_API.Add(api);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem occured when inserting history_api to database : " + ex.Message);
                }

            }

            public static async Task User(History_User user)
            {
                try
                {
                    //create a text log of user history
                    var debugLog = "User -> | Action - " + user.user_action + " | Id - " + user.user_id + " | ";
                    if (user.user_movie != null || user.user_movie != "") debugLog += "Movie - " + user.user_movie;

                    Debug.WriteLine(debugLog);
                    db.History_User.Add(user);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Problem occured when inserting history_user to database : " + ex.Message);
                }
            }
        }
        public static class Get
        {
            public static async Task<List<History_API>> API(string table, string[] par = null)
            {
                try
                {
                    if (par == null)
                    {
                        return db.History_API.ToList();
                    }
                    else
                    {
                        return new List<History_API>();
                    }
                }
                catch (Exception ex)
                {
                    await Set.API(new History_API() {
                        api_action = "Problem occured when getting data from history tables : " + ex.Message,
                        api_type = "Exception -> thrown at Get.API",
                        api_datetime = DateTime.Now
                    });
                    return new List<History_API>();
                }
            }

            public static async Task<List<History_User>> Users(string table, string[] par = null)
            {
                try
                {
                    if (par == null)
                    {
                        return db.History_User.ToList();
                    }
                    else
                    {
                        return new List<History_User>();
                    }
                }
                catch (Exception ex)
                {
                    await Set.API(new History_API()
                    {
                        api_action = "Problem occured when getting data from history tables : " + ex.Message,
                        api_type = "Exception -> thrown at Get.Users",
                        api_datetime = DateTime.Now
                    });
                    return new List<History_User>();
                }
            }
        }
        public static async Task<bool> Create(string table, object data)
        {
            switch (table.ToLower())
            {
                case "api": {
                        await Set.API((History_API)data);
                        return true;
                    };
                case "user": {
                        await Set.User((History_User)data);
                        return true;
                    };
            }
            return false;
        }
        
        
        /// <summary>
        /// Return list of database entries for specific table of history 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">Specify which table to read from</param>
        /// <param name="par">Specify additional parramaters ex: datetime > 10.10.2016</param>
        /// <returns></returns>
        public static async Task<List<T>> Return<T>(string table, string[] par = null) 
        {
            try
            {
                if(par == null)
                {
                    switch (table.ToLower())
                    {
                        case "api": {
                                var a = db.History_API.ToList();
                                return a as List<T>; };
                        case "user": {
                                var u = db.History_User.ToList();
                                return u as List<T>; };
                    }
                    return new List<T>();
                }
                else
                {
                    return new List<T>();
                }
            }
            catch (Exception ex)
            {
                await Set.API(new History_API()
                {
                    api_action = "Problem occured when getting data from history tables : " + ex.Message,
                    api_type = "Exception -> thrown at History.Return",
                    api_datetime = DateTime.Now
                });
                return new List<T>();
            }
        }
    }
}