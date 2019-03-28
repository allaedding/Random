using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Web.Configuration;

namespace Random
{
    public class DBConnection
    {
        public static string ConnectionString
        {
            get
            {
                return WebConfigurationManager.ConnectionStrings["randomxConnectionString"].ConnectionString.ToString();
            }


        }
    }
}