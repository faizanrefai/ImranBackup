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
using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp.Google;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace WebApplication1.CommonClass
{
    public class BaseClass
    {
        private SqlCommand cmd = null;
        private SQLAccess objSQLAccess = null;
        private byte[] appleCert;

        public class SecondList
        {
            public string Name { get; set; }
            public string ProfilePics { get; set; }
        }

        public DataTable GenerateData(string ColName, string DefultMesg, string tableName)
        {
            DataTable dtResponse = new DataTable();
            dtResponse.Columns.Add(ColName);
            DataRow drResponse = dtResponse.NewRow();
            drResponse[ColName] = DefultMesg;
            dtResponse.Rows.Add(drResponse);
            dtResponse.TableName = tableName;
            dtResponse.AcceptChanges();
            return dtResponse;
        }
        public DataSet GetHeaderErrorRespDatatable()
        {
            DataSet dsDataSet = new DataSet();
            DataTable dtHeader = new DataTable();
            DataTable dtError = new DataTable();
            DataTable dtResponse = new DataTable();

            dtHeader.Columns.Add("Message");
            DataRow drHeader = dtHeader.NewRow();
            drHeader["Message"] = "";
            dtHeader.Rows.Add(drHeader);
            dtHeader.TableName = "Header";
            dtHeader.AcceptChanges();

            dtError.Columns.Add("Message");
            DataRow drError = dtError.NewRow();
            drError["Message"] = "";
            dtError.Rows.Add(drError);
            dtError.TableName = "Error";
            dtError.AcceptChanges();

            dtResponse.Columns.Add("Status");
            DataRow drResponse = dtResponse.NewRow();
            drResponse["Status"] = "";
            dtResponse.Rows.Add(drResponse);
            dtResponse.TableName = "ResponseStatus";
            dtResponse.AcceptChanges();

            dsDataSet.Tables.Add(dtHeader);
            dsDataSet.Tables.Add(dtError);
            dsDataSet.Tables.Add(dtResponse);
            return dsDataSet;
        }

        public DataSet GetLoginDatatable()
        {
            DataSet dsDataSet = new DataSet();
            DataTable dtHeader = new DataTable();
            DataTable dtError = new DataTable();

            dtHeader.Columns.Add("Message");
            DataRow drHeader = dtHeader.NewRow();
            drHeader["Message"] = "";
            dtHeader.Rows.Add(drHeader);
            dtHeader.TableName = "Header";
            dtHeader.AcceptChanges();

            dtError.Columns.Add("Message");
            DataRow drError = dtError.NewRow();
            drError["Message"] = "";
            dtError.Rows.Add(drError);
            dtError.TableName = "Error";
            dtError.AcceptChanges();


            dsDataSet.Tables.Add(dtHeader);
            dsDataSet.Tables.Add(dtError);
            return dsDataSet;
        }

        public void sendNotication(string UserID, string Message)
        {

            var succeeded = 0;
            var failed = 0;
            string CertificationType = ConfigurationManager.AppSettings["CertificateType"];
            string patimagepath = "/CertificatePath/";
            string strFileName = string.Empty;
            if (CertificationType == "P")
            {
                strFileName = "ECO_Production.p12";
            }
            else
            {
                strFileName = "ECO_Sendbox.p12";
            }

            DataTable dtDevice = new DataTable();
            objSQLAccess = new SQLAccess();
            cmd = new SqlCommand();
            string exMessage = string.Empty;
            string ApnsCertificatePassword = "eco";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "DeviceDetailInsertUpdate";
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@Mode", "GET");
            dtDevice = objSQLAccess.GetDt(cmd, ref exMessage);
            if (exMessage == string.Empty && dtDevice != null && dtDevice.Rows.Count > 0)
            {
                for (int i = 0; i < dtDevice.Rows.Count; i++)
                {
                    string DeviceName = Convert.ToString(dtDevice.Rows[i]["DeviceName"]).Trim().ToUpper();
                    switch (DeviceName)
                    {
                        case "IOS":
                            if (CertificationType == "S")
                            {
                                appleCert = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~" + patimagepath + strFileName));
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

                                broker.QueueNotification(new ApnsNotification
                                {
                                    DeviceToken = Convert.ToString(dtDevice.Rows[i]["DeviceTokenID"]),
                                    Payload = JObject.Parse("{ \"aps\" : { \"alert\" : \"" + Message + "\",\"sound\":\"default\",\"badge\":1 } }")
                                });
                                broker.Stop();
                            }
                            else if (CertificationType == "P")
                            {
                                appleCert = File.ReadAllBytes(HttpContext.Current.Server.MapPath("~" + patimagepath + strFileName));
                                var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Production, appleCert, ApnsCertificatePassword);
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

                                broker.QueueNotification(new ApnsNotification
                                {
                                    DeviceToken = Convert.ToString(dtDevice.Rows[i]["DeviceTokenID"]),
                                    Payload = JObject.Parse("{ \"aps\" : { \"alert\" : \"" + Message + "\",\"sound\":\"default\",\"badge\":1 } }")
                                });
                                broker.Stop();
                            }

                            break;
                        case "ANDROID":

                            var configAndroid = new GcmConfiguration("116829411276012377672", "b7e565beae4901a4678071027909a7fb745746ff", null);
                            var brokerAndroid = new GcmServiceBroker(configAndroid);
                            brokerAndroid.OnNotificationFailed += (notification, exception) =>
                            {
                                failed++;
                            };
                            brokerAndroid.OnNotificationSucceeded += (notification) =>
                            {
                                succeeded++;
                            };

                            brokerAndroid.Start();

                            brokerAndroid.QueueNotification(new GcmNotification
                            {
                                RegistrationIds = new List<string> { 
                                    "116829411276012377672"
                                },
                                Data = JObject.Parse("{\"alert\":\"Hello World!\",\"badge\":7,\"sound\":\"sound.caf\"}")
                            });
                            brokerAndroid.Start();
                            break;
                    }
                }
            }
        }
    }
}