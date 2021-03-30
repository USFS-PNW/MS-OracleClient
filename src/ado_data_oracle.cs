using System;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Data.OracleClient;
using System.Linq;
using System.Collections.Generic;

namespace Oracle.ADO
{
    /// <summary>
    /// Summary description for Oracle.
    /// </summary>
    public class DataMgr
    {
        public System.Data.DataSet m_DataSet;
        public System.Data.OracleClient.OracleDataAdapter m_DataAdapter;
        public System.Data.OracleClient.OracleDataAdapter[] m_DataAdapterArray;
        public string[] m_DataAdapterArrayTableName;
        public System.Data.OracleClient.OracleCommand m_Command;
        public System.Data.OracleClient.OracleConnection m_Connection;
        public System.Data.OracleClient.OracleDataReader m_DataReader;
        public System.Data.DataTable m_DataTable;
        public System.Data.OracleClient.OracleTransaction m_Transaction;
        public string m_strDataSource = "";
        public string m_strConnection = "";
        public string m_strUserId = "";
        public string m_strError;
        public int m_intError;
        public string m_strResult = "";
        public string m_strSQL;
        public string m_strTable;
        private bool _bDisplayErrors = true;
        private string _strMsgBoxTitle = "BIOSUM";

        private int _intDataAdapterTableCount = 0;
        public int DataAdapterTableCount
        {
            get { return _intDataAdapterTableCount; }
            set { _intDataAdapterTableCount = value; }
        }
        private bool _bConnectionDisposed = true;
        public bool ConnectionDisposed
        {
            get { return _bConnectionDisposed; }
            set { _bConnectionDisposed = value; }
        }

        public DataMgr()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        ~DataMgr()
        {

        }

        public void OpenConnection(string strConn)
        {
            m_intError = 0;
            m_strError = "";
            System.Data.OracleClient.OracleConnection p_Connection = new System.Data.OracleClient.OracleConnection();

            try
            {


                p_Connection.ConnectionString = strConn;
                p_Connection.Open();

            }
            catch (Exception caught)
            {
                this.m_strError = caught.Message;
                this.m_intError = -1;
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:OpenConnection  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            finally
            {
                this.m_Connection = p_Connection;
                _bConnectionDisposed = false;
                p_Connection.Disposed += new EventHandler(this.DisposedEvent);
            }
        }
        public void OpenConnection(string strConn, ref System.Data.OracleClient.OracleConnection p_Connection)
        {
            this.m_intError = 0;
            this.m_strError = "";
            try
            {
                p_Connection.ConnectionString = strConn;
                _bConnectionDisposed = false;
                p_Connection.Open();

            }
            catch (Exception caught)
            {
                this.m_strError = caught.Message;
                this.m_intError = -1;
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:OpenConnection  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
            finally
            {
                if (m_intError == 0)
                {
                    _bConnectionDisposed = false;
                    p_Connection.Disposed += new EventHandler(this.DisposedEvent);
                }
            }
        }

        public void SqlNonQuery(string strConn, string strSQL)
        {
            m_intError = 0;
            m_strError = "";
            System.Data.OracleClient.OracleConnection p_Connection = new System.Data.OracleClient.OracleConnection();
            this.OpenConnection(strConn, ref p_Connection);
            if (this.m_intError == 0)
            {
                System.Data.OracleClient.OracleCommand p_Command = new System.Data.OracleClient.OracleCommand();
                p_Command.Connection = p_Connection;
                p_Command.CommandText = strSQL;
                try
                {
                    p_Command.ExecuteNonQuery();
                }
                catch (Exception caught)
                {
                    this.m_strError = caught.Message + " The SQL command " + strSQL + " Failed"; ;
                    this.m_intError = -1;
                    if (_bDisplayErrors)
                        MessageBox.Show("!!Error!! \n" +
                            "Module - Oracle:SqlNonQuery  \n" +
                            "Err Msg - " + this.m_strError,
                            "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Exclamation);
                }
                p_Connection.Close();
                p_Connection = null;
                p_Command = null;

            }
        }
        public void SqlNonQuery(System.Data.OracleClient.OracleConnection p_Connection, string strSQL)
        {
            m_intError = 0;
            m_strError = "";
            System.Data.OracleClient.OracleCommand p_Command = new System.Data.OracleClient.OracleCommand();
            p_Command.Connection = p_Connection;
            p_Command.CommandText = strSQL;

            try
            {
                p_Command.ExecuteNonQuery();
            }
            catch (Exception caught)
            {
                this.m_strError = caught.Message + " The SQL command " + strSQL + " Failed"; ;
                this.m_intError = -1;
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:SqlNonQuery  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
            p_Command = null;


        }



        public void SqlQueryReader(string strConn, string strSql)
        {
            this.m_intError = 0;
            this.m_strError = "";
            this.OpenConnection(strConn);
            if (this.m_intError == 0)
            {
                this.m_Command = this.m_Connection.CreateCommand();
                this.m_Command.CommandText = strSql;
                try
                {
                    this.m_DataReader = this.m_Command.ExecuteReader();
                }
                catch (Exception caught)
                {
                    this.m_intError = -1;
                    this.m_strError = caught.Message + " The Query Command " + this.m_Command.CommandText.ToString() + " Failed";
                    if (_bDisplayErrors)
                        MessageBox.Show("!!Error!! \n" +
                            "Module - Oracle:SqlQueryReader  \n" +
                            "Err Msg - " + this.m_strError,
                            "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Exclamation);
                    this.m_DataReader = null;
                    this.m_Command = null;
                    this.m_Connection.Close();
                    this.m_Connection = null;
                    return;
                }
            }
        }
        public void SqlQueryReader(System.Data.OracleClient.OracleConnection p_Connection, string strSql)
        {
            this.m_intError = 0;
            this.m_strError = "";

            this.m_Command = p_Connection.CreateCommand();
            this.m_Command.CommandText = strSql;
            try
            {
                this.m_DataReader = this.m_Command.ExecuteReader();
            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + " The Query Command " + this.m_Command.CommandText.ToString() + " Failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:SqlQueryReader  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.m_DataReader = null;
                this.m_Command = null;
                return;
            }


        }
        public void SqlQueryReader(System.Data.OracleClient.OracleConnection p_Connection, System.Data.OracleClient.OracleTransaction p_trans, string strSql)
        {
            this.m_intError = 0;
            this.m_strError = "";

            this.m_Command = p_Connection.CreateCommand();
            this.m_Command.CommandText = strSql;
            this.m_Command.Transaction = p_trans;

            try
            {
                this.m_DataReader = this.m_Command.ExecuteReader();
            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + " The Query Command " + this.m_Command.CommandText.ToString() + " Failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:SqlQueryReader  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.m_DataReader = null;
                this.m_Command = null;
                return;
            }
        }

        public void SqlQueryReader(System.Data.OracleClient.OracleConnection p_Connection,
                           string strSql, List<SQLUtility.MySQLParameter> parameters)
        {
            this.m_intError = 0;
            this.m_strError = "";

            this.m_Command = p_Connection.CreateCommand();
            this.m_Command.CommandText = strSql;
            //this.m_Command.BindByName = true;
            
            AddSQLParameters(parameters);

            try
            {
                this.m_DataReader = this.m_Command.ExecuteReader();
            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + " The Query Command " + this.m_Command.CommandText.ToString() + " Failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:SqlQueryReader  \n" +
                        "Err Msg - " + this.m_strError,
                        "QA Tools", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.m_DataReader = null;
                this.m_Command = null;
                return;
            }
        }

        private void AddSQLParameters(List<SQLUtility.MySQLParameter> parms)
        {
            foreach (SQLUtility.MySQLParameter parm in parms)
            {
                this.m_Command.Parameters.Add(new System.Data.OracleClient.OracleParameter(parm.name, parm.value));
            }
        }
        public System.Data.DataTable getTableSchema(System.Data.OracleClient.OracleConnection p_Connection, string strSql)
        {
            System.Data.DataTable p_dt;
            this.m_intError = 0;
            this.m_strError = "";

            this.m_Command = p_Connection.CreateCommand();

            m_Command.Transaction = m_Transaction;

            this.m_Command.CommandText = strSql;
            try
            {
                this.m_DataReader = this.m_Command.ExecuteReader(CommandBehavior.KeyInfo);
                p_dt = this.m_DataReader.GetSchemaTable();
            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + " The Query Command " + this.m_Command.CommandText.ToString() + " Failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:getTableSchema  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.m_DataReader = null;
                this.m_Command = null;
                return null;
            }
            this.m_DataReader.Close();
            return p_dt;

        }
        public System.Data.DataTable getTableSchema(System.Data.OracleClient.OracleConnection p_Connection,
                                                    System.Data.OracleClient.OracleTransaction p_trans,
                                                    string strSql)
        {
            System.Data.DataTable p_dt;
            this.m_intError = 0;
            this.m_strError = "";

            this.m_Command = p_Connection.CreateCommand();
            this.m_Command.CommandText = strSql;
            this.m_Command.Transaction = p_trans;
            try
            {
                this.m_DataReader = this.m_Command.ExecuteReader(CommandBehavior.KeyInfo);
                p_dt = this.m_DataReader.GetSchemaTable();
            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + " The Query Command " + this.m_Command.CommandText.ToString() + " Failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:getTableSchema  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.m_DataReader = null;
                this.m_Command = null;
                return null;
            }
            this.m_DataReader.Close();
            return p_dt;

        }

        public System.Data.DataTable getTableSchema(string strConn, string strSql)
        {
            System.Data.DataTable p_dt;
            this.m_intError = 0;
            this.m_strError = "";

            this.OpenConnection(strConn);
            if (this.m_intError == 0)
            {
                this.m_Command = this.m_Connection.CreateCommand();
                this.m_Command.CommandText = strSql;
                try
                {
                    this.m_DataReader = this.m_Command.ExecuteReader(CommandBehavior.KeyInfo);
                    p_dt = this.m_DataReader.GetSchemaTable();
                }
                catch (Exception caught)
                {
                    this.m_intError = -1;
                    this.m_strError = caught.Message + " The Query Command " + this.m_Command.CommandText.ToString() + " Failed";
                    if (_bDisplayErrors)
                        MessageBox.Show("!!Error!! \n" +
                            "Module - Oracle:getTableSchema  \n" +
                            "Err Msg - " + this.m_strError,
                            "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Exclamation);
                    this.m_DataReader = null;
                    this.m_Command = null;
                    return null;
                }
                this.m_DataReader.Close();
                return p_dt;
            }
            return null;


        }
        public bool FieldExist(System.Data.OracleClient.OracleConnection p_oConn, string p_strSql, string p_strField)
        {
            string strDelimiter = ",";
            string strList = getFieldNames(p_oConn, p_strSql);
            string[] strArray = strList.Split(strDelimiter.ToCharArray());
            for (int x = 0; x <= strArray.Length - 1; x++)
            {
                if (strArray[x] != null && strArray[x].Trim().Length > 0)
                {
                    if (strArray[x].Trim().ToUpper() == p_strField.Trim().ToUpper()) return true;
                }
            }
            return false;

        }
        public bool FieldExist(string p_strConn, string p_strSql, string p_strField)
        {
            string strDelimiter = ",";
            this.OpenConnection(p_strConn);
            if (this.m_intError == 0)
            {
                string strList = getFieldNames(this.m_Connection, p_strSql);
                string[] strArray = strList.Split(strDelimiter.ToCharArray());
                for (int x = 0; x <= strArray.Length - 1; x++)
                {
                    if (strArray[x] != null && strArray[x].Trim().Length > 0)
                    {
                        if (strArray[x].Trim().ToUpper() == p_strField.Trim().ToUpper())
                        {
                            this.m_Connection.Close();
                            return true;
                        }
                    }
                }
                this.m_Connection.Close();

            }
            return false;

        }
        public string getFieldNames(System.Data.OracleClient.OracleConnection p_oConn, string p_strSql)
        {
            this.m_intError = 0;
            System.Data.DataTable oTableSchema = this.getTableSchema(p_oConn, p_strSql);
            if (this.m_intError != 0) return "";
            string strFields = "";

            for (int x = 0; x <= oTableSchema.Rows.Count - 1; x++)
            {
                strFields = strFields + oTableSchema.Rows[x]["columnname"].ToString().Trim() + ",";
            }
            if (strFields.Trim().Length > 0) strFields = strFields.Substring(0, strFields.Trim().Length - 1);

            return strFields;

        }
        public string getFieldNames(string p_strConn, string p_strSql)
        {
            string strFields = "";
            this.m_intError = 0;
            this.OpenConnection(p_strConn);
            if (this.m_intError == 0)
            {
                System.Data.DataTable oTableSchema = this.getTableSchema(this.m_Connection, p_strSql);
                if (this.m_intError != 0) return "";


                for (int x = 0; x <= oTableSchema.Rows.Count - 1; x++)
                {
                    strFields = strFields + oTableSchema.Rows[x]["columnname"].ToString().Trim() + ",";
                }
                if (strFields.Trim().Length > 0) strFields = strFields.Substring(0, strFields.Trim().Length - 1);


            }
            return strFields;

        }
        /// <summary>
        /// Return an array of field names after executing the SELECT SQL 
        /// </summary>
        /// <param name="p_oConn"></param>
        /// <param name="p_strSql"></param>
        /// <returns></returns>
        public string[] getFieldNamesArray(System.Data.OracleClient.OracleConnection p_oConn, string p_strSql)
        {
            this.m_intError = 0;
            string strList = getFieldNames(p_oConn, p_strSql);
            if (strList.Trim().Length == 0) return null;

            string strDelimiter = ",";
            string[] strArray = strList.Split(strDelimiter.ToCharArray());
            return strArray;


        }


        /****
         **** format strings to be used in an sql statement
         ****/
        public static string FixString(string SourceString, string StringToReplace, string StringReplacement)
        {
            SourceString = SourceString.Replace(StringToReplace, StringReplacement);
            return (SourceString);
        }
        //returns Y or N for whether the field is a string or not
        public string getIsTheFieldAStringDataType(string strFieldType)
        {
            switch (strFieldType.Trim())
            {
                case "System.Single":
                    return "N";
                case "System.Double":
                    return "N";
                case "System.Decimal":
                    return "N";
                case "System.String":
                    return "Y";
                case "System.Int16":
                    return "N";
                case "System.Char":
                    return "Y";
                case "System.Int32":
                    return "N";
                case "System.DateTime":
                    return "N";
                case "System.DayOfWeek":
                    return "N";
                case "System.Int64":
                    return "N";
                case "System.Byte":
                    return "N";
                case "System.Boolean":
                    return "N";
                default:
                    //return "N";
                    MessageBox.Show(strFieldType + " is undefined");
                    return "N";
            }


        }
        public void CreateDataSet(string strConn,
            string strSQL, string strTableName)
        {
            this.m_intError = 0;
            this.m_strError = "";
            try
            {
                this.OpenConnection(strConn);
                if (this.m_intError == 0)
                {
                    this.m_DataAdapter = new System.Data.OracleClient.OracleDataAdapter(strSQL, this.m_Connection);
                    this.m_DataSet = new DataSet();
                    this.m_DataAdapter.Fill(this.m_DataSet, strTableName);
                    this.m_Connection.Close();
                }

            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + " : SQL query command " + strSQL + " failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:CreateDataSet  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.m_Connection.Close();
                this.m_DataAdapter = null;
                this.m_DataSet = null;
                return;
            }

        }

        public void CreateDataSet(System.Data.OracleClient.OracleConnection p_conn,
            string strSQL, string strTableName)
        {
            this.m_intError = 0;
            this.m_strError = "";
            try
            {
                this.m_DataAdapter = new System.Data.OracleClient.OracleDataAdapter(strSQL, p_conn);
                this.m_DataSet = new DataSet();
                this.m_DataAdapter.Fill(this.m_DataSet, strTableName);
                //this.m_Connection.Close();
            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + " : SQL query command " + strSQL + " failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:CreateDataSet  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                //this.m_Connection.Close();
                this.m_DataAdapter = null;
                this.m_DataSet = null;
                return;
            }

        }

        public void AddSQLQueryToDataSet(System.Data.OracleClient.OracleConnection p_conn,
            ref System.Data.OracleClient.OracleDataAdapter p_da,
            ref System.Data.DataSet p_ds,
            string strSQL,
            string strTableName)
        {
            this.m_intError = 0;
            this.m_strError = "";
            System.Data.OracleClient.OracleCommand p_Command;
            try
            {
                p_Command = p_conn.CreateCommand();
                p_Command.CommandText = strSQL;
                p_da.SelectCommand = p_Command;
                p_da.Fill(p_ds, strTableName);
            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + " : SQL query command " + strSQL + " failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:AddSQLQueryToDataSet  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                if (_bDisplayErrors)
                    MessageBox.Show(this.m_strError);
            }

        }
        public void AddSQLQueryToDataSet(System.Data.OracleClient.OracleConnection p_conn,
            ref System.Data.OracleClient.OracleDataAdapter p_da,
            ref System.Data.DataSet p_ds,
            ref System.Data.OracleClient.OracleTransaction p_trans,
            string strSQL,
            string strTableName)
        {
            this.m_intError = 0;
            this.m_strError = "";
            System.Data.OracleClient.OracleCommand p_Command;
            try
            {
                p_Command = p_conn.CreateCommand();
                p_Command.CommandText = strSQL;
                p_Command.Transaction = p_trans;
                p_da.SelectCommand = p_Command;
                p_da.Fill(p_ds, strTableName);
            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + " : SQL query command " + strSQL + " failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:AddSQLQueryToDataSet  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                if (_bDisplayErrors)
                    MessageBox.Show(this.m_strError);
            }

        }

        public void DatasetSQLInsertCommand(System.Data.OracleClient.OracleConnection p_conn,
            ref System.Data.OracleClient.OracleDataAdapter p_da,
            ref System.Data.DataSet p_ds,
            string strSQL,
            string strTableName)
        {

        }

        public long getRecordCount(string strConn,
            string strSQL, string strTableName)
        {
            System.Data.OracleClient.OracleConnection p_Conn;
            System.Data.OracleClient.OracleCommand p_Command;
            long intRecTtl = 0;
            this.m_intError = 0;
            this.m_strError = "";
            p_Conn = new System.Data.OracleClient.OracleConnection();
            try
            {
                this.OpenConnection(strConn, ref p_Conn);
                if (this.m_intError == 0)
                {
                    p_Command = p_Conn.CreateCommand();
                    p_Command.CommandText = strSQL;
                    intRecTtl = Convert.ToInt32(p_Command.ExecuteScalar());
                }

            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + "  SQL query command: " + strSQL + " failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:getRecordCount  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                p_Conn.Close();
                if (_bDisplayErrors) MessageBox.Show(this.m_strError);
            }
            try
            {
                p_Conn.Close();
            }
            catch
            {
            }
            p_Conn = null;
            p_Command = null;
            return intRecTtl;

        }

        public long getRecordCount(System.Data.OracleClient.OracleConnection p_conn,
            string strSQL, string strTableName)
        {
            System.Data.OracleClient.OracleCommand p_Command;
            long intRecTtl = 0;
            this.m_intError = 0;
            this.m_strError = "";
            try
            {

                p_Command = p_conn.CreateCommand();
                p_Command.CommandText = strSQL;
                intRecTtl = Convert.ToInt32(p_Command.ExecuteScalar());


            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + "  SQL query command: " + strSQL + " failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:getRecordCount  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
            }
            p_Command = null;
            return intRecTtl;

        }
        public long getRecordCount(System.Data.OracleClient.OracleConnection p_conn,
            System.Data.OracleClient.OracleTransaction p_trans,
            string strSQL, string strTableName)
        {
            System.Data.OracleClient.OracleCommand p_Command;
            long intRecTtl = 0;
            this.m_intError = 0;
            this.m_strError = "";
            try
            {

                p_Command = p_conn.CreateCommand();
                p_Command.CommandText = strSQL;
                p_Command.Transaction = p_trans;
                intRecTtl = Convert.ToInt32(p_Command.ExecuteScalar());


            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + "  SQL query command: " + strSQL + " failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:getRecordCount  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);

            }
            p_Command = null;
            return intRecTtl;

        }
        public string getOracleConnString(string strDataSource, string strUserId, string strPW)
        {
            this.m_strDataSource = strDataSource;
            this.m_strUserId = strUserId;
            return "Data Source=" + strDataSource.Trim() + ";User Id=" + strUserId.Trim() + ";Password=" + strPW.Trim() + ";";
        }

        public string CreateSQLNOTINString(System.Data.OracleClient.OracleConnection p_conn, string strSQL)
        {
            string str = "";
            this.SqlQueryReader(p_conn, strSQL);
            if (this.m_intError == 0)
            {
                if (this.m_DataReader.HasRows)
                {
                    while (this.m_DataReader.Read())
                    {
                        if (str.Trim().Length == 0)
                        {
                            str = this.m_DataReader[0].ToString().Trim();
                        }
                        else
                        {
                            str += "," + this.m_DataReader[0].ToString().Trim();
                        }
                    }
                }
                this.m_DataReader.Close();
            }
            return str;


        }
        public string getSingleStringValueFromSQLQuery(System.Data.OracleClient.OracleConnection p_conn,
            string strSQL, string strTableName)
        {
            System.Data.OracleClient.OracleCommand p_Command;
            string strValue = "";
            this.m_intError = 0;
            this.m_strError = "";
            try
            {

                p_Command = p_conn.CreateCommand();
                p_Command.CommandText = strSQL;
                strValue = Convert.ToString(p_Command.ExecuteScalar());


            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + "  SQL query command: " + strSQL + " failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:getSingleStringValueFromSQLQuery  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);

            }
            p_Command = null;
            return strValue;

        }
        public string getSingleStringValueFromSQLQuery(System.Data.OracleClient.OracleConnection p_conn,
            System.Data.OracleClient.OracleTransaction p_trans, string strSQL, string strTableName)
        {
            System.Data.OracleClient.OracleCommand p_Command;
            string strValue = "";
            this.m_intError = 0;
            this.m_strError = "";
            try
            {

                p_Command = p_conn.CreateCommand();
                p_Command.CommandText = strSQL;
                p_Command.Transaction = p_trans;
                strValue = Convert.ToString(p_Command.ExecuteScalar());


            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + "  SQL query command: " + strSQL + " failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:getSingleStringValueFromSQLQuery  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);



            }
            p_Command = null;
            return strValue;

        }
        /// <summary>
        /// Execute a query that returns a single string value
        /// </summary>
        /// <param name="strConn">Access connection string</param>
        /// <param name="strSQL">SQL that returns a single string value</param>
        /// <param name="strTableName">table name</param>
        /// <returns></returns>
        public string getSingleStringValueFromSQLQuery(string strConn,
            string strSQL, string strTableName)
        {
            System.Data.OracleClient.OracleConnection oOleDbConn;
            System.Data.OracleClient.OracleCommand oOleDbCommand;
            string strValue = "";
            this.m_intError = 0;
            this.m_strError = "";
            try
            {
                oOleDbConn = new System.Data.OracleClient.OracleConnection();
                this.OpenConnection(strConn, ref oOleDbConn);
                if (m_intError == 0)
                {
                    oOleDbCommand = oOleDbConn.CreateCommand();
                    oOleDbCommand.CommandText = strSQL;
                    strValue = Convert.ToString(oOleDbCommand.ExecuteScalar());
                    oOleDbConn.Close();
                }


            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + "  SQL query command: " + strSQL + " failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:getSingleStringValueFromSQLQuery  \n" +
                        "Err Msg - " + this.m_strError,
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);

            }
            oOleDbCommand = null;
            oOleDbConn = null;
            return strValue;

        }
        /// <summary>
        /// return a single numeric int value resulting from SQL command 
        /// </summary>
        /// <param name="p_conn"></param>
        /// <param name="strSQL"></param>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public int getSingleIntegerValueFromSQLQuery(System.Data.OracleClient.OracleConnection p_conn,
            string strSQL, string strTableName)
        {
            int? intValue = -1;
            this.m_intError = 0;
            this.m_strError = "";
            object objValue = null;
            try
            {
                using (System.Data.OracleClient.OracleCommand command = new System.Data.OracleClient.OracleCommand())
                {
                    command.CommandText = strSQL;
                    command.Connection = p_conn;
                    objValue = command.ExecuteScalar();
                    if (objValue != DBNull.Value) intValue = Convert.ToInt32(objValue);
                }


            }
            catch (Exception caught)
            {
                this.m_intError = -1;
                this.m_strError = caught.Message + "  SQL query command: " + strSQL + " failed";
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - ado_data_access:getSingleStringValueFromSQLQuery  \n" +
                        "Err Msg - " + this.m_strError,
                        "FIA Biosum", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);

            }
            if (objValue == DBNull.Value) intValue = -1;
            return (int)intValue;

        }

        public System.Data.DataTable ConvertDataViewToDataTable(
            System.Data.DataView p_dv)
        {
            int x = 0;
            System.Data.DataTable p_dtNew;
            //copy exact structure from the view to the new table
            p_dtNew = p_dv.Table.Clone();
            int idx = 0;
            //create an array containing all the column names in the new data table
            string[] strColNames = new string[p_dtNew.Columns.Count];
            for (x = 0; x <= p_dtNew.Columns.Count - 1; x++)
            {
                strColNames[idx++] = p_dtNew.Columns[x].ColumnName;
            }
            //append each row in the dataview to the new table
            System.Collections.IEnumerator viewEnumerator = p_dv.GetEnumerator();

            while (viewEnumerator.MoveNext())
            {
                DataRowView drv = (DataRowView)viewEnumerator.Current;
                DataRow dr = p_dtNew.NewRow();
                try
                {
                    foreach (string strName in strColNames)
                    {
                        //value in data table row and column equal to value in 
                        //dataview row and column value
                        dr[strName] = drv[strName];

                    }
                }
                catch (Exception ex)
                {
                    if (_bDisplayErrors)
                        MessageBox.Show("!!Error!! \n" +
                            "Module - Oracle:ConvertDataViewToDataTable  \n" +
                            "Err Msg - " + ex.Message,
                            "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Exclamation);

                }
                //append the new row to the data table
                p_dtNew.Rows.Add(dr);
            }
            return p_dtNew;
        }
        /// <summary>
        /// Converts a given delimited file into a dataset. 
        /// Assumes that the first line    
        /// of the text file contains the column names.
        /// </summary>
        /// <param name="File">The name of the file to open</param>    
        /// <param name="TableName">The name of the 
        /// Table to be made within the DataSet returned</param>
        /// <param name="delimiter">The string to delimit by</param>
        /// <returns></returns>  
        public void ConvertDelimitedTextToDataTable(System.Data.DataSet p_ds,
                                                        string p_strFile,
                                                        string p_strTableName, string p_strDelimiter)
        {
            this.m_intError = 0;
            try
            {
                //Open the file in a stream reader.
                StreamReader s = new StreamReader(p_strFile);

                //Split the first line into the columns       
                string[] columns = s.ReadLine().Split(p_strDelimiter.ToCharArray());

                //Add the new DataTable to the RecordSet
                p_ds.Tables.Add(p_strTableName);

                //Cycle the colums, adding those that don't exist yet 
                //and sequencing the one that do.
                foreach (string col in columns)
                {
                    bool added = false;
                    string next = "";
                    int i = 0;
                    while (!added)
                    {
                        //Build the column name and remove any unwanted characters.
                        string columnname = col + next;
                        columnname = columnname.Replace("#", "");
                        columnname = columnname.Replace("'", "");
                        columnname = columnname.Replace("&", "");
                        columnname = columnname.Replace("\"", "");

                        //See if the column already exists
                        if (!p_ds.Tables[p_strTableName].Columns.Contains(columnname))
                        {
                            //if it doesn't then we add it here and mark it as added
                            p_ds.Tables[p_strTableName].Columns.Add(columnname);
                            added = true;
                        }
                        else
                        {
                            //if it did exist then we increment the sequencer and try again.
                            i++;
                            next = "_" + i.ToString();
                        }
                    }
                }

                //Read the rest of the data in the file.        
                string AllData = s.ReadToEnd();

                //Split off each row at the Carriage Return/Line Feed
                //Default line ending in most <A class=iAs style="FONT-WEIGHT: normal; FONT-SIZE: 100%; PADDING-BOTTOM: 1px; COLOR: darkgreen; BORDER-BOTTOM: darkgreen 0.07em solid; BACKGROUND-COLOR: transparent; TEXT-DECORATION: underline" href="#" target=_blank itxtdid="2592535">windows</A> exports.  
                //You may have to edit this to match your particular file.
                //This will work for Excel, Access, etc. default exports.
                string[] rows = AllData.Split("\r\n".ToCharArray());

                //Now add each row to the DataSet        
                foreach (string r in rows)
                {
                    //Split the row at the delimiter.
                    string[] items = r.Split(p_strDelimiter.ToCharArray());

                    //Add the item
                    p_ds.Tables[p_strTableName].Rows.Add(items);
                }
            }
            catch (Exception caught)
            {

                this.m_intError = -1;
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:ConvertDelimitedTextToDataTable  \n" +
                        "Err Msg - " + caught.Message.ToString().Trim(),
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.m_intError = -1;
            }

            //Return the imported data.        

        }
        /// <summary>
        /// Dispose of dataadapter object and reconfigure a new dataadapter
        /// </summary>
        /// <param name="TABLE_INDEX">the index within the dataadapter array object m_DataAdapterArray</param>
        /// <param name="p_strSchema">oracle db schema</param>
        /// <param name="p_strTableName">table name within the schema</param>
        /// <param name="p_strColumns">Comma delimited list of all the columns to be configured by the dataadapter.
        ///Can be a subset of all the columns in the table. VALID VALUES: * or comma-delimited list </param>
        /// <param name="p_strPrimaryKey">Comma-delimited list of the primary key(s)</param>
        public void InitializeDataAdapterArrayItem(int TABLE_INDEX, string p_strSchema, string p_strTableName, string p_strColumns, string p_strPrimaryKeyColumns)
        {


            if (m_DataAdapterArray[TABLE_INDEX] != null)
            {
                if (m_DataAdapterArray[TABLE_INDEX].SelectCommand != null)
                {
                    // m_DataAdapterArray[TABLE_INDEX].SelectCommand.Transaction.Dispose();
                    m_DataAdapterArray[TABLE_INDEX].SelectCommand.Dispose();
                }
                if (m_DataAdapterArray[TABLE_INDEX].UpdateCommand != null)
                {
                    //m_DataAdapterArray[TABLE_INDEX].UpdateCommand.Transaction.Dispose();
                    m_DataAdapterArray[TABLE_INDEX].UpdateCommand.Dispose();
                }
                if (m_DataAdapterArray[TABLE_INDEX].DeleteCommand != null)
                {
                    //m_DataAdapterArray[TABLE_INDEX].DeleteCommand.Transaction.Dispose();
                    m_DataAdapterArray[TABLE_INDEX].DeleteCommand.Dispose();
                }
                if (m_DataAdapterArray[TABLE_INDEX].InsertCommand != null)
                {
                    //m_DataAdapterArray[TABLE_INDEX].InsertCommand.Transaction.Dispose();
                    m_DataAdapterArray[TABLE_INDEX].InsertCommand.Dispose();
                }
                m_DataAdapterArray[TABLE_INDEX].Dispose();
            }
            for (int x = 0; x <= m_DataSet.Tables.Count - 1; x++)
            {
                if (m_DataSet.Tables[x].TableName.ToUpper().Trim() == p_strTableName.ToUpper().Trim())
                {
                    m_DataSet.Tables[p_strTableName].Clear();
                    m_DataSet.Tables[p_strTableName].Dispose();
                    break;
                }
            }
            m_DataAdapterArray[TABLE_INDEX] = new OracleDataAdapter();
            m_strSQL = "SELECT " + p_strColumns + " FROM " + p_strSchema + "." + p_strTableName;
            InitializeOleDbTransactionCommands(m_DataAdapterArray[TABLE_INDEX], p_strSchema, p_strTableName, p_strColumns, p_strPrimaryKeyColumns);
            m_strSQL = "SELECT " + p_strColumns + " FROM " + p_strSchema + "." + p_strTableName;
            m_Command = m_Connection.CreateCommand();
            m_Command.CommandText = m_strSQL;
            m_DataAdapterArray[TABLE_INDEX].SelectCommand = m_Command;
            m_DataAdapterArray[TABLE_INDEX].SelectCommand.Transaction = m_Transaction;
            m_DataAdapterArray[TABLE_INDEX].Fill(this.m_DataSet, p_strTableName);
            m_DataSet.Tables[p_strTableName].PrimaryKey = new System.Data.DataColumn[] { this.m_DataSet.Tables[p_strTableName].Columns[p_strPrimaryKeyColumns] };
            m_DataAdapterArrayTableName[TABLE_INDEX] = p_strTableName;
        }
        public int getDataAdaperArrayItemIndex(string p_strTableName)
        {
            int index = -1;
            if (m_DataAdapterArrayTableName == null) return -1;
            if (m_DataAdapterArrayTableName.Length == 0) return -1;
            if (m_DataAdapterArrayTableName[0] == null) return -1;
            for (index = 0; index <= m_DataAdapterArrayTableName.Length - 1; index++)
            {
                if (m_DataAdapterArrayTableName[index] != null)
                {
                    if (m_DataAdapterArrayTableName[index].Trim().ToUpper() == p_strTableName.Trim().ToUpper()) break;
                }
            }
            if (index > m_DataAdapterArrayTableName.Length - 1) index = -1;
            return index;
        }

        public void ResetTransactionObjectToDataAdapterArray()
        {
            m_Transaction = m_Connection.BeginTransaction();


            foreach (OracleDataAdapter da in m_DataAdapterArray)
            {
                if (da != null)
                {
                    da.InsertCommand.Transaction = m_Transaction;
                    da.DeleteCommand.Transaction = m_Transaction;
                    da.UpdateCommand.Transaction = m_Transaction;
                }


            }

        }
        public void ResetTransactionObjectToDataAdapter()
        {
            m_Transaction = m_Connection.BeginTransaction();

            m_DataAdapter.InsertCommand.Transaction = m_Transaction;
            m_DataAdapter.DeleteCommand.Transaction = m_Transaction;
            m_DataAdapter.UpdateCommand.Transaction = m_Transaction;


        }
        public void InitializeDataAdapterArray()
        {
            InitializeDataAdapterArray(DataAdapterTableCount);
        }
        /// <summary>
        /// Size and create dataadapter array, dispose of each existing dataadapter object in the array,
        /// and instantiate each dataadapter object in the array.
        /// </summary>
        /// <param name="p_intTableCount">Size of the array</param>
        public void InitializeDataAdapterArray(int p_intTableCount)
        {


            int x = 0;

            if (m_DataAdapterArray != null)
            {
                for (x = 0; x <= p_intTableCount - 1; x++)
                {
                    if (m_DataAdapterArray[x] != null)
                    {
                        if (m_DataAdapterArray[x].SelectCommand != null)
                        {
                            //m_DataAdapterArray[x].SelectCommand.Transaction.Dispose();
                            m_DataAdapterArray[x].SelectCommand.Dispose();
                        }
                        if (m_DataAdapterArray[x].UpdateCommand != null)
                        {
                            // m_DataAdapterArray[x].UpdateCommand.Transaction.Dispose();
                            m_DataAdapterArray[x].UpdateCommand.Dispose();
                        }
                        if (m_DataAdapterArray[x].DeleteCommand != null)
                        {
                            //m_DataAdapterArray[x].DeleteCommand.Transaction.Dispose();
                            m_DataAdapterArray[x].DeleteCommand.Dispose();
                        }
                        if (m_DataAdapterArray[x].InsertCommand != null)
                        {
                            //m_DataAdapterArray[x].InsertCommand.Transaction.Dispose();
                            m_DataAdapterArray[x].InsertCommand.Dispose();
                        }
                        m_DataAdapterArray[x].Dispose();
                    }
                    m_DataAdapterArray[x] = new OracleDataAdapter();
                    m_DataAdapterArrayTableName[x] = "";
                }
            }
            else
            {
                if (m_DataAdapterArray == null)
                {
                    m_DataAdapterArray = new OracleDataAdapter[p_intTableCount];
                    m_DataAdapterArrayTableName = new string[p_intTableCount];
                    for (x = 0; x <= m_DataAdapterArray.Count() - 1; x++)
                    {
                        m_DataAdapterArray[x] = null;
                        m_DataAdapterArrayTableName[x] = "";

                    }
                }
            }

        }
        /// <summary>
        /// Dispose of dataadapter object and reconfigure a new dataadapter
        /// </summary>
        /// <param name="p_strSchema">oracle db schema</param>
        /// <param name="p_strTableName">table name within the schema</param>
        /// <param name="p_strColumns">Comma delimited list of all the columns to be configured by the dataadapter.
        ///Can be a subset of all the columns in the table. VALID VALUES: * or comma-delimited list </param>
        /// <param name="p_strPrimaryKey">Comma-delimited list of the primary key(s)</param>
        public void InitializeDataAdapter(string p_strSchema, string p_strTableName, string p_strColumns, string p_strPrimaryKeyColumns, int p_intMAXRecords, string p_strWhereCondition)
        {
            if (m_DataAdapter != null)
            {
                if (m_DataAdapter.SelectCommand != null)
                {
                    m_DataAdapter.SelectCommand.Dispose();
                }
                if (m_DataAdapter.UpdateCommand != null)
                {
                    m_DataAdapter.UpdateCommand.Dispose();
                }
                if (m_DataAdapter.DeleteCommand != null)
                {
                    m_DataAdapter.DeleteCommand.Dispose();
                }
                if (m_DataAdapter.InsertCommand != null)
                {
                    m_DataAdapter.InsertCommand.Dispose();
                }
                m_DataAdapter.Dispose();
            }
            //if (m_Transaction != null) m_Transaction.Dispose();
            for (int x = 0; x <= m_DataSet.Tables.Count - 1; x++)
            {
                if (m_DataSet.Tables[x].TableName.ToUpper().Trim() == p_strTableName.ToUpper().Trim())
                {
                    m_DataSet.Tables[p_strTableName].Clear();
                    m_DataSet.Tables[p_strTableName].Dispose();
                    break;
                }
            }
            this.m_DataAdapter = new OracleDataAdapter();
            m_strSQL = "SELECT " + p_strColumns + " FROM " + p_strSchema + "." + p_strTableName;
            InitializeOleDbTransactionCommands(p_strSchema, p_strTableName, p_strColumns, p_strPrimaryKeyColumns);
            if (p_strWhereCondition != null && p_strWhereCondition.Trim().Length > 0)
                m_strSQL = "SELECT " + p_strColumns + " FROM " + p_strSchema + "." + p_strTableName + " WHERE " + p_strWhereCondition;
            else
                m_strSQL = "SELECT " + p_strColumns + " FROM " + p_strSchema + "." + p_strTableName;
            m_Command = m_Connection.CreateCommand();
            m_Command.CommandText = m_strSQL;
            m_DataAdapter.SelectCommand = m_Command;
            m_DataAdapter.SelectCommand.Transaction = m_Transaction;
            if (p_intMAXRecords > 0)
            {
                this.m_DataAdapter.Fill(this.m_DataSet, 0, p_intMAXRecords, p_strTableName);
            }
            else
            {
                this.m_DataAdapter.Fill(this.m_DataSet, p_strTableName);
            }
            m_DataSet.Tables[p_strTableName].PrimaryKey = new System.Data.DataColumn[] { this.m_DataSet.Tables[p_strTableName].Columns[p_strPrimaryKeyColumns] };
        }
        public void InitializeOleDbTransactionCommands(OracleDataAdapter p_DataAdapter, string p_strSchema, string p_strTableName, string p_strColumns, string p_strPrimaryKey)
        {
            string strDelimiter = ",";
            string strNonPrimaryKeyColumns = "";
            string[] strColumnArray = getFieldNamesArray(m_Connection, "select " + p_strColumns + " from " + p_strSchema + "." + p_strTableName);
            string[] strPrimaryKeyArray;
            //check if more than one primary key column
            if (p_strPrimaryKey.IndexOf(",", 0) > 0)
                strPrimaryKeyArray = p_strPrimaryKey.Split(strDelimiter.ToCharArray());
            else
                strPrimaryKeyArray = new string[1]; strPrimaryKeyArray[0] = p_strPrimaryKey;

            for (int x = 0; x <= strColumnArray.Length - 1; x++)
            {
                if (strColumnArray[x] != null && strColumnArray[x].Trim().Length > 0)
                {
                    //make sure column is not part of the primary key
                    int COUNT = strPrimaryKeyArray.Where(pk => pk.Trim().ToUpper() == strColumnArray[x].Trim().ToUpper()).Count();
                    if (COUNT == 0)
                        strNonPrimaryKeyColumns = strNonPrimaryKeyColumns + strColumnArray[x].Trim() + ",";
                }
            }
            //remove last comma
            if (strNonPrimaryKeyColumns.Length > 0) strNonPrimaryKeyColumns = strNonPrimaryKeyColumns.Substring(0, strNonPrimaryKeyColumns.Length - 1);

            this.m_strSQL = "select " + p_strColumns + " from " + p_strSchema + "." + p_strTableName;
            //initialize the transaction object with the connection
            //this.m_Transaction = this.m_Connection.BeginTransaction();
            this.ConfigureDataAdapterInsertCommand(this.m_Connection,
                p_DataAdapter,
                this.m_Transaction,
                this.m_strSQL,
                p_strSchema,
                p_strTableName);

            this.m_strSQL = "select " + strNonPrimaryKeyColumns + " from " + p_strSchema + "." + p_strTableName;
            this.ConfigureDataAdapterUpdateCommand(this.m_Connection,
                p_DataAdapter,
                this.m_Transaction,
                this.m_strSQL, "select " + p_strPrimaryKey + " FROM " + p_strSchema + "." + p_strTableName,
                p_strSchema,
                p_strTableName);

            this.m_strSQL = "select " + strNonPrimaryKeyColumns + " from " + p_strSchema + "." + p_strTableName;
            this.ConfigureDataAdapterDeleteCommand(this.m_Connection,
                p_DataAdapter,
                this.m_Transaction,
                "select " + p_strPrimaryKey + " FROM " + p_strSchema + "." + p_strTableName,
                p_strSchema,
                p_strTableName);




        }
        /// <summary>
        /// identify primary key columns and configure dataadaper for INSERT,UPDATE, and DELETE.
        /// </summary>
        /// <param name="p_strSchema">oracle db schema</param>
        /// <param name="p_strTableName">table name within the schema</param>
        /// <param name="p_strColumns">Comma delimited list of all the columns to be configured by the dataadapter.
        ///Can be a subset of all the columns in the table. VALID VALUES: * or comma-delimited list </param>
        /// <param name="p_strPrimaryKey">Comma-delimited list of the primary key(s)</param>
        public void InitializeOleDbTransactionCommands(string p_strSchema, string p_strTableName, string p_strColumns, string p_strPrimaryKey)
        {
            string strDelimiter = ",";
            string strNonPrimaryKeyColumns = "";
            string[] strColumnArray = getFieldNamesArray(m_Connection, "select " + p_strColumns + " from " + p_strSchema + "." + p_strTableName);
            string[] strPrimaryKeyArray;
            //check if more than one primary key column
            if (p_strPrimaryKey.IndexOf(",", 0) > 0)
                strPrimaryKeyArray = p_strPrimaryKey.Split(strDelimiter.ToCharArray());
            else
                strPrimaryKeyArray = new string[1]; strPrimaryKeyArray[0] = p_strPrimaryKey;

            for (int x = 0; x <= strColumnArray.Length - 1; x++)
            {
                if (strColumnArray[x] != null && strColumnArray[x].Trim().Length > 0)
                {
                    //make sure column is not part of the primary key
                    int COUNT = strPrimaryKeyArray.Where(pk => pk.Trim().ToUpper() == strColumnArray[x].Trim().ToUpper()).Count();
                    if (COUNT == 0)
                        strNonPrimaryKeyColumns = strNonPrimaryKeyColumns + strColumnArray[x].Trim() + ",";
                }
            }
            //remove last comma
            if (strNonPrimaryKeyColumns.Length > 0) strNonPrimaryKeyColumns = strNonPrimaryKeyColumns.Substring(0, strNonPrimaryKeyColumns.Length - 1);

            m_strSQL = "select " + p_strColumns + " from " + p_strSchema + "." + p_strTableName;
            //initialize the transaction object with the connection
            //m_Transaction = m_Connection.BeginTransaction();
            ConfigureDataAdapterInsertCommand(m_Connection,
                m_DataAdapter,
                m_Transaction,
                m_strSQL,
                p_strSchema,
                p_strTableName);

            m_strSQL = "select " + strNonPrimaryKeyColumns + " from " + p_strSchema + "." + p_strTableName;
            ConfigureDataAdapterUpdateCommand(m_Connection,
                m_DataAdapter,
                m_Transaction,
                m_strSQL, "select " + p_strPrimaryKey + " FROM " + p_strSchema + "." + p_strTableName,
                p_strSchema,
                p_strTableName);

            m_strSQL = "select " + strNonPrimaryKeyColumns + " from " + p_strSchema + "." + p_strTableName;
            ConfigureDataAdapterDeleteCommand(m_Connection,
                m_DataAdapter,
                m_Transaction,
                "select " + p_strPrimaryKey + " FROM " + p_strSchema + "." + p_strTableName,
                p_strSchema,
                p_strTableName);


        }

        /// <summary>
        /// Create an oledb data adapter insert command. The select sql statement
        /// is used to get the data types of the fields used in the insert.
        /// </summary>
        /// <param name="p_conn">the oledb database connection</param>
        /// <param name="p_da">the data adapter</param>
        /// <param name="p_trans">oledb transaction object</param>
        /// <param name="p_strSQL">select sql statement containing fields in the insert command</param>
        /// <param name="p_strSchema">oracle schema</param>
        /// <param name="p_strTable">table name that records are inserted</param>
        public void ConfigureDataAdapterInsertCommand(System.Data.OracleClient.OracleConnection p_conn,
                                                      System.Data.OracleClient.OracleDataAdapter p_da,
                                                       System.Data.OracleClient.OracleTransaction p_trans,
                                                       string p_strSQL, string p_strSchema, string p_strTable)
        {

            this.m_intError = 0;
            System.Data.DataTable p_dtTableSchema = this.getTableSchema(p_conn, p_trans, p_strSQL);
            if (this.m_intError != 0) return;
            string strFields = "";
            string strValues = "";
            int x;
            try
            {
                //Build the plot insert sql
                for (x = 0; x <= p_dtTableSchema.Rows.Count - 1; x++)
                {
                    if (strFields.Trim().Length == 0)
                    {
                        strFields = "(";
                    }
                    else
                    {
                        strFields = strFields + ",";
                    }
                    strFields = strFields + p_dtTableSchema.Rows[x]["columnname"].ToString().Trim();
                    if (strValues.Trim().Length == 0)
                    {
                        strValues = "(";
                    }
                    else
                    {
                        strValues = strValues + ",";
                    }
                    //strValues = strValues + "?";
                    strValues = strValues + ":" + p_dtTableSchema.Rows[x]["columnname"].ToString().Trim();

                }
                strFields = strFields + ")";
                strValues = strValues + ")";
                //create an insert command 
                p_da.InsertCommand = p_conn.CreateCommand();
                //bind the transaction object to the insert command
                p_da.InsertCommand.Transaction = p_trans;
                p_da.InsertCommand.CommandText =
                    "INSERT INTO " + p_strSchema + "." + p_strTable + " " + strFields + " VALUES " + strValues;
                //define field datatypes for the data adapter
                for (x = 0; x <= p_dtTableSchema.Rows.Count - 1; x++)
                {
                    strFields = p_dtTableSchema.Rows[x]["columnname"].ToString().Trim();
                    switch (p_dtTableSchema.Rows[x]["datatype"].ToString().Trim())
                    {
                        case "System.String":
                            p_da.InsertCommand.Parameters.Add
                                (strFields,
                                System.Data.OracleClient.OracleType.VarChar,
                                0,
                                strFields);
                            break;
                        case "System.Double":
                            p_da.InsertCommand.Parameters.Add
                                (strFields,
                                System.Data.OracleClient.OracleType.Double,
                                0,
                                strFields);
                            break;
                        //case "System.Boolean":
                        //	p_da.InsertCommand.Parameters.Add
                        //		(strFields, 
                        //		System.Data.OracleClient.OracleDbType.Boolean,
                        //		0,
                        //		strFields);
                        //	break;
                        case "System.DateTime":
                            p_da.InsertCommand.Parameters.Add
                                (strFields,
                                System.Data.OracleClient.OracleType.DateTime,
                                0,
                                strFields);
                            break;
                        case "System.Decimal":
                            p_da.InsertCommand.Parameters.Add
                                (strFields,
                                System.Data.OracleClient.OracleType.Double,
                                0,
                                strFields);
                            break;
                        case "System.Int16":
                            p_da.InsertCommand.Parameters.Add
                                (strFields,
                                System.Data.OracleClient.OracleType.Int16,
                                0,
                                strFields);
                            break;
                        case "System.Int32":
                            p_da.InsertCommand.Parameters.Add
                                (strFields,
                                System.Data.OracleClient.OracleType.Int32,
                                0,
                                strFields);
                            break;
                        case "System.Int64":
                            p_da.InsertCommand.Parameters.Add
                                (strFields,
                                System.Data.OracleClient.OracleType.Number,
                                0,
                                strFields);
                            break;
                        
                         case "System.SByte":
                             p_da.InsertCommand.Parameters.Add
                                 (strFields,
                                 System.Data.OracleClient.OracleType.SByte,
                                 0,
                                 strFields);
                             break;
                        
                        case "System.Byte":
                            p_da.InsertCommand.Parameters.Add
                                (strFields,
                                System.Data.OracleClient.OracleType.Byte,
                                0,
                                strFields);
                            break;
                        case "System.Single":
                            p_da.InsertCommand.Parameters.Add
                                (strFields,
                                System.Data.OracleClient.OracleType.Float,
                                0,
                                strFields);
                            break;
                        default:
                            MessageBox.Show("Could Not Set Data Adapter Parameter For DataType " + p_dtTableSchema.Rows[x]["datatype"].ToString().Trim());
                            break;
                    }

                }
            }
            catch (Exception e)
            {
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:ConfigureDataAdapterInsertCommand  \n" +
                        "Err Msg - " + e.Message.ToString().Trim(),
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.m_intError = -1;
            }

        }
        /// <summary>
        /// create the update command for the data adapter. 
        /// </summary>
        /// <param name="p_conn">oracle connection object</param>
        /// <param name="p_da">oracle dataadapter object</param>
        /// <param name="p_trans">oracle transaction object</param>
        /// <param name="p_strSQL">select sql statement to get the update field data types</param>
        /// <param name="p_strSQLUniqueRecordFields">select SQL statement listing fields used for a records unique id are queried and their field types obtained and added to the dataadapter updates parameters list</param>
        /// <param name="p_strSchema">oracle schema that contains table name</param>
        /// <param name="p_strTable">table name to be updated</param>
        public void ConfigureDataAdapterUpdateCommand(System.Data.OracleClient.OracleConnection p_conn,
            System.Data.OracleClient.OracleDataAdapter p_da,
            System.Data.OracleClient.OracleTransaction p_trans,
            string p_strSQL, string p_strSQLUniqueRecordFields, string p_strSchema, string p_strTable)
        {

            this.m_intError = 0;
            System.Data.DataTable p_dtTableSchema = this.getTableSchema(p_conn, p_trans, p_strSQL);
            System.Data.DataTable p_dtTableSchema2 = new DataTable();
            if (this.m_intError != 0) return;
            string strField = "";
            string strValue = "";
            string strSQL = "";
            int x;
            try
            {
                //Build the plot update sql
                for (x = 0; x <= p_dtTableSchema.Rows.Count - 1; x++)
                {
                    strField = p_dtTableSchema.Rows[x]["columnname"].ToString().Trim();
                    if (strValue.Trim().Length == 0)
                    {
                        strValue = strField + "=:" + strField;
                    }
                    else
                    {
                        strValue += "," + strField + "=:" + strField;
                    }
                }

                strSQL =
                    "UPDATE " + p_strSchema + "." + p_strTable + " SET " + strValue;

                //get the unique record id
                if (p_strSQLUniqueRecordFields.Trim().Length > 0)
                {
                    strValue = "";
                    p_dtTableSchema2 = this.getTableSchema(p_conn, p_trans, p_strSQLUniqueRecordFields);
                    if (this.m_intError != 0) return;
                    //build the where condition
                    for (x = 0; x <= p_dtTableSchema2.Rows.Count - 1; x++)
                    {
                        strField = p_dtTableSchema2.Rows[x]["columnname"].ToString().Trim();
                        if (strValue.Trim().Length == 0)
                        {
                            strValue = strField + "=:" + strField;
                        }
                        else
                        {
                            strValue += " AND " + strField + "=:" + strField;
                        }
                    }
                    strSQL += " WHERE " + strValue;
                }


                //create an insert command 
                p_da.UpdateCommand = p_conn.CreateCommand();
                //bind the transaction object to the insert command
                p_da.UpdateCommand.Transaction = p_trans;
                p_da.UpdateCommand.CommandText = strSQL;

                //copy the table schema records containing update fields info to a new table
                System.Data.DataTable p_dt = p_dtTableSchema.Copy();

                //define field datatypes for the data adapter
                for (; ; )
                {
                    for (x = 0; x <= p_dt.Rows.Count - 1; x++)
                    {
                        strField = p_dt.Rows[x]["columnname"].ToString().Trim();
                        switch (p_dt.Rows[x]["datatype"].ToString().Trim())
                        {
                            case "System.String":
                                p_da.UpdateCommand.Parameters.Add
                                    (strField,
                                    System.Data.OracleClient.OracleType.VarChar,
                                    0,
                                    strField);
                                break;
                            case "System.Double":
                                p_da.UpdateCommand.Parameters.Add
                                    (strField,
                                    System.Data.OracleClient.OracleType.Double,
                                    0,
                                    strField);
                                break;
                            //case "System.Boolean":
                            //	p_da.UpdateCommand.Parameters.Add
                            //		(strField, 
                            //		System.Data.OracleClient.OracleDbType.,
                            //		0,
                            //		strField);
                            //	break;
                            case "System.DateTime":
                                p_da.UpdateCommand.Parameters.Add
                                    (strField,
                                    System.Data.OracleClient.OracleType.DateTime,
                                    0,
                                    strField);
                                break;
                            case "System.Decimal":
                                p_da.UpdateCommand.Parameters.Add
                                    (strField,
                                    System.Data.OracleClient.OracleType.Double,
                                    0,
                                    strField);
                                break;
                            case "System.Int16":
                                p_da.UpdateCommand.Parameters.Add
                                    (strField,
                                    System.Data.OracleClient.OracleType.Int16,
                                    0,
                                    strField);
                                break;
                            case "System.Int32":
                                p_da.UpdateCommand.Parameters.Add
                                    (strField,
                                    System.Data.OracleClient.OracleType.Int32,
                                    0,
                                    strField);
                                break;
                            case "System.Int64":
                                p_da.UpdateCommand.Parameters.Add
                                    (strField,
                                    System.Data.OracleClient.OracleType.Number,
                                    0,
                                    strField);
                                break;
                            
                            case "System.SByte":
                                p_da.UpdateCommand.Parameters.Add
                                    (strField,
                                    System.Data.OracleClient.OracleType.SByte,
                                    0,
                                    strField);
                                break;
                            
                            case "System.Byte":
                                p_da.UpdateCommand.Parameters.Add
                                    (strField,
                                    System.Data.OracleClient.OracleType.Byte,
                                    0,
                                    strField);
                                break;
                            case "System.Single":
                                p_da.UpdateCommand.Parameters.Add
                                    (strField,
                                    System.Data.OracleClient.OracleType.Float,
                                    0,
                                    strField);
                                break;
                            default:
                                MessageBox.Show("Could Not Set Data Adapter Parameter For DataType " + p_dt.Rows[x]["datatype"].ToString().Trim());
                                break;
                        }

                    }
                    if (p_strSQLUniqueRecordFields.Trim().Length > 0)
                    {
                        //clear the data table of all its records
                        p_dt.Clear();
                        //copy the table schema records containing where clause fields info to a new table
                        p_dt = p_dtTableSchema2.Copy();
                        p_strSQLUniqueRecordFields = "";
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:ConfigureDataAdapterUpdateCommand  \n" +
                        "Err Msg - " + e.Message.ToString().Trim(),
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.m_intError = -1;
            }
        }

        /// <summary>
        /// create the delete command for the data adapter. 
        /// </summary>
        /// <param name="p_conn">oledb connection object</param>
        /// <param name="p_da">oledb dataadapter object</param>
        /// <param name="p_trans">oledb transaction object</param>
        /// <param name="p_strSQLUniqueRecordFields">select SQL statement listing fields used for a records unique id are queried and their field types obtained and added to the dataadapter delete command parameters list</param>
        /// <param name="p_strSchema">oracle schema that contains table</param>
        /// <param name="p_strTable">table name to be updated</param>
        public void ConfigureDataAdapterDeleteCommand(System.Data.OracleClient.OracleConnection p_conn,
            System.Data.OracleClient.OracleDataAdapter p_da,
            System.Data.OracleClient.OracleTransaction p_trans,
            string p_strSQLUniqueRecordFields, string p_strSchema, string p_strTable)
        {

            this.m_intError = 0;
            System.Data.DataTable p_dt = this.getTableSchema(p_conn, p_trans, p_strSQLUniqueRecordFields);
            if (this.m_intError != 0) return;
            string strField = "";
            string strValue = "";
            string strSQL = "";
            int x;
            try
            {
                strSQL = "DELETE FROM " + p_strSchema + "." + p_strTable + " ";
                //build the where condition
                for (x = 0; x <= p_dt.Rows.Count - 1; x++)
                {
                    strField = p_dt.Rows[x]["columnname"].ToString().Trim();
                    if (strValue.Trim().Length == 0)
                    {
                        strValue = strField + "=:" + strField;
                    }
                    else
                    {
                        strValue += " AND " + strField + "=:" + strField;
                    }
                }
                strSQL += " WHERE " + strValue;



                //create an insert command 
                p_da.DeleteCommand = p_conn.CreateCommand();
                //bind the transaction object to the insert command
                p_da.DeleteCommand.Transaction = p_trans;
                p_da.DeleteCommand.CommandText = strSQL;



                //define field datatypes for the data adapter


                for (x = 0; x <= p_dt.Rows.Count - 1; x++)
                {
                    strField = p_dt.Rows[x]["columnname"].ToString().Trim();
                    switch (p_dt.Rows[x]["datatype"].ToString().Trim())
                    {
                        case "System.String":
                            p_da.DeleteCommand.Parameters.Add
                                (strField,
                                System.Data.OracleClient.OracleType.VarChar,
                                0,
                                strField);
                            break;
                        case "System.Double":
                            p_da.DeleteCommand.Parameters.Add
                                (strField,
                                System.Data.OracleClient.OracleType.Double,
                                0,
                                strField);
                            break;
                        //case "System.Boolean":
                        //	p_da.DeleteCommand.Parameters.Add
                        //		(strField, 
                        //		System.Data.OracleClient.OracleDbType.Boolean,
                        //		0,
                        //		strField);
                        //	break;
                        case "System.DateTime":
                            p_da.DeleteCommand.Parameters.Add
                                (strField,
                                System.Data.OracleClient.OracleType.DateTime,
                                0,
                                strField);
                            break;
                        case "System.Decimal":
                            p_da.DeleteCommand.Parameters.Add
                                (strField,
                                System.Data.OracleClient.OracleType.Double,
                                0,
                                strField);
                            break;
                        case "System.Int16":
                            p_da.DeleteCommand.Parameters.Add
                                (strField,
                                System.Data.OracleClient.OracleType.Int16,
                                0,
                                strField);
                            break;
                        case "System.Int32":
                            p_da.DeleteCommand.Parameters.Add
                                (strField,
                                System.Data.OracleClient.OracleType.Int32,
                                0,
                                strField);
                            break;
                        case "System.Int64":
                            p_da.DeleteCommand.Parameters.Add
                                (strField,
                                System.Data.OracleClient.OracleType.Number,
                                0,
                                strField);
                            break;
                        
                        case "System.SByte":
                            p_da.DeleteCommand.Parameters.Add
                                (strField,
                                System.Data.OracleClient.OracleType.SByte,
                                0,
                                strField);
                            break;
                         
                        case "System.Byte":
                            p_da.DeleteCommand.Parameters.Add
                                (strField,
                                System.Data.OracleClient.OracleType.Byte,
                                0,
                                strField);
                            break;
                        case "System.Single":
                            p_da.DeleteCommand.Parameters.Add
                                (strField,
                                System.Data.OracleClient.OracleType.Float,
                                0,
                                strField);
                            break;
                        default:
                            MessageBox.Show("Could Not Set Data Adapter Parameter For DataType " + p_dt.Rows[x]["datatype"].ToString().Trim());
                            break;
                    }

                }



            }
            catch (Exception e)
            {
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:ConfigureDataAdapterUpdateCommand  \n" +
                        "Err Msg - " + e.Message.ToString().Trim(),
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.m_intError = -1;
            }
        }
        /// <summary>
        /// check if a table exists within a particular schema
        /// </summary>
        /// <param name="p_conn"></param>
        /// <param name="p_strOwner">schema that contains the table</param>
        /// <param name="p_strTable"></param>
        /// <returns></returns>
        public bool TableExist(System.Data.OracleClient.OracleConnection p_conn, string p_strOwner, string p_strTable)
        {
            System.Data.OracleClient.OracleDataReader oDataReader; // = new System.Data.OracleClient.OracleDataReader();
            System.Data.OracleClient.OracleCommand oCommand = new System.Data.OracleClient.OracleCommand();
            string strSQL = "SELECT table_name FROM all_tables WHERE TRIM(owner) = '" + p_strOwner.Trim() + "' AND TRIM(table_name) = '" + p_strTable.Trim() + "'";
            oCommand = p_conn.CreateCommand();
            oCommand.CommandText = strSQL;
            try
            {
                oDataReader = oCommand.ExecuteReader();
                if (oDataReader.HasRows)
                {
                    oDataReader.Close();
                    oDataReader = null;
                    oCommand = null;
                    return true;

                }
                oDataReader.Close();

            }
            catch (Exception e)
            {
                if (_bDisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - Oracle:TableExist  \n" +
                        "Err Msg - " + e.Message.ToString().Trim(),
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                this.m_intError = -1;
            }
            oDataReader = null;
            oCommand = null;
            return false;



        }
        public List<long> getInt64List(System.Data.OracleClient.OracleConnection p_Connection, string p_strSQL)
        {
            List<long> intList = new List<long>();
            SqlQueryReader(p_Connection, p_strSQL);
            if (m_intError == 0 && m_DataReader.HasRows)
            {
                while (m_DataReader.Read())
                {
                    if (m_DataReader != null && m_DataReader[0] != DBNull.Value)
                    {
                        intList.Add(Convert.ToInt64(m_DataReader[0]));
                    }
                }
                m_DataReader.Close();
            }
            return intList;
        }
        public List<string> getStringList(System.Data.OracleClient.OracleConnection p_Connection, string p_strSQL)
        {
            List<string> strList = new List<string>();
            SqlQueryReader(p_Connection, p_strSQL);
            if (m_intError == 0 && m_DataReader.HasRows)
            {
                while (m_DataReader.Read())
                {
                    if (m_DataReader != null && m_DataReader[0] != DBNull.Value)
                    {
                        strList.Add(m_DataReader[0].ToString());
                    }
                }
                m_DataReader.Close();
            }
            return strList;
        }
        public void getStringList(System.Data.OracleClient.OracleConnection p_Connection, string p_strSQL, ref List<string> p_strList)
        {
            SqlQueryReader(p_Connection, p_strSQL);
            if (m_intError == 0 && m_DataReader.HasRows)
            {
                while (m_DataReader.Read())
                {
                    if (m_DataReader != null && m_DataReader[0] != DBNull.Value)
                    {
                        p_strList.Add(m_DataReader[0].ToString());
                    }
                }
                m_DataReader.Close();
            }
        }
        public void CloseConnection(System.Data.OracleClient.OracleConnection p_Connection)
        {
            try
            {
                while (p_Connection.State != System.Data.ConnectionState.Closed)
                {
                    p_Connection.Close();
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                m_intError = -1;
            }
        }
        public void CloseAndDisposeConnection(System.Data.OracleClient.OracleConnection p_Connection, bool p_bClearPool)
        {
            try
            {
                if (p_Connection == null) return;

                if (p_Connection.State != ConnectionState.Closed)
                {
                    if (m_DataReader != null) m_DataReader.Dispose();

                    if (m_Command != null) m_Command.Dispose();

                    if (m_DataAdapter != null)
                    {
                        if (m_DataAdapter.SelectCommand != null)
                        {
                            m_DataAdapter.SelectCommand.Dispose();
                        }
                        if (m_DataAdapter.UpdateCommand != null)
                        {
                            m_DataAdapter.UpdateCommand.Dispose();
                        }
                        if (m_DataAdapter.DeleteCommand != null)
                        {
                            m_DataAdapter.DeleteCommand.Dispose();
                        }
                        if (m_DataAdapter.InsertCommand != null)
                        {
                            m_DataAdapter.InsertCommand.Dispose();
                        }
                    }

                }
                while (p_Connection.State != System.Data.ConnectionState.Closed)
                {

                    p_Connection.Close();
                    System.Threading.Thread.Sleep(1000);



                }
                if (p_Connection.State == ConnectionState.Closed)
                {
                    if (p_bClearPool) OracleConnection.ClearPool(p_Connection);
                    p_Connection.Dispose();
                    p_Connection = null;

                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                m_intError = -1;
            }
        }
        public void DisposedEvent(object sender, EventArgs args)
        {
            _bConnectionDisposed = true;
        }
        public bool DisplayErrors
        {
            get { return _bDisplayErrors; }
            set { _bDisplayErrors = value; }
        }
        public string MessageBoxTitle
        {
            get { return _strMsgBoxTitle; }
            set { _strMsgBoxTitle = value; }
        }





    }
    public partial class FCSOracle : Oracle.ADO.DataMgr
    {
        string app_role_list = "";

        private static string _strFCSConnectionString = "";
        public static string FCSConnectionString
        {
            set { _strFCSConnectionString = value; }
            get { return _strFCSConnectionString; }
        }
        private static string _strFCSSchema="";
        public static string FCSSchema
        {
            set { _strFCSSchema = value; }
            get {return _strFCSSchema;}
        }
        
        /// <summary>
        /// Ensure the current logged in user is a member of the NOMS application role. 
        /// </summary>
        /// <param name="p_strUserOrRole">the application role</param>
        /// <param name="p_strPackageSchema">the schema location of the package that assigns the user to the application role</param>
        /// <param name="p_strDatabaseSchema">the application schema</param>
        public void EnsureAppRoleSet(string p_strUserOrRole, string p_strPackageSchema, string p_strDatabaseSchema)
        {
            string roleReset = string.Empty;
            m_strError = string.Empty;
            m_strResult = string.Empty;
            m_intError = 0;

            this.CHK_ENABLED_APP_ROLE_AND_SET(p_strUserOrRole,
                                              p_strPackageSchema,
                                              p_strDatabaseSchema,
                                              app_role_list, ref roleReset, ref m_strError, ref m_strResult);

            if (m_strResult == "FALSE")
            {
                m_strError = "NOMSOracle.EnsureAppRoleSet: App role " + p_strUserOrRole + " was NOT reset\n" + m_strError;
                m_intError = -1;
            }
        }
        /// <summary>
        /// Check to ensure the user is a member of the oracle role associated with NOMS
        /// </summary>
        /// <param name="APP_ROLE">Oracle role used by NOMS. Current possible values: FS_NOMS_SIUD, ANL_PNW_FIA_LCD_SIUD</param>
        /// <param name="PKG_SCHEMA">Oracle schema location of the package that assigns the user to the NOMS role</param>
        /// <param name="APP_SCHEMA">Oracle schema location that houses the NOMS tables</param>
        /// <param name="APP_ROLE_LIST">All the Oracle roles assigned to the current logged in user</param>
        /// <param name="ROLE_RESET">TRUE: success resetting the role assignment to the user. FALSE: failure resetting the role assignment to the user</param>
        /// <param name="ERR">holds the Oracle error value if there is a problem</param>
        /// <param name="P_RESULT">Returns the end result of the oracle package. TRUE: success resetting the role assignment to the user. FALSE: failure resetting the role assignment to the user</param>
        private void CHK_ENABLED_APP_ROLE_AND_SET(string APP_ROLE, string PKG_SCHEMA, string APP_SCHEMA, string APP_ROLE_LIST, ref string ROLE_RESET, ref string ERR, ref string P_RESULT)
        {
            bool needClose = false;
            if (m_Connection.State != System.Data.ConnectionState.Open)
            {
                m_Connection.Open();
                needClose = true;
            }

            try
            {
                using (System.Data.OracleClient.OracleCommand command = new System.Data.OracleClient.OracleCommand())
                {

                    System.Data.OracleClient.OracleParameter oParam;

                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.CommandText = PKG_SCHEMA + ".APP_SEC_PKG.CHK_ENABLED_APP_ROLE_AND_SET";
                    command.Connection = this.m_Connection;
                    //0
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "APP_ROLE";
                    oParam.DbType = System.Data.DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Input;
                    command.Parameters.Add(oParam);
                    if (APP_ROLE != null)
                        command.Parameters[command.Parameters.Count - 1].Value = APP_ROLE;
                    //1
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "APP_SCHEMA";
                    oParam.DbType = System.Data.DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Input;
                    command.Parameters.Add(oParam);
                    if (APP_SCHEMA != null)
                        command.Parameters[command.Parameters.Count - 1].Value = APP_SCHEMA;

                    //2
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "APP_ROLE_LIST";
                    oParam.DbType = System.Data.DbType.String;
                    oParam.Size = 1000;
                    oParam.Direction = ParameterDirection.Input;
                    command.Parameters.Add(oParam);
                    if (APP_ROLE_LIST != null)
                        command.Parameters[command.Parameters.Count - 1].Value = APP_ROLE_LIST;
                    //3
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "ROLE_RESET";
                    oParam.DbType = System.Data.DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(oParam);
                    if (ROLE_RESET != null)
                        command.Parameters[command.Parameters.Count - 1].Value = ROLE_RESET;
                    //4
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "ERR";
                    oParam.DbType = System.Data.DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(oParam);
                    if (ERR != null)
                        command.Parameters[command.Parameters.Count - 1].Value = ERR;

                    //5
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "P_RESULT";
                    oParam.DbType = System.Data.DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(oParam);
                    if (P_RESULT != null)
                        command.Parameters[command.Parameters.Count - 1].Value = P_RESULT;


                    command.ExecuteNonQuery();

                    if (command.Parameters[3].Value != null &&
                        !(command.Parameters[3].Value is System.DBNull))
                        ROLE_RESET = (string)command.Parameters[3].Value;
                    else
                        ROLE_RESET = default(string);

                    if (command.Parameters[4].Value != null &&
                        !(command.Parameters[4].Value is System.DBNull))
                    {
                        ERR = (string)command.Parameters[4].Value;
                        m_intError = -1;
                        m_strError = ERR;
                    }
                    else
                        ERR = default(string);


                    if (command.Parameters[5].Value != null &&
                        !(command.Parameters[5].Value is System.DBNull))
                        P_RESULT = (string)command.Parameters[5].Value;
                    else
                        P_RESULT = default(string);
                }


            }
            catch (Exception e)
            {
                this.m_intError = -1;
                this.m_strError = e.Message;
                if (DisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - NOMSOracle:CHK_ENABLED_APP_ROLE_AND_SET  \n" +
                        "Err Msg - " + e.Message.ToString().Trim(),
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);

            }
            finally
            {
                if (needClose)
                    this.CloseConnection(m_Connection);
            }
        }
        /// <summary>
        /// Set the current logged in user to be a member of the NOMS application role. 
        /// </summary>
        /// <param name="p_strUserOrRole">the application role</param>
        /// <param name="p_strPackageSchema">the schema location of the package that assigns the user to the application role</param>
        /// <param name="p_strDatabaseSchema">the application schema</param>
        public void SetAppRoleForUser(string p_strUserOrRole, string p_strPackageSchema, string p_strDatabaseSchema)
        {

            m_strError = string.Empty;
            m_strResult = string.Empty;
            m_intError = 0;
            app_role_list = string.Empty;

            this.SET_APP_ROLE_FOR_USER(p_strUserOrRole,
                                       p_strPackageSchema,
                                              p_strDatabaseSchema,
                                              app_role_list, ref m_strError, ref m_strResult);

            if (m_strResult == "FALSE")
            {
                m_strError = "ODP.NOMSOracle.SetAppRoleForUser: App role " + p_strUserOrRole + " was NOT granted and set \n" + m_strError;
                m_intError = -1;
            }
        }
        /// <summary>
        /// Set the user to be a member of the oracle role associated with NOMS. 
        /// </summary>
        /// <param name="APP_ROLE">Oracle role used by NOMS. Current possible values: FS_NOMS_SIUD, ANL_PNW_FIA_LCD_SIUD</param>
        /// <param name="PKG_SCHEMA">Oracle schema location of the package that assigns the user to the NOMS role</param>
        /// <param name="APP_SCHEMA">Oracle schema location that houses the NOMS tables</param>
        /// <param name="APP_ROLE_LIST">All the Oracle roles assigned to the current logged in user</param>
        /// <param name="ERR">holds the Oracle error value if there is a problem</param>
        /// <param name="P_RESULT">Returns the end result of the oracle package. TRUE: success setting the role assignment to the user. FALSE: failure setting the role assignment to the user</param>
        private void SET_APP_ROLE_FOR_USER(string APP_ROLE, string PKG_SCHEMA, string APP_SCHEMA, string APP_ROLE_LIST, ref string ERR, ref string P_RESULT)
        {
            
            bool needClose = false;
            if (m_Connection.State != System.Data.ConnectionState.Open)
            {
                m_Connection.Open();
                needClose = true;
            }

            try
            {
                using (System.Data.OracleClient.OracleCommand command = new System.Data.OracleClient.OracleCommand())
                {
                    System.Data.OracleClient.OracleParameter oParam;

                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.CommandText = PKG_SCHEMA + ".APP_SEC_PKG.SET_APP_ROLE_FOR_USER";
                    command.Connection = this.m_Connection;
                    //0
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "APP_ROLE";
                    oParam.DbType = DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Input;
                    command.Parameters.Add(oParam);
                    if (APP_ROLE != null)
                        command.Parameters[command.Parameters.Count - 1].Value = APP_ROLE;
                    //1
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "APP_SCHEMA";
                    oParam.DbType = DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Input;
                    command.Parameters.Add(oParam);
                    if (APP_SCHEMA != null)
                        command.Parameters[command.Parameters.Count - 1].Value = APP_SCHEMA;

                    //2
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "APP_ROLE_LIST";
                    oParam.DbType = DbType.String;
                    oParam.Size = 1000;
                    oParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(oParam);
                    if (APP_ROLE_LIST != null)
                        command.Parameters[command.Parameters.Count - 1].Value = APP_ROLE_LIST;
                    //3
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "ERR";
                    oParam.DbType = DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(oParam);
                    if (ERR != null)
                        command.Parameters[command.Parameters.Count - 1].Value = ERR;

                    //4
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "P_RESULT";
                    oParam.DbType = DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(oParam);
                    if (P_RESULT != null)
                        command.Parameters[command.Parameters.Count - 1].Value = P_RESULT;


                    command.ExecuteNonQuery();

                    if (command.Parameters[2].Value != null &&
                       !(command.Parameters[2].Value is System.DBNull))
                    {
                        app_role_list = (string)command.Parameters[2].Value;
                    }
                    else
                        app_role_list = default(string);

                    if (command.Parameters[3].Value != null &&
                        !(command.Parameters[3].Value is System.DBNull))
                    {
                        ERR = (string)command.Parameters[3].Value;
                        m_strError = ERR;
                        m_intError = -1;
                    }
                    else
                        ERR = default(string);


                    if (command.Parameters[4].Value != null &&
                        !(command.Parameters[4].Value is System.DBNull))
                    {
                        P_RESULT = (string)command.Parameters[4].Value;
                    }
                    else
                        P_RESULT = default(string);
                }


            }
            catch (Exception e)
            {
                this.m_intError = -1;
                this.m_strError = e.Message;
                if (DisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - NOMSOracle:SET_APP_ROLE_FOR_USER  \n" +
                        "Err Msg - " + e.Message.ToString().Trim(),
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);

            }
            finally
            {
                if (needClose)
                    this.CloseConnection(m_Connection);
            }
        }

        /// <summary>
        /// Set the current logged in user to be a member of the NOMS application role. 
        /// </summary>
        /// <param name="p_strUserOrRole">the application role</param>
        /// <param name="p_strPackageSchema">the schema location of the package that assigns the user to the application role</param>
        /// <param name="p_strDatabaseSchema">the application schema</param>
        public void RemoveAppRoleFromUser(string p_strAppRole, string p_strPackageSchema)
        {

            m_strError = string.Empty;
            m_strResult = string.Empty;
            m_intError = 0;

            this.REMOVE_APP_ROLE_FROM_USER(p_strAppRole, p_strPackageSchema, ref m_strError, ref m_strResult);

            if (m_strResult == "FALSE")
            {
                m_strError = "ODP.NOMSOracle.RemoveAppRoleForUser: Remove App Role was NOT successful \n" + m_strError;
            }
        }
        /// <summary>
        /// Set the user to be a member of the oracle role associated with NOMS. 
        /// </summary>
        /// <param name="ERR">holds the Oracle error value if there is a problem</param>
        /// <param name="P_RESULT">Returns the end result of the oracle package. TRUE: success setting the role assignment to the user. FALSE: failure setting the role assignment to the user</param>
        private void REMOVE_APP_ROLE_FROM_USER(string APP_ROLE, string PKG_SCHEMA, ref string ERR, ref string P_RESULT)
        {
            bool needClose = false;
            if (m_Connection.State != System.Data.ConnectionState.Open)
            {
                m_Connection.Open();
                needClose = true;
            }

            try
            {
                using (System.Data.OracleClient.OracleCommand command = new System.Data.OracleClient.OracleCommand())
                {

                    System.Data.OracleClient.OracleParameter oParam;

                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.CommandText = PKG_SCHEMA + ".APP_SEC_PKG.REMOVE_APP_ROLE_FROM_USER";
                    command.Connection = this.m_Connection;
                    //0
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "APP_ROLE";
                    oParam.DbType = DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Input;
                    command.Parameters.Add(oParam);
                    if (APP_ROLE != null)
                        command.Parameters[command.Parameters.Count - 1].Value = APP_ROLE;
                    //1
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "ERR";
                    oParam.DbType = DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(oParam);
                    if (ERR != null)
                        command.Parameters[command.Parameters.Count - 1].Value = ERR;

                    //2
                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "P_RESULT";
                    oParam.DbType = DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(oParam);
                    if (P_RESULT != null)
                        command.Parameters[command.Parameters.Count - 1].Value = P_RESULT;


                    command.ExecuteNonQuery();

                    if (command.Parameters[1].Value != null &&
                        !(command.Parameters[1].Value is System.DBNull))
                    {
                        ERR = (string)command.Parameters[1].Value;
                        m_strError = ERR;
                        m_intError = -1;
                    }
                    else
                        ERR = default(string);


                    if (command.Parameters[2].Value != null &&
                        !(command.Parameters[2].Value is System.DBNull))
                    {
                        P_RESULT = (string)command.Parameters[2].Value;
                    }
                    else
                        P_RESULT = default(string);
                }


            }
            catch (Exception e)
            {
                this.m_intError = -1;
                this.m_strError = e.Message;
                if (DisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - NOMSOracle:REMOVE_APP_ROLE_FROM_USER  \n" +
                        "Err Msg - " + e.Message.ToString().Trim(),
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);

            }
            finally
            {
                if (needClose)
                    this.CloseConnection(m_Connection);
            }
        }
        /// <summary>
        /// There are no comments for GETCN_ID in the schema.
        /// </summary>
        public void GETCN_ID(ref string P_STRNEWCN,string PKG_SCHEMA)
        {
            bool needClose = false;
            if (this.m_Connection.State != System.Data.ConnectionState.Open)
            {
                this.m_Connection.Open();
                needClose = true;
            }

            try
            {
                using (System.Data.OracleClient.OracleCommand command = new System.Data.OracleClient.OracleCommand())
                {
                    System.Data.OracleClient.OracleParameter oParam;
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = PKG_SCHEMA + ".GETCN_PKG.GETCN_ID";
                    command.Connection = this.m_Connection;

                    oParam = new System.Data.OracleClient.OracleParameter();
                    oParam.ParameterName = "P_STRNEWCN";
                    oParam.DbType = DbType.String;
                    oParam.Size = 255;
                    oParam.Direction = ParameterDirection.Output;
                    if (P_STRNEWCN != null)
                        oParam.Value = P_STRNEWCN;
                    command.Parameters.Add(oParam);
                    
                    
                    command.ExecuteNonQuery();

                    if (oParam.Value != null && !(oParam.Value is System.DBNull))
                        P_STRNEWCN = (string)oParam.Value;
                    else
                        P_STRNEWCN = default(string);
                }
            }
            finally
            {
                if (needClose)
                    this.m_Connection.Close();
            }
        }

       

    }
    /*
    public partial class NOMSOracle : ODP.OracleClient
    {
        string app_role_list = "";

        /// <summary>
        /// Ensure the current logged in user is a member of the NOMS application role. 
        /// </summary>
        /// <param name="p_strUserOrRole">the application role</param>
        /// <param name="p_strPackageSchema">the schema location of the package that assigns the user to the application role</param>
        /// <param name="p_strDatabaseSchema">the application schema</param>
        public void EnsureAppRoleSet(string p_strUserOrRole,string p_strPackageSchema, string p_strDatabaseSchema)
        {
            string roleReset = string.Empty;
            m_strError = string.Empty;
            m_strResult = string.Empty;
            m_intError = 0;
            
            this.CHK_ENABLED_APP_ROLE_AND_SET(p_strUserOrRole,
                                              p_strPackageSchema,
                                              p_strDatabaseSchema,
                                              app_role_list, ref roleReset, ref m_strError, ref m_strResult); 

            if (m_strResult == "FALSE")
            {
                m_strError = "App role " + p_strUserOrRole + " was NOT reset\n" + m_strError;
                throw new System.ApplicationException(m_strError);
            }
        }
       /// <summary>
       /// Check to ensure the user is a member of the oracle role associated with NOMS
       /// </summary>
       /// <param name="APP_ROLE">Oracle role used by NOMS. Current possible values: FS_NOMS_SIUD, ANL_PNW_FIA_LCD_SIUD</param>
       /// <param name="PKG_SCHEMA">Oracle schema location of the package that assigns the user to the NOMS role</param>
       /// <param name="APP_SCHEMA">Oracle schema location that houses the NOMS tables</param>
       /// <param name="APP_ROLE_LIST">All the Oracle roles assigned to the current logged in user</param>
       /// <param name="ROLE_RESET">TRUE: success resetting the role assignment to the user. FALSE: failure resetting the role assignment to the user</param>
       /// <param name="ERR">holds the Oracle error value if there is a problem</param>
        /// <param name="P_RESULT">Returns the end result of the oracle package. TRUE: success resetting the role assignment to the user. FALSE: failure resetting the role assignment to the user</param>
        public void CHK_ENABLED_APP_ROLE_AND_SET(string APP_ROLE, string PKG_SCHEMA, string APP_SCHEMA, string APP_ROLE_LIST, ref string ROLE_RESET, ref string ERR, ref string P_RESULT)
        {
            bool needClose = false;
            if (m_Connection.State != System.Data.ConnectionState.Open)
            {
                m_Connection.Open();
                needClose = true;
            }

            try
            {
                System.Data.OracleClient.OracleCommand command = new System.Data.OracleClient.OracleCommand();
                command = m_Connection.CreateCommand();
                System.Data.OracleClient.OracleParameter oParam;
               
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.CommandText = PKG_SCHEMA + ".LCD_APP_SEC_PKG.CHK_ENABLED_APP_ROLE_AND_SET";
                command.Connection = this.m_Connection;
                //0
                oParam = new System.Data.OracleClient.OracleParameter();
                oParam.ParameterName = "APP_ROLE";
                oParam.DbType = System.Data.DbType.String;
                oParam.Size = 255;
                oParam.Direction = ParameterDirection.Input;
                command.Parameters.Add(oParam);
                if (APP_ROLE != null)
                    command.Parameters[command.Parameters.Count - 1].Value = APP_ROLE;
                //1
                oParam = new System.Data.OracleClient.OracleParameter();
                oParam.ParameterName = "APP_SCHEMA";
                oParam.DbType = System.Data.DbType.String;
                oParam.Size = 255;
                oParam.Direction = ParameterDirection.Input;
                command.Parameters.Add(oParam);
                if (APP_SCHEMA != null)
                    command.Parameters[command.Parameters.Count - 1].Value = APP_SCHEMA;

                //2
                oParam = new System.Data.OracleClient.OracleParameter();
                oParam.ParameterName = "APP_ROLE_LIST";
                oParam.DbType = System.Data.DbType.String;
                oParam.Size = 500;
                oParam.Direction = ParameterDirection.Input;
                command.Parameters.Add(oParam);
                if (APP_ROLE_LIST != null)
                    command.Parameters[command.Parameters.Count - 1].Value = APP_ROLE_LIST;
                //3
                oParam = new System.Data.OracleClient.OracleParameter();
                oParam.ParameterName = "ROLE_RESET";
                oParam.DbType = System.Data.DbType.String;
                oParam.Size = 255;
                oParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(oParam);
                if (ROLE_RESET != null)
                    command.Parameters[command.Parameters.Count - 1].Value = ROLE_RESET;
                //4
                oParam = new System.Data.OracleClient.OracleParameter();
                oParam.ParameterName = "ERR";
                oParam.DbType = System.Data.DbType.String;
                oParam.Size = 255;
                oParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(oParam);
                if (ERR != null)
                    command.Parameters[command.Parameters.Count - 1].Value = ERR;

                //5
                oParam = new System.Data.OracleClient.OracleParameter();
                oParam.ParameterName = "P_RESULT";
                oParam.DbType = System.Data.DbType.String;
                oParam.Size = 255;
                oParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(oParam);
                if (P_RESULT != null)
                    command.Parameters[command.Parameters.Count - 1].Value = P_RESULT;


                command.ExecuteNonQuery();

                if (command.Parameters[3].Value != null &&
                    !(command.Parameters[3].Value is System.DBNull))
                    ROLE_RESET = (string)command.Parameters[3].Value;
                else
                    ROLE_RESET = default(string);

                if (command.Parameters[4].Value != null &&
                    !(command.Parameters[4].Value is System.DBNull))
                {
                    ERR = (string)command.Parameters[4].Value;
                    m_intError = -1;
                    m_strError = ERR;
                }
                else
                    ERR = default(string);


                if (command.Parameters[5].Value != null &&
                    !(command.Parameters[5].Value is System.DBNull))
                    P_RESULT = (string)command.Parameters[5].Value;
                else
                    P_RESULT = default(string);


            }
            catch (Exception e)
            {
                this.m_intError = -1;
                this.m_strError = e.Message;
                if (DisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - NOMSOracle:CHK_ENABLED_APP_ROLE_AND_SET  \n" +
                        "Err Msg - " + e.Message.ToString().Trim(),
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);
                
            }
            finally
            {
                if (needClose)
                    this.CloseConnection(m_Connection);
            }
        }
        /// <summary>
        /// Set the current logged in user to be a member of the NOMS application role. 
        /// </summary>
        /// <param name="p_strUserOrRole">the application role</param>
        /// <param name="p_strPackageSchema">the schema location of the package that assigns the user to the application role</param>
        /// <param name="p_strDatabaseSchema">the application schema</param>
        public void SetAppRoleForUser(string p_strUserOrRole, string p_strPackageSchema, string p_strDatabaseSchema)
        {
           
            m_strError = string.Empty;
            m_strResult = string.Empty;
            m_intError = 0;
           
            this.SET_APP_ROLE_FOR_USER(p_strUserOrRole,
                                              p_strPackageSchema,
                                              p_strDatabaseSchema,
                                              app_role_list, ref m_strError, ref m_strResult); 

            if (m_strResult == "FALSE")
            {
                m_strError = "App role " + p_strUserOrRole + " was NOT granted and set \n" + m_strError;
                throw new System.ApplicationException(m_strError);
            }
        }
        /// <summary>
        /// Set the user to be a member of the oracle role associated with NOMS. 
        /// </summary>
        /// <param name="APP_ROLE">Oracle role used by NOMS. Current possible values: FS_NOMS_SIUD, ANL_PNW_FIA_LCD_SIUD</param>
        /// <param name="PKG_SCHEMA">Oracle schema location of the package that assigns the user to the NOMS role</param>
        /// <param name="APP_SCHEMA">Oracle schema location that houses the NOMS tables</param>
        /// <param name="APP_ROLE_LIST">All the Oracle roles assigned to the current logged in user</param>
        /// <param name="ERR">holds the Oracle error value if there is a problem</param>
        /// <param name="P_RESULT">Returns the end result of the oracle package. TRUE: success setting the role assignment to the user. FALSE: failure setting the role assignment to the user</param>
        public void SET_APP_ROLE_FOR_USER(string APP_ROLE, string PKG_SCHEMA, string APP_SCHEMA, string APP_ROLE_LIST, ref string ERR, ref string P_RESULT)
        {
            bool needClose = false;
            if (m_Connection.State != System.Data.ConnectionState.Open)
            {
                m_Connection.Open();
                needClose = true;
            }

            try
            {
                System.Data.OracleClient.OracleCommand command = new System.Data.OracleClient.OracleCommand();
                command = m_Connection.CreateCommand();
                System.Data.OracleClient.OracleParameter oParam;
                
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.CommandText = PKG_SCHEMA + ".LCD_APP_SEC_PKG.SET_APP_ROLE_FOR_USER";
                command.Connection = this.m_Connection;
                //0
                oParam = new System.Data.OracleClient.OracleParameter();
                oParam.ParameterName = "APP_ROLE";
                oParam.DbType = DbType.String;
                oParam.Size = 255;
                oParam.Direction = ParameterDirection.Input;
                command.Parameters.Add(oParam);
                if (APP_ROLE != null)
                    command.Parameters[command.Parameters.Count - 1].Value = APP_ROLE;
                //1
                oParam = new System.Data.OracleClient.OracleParameter();
                oParam.ParameterName = "APP_SCHEMA";
                oParam.DbType = DbType.String;
                oParam.Size = 255;
                oParam.Direction = ParameterDirection.Input;
                command.Parameters.Add(oParam);
                if (APP_SCHEMA != null)
                    command.Parameters[command.Parameters.Count - 1].Value = APP_SCHEMA;

                //2
                oParam = new System.Data.OracleClient.OracleParameter();
                oParam.ParameterName = "APP_ROLE_LIST";
                oParam.DbType = DbType.String;
                oParam.Size = 500;
                oParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(oParam);
                if (APP_ROLE_LIST != null)
                    command.Parameters[command.Parameters.Count - 1].Value = APP_ROLE_LIST;
                //3
                oParam = new System.Data.OracleClient.OracleParameter();
                oParam.ParameterName = "ERR";
                oParam.DbType = DbType.String;
                oParam.Size = 255;
                oParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(oParam);
                if (ERR != null)
                    command.Parameters[command.Parameters.Count - 1].Value = ERR;

                //4
                oParam = new System.Data.OracleClient.OracleParameter();
                oParam.ParameterName = "P_RESULT";
                oParam.DbType = DbType.String;
                oParam.Size = 255;
                oParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(oParam);
                if (P_RESULT != null)
                    command.Parameters[command.Parameters.Count - 1].Value = P_RESULT;


                command.ExecuteNonQuery();

                if (command.Parameters[3].Value != null &&
                    !(command.Parameters[3].Value is System.DBNull))
                {
                    ERR = (string)command.Parameters[3].Value;
                    m_strError = ERR;
                    m_intError = -1;
                }
                else
                    ERR = default(string);


                if (command.Parameters[4].Value != null &&
                    !(command.Parameters[4].Value is System.DBNull))
                {
                    P_RESULT = (string)command.Parameters[4].Value;
                }
                else
                    P_RESULT = default(string);


            }
            catch (Exception e)
            {
                this.m_intError = -1;
                this.m_strError = e.Message;
                if (DisplayErrors)
                    MessageBox.Show("!!Error!! \n" +
                        "Module - NOMSOracle:SET_APP_ROLE_FOR_USER  \n" +
                        "Err Msg - " + e.Message.ToString().Trim(),
                        "BIOSUM", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Exclamation);

            }
            finally
            {
                if (needClose)
                    this.CloseConnection(m_Connection);
            }
        }

    }
     */


}
