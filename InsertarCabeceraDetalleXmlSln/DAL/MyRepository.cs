using InsertarCabeceraDetalleXml.Common;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace InsertarCabeceraDetalleXml.DAL
{
    public class MyRepository : DataServiceBase
    {
        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///	Creates a new DataService
        /// </summary>
        ////////////////////////////////////////////////////////////////////////
        public MyRepository() : base() { }

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///	Creates a new DataService and specifies a transaction with
        ///	which to operate
        /// </summary>
        ////////////////////////////////////////////////////////////////////////
        public MyRepository(IDbTransaction txn) : base(txn) { }

        public string Insert(Order order)
        {
            //string xmlTemp = "<Order><Id>0</Id><Fecha>20/12/2016</Fecha><Details>	<OrderDetail>		<Id>0</Id>		<ProductId>1000</ProductId>		<IdCabecera>0</IdCabecera>	</OrderDetail>	<OrderDetail>		<Id>0</Id>		<ProductId>6000</ProductId>		<IdCabecera>0</IdCabecera>	</OrderDetail>	<OrderDetail>		<Id>0</Id>		<ProductId>3000</ProductId>		<IdCabecera>0</IdCabecera>	</OrderDetail></Details></Order>";
            string xmlTemp = Helpers.CreateXML(order);
            OracleCommand cmd;
            ExecuteNonQuery(out cmd,
                "PackageTest2.INS_CABECERADETALLE",
                CreateParameter("P_STRXML", OracleDbType.Clob, xmlTemp),
                CreateParameter("P_TRANSACCION_ID", OracleDbType.Int32, ParameterDirection.Output)
                );

            cmd.Dispose();
            return cmd.Parameters["P_TRANSACCION_ID"].Value.ToString();    
        }
    }
}