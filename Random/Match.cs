using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;

namespace Random
{
    public class Match
    {

        public Int64 IsAvailable(Int64 Host_ID)
        {
            Int64 Match_ID = 0;
            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT Match_ID FROM match where Host_ID=@Host_ID ");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Host_ID", Host_ID);
                 
                    connection.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.FieldCount > 0)
                    {
                        reader.Read();
                        Match_ID = reader.GetInt64(0);
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return Match_ID;
        }

        public Int64 IsGuestEx(Int64 Guest_ID)
        {
            Int64 Match_ID = 0;
            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT Match_ID FROM match where Guest_ID=@Guest_ID ");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Guest_ID", Guest_ID);

                    connection.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.FieldCount > 0)
                    {
                        reader.Read();
                        Match_ID = reader.GetInt64(0);
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return Match_ID;
        }
    }
}
