using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using api.Resources.Functions;

namespace api.Resources.Email
{
    public class Email
    {
        private static string Username      = "zan1515@gmail.com";
        private static string Password      = "TRAPSDOU1fN26yzc";
        private static string SmtpServer    = "smtp-relay.sendinblue.com";
        private static int SmtpPort         = 587;
        private static SmtpClient client = new SmtpClient(SmtpServer, SmtpPort)
        {
            Credentials = new NetworkCredential(Username, Password),
            EnableSsl = true
        };
        
        public static bool Send(User_Info u)
        {
            MailAddress m = new MailAddress(u.email);
            if (m != null)
            {
                try
                {
                    client.Send("zan.pangrsic@gmail.com", m.ToString(), "Thank you for your registration", 
                        "Hello " + u.username +"\n Thank you for registering on MovieDatabase website.\nWe hope you will enjoy your stay and movies." +
                        "\nHere is your login information:" +
                        "\n"+u.username +
                        "\n"+ Functions.Functions.Decode.Base64toString(u.password)
                        );
                    return true;
                }
                catch (SmtpException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return false;
           
        }
    }
}