using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SignalrChatHub
{
    public class BaseClass
    {
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
    }
}
