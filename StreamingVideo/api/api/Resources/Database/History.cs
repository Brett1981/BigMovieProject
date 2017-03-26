using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace api.Resources
{

    public class History
    {
        private static MDBSQLEntities db = new MDBSQLEntities();

        public static async Task<bool> Set(string table, object data)
        {
            switch (table.ToLower())
            {
                case "api": {
                        await SetDataAPI((History_API)data);
                        return true;
                    };
                case "user": {
                        await SetDataUser((History_User)data);
                        return true;
                    };
            }
            return false;
        }
        private static async Task SetDataAPI(History_API api)
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
        private static async Task SetDataUser(History_User user)
        {
            try
            {
                //create a text log of user history
                var debugLog = "User -> | Action - " + user.user_action + " | Id - " + user.user_id + " | ";
                if(user.user_movie != null || user.user_movie != "") debugLog += "Movie - " + user.user_movie;

                Debug.WriteLine(debugLog);
                db.History_User.Add(user);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Problem occured when inserting history_user to database : " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">Specify which table to read from</param>
        /// <param name="par">Specify additional parramaters ex: datetime > 10.10.2016</param>
        /// <returns></returns>
        public static List<T> Get<T>(string table, string[] par = null) 
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
                Debug.WriteLine("Problem occured when getting data from history tables : " + ex.Message);
                return new List<T>();
            }
        }

        public static List<History_User> GetUsers(string table, string[] par = null)
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
                Debug.WriteLine("Problem occured when getting data from history tables : " + ex.Message);
                return new List<History_User>();
            }
        }

        public static List<History_API> GetAPI(string table, string[] par = null)
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
                Debug.WriteLine("Problem occured when getting data from history tables : " + ex.Message);
                return new List<History_API>();
            }
        }


    }
}