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
using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp.Google;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace WebApplication1.Webservices
{
    /// <summary>
    /// Summary description for MessageList
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MessageList : System.Web.Services.WebService
    {
        private SqlCommand cmd = null;
        SQLAccess objSQLAccess;
        private byte[] appleCert;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void sendNotication(string UserID, string Message)
        {
            BaseClass objBase = new BaseClass();
            objBase.sendNotication(UserID, Message);
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void APNS_Send_Single()
        {
            var succeeded = 0;
            var failed = 0;
            var attempted = 0;
            string strFileName = "PushSharp.Apns.Sandbox.p12";
            string patimagepath = "/CertificatePath/";
            string ApnsCertificatePassword = string.Empty;
            appleCert = File.ReadAllBytes(Server.MapPath("~" + patimagepath + strFileName));
            var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Sandbox, appleCert, ApnsCertificatePassword);
            var broker = new ApnsServiceBroker(config);
            broker.OnNotificationFailed += (notification, exception) =>
            {
                failed++;
            };
            broker.OnNotificationSucceeded += (notification) =>
            {
                succeeded++;
            };
            broker.Start();
            string strTesting = "erwrwerwe";

            broker.QueueNotification(new ApnsNotification
            {
                DeviceToken = "",
                Payload = JObject.Parse("{ \"aps\" : { \"alert\" : \"" + strTesting + "\" } }")
            });

            //foreach (var dt in Settings.Instance.ApnsDeviceTokens)
            //{
            //    attempted++;
            //    broker.QueueNotification(new ApnsNotification
            //    {
            //        DeviceToken = dt,
            //        Payload = JObject.Parse("{ \"aps\" : { \"alert\" : \"" + strTesting + "\" } }")
            //    });
            //}

            broker.Stop();

            Assert.AreEqual(attempted, succeeded);
            Assert.AreEqual(0, failed);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void Gcm_Send_Single()
        {
            var succeeded = 0;
            var failed = 0;
            var attempted = 0;

            //var config = new GcmConfiguration(Settings.Instance.GcmSenderId, Settings.Instance.GcmAuthToken, null);
            var config = new GcmConfiguration("116829411276012377672", "b7e565beae4901a4678071027909a7fb745746ff", null);
            var broker = new GcmServiceBroker(config);
            broker.OnNotificationFailed += (notification, exception) =>
            {
                failed++;
            };
            broker.OnNotificationSucceeded += (notification) =>
            {
                succeeded++;
            };

            broker.Start();
            attempted++;

            broker.QueueNotification(new GcmNotification
            {
                RegistrationIds = new List<string> { 
                        "116829411276012377672"
                    },
                Data = JObject.Parse("{\"alert\":\"Hello World!\",\"badge\":7,\"sound\":\"sound.caf\"}")
            });
            //foreach (var regId in Settings.Instance.GcmRegistrationIds)
            //{
            //    attempted++;

            //    broker.QueueNotification(new GcmNotification
            //    {
            //        RegistrationIds = new List<string> { 
            //            regId
            //        },
            //        Data = JObject.Parse("{ \"somekey\" : \"somevalue\" }")
            //    });
            //}

            broker.Stop();

            Assert.AreEqual(attempted, succeeded);
            Assert.AreEqual(0, failed);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string InsertDeviceDetail(string UserID, string DeviceName, string DeviceTokenID)
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
                cmd.CommandText = "DeviceDetailInsertUpdate";
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@DeviceName", DeviceName);
                cmd.Parameters.AddWithValue("@DeviceTokenID", DeviceTokenID);
                cmd.Parameters.AddWithValue("@Mode", "INSERT");
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
        public string GetUnreadMessageCount(string UserID)
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
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetMessageCount";
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@IsCount", "1");
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
            dtResult.TableName = "MessageCountDetail";
            dsDataSet.Tables.Add(dtResult);
            dsDataSet.AcceptChanges();
            return JsonConvert.SerializeObject(dsDataSet, Formatting.None);
        }

    }
}
