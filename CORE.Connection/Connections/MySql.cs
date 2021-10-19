using CORE.Connection.Interfaces;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Connection.Connections
{
    internal class MySql<T>: IConnectionDB<T>
    {
        #region Constructor estático y variables globales
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private MySqlConnection _clsMySqlConnection = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]

        bool _blnConectado = false;
        bool _blnPreparado = false;
        string _nombreProcedimiento = string.Empty;
        DynamicParameters _dynParameters;
        CommandType _commandType;
        int _timeOut = 12000;

        private MySql()
        {

        }


        public static MySql<T> Conectar(string strConnectionString)
        {
            MySql<T> modSql = new MySql<T>()
            {
                _clsMySqlConnection = new MySqlConnection(strConnectionString)
            };

            try
            {
                modSql._clsMySqlConnection.Open();
                modSql._blnConectado = true;
            }
            catch (MySqlException sqlEx)
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

        public void PrepararProcedimiento(string nombreProcedimiento, DynamicParameters dynParameters, CommandType enuTipoComando = CommandType.StoredProcedure)
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
                return _clsMySqlConnection.Execute(_nombreProcedimiento, _dynParameters, null, _timeOut, _commandType);
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
                return _clsMySqlConnection.QueryFirstOrDefault<T>(_nombreProcedimiento, _dynParameters, null, _timeOut, _commandType);
            }
            else
            {
                _blnPreparado = false;
                throw new Exception("Procedimiento no preparado");
            }
        }
        public object QueryFirstOrDefaultDapper(Models.TipoDato tipo)
        {
            if (_blnPreparado)
            {
                _blnPreparado = false;
                return tipo == Models.TipoDato.Numerico ? _clsMySqlConnection.QueryFirstOrDefault<long>(_nombreProcedimiento, _dynParameters, null, _timeOut, _commandType) : _clsMySqlConnection.QueryFirstOrDefault<string>(_nombreProcedimiento, _dynParameters, null, _timeOut, _commandType);
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
                return _clsMySqlConnection.Query<T>(_nombreProcedimiento, _dynParameters, null, true, _timeOut, _commandType);
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
                _clsMySqlConnection.Dispose();
                _dynParameters = null;
                _blnPreparado = false;
            }
            catch { }
        }
        public void Desconectar()
        {
            _clsMySqlConnection.Close();
        }



        #endregion

    }
}
