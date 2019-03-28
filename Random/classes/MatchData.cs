using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;

namespace Random
{
    public class MatchData
    {
        public Int64 Match_ID;
        public Int64 Host_ID;
        public string Host_Name;
        public Int64 Host_Total;
        public int Host_Stars;
        public Int64 Guest_ID;
        public int Host_Num;
        public int Guest_Num;
        public string Guest_Name;
        public Int64 Guest_Total;
        public int Guest_Stars;
        public Int64 Winner_ID;
        public string Match_Date;
        public int Match_Status;
        public Int64 Match_Price;
        public Int64 Host_Rem_amount;
        public Int64 Guest_Rem_amount;

        // public string Match_Remove_Date;

    }
}