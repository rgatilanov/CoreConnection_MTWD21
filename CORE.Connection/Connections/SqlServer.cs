using CORE.Connection.Interfaces;
using Dapper;
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

        bool _blnConectado = false;
        bool _blnPreparado = false;
        string _nombreProcedimiento = string.Empty;
        List<DynamicParameters> _dynParameters;
        CommandType _commandType;
        int _timeOut = 12000;

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
            catch (SqlException sqlEx)
            {
                throw sqlEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return modSql;
        }
        #endregion


        #region Métodos públicos

        public void PrepararProcedimiento(string nombreProcedimiento, List<DynamicParameters> dynParameters, CommandType enuTipoComando = CommandType.StoredProcedure)
        {
            if (_blnConectado)
            {
                _nombreProcedimiento = nombreProcedimiento;
                _dynParameters = dynParameters;
                _commandType = enuTipoComando;
                _blnPreparado = true;
            }
            else
            {
                throw new Exception("No hay conexion con la bd");
            }
        }

        public long ExecuteDapper()
        {
            if (_blnPreparado)
            {
                _blnPreparado = false;
                return _clsSqlConnection.Execute(_nombreProcedimiento, _dynParameters,null, _timeOut, _commandType);
            }
            else
            {
                _blnPreparado = false;
                throw new Exception("Procedimiento no preparado");
            }
        }

        public T QueryFirstOrDefaultDapper()
        {
            if (_blnPreparado)
            {
                _blnPreparado = false;
                return _clsSqlConnection.QueryFirstOrDefault<T>(_nombreProcedimiento, _dynParameters, null, _timeOut, _commandType);
            }
            else
            {
                _blnPreparado = false;
                throw new Exception("Procedimiento no preparado");
            }
        }
        public IEnumerable<T> Query()
        {
            if (_blnPreparado)
            {
                _blnPreparado = false;
                return _clsSqlConnection.Query<T>(_nombreProcedimiento, _dynParameters, null,true, _timeOut, _commandType);
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
                _dynParameters = null;
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
