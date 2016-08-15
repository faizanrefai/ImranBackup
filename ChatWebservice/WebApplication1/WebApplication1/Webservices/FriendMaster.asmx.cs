using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;
using System.Web.Script.Services;
using WebApplication1.CommonClass;

namespace WebApplication1.Webservices
{
    /// <summary>
    /// Summary description for FriendMaster
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class FriendMaster : System.Web.Services.WebService
    {
        private SqlCommand cmd = null;
        SQLAccess objSQLAccess = new SQLAccess();

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string REQUESTINSERT(string FromFriendID, string ToFriendID,string FromUserName)
        {
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "FriendMasterInserUpdateGet";
                cmd.Parameters.AddWithValue("@FromFriendID", FromFriendID);
                cmd.Parameters.AddWithValue("@ToFriendID", ToFriendID);
                cmd.Parameters.AddWithValue("@Mode", "REQUESTINSERT");
                int i = objSQLAccess.ExecuteNonQuery(cmd, ref exMessage);
                if (i >= 1)
                {
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "True";
                    objBase.sendNotication(ToFriendID, "You have friend request from " + FromUserName);
                }
                else
                {
                    if (Convert.ToString(exMessage).Length > 0)
                    {
                        dsDataSet.Tables["Error"].Rows[0]["Message"] = exMessage.ToString();
                    }
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";

                }
            }
            catch (Exception ex)
            {
                dsDataSet.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();
                dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
            }
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string REQUESTACCEPT(string FriendID, string FromFriendID,string UserName)
        {
            DataTable dtResult = new DataTable();
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "FriendMasterInserUpdateGet";
                cmd.Parameters.AddWithValue("@FriendID", FriendID);
                cmd.Parameters.AddWithValue("@Mode", "REQUESTACCEPT");
                int i = objSQLAccess.ExecuteNonQuery(cmd, ref exMessage);
                if (i >= 1)
                {
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "True";
                    objBase.sendNotication(FromFriendID, UserName + " is now your friend");
                }
                else
                {
                    if (Convert.ToString(exMessage).Length > 0)
                    {
                        dsDataSet.Tables["Error"].Rows[0]["Message"] = exMessage.ToString();
                    }
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";

                }
            }
            catch (Exception ex)
            {
                dsDataSet.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();
                dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
            }
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string REQUESTREJECT(string FriendID, string FromFriendID, string UserName)
        {
            DataTable dtResult = new DataTable();
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "FriendMasterInserUpdateGet";
                cmd.Parameters.AddWithValue("@FriendID", FriendID);
                cmd.Parameters.AddWithValue("@Mode", "REQUESTREJECT");
                int i = objSQLAccess.ExecuteNonQuery(cmd, ref exMessage);
                if (i >= 1)
                {
                    objBase.sendNotication(FromFriendID, UserName + " reject a friend request");
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "True";
                }
                else
                {
                    if (Convert.ToString(exMessage).Length > 0)
                    {
                        dsDataSet.Tables["Error"].Rows[0]["Message"] = exMessage.ToString();
                    }
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
                }
            }
            catch (Exception ex)
            {
                dsDataSet.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();
                dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
            }
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RemoveFriend(string FromFriendID, string ToFriendID)
        {
            DataTable dtResult = new DataTable();
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "FriendMasterInserUpdateGet";
                cmd.Parameters.AddWithValue("@FromFriendID", FromFriendID);
                cmd.Parameters.AddWithValue("@ToFriendID", ToFriendID);
                cmd.Parameters.AddWithValue("@Mode", "REMOVEFRIEND");
                int i = objSQLAccess.ExecuteNonQuery(cmd, ref exMessage);
                if (i >= 1)
                {
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "True";
                }
                else
                {
                    if (Convert.ToString(exMessage).Length > 0)
                    {
                        dsDataSet.Tables["Error"].Rows[0]["Message"] = exMessage.ToString();
                    }
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
                }
            }
            catch (Exception ex)
            {
                dsDataSet.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();
                dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
            }
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string REQUESTPENDINGLIST(string LoginID) // Here use FromFriendid
        {
            DataTable dtResult = new DataTable();
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            string exMessage = string.Empty;
            cmd = new SqlCommand();
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "FriendMasterInserUpdateGet";
                cmd.Parameters.AddWithValue("@ToFriendID", LoginID);
                cmd.Parameters.AddWithValue("@Mode", "REQUESTPENDINGLIST");
                dtResult = objSQLAccess.GetDt(cmd, ref exMessage);
                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "True";
                }
                else
                {
                    if (Convert.ToString(exMessage).Length > 0)
                    {
                        dsDataSet.Tables["Error"].Rows[0]["Message"] = exMessage.ToString();
                    }
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
                }
            }
            catch (Exception ex)
            {
                dsDataSet.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();
                dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
            }
            dtResult.TableName = "REQUESTPENDINGLIST";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string USERFRIENDLIST(string LoginID) //Here use ToFriendid
        {
            DataTable dtResult = new DataTable();
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "FriendMasterInserUpdateGet";
                cmd.Parameters.AddWithValue("@FromFriendID", LoginID);
                cmd.Parameters.AddWithValue("@Mode", "USERFRIENDLIST");
                dtResult = objSQLAccess.GetDt(cmd, ref exMessage);

                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "True";
                }
                else
                {
                    if (Convert.ToString(exMessage).Length > 0)
                    {
                        dsDataSet.Tables["Error"].Rows[0]["Message"] = exMessage.ToString();
                    }
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
                }
            }
            catch (Exception ex)
            {
                dsDataSet.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();
                dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
            }
            dtResult.TableName = "USERFRIENDLIST";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }
    }
}
