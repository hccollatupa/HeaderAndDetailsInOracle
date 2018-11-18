using InsertarCabeceraDetalleXml.Common;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data;

namespace InsertarCabeceraDetalleXml.DAL
{
    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///   Defines common DataService routines for transaction management, 
    ///   stored procedure execution, parameter creation, and null value 
    ///   checking
    /// </summary>	
    ////////////////////////////////////////////////////////////////////////////
    public class DataServiceBase
    {

        ////////////////////////////////////////////////////////////////////////
        // Fields
        ////////////////////////////////////////////////////////////////////////
        private bool _isOwner = false;   //True if service owns the transaction        
        private OracleTransaction _txn;     //Reference to the current transaction


        ////////////////////////////////////////////////////////////////////////
        // Properties 
        ////////////////////////////////////////////////////////////////////////
        public IDbTransaction Txn
        {
            get { return (IDbTransaction)_txn; }
            set { _txn = (OracleTransaction)value; }
        }


        ////////////////////////////////////////////////////////////////////////
        // Constructors
        ////////////////////////////////////////////////////////////////////////

        public DataServiceBase() : this(null) { }


        public DataServiceBase(IDbTransaction txn)
        {
            if (txn == null)
            {
                _isOwner = true;
            }
            else
            {
                _txn = (OracleTransaction)txn;
                _isOwner = false;
            }
        }


        ////////////////////////////////////////////////////////////////////////
        // Connection and Transaction Methods
        ////////////////////////////////////////////////////////////////////////
        protected static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["MyContext"].ConnectionString;
        }


        public static IDbTransaction BeginTransaction()
        {
            OracleConnection txnConnection =
                new OracleConnection(GetConnectionString());
            txnConnection.Open();
            return txnConnection.BeginTransaction();
        }


        ////////////////////////////////////////////////////////////////////////
        // ExecuteDataSet Methods
        ////////////////////////////////////////////////////////////////////////
        protected DataSet ExecuteDataSet(string procName,
            params IDataParameter[] procParams)
        {
            OracleCommand cmd;
            return ExecuteDataSet(out cmd, procName, procParams);
        }


        protected DataSet ExecuteDataSet(out OracleCommand cmd, string procName,
            params IDataParameter[] procParams)
        {
            OracleConnection cnx = null;
            DataSet ds = new DataSet();
            OracleDataAdapter da = new OracleDataAdapter();
            cmd = null;

            try
            {
                //Setup command object
                cmd = new OracleCommand(procName);
                cmd.CommandType = CommandType.StoredProcedure;
                if (procParams != null)
                {
                    for (int index = 0; index < procParams.Length; index++)
                    {
                        cmd.Parameters.Add(procParams[index]);
                    }
                }
                da.SelectCommand = (OracleCommand)cmd;

                //Determine the transaction owner and process accordingly
                if (_isOwner)
                {
                    cnx = new OracleConnection(GetConnectionString());
                    cmd.Connection = cnx;
                    cnx.Open();
                }
                else
                {
                    cmd.Connection = _txn.Connection;
                    cmd.Transaction = _txn;
                }

                //Fill the dataset
                da.Fill(ds);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (da != null) da.Dispose();
                if (cmd != null) cmd.Dispose();
                if (_isOwner)
                {
                    cnx.Dispose(); //Implicitly calls cnx.Close()
                }
            }
            return ds;
        }


        ////////////////////////////////////////////////////////////////////////
        // ExecuteNonQuery Methods
        ////////////////////////////////////////////////////////////////////////
        protected void ExecuteNonQuery(string procName,
            params IDataParameter[] procParams)
        {
            OracleCommand cmd;
            ExecuteNonQuery(out cmd, procName, procParams);
        }


        protected void ExecuteNonQuery(out OracleCommand cmd, string procName,
            params IDataParameter[] procParams)
        {
            //Method variables
            OracleConnection cnx = null;
            cmd = null;  //Avoids "Use of unassigned variable" compiler error

            try
            {
                //Setup command object
                cmd = new OracleCommand(procName);
                cmd.CommandType = CommandType.StoredProcedure;
                for (int index = 0; index < procParams.Length; index++)
                {
                    cmd.Parameters.Add(procParams[index]);
                }

                //Determine the transaction owner and process accordingly
                if (_isOwner)
                {
                    cnx = new OracleConnection(GetConnectionString());
                    cmd.Connection = cnx;
                    cnx.Open();
                }
                else
                {
                    cmd.Connection = _txn.Connection;
                    cmd.Transaction = _txn;
                }

                //Execute the command
                cmd.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_isOwner)
                {
                    cnx.Dispose(); //Implicitly calls cnx.Close()
                }
                if (cmd != null) cmd.Dispose();
            }
        }


        ////////////////////////////////////////////////////////////////////////
        // CreateParameter Methods
        ////////////////////////////////////////////////////////////////////////
        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue)
        {
            OracleParameter param = new OracleParameter(paramName, paramType);

            if (paramValue != DBNull.Value)
            {
                switch (paramType)
                {
                    case OracleDbType.Varchar2:
                    case OracleDbType.NVarchar2:
                        break;
                    case OracleDbType.Char:
                    case OracleDbType.NChar:
                    case OracleDbType.Date:
                        paramValue = CheckParamValue((DateTime)paramValue);
                        break;
                    case OracleDbType.Int16:
                        paramValue = CheckParamValue((Int16)paramValue);
                        break;
                    case OracleDbType.Int32:
                        paramValue = CheckParamValue((Int32)paramValue);
                        break;
                    case OracleDbType.Byte:
                        if (paramValue is bool) paramValue = (int)((bool)paramValue ? 1 : 0);
                        if ((int)paramValue < 0 || (int)paramValue > 1) paramValue = Constants.NullInt;
                        paramValue = CheckParamValue((int)paramValue);
                        break;
                    case OracleDbType.Single:
                        paramValue = CheckParamValue(Convert.ToSingle(paramValue));
                        break;
                    case OracleDbType.Decimal:
                        paramValue = CheckParamValue((decimal)paramValue);
                        break;
                    case OracleDbType.Double:
                        paramValue = CheckParamValue((double)paramValue);
                        break;
                }
            }
            param.Value = paramValue;
            return param;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, ParameterDirection direction)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, DBNull.Value);
            returnVal.Direction = direction;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue, ParameterDirection direction)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, paramValue);
            returnVal.Direction = direction;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue, int size)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, paramValue);
            returnVal.Size = size;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue, int size, ParameterDirection direction)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, paramValue);
            returnVal.Direction = direction;
            returnVal.Size = size;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue, int size, byte precision)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, paramValue);
            returnVal.Size = size;
            ((OracleParameter)returnVal).Precision = precision;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue, int size, byte precision, ParameterDirection direction)
        {
            OracleParameter returnVal = CreateParameter(paramName, paramType, paramValue);
            returnVal.Direction = direction;
            returnVal.Size = size;
            returnVal.Precision = precision;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleCollectionType CollectionType, object paramCollectionValue, ParameterDirection direction)
        {
            OracleParameter param = new OracleParameter(paramName, CollectionType);

            //OracleParameter returnVal = CreateParameter(paramName, CollectionType, paramCollectionTypeValue);

            param.Value = paramCollectionValue;
            return param;
        }


        ////////////////////////////////////////////////////////////////////////
        // CheckParamValue Methods
        ////////////////////////////////////////////////////////////////////////
        protected Guid GetGuid(object value)
        {
            Guid returnVal = Constants.NullGuid;
            if (value is string)
            {
                returnVal = new Guid((string)value);
            }
            else if (value is Guid)
            {
                returnVal = (Guid)value;
            }
            return returnVal;
        }

        protected object CheckParamValue(string paramValue)
        {
            if (string.IsNullOrEmpty(paramValue))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(Guid paramValue)
        {
            if (paramValue.Equals(Constants.NullGuid))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(DateTime paramValue)
        {
            if (paramValue.Equals(Constants.NullDateTime))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(double paramValue)
        {
            if (paramValue.Equals(Constants.NullDouble))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(float paramValue)
        {
            if (paramValue.Equals(Constants.NullFloat))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(Decimal paramValue)
        {
            if (paramValue.Equals(Constants.NullDecimal))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        protected object CheckParamValue(int paramValue)
        {
            if (paramValue.Equals(Constants.NullInt))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

    } //class
}