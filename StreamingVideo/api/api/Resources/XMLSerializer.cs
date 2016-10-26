using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace api.Resources
{
    public class XMLSerializer
    {
        /*public static XMLSerializer createXMLObject(MovieData data)
        {
            XmlSerializer ser = new XmlSerializer(data.GetType());
            string result = string.Empty;

            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, data);

                memStm.Position = 0;
                result = new StreamReader(memStm).ReadToEnd();
            }

            return result;
        }*/
        
    }
}