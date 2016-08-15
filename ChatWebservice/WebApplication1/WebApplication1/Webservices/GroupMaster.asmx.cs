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
using System.Configuration;
using System.IO;

namespace WebApplication1.Webservices
{
    /// <summary>
    /// Summary description for GroupMaster
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class GroupMaster : System.Web.Services.WebService
    {

        private SqlCommand cmd = null;
        SQLAccess objSQLAccess;

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CREATEGROUP(string GroupName, string UserID, byte[] GroupImage)
        {
            DataTable dtResult = new DataTable();
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            string strFileName = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                if (GroupImage != null && GroupImage.Length > 0)
                {
                    strFileName = (DateTime.Now.ToString("yyyyMMddHHmmssfff") + DateTime.Now.Millisecond.ToString() + DateTime.Now.Second.ToString());
                    string patimagepath = ConfigurationManager.AppSettings["patimagepath"];
                    MemoryStream ms = new MemoryStream(GroupImage, 0, GroupImage.Length);
                    ms.Write(GroupImage, 0, GroupImage.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    image.Save(Server.MapPath("~" + patimagepath + strFileName + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GROUPINSERTUPDATE";
                cmd.Parameters.AddWithValue("@GroupName", GroupName);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@GroupImage", strFileName);
                cmd.Parameters.AddWithValue("@Mode", "CREATEGROUP");
                dtResult = objSQLAccess.GetDt(cmd,ref exMessage);
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
            dtResult.TableName = "CREATEGROUP";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string REMOVEMEMBERFROMGROUP(string UserID, string GroupID)
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
                cmd.CommandText = "GROUPINSERTUPDATE";
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@GroupID", GroupID);
                cmd.Parameters.AddWithValue("@Mode", "REMOVEMEMBERFROMGROUP");
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
            dtResult.TableName = "REMOVEMEMBERFROMGROUP";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ADDMEMBERTOGROUP(string UserID, string GroupID,string UserName)
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
                cmd.CommandText = "GROUPINSERTUPDATE";
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@GroupID", GroupID);
                cmd.Parameters.AddWithValue("@Mode", "ADDMEMBERTOGROUP");
                int i = objSQLAccess.ExecuteNonQuery(cmd,ref exMessage);
                if (i >= 1)
                {
                    objBase.sendNotication(UserID, "You added in Groups by " + UserName);
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
            dtResult.TableName = "ADDMEMBERTOGROUP";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GROUPDETAILWITHLISTOFGROUPMEMBER(string GroupID)
        {
            DataSet dtResultDS = new DataSet();
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GROUPINSERTUPDATE";
                cmd.Parameters.AddWithValue("@GroupID", GroupID);
                cmd.Parameters.AddWithValue("@Mode", "GROUPDETAILWITHLISTOFGROUPMEMBER");
                dtResultDS = objSQLAccess.GetDs(cmd, ref exMessage);
                if (dtResultDS != null && dtResultDS.Tables.Count > 0)
                {
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "True";
                    if (dtResultDS != null && dtResultDS.Tables[0] != null && dtResultDS.Tables[0].Rows.Count > 0)
                    {
                        dtResultDS.Tables[0].TableName = "USERDETAIL";
                    }
                    if (dtResultDS != null && dtResultDS.Tables.Count >= 2 && dtResultDS.Tables[1] != null && dtResultDS.Tables[1].Rows.Count > 0)
                    {
                        dtResultDS.Tables[1].TableName = "GROUPDETAIL";
                    }
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
            //dtResult.TableName = "USERDETAIL";
            //dsDataSet.Tables.Add(dtResult);
            //dsDataSet.AcceptChanges();
            dsDataSet.Merge(dtResultDS);
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);


        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SEARCHFRIENDFORGROUP(string GroupID, string UserID)
        {
            DataSet dtResultDS = new DataSet();
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GROUPINSERTUPDATE";
                cmd.Parameters.AddWithValue("@GroupID", GroupID);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Mode", "SEARCHFRIENDFORGROUP");
                dtResultDS = objSQLAccess.GetDs(cmd, ref exMessage);
                if (dtResultDS != null && dtResultDS.Tables.Count > 0)
                {
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "True";
                    if (dtResultDS != null && dtResultDS.Tables[0] != null && dtResultDS.Tables[0].Rows.Count > 0)
                    {
                        dtResultDS.Tables[0].TableName = "USERDETAIL";
                    }
                    if (dtResultDS != null && dtResultDS.Tables.Count >=2 && dtResultDS.Tables[1] != null && dtResultDS.Tables[1].Rows.Count > 0)
                    {
                        dtResultDS.Tables[1].TableName = "GROUPDETAIL";
                    }
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
            //dtResult.TableName = "USERDETAIL";
            //dsDataSet.Tables.Add(dtResult);
            dsDataSet.Merge(dtResultDS);
            //dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);


        }
    }
}
