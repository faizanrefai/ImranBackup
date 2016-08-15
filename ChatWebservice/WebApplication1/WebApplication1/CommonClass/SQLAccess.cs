using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1.CommonClass
{
    /// <summary>
    /// SQLAccess Contains commonly used Data Logic Functions     
    /// <author>

    /// </author>
    /// Version 1.0
    /// </summary>
    public class SQLAccess
    {
        #region Constructor
        public SQLAccess()
        {
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["getconn"].ConnectionString);

        }
        #endregion

        #region Declaration

        private SqlConnection conn = null;
        private SqlConnection connCreditcard = null;
        public SqlCommand cmd;

        public DataSet ds;
        public DataTable dt;

        #endregion

        #region Key Functions

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="Query">Query</param>
        /// <returns>true if executed successfully else false</returns>
        public bool ExecuteNonQuery(string Query)
        {

            bool result;
            cmd = new SqlCommand(Query, conn);

            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                int index = cmd.ExecuteNonQuery();
                if (index != 1)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }

            catch (Exception)
            {
                result = false;
                HttpContext context = HttpContext.Current;
                //CommonDAC.ErrorLog("SQLAccess.cs", ex.Message, ex.StackTrace);
            }
            finally
            {
                if (conn != null)
                    if (conn.State == ConnectionState.Open) conn.Close();
                cmd.Dispose();
            }
            return result;
        }

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="cmd">cmd</param>
        /// <returns>true if executed successfully else false</returns>
        public int ExecuteNonQuery(SqlCommand cmd,ref string exMessage)
        {
            int Obj;
            try
            {
                cmd.Connection = conn;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                Obj = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Obj = 0;
                exMessage = ex.Message.ToString();
                HttpContext context = HttpContext.Current;
                //CommonDAC.ErrorLog("SQLAccess.cs", ex.Message, ex.StackTrace);
            }
            finally
            {
                if (conn != null)
                    if (conn.State == ConnectionState.Open) conn.Close();
                cmd.Dispose();
            }
            return Obj;
        }

        /// <summary>
        /// ExecuteScalarQuery
        /// </summary>
        /// <param name="Query">Query</param>
        /// <returns>Object</returns>
        public Object ExecuteScalarQuery(string Query)
        {
            Object Obj = new Object();
            try
            {
                cmd = new SqlCommand();
                cmd.CommandText = Query;
                Obj = ExecuteScalarQuery(cmd);
            }
            catch (Exception)
            {
                Obj = null;
                HttpContext context = HttpContext.Current;
                // CommonDAC.ErrorLog("SQLAccess.cs", ex.Message, ex.StackTrace);
            }
            finally
            {
                cmd.Dispose();
            }
            return Obj;
        }

        /// <summary>
        /// ExecuteScalarQuery
        /// </summary>
        /// <param name="cmd">cmd</param>
        /// <returns>Object</returns>
        public Object ExecuteScalarQuery(SqlCommand cmd)
        {
            Object Obj = new Object();
            try
            {
                cmd.Connection = conn;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                Obj = cmd.ExecuteScalar();
            }
            catch (Exception)
            {
                Obj = null;
                HttpContext context = HttpContext.Current;
                // CommonDAC.ErrorLog("SQLAccess.cs", ex.Message, ex.StackTrace);
            }
            finally
            {
                if (conn != null)
                    if (conn.State == ConnectionState.Open) conn.Close();
                cmd.Dispose();
            }
            return Obj;
        }

        /// <summary>
        /// Get DataSet
        /// </summary>
        /// <param name="Sql">Sql</param>
        /// <returns>DataSet</returns>
        public DataSet GetDs(string Sql)
        {
            SqlDataAdapter Adpt = new SqlDataAdapter();
            try
            {
                ds = new DataSet();
                cmd = new SqlCommand();
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = Sql;
                Adpt.SelectCommand = cmd;
                Adpt.Fill(ds);
            }
            catch (Exception)
            {
                ds = null;
                HttpContext context = HttpContext.Current;
                //CommonDAC.ErrorLog("SQLAccess.cs", ex.Message, ex.StackTrace);
            }
            finally
            {
                if (conn != null)
                    if (conn.State == ConnectionState.Open) conn.Close();
                cmd.Dispose();
                Adpt.Dispose();
            }
            return ds;
        }
        public DataTable GetDt(SqlCommand cmd,ref string exMessage)
        {
            SqlDataAdapter Adpt = new SqlDataAdapter();
            try
            {
                dt = new DataTable();
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                cmd.Connection = conn;

                Adpt.SelectCommand = cmd;
                Adpt.Fill(dt);
            }
            catch (Exception ex)
            {
                dt = new DataTable();
                exMessage = ex.Message.ToString();
                HttpContext context = HttpContext.Current;
                //CommonDAC.ErrorLog("SQLAccess.cs", ex.Message, ex.StackTrace);
            }
            finally
            {
                if (conn != null)
                    if (conn.State == ConnectionState.Open) conn.Close();
                cmd.Dispose();
                Adpt.Dispose();
            }
            return dt;
        }
        /// <summary>
        /// Get Dataset
        /// </summary>
        /// <param name="cmd">cmd</param>
        /// <returns>DataSet</returns>
        public DataSet GetDs(SqlCommand cmd, ref string exMessage)
        {
            SqlDataAdapter Adpt = new SqlDataAdapter();
            try
            {
                ds = new DataSet();
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                cmd.Connection = conn;

                Adpt.SelectCommand = cmd;
                Adpt.Fill(ds);
            }
            catch (Exception)
            {
                ds = new DataSet();
                exMessage = exMessage.ToString();
                HttpContext context = HttpContext.Current;
                //CommonDAC.ErrorLog("SQLAccess.cs", ex.Message, ex.StackTrace);
            }
            finally
            {
                if (conn != null)
                    if (conn.State == ConnectionState.Open) conn.Close();
                cmd.Dispose();
                Adpt.Dispose();
            }
            return ds;
        }
        public DataSet GetCreditcarddetails(SqlCommand cmd)
        {

            SqlDataAdapter Adpt = new SqlDataAdapter();
            try
            {
                ds = new DataSet();
                if (connCreditcard.State == ConnectionState.Closed)
                    connCreditcard.Open();

                cmd.Connection = connCreditcard;

                Adpt.SelectCommand = cmd;
                Adpt.Fill(ds);
            }
            catch (Exception)
            {
                ds = null;
                HttpContext context = HttpContext.Current;
                //CommonDAC.ErrorLog("SQLAccess.cs", ex.Message, ex.StackTrace);
            }
            finally
            {
                if (connCreditcard != null)
                    if (connCreditcard.State == ConnectionState.Open) connCreditcard.Close();
                cmd.Dispose();
                Adpt.Dispose();
            }
            return ds;
        }

        public void HandleException(Exception e)
        {
            HttpContext context = HttpContext.Current;
            if (e != null)
            {
                ErrorLog(context.Request.Url.ToString(), e.Message, e.StackTrace);
                if (HttpContext.Current.Request.Url.LocalPath.ToUpper().Contains("PROVIDER"))
                {
                    context.RewritePath("~/provider/error.aspx");
                }
                else if (HttpContext.Current.Request.Url.LocalPath.ToUpper().Contains("ADMINISTRATION"))
                {
                    context.RewritePath("~/Administration/ErrorAdmin.aspx");
                }
               


            }
        }

        public  void ErrorLog(string PageName, string Error, string ErrorDetails)
        {
             SqlCommand cmd = null;
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Trace_Error";
            cmd.Parameters.AddWithValue("@PageName", PageName);
            cmd.Parameters.AddWithValue("@Error", Error);
            cmd.Parameters.AddWithValue("@ErrorDetails", ErrorDetails);
            //ExecuteNonQuery(cmd);
        }
        #endregion

    }
}
