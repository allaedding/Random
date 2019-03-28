using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace Random
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void Register(String Player_Name, String Player_Email, String Player_Password)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsAdded = 1;
            String Message = "";
            Int64 Player_id = 0;
            Player Iplayer = new Player();
            if (Iplayer.IsAvailable(Player_Name, Player_Email) == 0)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO player(Player_Name,Player_Password,Player_Email) Output Inserted.Player_ID VALUES (@Player_Name,@Player_Password,@Player_Email)");
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = connection;
                        cmd.Parameters.AddWithValue("@Player_Name", Player_Name);
                        cmd.Parameters.AddWithValue("@Player_Password", Player_Password);
                        cmd.Parameters.AddWithValue("@Player_Email", Player_Email);

                        connection.Open();
                        //cmd.ExecuteNonQuery();
                        Player_id = (Int64)cmd.ExecuteScalar();
                        connection.Close();

                    }
                    Message = "Your account is created succefully";

                }
                catch (Exception ex)
                {
                    IsAdded = 0;
                    Message = ex.Message;

                }
            }
            else
            {
                IsAdded = 0;
                Message = "Username Or Email Is Reserved";
            }
            var JsonData = new
            {
                IsAdded = IsAdded,
                Message = Message,
                Player_ID = Player_id
            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void Login(String Player_Name, String Player_Password)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            Int64 Player_ID = 0;
            Int64 Player_Total = 0;
            String Message = "Successfully Loged In";

            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Player_ID , Player_Total FROM player where Player_Name=@Player_Name and Player_Password=@Player_Password");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Player_Name", Player_Name);
                    cmd.Parameters.AddWithValue("@Player_Password", Player_Password);
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);



                    Player_ID = Convert.ToInt64(dataTable.Rows[0]["Player_ID"]);
                    Player_Total = Convert.ToInt64(dataTable.Rows[0]["Player_Total"]);
                    dataTable.Clear();
                    connection.Close();

                  /* 
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Player_ID = reader.GetInt64(0);
                     
                    }
                    */
                    if (Player_ID == 0)
                    {
                        Message = "Player_Name or Password is incorrect";
                    }

                //    reader.Close();

                //    connection.Close();

                }
            }
            catch (Exception e)
            {
                Message = e.Message;

            }
            var JsonData = new
            {
                Player_ID = Player_ID,
                Player_Total = Player_Total,
                Message = Message
            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }




        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void CreateMatch(Int64 Host_ID, Int64 Host_Num, Int64 Match_Price)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsAdded = 1;
            Int64 match_ID = 0;
            String Message = "";
            Match Imatch = new Match();
            Int64 OldMatchID = Imatch.IsAvailable(Host_ID);
            if (!(OldMatchID == 0))
            {
                //nsertHistoryRemove(OldMatchID);
                InsertHistory(OldMatchID);

            }

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO match(Host_ID,Host_Num,Match_Status,Match_Price) Output Inserted.Match_ID VALUES (@Host_ID,@Host_Num,@Match_Status,@Match_Price)");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    // cmd.Parameters.AddWithValue("@match_ID", match_ID);
                    cmd.Parameters.AddWithValue("@Host_ID", Host_ID);
                    cmd.Parameters.AddWithValue("@Host_Num", Host_Num);
                    cmd.Parameters.AddWithValue("@Match_Status", 1);
                    cmd.Parameters.AddWithValue("@Match_Price", Match_Price);


                    connection.Open();
                    match_ID = (Int64)cmd.ExecuteScalar();
                    // cmd.ExecuteNonQuery();

                    connection.Close();

                }
                Message = "Your Match is created succefully";

            }
            catch (Exception ex)
            {

                IsAdded = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsAdded = IsAdded,
                Message = Message,
                match_ID = match_ID
            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetGuestNum(Int64 Match_ID)
        {

            JavaScriptSerializer ser = new JavaScriptSerializer();


            int guestnum = 0;
            string Message = "";

            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {

                    SqlCommand cmd = new SqlCommand("SELECT Guest_Num FROM match where Match_ID=@Match_ID");
                    cmd.CommandType = CommandType.Text;

                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Match_ID", Match_ID);

                    connection.Open();
                    reader = cmd.ExecuteReader();
                    reader.Read();

                    guestnum = reader.GetInt32(0);

                    connection.Close();

                }
                Message = "Succesful";


            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            var jsonData = new
            {
                GuestNum = guestnum,
                Message = Message

            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));


        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetHostNum(Int64 Match_ID)
        {

            JavaScriptSerializer ser = new JavaScriptSerializer();


            int hostnum = 0;
            string Message = "";

            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {

                    SqlCommand cmd = new SqlCommand("SELECT Host_Num FROM match where Match_ID=@Match_ID");
                    cmd.CommandType = CommandType.Text;

                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Match_ID", Match_ID);

                    connection.Open();
                    reader = cmd.ExecuteReader();
                    reader.Read();

                    hostnum = reader.GetInt32(0);


                    connection.Close();

                }
                Message = "Succesful";


            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            var jsonData = new
            {
                HostNum = hostnum,
                Message = Message

            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));


        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void JoinMatch(Int64 Match_ID, int Guest_Num, Int64 Guest_ID)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsAdded = 1;
            int HostNum = 0;
            String Message = "";
            Match guestex = new Match();
            Int64 oldmatchid = guestex.IsGuestEx(Guest_ID);
            if (!(oldmatchid == 0))
            {
                UpdateMatchbyGuest(oldmatchid);
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE match set Guest_ID=@Guest_ID ,  Guest_Num = @Guest_Num  , Match_Status=0  where Match_ID=@Match_ID ");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Guest_Num", Guest_Num);
                    cmd.Parameters.AddWithValue("@Guest_ID", Guest_ID);
                    cmd.Parameters.AddWithValue("@Match_ID", Match_ID);

                    connection.Open();
                    cmd.ExecuteNonQuery();

                    connection.Close();

                }
                Message = "you have joined the match successfully";

            }
            catch (Exception ex)
            {

                IsAdded = 0;
                Message = ex.Message;

            }
            HostNum = GetHostNumInt(Match_ID);

            var JsonData = new
            {

                IsAdded = IsAdded,
                Message = Message,
                Host_Num = HostNum

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void PlayerStatusUpdate(Int64 player_ID, int PlayerStatus)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdated = 1;
            String Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE player set Player_Status = @PlayerStatus where Player_ID=@player_ID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@PlayerStatus", PlayerStatus);
                    cmd.Parameters.AddWithValue("@player_ID", player_ID);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                }
                Message = "you have update the player Status successfully";

            }
            catch (Exception ex)
            {

                IsUpdated = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsUpdated = IsUpdated,
                Message = Message,

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void PlayerWinUpdate(Int64 player_ID)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdated = 1;
            String Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE player set Player_win = (dbo.player.Player_win + 1) where Player_ID=@player_ID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@player_ID", player_ID);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                }
                Message = "you have update the player Wins successfully";

            }
            catch (Exception ex)
            {

                IsUpdated = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsUpdated = IsUpdated,
                Message = Message,

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void PlayerLossUpdate(Int64 player_ID)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdated = 1;
            String Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE player set Player_Loss = (dbo.player.Player_Loss + 1) where Player_ID=@player_ID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@player_ID", player_ID);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                }
                Message = "you have update the player Losses successfully";

            }
            catch (Exception ex)
            {

                IsUpdated = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsUpdated = IsUpdated,
                Message = Message,

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }



        /*
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void PlayerStarsAdd(string player_ID)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdated = 1;
            String Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE player set Player_Stars = (dbo.player.Player_Stars + 1) where Player_ID=@player_ID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@player_ID", player_ID);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                }
                Message = "you have update the player Stars successfully";

            }
            catch (Exception ex)
            {

                IsUpdated = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsUpdated = IsUpdated,
                Message = Message,

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }

    */ //old method to add stars


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void PlayerStarsUpdate(Int64 player_ID, int Stars) //stars can be 2 for adding a star || 0 for deleting a star
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdated = 1;
            String Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE player set Player_Stars = (dbo.player.Player_Stars - 1 + @Stars) where Player_ID=@player_ID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@player_ID", player_ID);
                    cmd.Parameters.AddWithValue("@Stars", Stars);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                }
                Message = "you have update the player Stars successfully";

            }
            catch (Exception ex)
            {

                IsUpdated = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsUpdated = IsUpdated,
                Message = Message,

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void PlayerTotalUpdate(Int64 player_ID, Int64 NewAmount)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdated = 1;
            String Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE player set Player_Total =@NewAmount where Player_ID=@player_ID");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@player_ID", player_ID);
                    cmd.Parameters.AddWithValue("@NewAmount", NewAmount);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                }
                Message = "you have update the player total successfully";

            }
            catch (Exception ex)
            {

                IsUpdated = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsUpdated = IsUpdated,
                Message = Message,

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetPlayerData(Int64 Player_ID)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            PlayerData playerdata = new PlayerData();
            int IsAny = 1;
            try
            {
                // SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT * FROM player where Player_ID=@Player_ID";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Player_ID", Player_ID);
                    // connection.Open();
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);



                    playerdata.PlayerName = Convert.ToString(dataTable.Rows[0]["Player_Name"]);
                    playerdata.PlayerTotal = Convert.ToInt64(dataTable.Rows[0]["Player_Total"]);
                    playerdata.PlayerStars = Convert.ToInt32(dataTable.Rows[0]["Player_Stars"]);
                    playerdata.PlayerWins = Convert.ToInt64(dataTable.Rows[0]["Player_Win"]);
                    playerdata.PlayerLoss = Convert.ToInt64(dataTable.Rows[0]["Player_Loss"]);

                    // dataTable.Clear();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                IsAny = 0;
            }
            var jsonData = new
            {

                IsAny = IsAny,
                PlayerName =  playerdata.PlayerName ,
                PlayerTotal =  playerdata.PlayerTotal ,
                PlayerStars = playerdata.PlayerStars ,
                PlayerWins = playerdata.PlayerWins ,
                PlayerLoss = playerdata.PlayerLoss
        };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));

        }




        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void PlayerDataUpdate(Int64 Player_ID, Int64 NewAmount, int Stars, Int64 Win, Int64 Loss)
        { // 2 stars to add a star and 0 to delete a star ** newamount to add it to the player total ** 1 or 0 to win and loss
            // newamount paramiter accept negative values **
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdated = 1;
            String Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE player set Player_Total=(dbo.player.Player_Total +@NewAmount ) , Player_Stars=(dbo.player.Player_Stars - 1 + @Stars) , Player_Win=(dbo.player.Player_Win + @Win) , Player_Loss=(dbo.player.Player_Loss + @Loss) where Player_ID=@Player_ID ");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Player_ID", Player_ID);
                    cmd.Parameters.AddWithValue("@NewAmount", NewAmount);
                    cmd.Parameters.AddWithValue("@Stars", Stars);
                    cmd.Parameters.AddWithValue("@Win", Win);
                    cmd.Parameters.AddWithValue("@Loss", Loss);


                    connection.Open();
                    cmd.ExecuteNonQuery();

                    connection.Close();

                }
                Message = "you have Updares the Player Data successfully";

            }
            catch (Exception ex)
            {

                IsUpdated = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsUpdated = IsUpdated,
                Message = Message,

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }






        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetOnlineMatches()
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MatchData[] matchdata = null;
            PlayerData HostData = new PlayerData();
            Int64 playerid = 0;
            int IsAny = 1;
            String message = "";
            try
            {
                // SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    connection.Open();
                    SqlDataAdapter adpt = new SqlDataAdapter("SELECT Match_ID , Host_ID ,Match_Price FROM match where Match_Status=1", connection);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);
                    matchdata = new MatchData[dataTable.Rows.Count];
                    int Count = 0;
                    if (dataTable.Rows.Count == 0) { IsAny = 0; }
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        matchdata[Count] = new MatchData();
                        matchdata[Count].Match_ID = Convert.ToInt64(dataTable.Rows[i]["Match_ID"]);
                        matchdata[Count].Host_ID = Convert.ToInt64(dataTable.Rows[i]["Host_ID"]);
                        matchdata[Count].Match_Price = Convert.ToInt64(dataTable.Rows[i]["Match_Price"]);
                        // matchdata[Count].Guest_Rem_amount = Convert.ToInt64(dataTable.Rows[i]["Guest_Rem_amount"]);
                        //   matchdata[Count].Host_Rem_amount = Convert.ToInt64(dataTable.Rows[i]["Host_Rem_amount"]);
                        playerid = matchdata[Count].Host_ID;
                        HostData = GetPlayertData(playerid);
                        matchdata[Count].Host_Name = Convert.ToString(HostData.PlayerName);
                        matchdata[Count].Host_Total = Convert.ToInt64(HostData.PlayerTotal);
                        matchdata[Count].Host_Stars = Convert.ToInt32(HostData.PlayerStars);
                        Count++;

                    }


                    dataTable.Clear();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                IsAny = 0;
            }
            var jsonData = new
            {
                IsAny = IsAny,
                message = message,
                MatchData = matchdata
            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));

        }




        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetTopPlayers()
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            PlayerData[] playerdata = null;
            try
            {
                // SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    connection.Open();
                    SqlDataAdapter adpt = new SqlDataAdapter("select top(10) * from player order by Player_Total DESC ", connection);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);
                    playerdata = new PlayerData[dataTable.Rows.Count];
                    int Count = 0;
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        playerdata[Count] = new PlayerData();
                        playerdata[Count].PlayerName = Convert.ToString(dataTable.Rows[i]["Player_Name"]);
                        playerdata[Count].PlayerTotal = Convert.ToInt64(dataTable.Rows[i]["Player_Total"]);
                        playerdata[Count].PlayerStars = Convert.ToInt32(dataTable.Rows[i]["Player_Stars"]);
                        playerdata[Count].PlayerWins = Convert.ToInt64(dataTable.Rows[i]["Player_Win"]);
                        playerdata[Count].PlayerLoss = Convert.ToInt64(dataTable.Rows[i]["Player_Loss"]);
                        Count++;

                    }
                    dataTable.Clear();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {

            }
            var jsonData = new
            {
                PlayerData = playerdata
            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));

        }




        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetMatchData(Int64 Match_ID)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            MatchData matchdata = new MatchData();
            int IsAny = 1;
            string message = "";
            PlayerData GuestData = new PlayerData();
            Int64 playerid = 0;
            try
            {
                // SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT * FROM match where Match_ID=@Match_ID";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Match_ID", Match_ID);
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);

                      matchdata.Match_Price = Convert.ToInt64(dataTable.Rows[0]["Match_Price"]);
                        matchdata.Host_Rem_amount = Convert.ToInt64(dataTable.Rows[0]["Host_Rem_amount"]);
                        matchdata.Guest_Rem_amount = Convert.ToInt64(dataTable.Rows[0]["Guest_Rem_amount"]);

                      matchdata.Match_Status = Convert.ToInt32(dataTable.Rows[0]["Match_Status"]);
                      matchdata.Match_ID = Convert.ToInt64(dataTable.Rows[0]["Match_ID"]);
                    matchdata.Host_ID = Convert.ToInt64(dataTable.Rows[0]["Host_ID"]);
                       matchdata.Guest_ID = Convert.ToInt64(dataTable.Rows[0]["Guest_ID"]);
                    playerid = matchdata.Guest_ID;
                    GuestData = GetPlayertData(playerid);
                    matchdata.Guest_Name = Convert.ToString(GuestData.PlayerName);
                    matchdata.Guest_Total = Convert.ToInt64(GuestData.PlayerTotal);
                    matchdata.Guest_Stars = Convert.ToInt32(GuestData.PlayerStars);

                     matchdata.Host_Num = Convert.ToInt32(dataTable.Rows[0]["Host_Num"]);
                    matchdata.Guest_Num = Convert.ToInt32(dataTable.Rows[0]["Guest_Num"]);
                    matchdata.Winner_ID = Convert.ToInt64(dataTable.Rows[0]["Winner_ID"]);



                    // dataTable.Clear();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                IsAny = 0;
                message = ex.Message;
            }
            var jsonData = new
            {
                IsAny = IsAny,
                message = message,
                Match_Status = matchdata.Match_Status,
                Match_ID = matchdata.Match_ID,
                Match_Price = matchdata.Match_Price,
                Host_Rem_amount = matchdata.Host_Rem_amount,
                Guest_Rem_amount = matchdata.Guest_Rem_amount,
                Host_Num = matchdata.Host_Num,
                Host_ID = matchdata.Host_ID,
                Guest_ID = matchdata.Guest_ID,
                Guest_Num = matchdata.Guest_Num,
                Winner_ID = matchdata.Winner_ID,
                Guest_Name = matchdata.Guest_Name,
                Guest_Total = matchdata.Guest_Total,
                Guest_Stars = matchdata.Guest_Stars

            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));

        }





        //GetString() methode was created to give a unique ID to the match 
        public static string GetString(Int64 hostid)
        {
            string str = DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "");
            str = str.Substring(0, str.Length - 2);
            str = str + hostid;
            return str;
        }

        public static PlayerData GetPlayertData(Int64 HostID)
        {
            PlayerData playerdata = new PlayerData();
            try
            {
                // SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT * FROM player where Player_ID=@Player_ID";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Player_ID", HostID);
                    // connection.Open();
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);



                    playerdata.PlayerName = Convert.ToString(dataTable.Rows[0]["Player_Name"]);
                    playerdata.PlayerTotal = Convert.ToInt64(dataTable.Rows[0]["Player_Total"]);
                    playerdata.PlayerStars = Convert.ToInt32(dataTable.Rows[0]["Player_Stars"]);
                    playerdata.PlayerWins = Convert.ToInt64(dataTable.Rows[0]["Player_Win"]);
                    playerdata.PlayerLoss = Convert.ToInt64(dataTable.Rows[0]["Player_Loss"]);

                    // dataTable.Clear();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {

            }
            return playerdata;

        }


        public static void RemoveMatch(Int64 MatchID)
        {

            GetMatcheDataByHost(MatchID);




            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("delete from match where Match_ID = @MatchID");
                    cmd.Parameters.AddWithValue("@MatchID", MatchID);
                    cmd.Connection = connection;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();

                }
            }



            catch (Exception ex)
            {

            }

        }




        public static MatchData GetMatcheDataByHost(Int64 MatchID)
        {

            MatchData matchdata = new MatchData();
            try
            {
                // SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "SELECT * FROM match where Match_ID=@MatchID";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@MatchID", MatchID);
                    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adpt.Fill(dataTable);


                    matchdata.Match_ID = Convert.ToInt64(dataTable.Rows[0]["Match_ID"]);
                    matchdata.Host_ID = Convert.ToInt64(dataTable.Rows[0]["Host_ID"]);
                    matchdata.Guest_ID = Convert.ToInt64(dataTable.Rows[0]["Guest_ID"]);
                    matchdata.Host_Num = Convert.ToInt32(dataTable.Rows[0]["Host_Num"]);
                    matchdata.Guest_Num = Convert.ToInt32(dataTable.Rows[0]["Guest_Num"]);
                    matchdata.Winner_ID = Convert.ToInt64(dataTable.Rows[0]["Winner_ID"]);
                    matchdata.Match_Date = Convert.ToString(dataTable.Rows[0]["Match_Date"]);



                    // dataTable.Clear();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {

            }
            if (matchdata.Match_ID == 0)
            {
                return null;
            }
            else
            {
                return matchdata;
            }
        }




        public static void InsertHistory(Int64 MatchID)
        {

            MatchData matchdata = new MatchData();

            matchdata = GetMatcheDataByHost(MatchID);
            if (!(matchdata.Match_ID == 0))
            {
                RemoveMatch(MatchID);

                try
                {
                    using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO history(Match_ID,Host_ID,Guest_ID,Host_Num,Guest_Num,Winner_ID) VALUES (@Match_ID,@Host_ID,@Guest_ID,@Host_Num,@Guest_Num,@Winner_ID)");
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = connection;

                        cmd.Parameters.AddWithValue("@Match_ID", Convert.ToInt64(matchdata.Match_ID));
                        cmd.Parameters.AddWithValue("@Host_ID", Convert.ToInt64(matchdata.Host_ID));
                        cmd.Parameters.AddWithValue("@Guest_ID", Convert.ToInt64(matchdata.Guest_ID));
                        cmd.Parameters.AddWithValue("@Host_Num", Convert.ToInt32(matchdata.Host_Num));
                        cmd.Parameters.AddWithValue("@Guest_Num", Convert.ToInt32(matchdata.Guest_Num));
                        cmd.Parameters.AddWithValue("@Winner_ID", Convert.ToInt64(matchdata.Winner_ID));
                        //   cmd.Parameters.AddWithValue("@Match_Date", matchdata.Match_Date);


                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();

                    }


                }
                catch (Exception ex)
                {

                }
            }

        }


        /* public static void InsertHistoryRemove(Int64 MatchID)
          {
              InsertHistory( MatchID);
              RemoveMatch(MatchID);

          }
        */




        public static void UpdateMatchbyGuest(Int64 Match_ID)
        {

            InsertHistory(Match_ID);
            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE match set Guest_ID=Null ,  Guest_Num =Null,  Winner_ID =Null  , Match_Status=1  where Match_ID=@Match_ID ");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Match_ID", Convert.ToInt64(Match_ID));

                    connection.Open();
                    cmd.ExecuteNonQuery();

                    connection.Close();

                }

            }
            catch (Exception ex)
            {

            }


        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void RemoveMatchEnd(Int64 Match_ID)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsRemoved = 1;
            string message = "Match has being Moved to History";
            try
            {
                InsertHistory(Match_ID);
            }
            catch (Exception ex)
            {
                IsRemoved = 0;
                message = ex.Message;
            }
            var jsonData = new
            {
                IsRemoved = IsRemoved,
                Message = message

            };
            HttpContext.Current.Response.Write(ser.Serialize(jsonData));

        }

        public static int GetHostNumInt(Int64 Match_ID)
        {

            JavaScriptSerializer ser = new JavaScriptSerializer();


            int hostnum = 0;
            string Message = "";

            try
            {
                SqlDataReader reader;
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {

                    SqlCommand cmd = new SqlCommand("SELECT Host_Num FROM match where Match_ID=@Match_ID");
                    cmd.CommandType = CommandType.Text;

                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Match_ID", Match_ID);

                    connection.Open();
                    reader = cmd.ExecuteReader();
                    reader.Read();

                    hostnum = reader.GetInt32(0);


                    connection.Close();

                }
                Message = "Succesful";


            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            return hostnum;
        }


        //************************ TESTING ********************************





        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void MatchDataUpdate(Int64 Match_ID, int Match_Status, Int64 Host_Rem_amount, Int64 Guest_Rem_amount, Int64 Winner_ID)
        { // 2 stars to add a star and 0 to delete a star ** newamount to add it to the player total ** 1 or 0 to win and loss
            // newamount paramiter accept negative values **
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdated = 1;
            String Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE match set Match_Status=@Match_Status ,Host_Rem_amount=@Host_Rem_amount , Guest_Rem_amount=@Guest_Rem_amount ,Winner_ID=@Winner_ID    where Match_ID=@Match_ID ");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Match_ID", Match_ID);
                    cmd.Parameters.AddWithValue("@Match_Status", Match_Status);
                    cmd.Parameters.AddWithValue("@Host_Rem_amount", Host_Rem_amount);
                    cmd.Parameters.AddWithValue("@Guest_Rem_amount", Guest_Rem_amount);
                    cmd.Parameters.AddWithValue("@Winner_ID", Winner_ID);
                    


                    connection.Open();
                    cmd.ExecuteNonQuery();

                    connection.Close();

                }
                Message = "you have Updares the Match Data successfully";

            }
            catch (Exception ex)
            {

                IsUpdated = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsUpdated = IsUpdated,
                Message = Message,

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }




        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void MatchDataWinnerUpdate(Int64 Match_ID, Int64 Winner_ID)
        { // 2 stars to add a star and 0 to delete a star ** newamount to add it to the player total ** 1 or 0 to win and loss
            // newamount paramiter accept negative values **
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdated = 1;
            String Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE match set Winner_ID=@Winner_ID    where Match_ID=@Match_ID ");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Match_ID", Match_ID);
                    cmd.Parameters.AddWithValue("@Winner_ID", Winner_ID);



                    connection.Open();
                    cmd.ExecuteNonQuery();

                    connection.Close();

                }
                Message = "you have Update the Match Data successfully";

            }
            catch (Exception ex)
            {

                IsUpdated = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsUpdated = IsUpdated,
                Message = Message,

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }




        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void MatchDataHostRemUpdate(Int64 Match_ID, Int64 Host_Rem_amount)
        { // 2 stars to add a star and 0 to delete a star ** newamount to add it to the player total ** 1 or 0 to win and loss
            // newamount paramiter accept negative values **
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdated = 1;
            String Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE match set Host_Rem_amount=@Host_Rem_amount where Match_ID=@Match_ID ");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Match_ID", Match_ID);
                    cmd.Parameters.AddWithValue("@Host_Rem_amount", Host_Rem_amount);
                    



                    connection.Open();
                    cmd.ExecuteNonQuery();

                    connection.Close();

                }
                Message = "you have Updares the Match Data successfully";

            }
            catch (Exception ex)
            {

                IsUpdated = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsUpdated = IsUpdated,
                Message = Message,

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void MatchDataGuestRemUpdate(Int64 Match_ID, Int64 Guest_Rem_amount)
        { // 2 stars to add a star and 0 to delete a star ** newamount to add it to the player total ** 1 or 0 to win and loss
            // newamount paramiter accept negative values **
            JavaScriptSerializer ser = new JavaScriptSerializer();
            int IsUpdated = 1;
            String Message = "";

            try
            {
                using (SqlConnection connection = new SqlConnection(DBConnection.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("UPDATE match set Guest_Rem_amount=@Guest_Rem_amount where Match_ID=@Match_ID ");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Match_ID", Match_ID);
                    cmd.Parameters.AddWithValue("@Guest_Rem_amount", Guest_Rem_amount);
                    

                    connection.Open();
                    cmd.ExecuteNonQuery();

                    connection.Close();

                }
                Message = "you have Updares the Match Data successfully";

            }
            catch (Exception ex)
            {

                IsUpdated = 0;
                Message = ex.Message;

            }

            var JsonData = new
            {

                IsUpdated = IsUpdated,
                Message = Message,

            };
            HttpContext.Current.Response.Write(ser.Serialize(JsonData));
        }



        //************************END WebService**********************

    }
    //************************END Playersws class***********************
}
