using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;

namespace Random
{
    public class Player
    {

        public Int64 IsAvailable(string Player_Name, string Player_Email)
        {
            Int64 Player_ID = 0;
            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT Player_ID FROM player where Player_Name= @Player_Name or Player_Email=@Player_Email ");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Player_Name", Player_Name);
                    cmd.Parameters.AddWithValue("@Player_Email", Player_Email);
                    connection.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.FieldCount > 0)
                    {
                        reader.Read();
                        Player_ID = reader.GetInt64(0);
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return Player_ID;
        }
    }
}
