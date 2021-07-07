using CORE.Connection.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Connection.Connections
{
    internal class SqlServer<T> : IConnectionDB<T>
    {
        #region Constructor estático y variables globales
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SqlConnection _clsSqlConnection = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SqlCommand _clsSqlCommand = null;

        bool _blnConectado = false;
        bool _blnPreparado = false;


        private SqlServer()
        {

        }


        public static SqlServer<T> Conectar(string strConnectionString)
        {
            SqlServer<T> modSql = new SqlServer<T>()
            {
                _clsSqlConnection = new SqlConnection(strConnectionString)
            };

            try
            {
                modSql._clsSqlConnection.Open();
                modSql._blnConectado = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return modSql;
        }
        #endregion


        #region Métodos públicos

        public void PrepararProcedimiento(string strNombreProcedimiento, List<Tuple<string, object, int>> tplParametros, CommandType enuTipoComando = CommandType.StoredProcedure)
        {
            if (_blnConectado)
            {
                _clsSqlCommand = new SqlCommand(strNombreProcedimiento, _clsSqlConnection)
                {
                    CommandTimeout = 0,
                    CommandType = enuTipoComando
                };

                List<SqlParameter> lstParametros = new List<SqlParameter>();
                tplParametros.ForEach(delegate (Tuple<string, object, int> parametro)
                {
                    SqlParameter sqlparameter = new SqlParameter(parametro.Item1, parametro.Item2)
                    {
                        SqlDbType = (SqlDbType)parametro.Item3
                    };
                    lstParametros.Add(sqlparameter);
                });

                _clsSqlCommand.Parameters.AddRange(lstParametros.ToArray());

                _blnPreparado = true;
            }
            else
            {
                throw new Exception("No hay conexion con la bd");
            }
        }

        public int EjecutarProcedimiento()
        {
            if (_blnPreparado)
            {
                _blnPreparado = false;
                return _clsSqlCommand.ExecuteNonQuery();

            }
            else
            {
                _blnPreparado = false;
                throw new Exception("Procedimiento no preparado");
            }
        }

        public object EjecutarScalar()
        {
            if (_blnPreparado)
            {
                _blnPreparado = false;
                return _clsSqlCommand.ExecuteScalar();
            }
            else
            {
                _blnPreparado = false;
                throw new Exception("Procedimiento no preparado");
            }
        }

        public DataTableReader EjecutarTableReader()
        {
            if (_blnPreparado)
            {
                _blnPreparado = false;
                DataTable clsDataTable = new DataTable();
                SqlDataAdapter clsDataAdapter = new SqlDataAdapter(_clsSqlCommand);
                clsDataAdapter.Fill(clsDataTable);
                return clsDataTable.CreateDataReader();
            }
            else
            {
                _blnPreparado = false;
                throw new Exception("Procedimiento no preparado");
            }
        }

        public DataTable EjecutarTable()
        {
            if (_blnPreparado)
            {
                _blnPreparado = false;
                DataTable clsDataTable = new DataTable();
                SqlDataAdapter clsDataAdapter = new SqlDataAdapter(_clsSqlCommand);
                clsDataAdapter.Fill(clsDataTable);
                return clsDataTable.Copy();
            }
            else
            {
                _blnPreparado = false;
                throw new Exception("Procedimiento no preparado");
            }
        }
        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                Desconectar();
                _clsSqlConnection.Dispose();
                if (_clsSqlCommand != null) _clsSqlCommand.Dispose();
                _blnPreparado = false;
            }
            catch { }
        }
        public void Desconectar()
        {
            _clsSqlConnection.Close();
        }



        #endregion
    }
}
