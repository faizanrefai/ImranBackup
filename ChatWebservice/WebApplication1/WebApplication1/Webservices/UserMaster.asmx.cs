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
using System.IO;
using System.Configuration;

namespace WebApplication1.Webservices
{
    /// <summary>
    /// Summary description for UserMaster
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class UserMaster : System.Web.Services.WebService
    {

        private SqlCommand cmd = null;
        private SQLAccess objSQLAccess = null;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string USERINSERT(string UserName, string Password, string FName, string LName, string MobileNo,
                                 string EmailID, string Address, byte[] UserImage)
        {
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            DataTable dtResult = new DataTable();
            cmd = new SqlCommand();
            string strFileName = string.Empty;
            string exMessage = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                if (UserImage != null && UserImage.Length > 0)
                {
                    strFileName = (DateTime.Now.ToString("yyyyMMddHHmmssfff") + DateTime.Now.Millisecond.ToString() + DateTime.Now.Second.ToString());
                    string patimagepath = ConfigurationManager.AppSettings["patimagepath"];
                    MemoryStream ms = new MemoryStream(UserImage, 0, UserImage.Length);
                    ms.Write(UserImage, 0, UserImage.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    image.Save(Server.MapPath("~" + patimagepath + strFileName + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserMasterInserUpdateGet";
                cmd.Parameters.AddWithValue("@UserName", UserName);
                cmd.Parameters.AddWithValue("@Password", Password);
                cmd.Parameters.AddWithValue("@FName", FName);
                cmd.Parameters.AddWithValue("@LName", LName);
                cmd.Parameters.AddWithValue("@MobileNo", MobileNo);
                cmd.Parameters.AddWithValue("@EmailID", EmailID);
                cmd.Parameters.AddWithValue("@Address", Address);
                cmd.Parameters.AddWithValue("@UserImage", strFileName);
                cmd.Parameters.AddWithValue("@Mode", "USERINSERT");
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
                    else
                    {
                        dsDataSet.Tables["Header"].Rows[0]["Message"] = Properties.Resources.EmailIDValid;
                    }
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
                }
            }
            catch (Exception ex)
            {
                dsDataSet.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();
                dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
            }
            dtResult.TableName = "USERDETAIL";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string USERUPDATE(string UserID, string UserName, string Password, string FName, string LName, string MobileNo,
                                 string EmailID, string Address, byte[] UserImage)
        {
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            cmd = new SqlCommand();
            DataTable dtResult = new DataTable();
            string strFileName = string.Empty;
            string exMessage = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                if (UserImage != null && UserImage.Length > 0)
                {
                    strFileName = (DateTime.Now.ToString("yyyyMMddHHmmssfff") + DateTime.Now.Millisecond.ToString() + DateTime.Now.Second.ToString());
                    string patimagepath = ConfigurationManager.AppSettings["patimagepath"];
                    MemoryStream ms = new MemoryStream(UserImage, 0, UserImage.Length);
                    ms.Write(UserImage, 0, UserImage.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    image.Save(Server.MapPath("~" + patimagepath + strFileName + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserMasterInserUpdateGet";
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@UserName", UserName);
                cmd.Parameters.AddWithValue("@Password", Password);
                cmd.Parameters.AddWithValue("@FName", FName);
                cmd.Parameters.AddWithValue("@LName", LName);
                cmd.Parameters.AddWithValue("@MobileNo", MobileNo);
                cmd.Parameters.AddWithValue("@EmailID", EmailID);
                cmd.Parameters.AddWithValue("@Address", Address);
                cmd.Parameters.AddWithValue("@UserImage", strFileName);
                cmd.Parameters.AddWithValue("@Mode", "USERUPDATE");
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
                    else
                    {
                        dsDataSet.Tables["Header"].Rows[0]["Message"] = Properties.Resources.EmailIDValid;
                    }
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
                }
            }
            catch (Exception ex)
            {
                dsDataSet.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();
                dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
            }
            dtResult.TableName = "USERDETAIL";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string LOGINUSER(string UserName, string Password)
        {
            DataSet dsMainDataset = new DataSet();
            DataSet dsDataMerge = new DataSet();

            DataTable dtResult = new DataTable();
            DataTable dtResponse = new DataTable();

            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            Boolean blnflag = false;
            try
            {
                dsMainDataset = objBase.GetLoginDatatable();
                dtResponse = objBase.GenerateData("Status", "", "ResponseStatus");
                if (Convert.ToString(UserName) == string.Empty)
                {
                    dsMainDataset.Tables["Header"].Rows[0]["Message"] = Properties.Resources.EnterUserName;
                    blnflag = true;
                }
                if (Convert.ToString(Password) == string.Empty && blnflag == false)
                {
                    dsMainDataset.Tables["Header"].Rows[0]["Message"] = Properties.Resources.EnterPassword;
                }
                if (Convert.ToString(UserName) != string.Empty && Convert.ToString(Password) != string.Empty)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "UserMasterInserUpdateGet";
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    cmd.Parameters.AddWithValue("@Mode", "LOGINUSER");
                    dtResult = objSQLAccess.GetDt(cmd, ref exMessage);
                    if (dtResult != null && dtResult.Rows.Count > 0)
                    {
                        dsMainDataset.Tables["Header"].Rows[0]["Message"] = Properties.Resources.LoginSucess;
                        dtResponse.Rows[0]["Status"] = "True";
                    }
                    else
                    {
                        if (Convert.ToString(exMessage).Length > 0)
                        {
                            dsMainDataset.Tables["Error"].Rows[0]["Message"] = exMessage.ToString();
                        }
                        dsMainDataset.Tables["Header"].Rows[0]["Message"] = Properties.Resources.LoginError;
                        dtResponse.Rows[0]["Status"] = "False";
                    }
                }
                else
                {
                    dtResponse.Rows[0]["Status"] = "False";
                    dsDataMerge.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                dtResponse.Rows[0]["Status"] = "False";
                dsMainDataset.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();

            }
            dsDataMerge.Tables.Add(dtResponse);
            dtResult.TableName = "LOGINUSER";
            dsMainDataset.Tables.Add(dtResult);
            dsMainDataset.AcceptChanges();
            dsMainDataset.Merge(dsDataMerge);
            return JsonConvert.SerializeObject(dsMainDataset, Formatting.None);

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SEARCHFRIEND(string SearchText,string UserID)
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
                cmd.CommandText = "UserMasterInserUpdateGet";
                cmd.Parameters.AddWithValue("@SearchText", SearchText);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Mode", "SEARCHFRIEND");
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
            dtResult.TableName = "USERDETAIL";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);


        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string PROFILEDETAIL(string UserID)
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
                cmd.CommandText = "UserMasterInserUpdateGet";
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Mode", "PROFILEDETAIL");
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
            dtResult.TableName = "PROFILEDETAIL";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UPDATEUSERIMAGE(byte[] UserImage, string UserID)
        {
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            DataTable dtResult = new DataTable();
            cmd = new SqlCommand();
            string strFileName = string.Empty;
            string exMessage = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                if (UserImage != null && UserImage.Length > 0)
                {
                    strFileName = (DateTime.Now.ToString("yyyyMMddHHmmssfff") + DateTime.Now.Millisecond.ToString() + DateTime.Now.Second.ToString());
                    string patimagepath = ConfigurationManager.AppSettings["patimagepath"];
                    MemoryStream ms = new MemoryStream(UserImage, 0, UserImage.Length);
                    ms.Write(UserImage, 0, UserImage.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    image.Save(Server.MapPath("~" + patimagepath + strFileName + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UserMasterInserUpdateGet";
                cmd.Parameters.AddWithValue("@UserImage", strFileName);
                cmd.Parameters.AddWithValue("@Mode", "UPDATEUSERIMAGE");
                cmd.Parameters.AddWithValue("@UserID", UserID);
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
            dtResult.TableName = "PROFILEDETAIL";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);
        }
        //private string JsonSerializerClass(object objMainDataset)
        //{
        //    return JsonConvert.SerializeObject(objMainDataset, Formatting.Indented,
        //                     new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //}

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ChangePassword(string UserName, string Password, string NewPassword)
        {
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            objSQLAccess = new SQLAccess();
            DataTable dtResult = new DataTable();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            Boolean blnflag = false;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                if (Convert.ToString(UserName) == string.Empty)
                {
                    dsDataSet.Tables["Header"].Rows[0]["Message"] = Properties.Resources.EnterUserName;
                    blnflag = true;
                }
                if (Convert.ToString(Password) == string.Empty && blnflag == false)
                {
                    dsDataSet.Tables["Header"].Rows[0]["Message"] = Properties.Resources.EnterPassword;
                    blnflag = true;
                }
                if (Convert.ToString(NewPassword) == string.Empty && blnflag == false)
                {
                    dsDataSet.Tables["Header"].Rows[0]["Message"] = Properties.Resources.EnterPassword;
                }
                if (Convert.ToString(UserName) != string.Empty && Convert.ToString(Password) != string.Empty && Convert.ToString(NewPassword) != string.Empty)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "UserMasterInserUpdateGet";
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    cmd.Parameters.AddWithValue("@NewPassword", NewPassword);
                    cmd.Parameters.AddWithValue("@Mode", "CHANGEPASSWORD");
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
                        else
                        {
                            dsDataSet.Tables["Header"].Rows[0]["Message"] = Properties.Resources.ValidPassword;
                        }
                        dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
                    }
                }
                else
                {
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
                }
            }
            catch (Exception ex)
            {
                dsDataSet.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();
                dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
            }
            dtResult.TableName = "ChangePassword";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ChatImage(byte[] UserImage)
        {
            DataSet dsDataSet = new DataSet();
            BaseClass objBase = new BaseClass();
            DataTable dtResult = new DataTable();
            string strFileName = string.Empty;
            try
            {
                dsDataSet = objBase.GetHeaderErrorRespDatatable();
                if (UserImage != null && UserImage.Length > 0)
                {
                    strFileName = (DateTime.Now.ToString("yyyyMMddHHmmssfff") + DateTime.Now.Millisecond.ToString() + DateTime.Now.Second.ToString());
                    string patimagepath = ConfigurationManager.AppSettings["patimagepath"];
                    MemoryStream ms = new MemoryStream(UserImage, 0, UserImage.Length);
                    ms.Write(UserImage, 0, UserImage.Length);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                    image.Save(Server.MapPath("~" + patimagepath + strFileName + ".png"), System.Drawing.Imaging.ImageFormat.Png);

                    dtResult.Columns.Add("ImagePath");
                    DataRow drHeader = dtResult.NewRow();
                    drHeader["ImagePath"] = ConfigurationManager.AppSettings["serverPath"] + strFileName + ".png";
                    dtResult.Rows.Add(drHeader);
                    dtResult.AcceptChanges();
                }
                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                   
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "True";
                }
                else
                {
                    dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
                }
            }
            catch (Exception ex)
            {
                dsDataSet.Tables["Error"].Rows[0]["Message"] = ex.Message.ToString();
                dsDataSet.Tables["ResponseStatus"].Rows[0]["Status"] = "False";
            }
            dtResult.TableName = "ChatImages";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);
        }

    }
}
