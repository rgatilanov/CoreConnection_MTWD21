using CORE.Connection.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Npgsql;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace CORE.Connection.Connections
{
    internal class PostgreSQL<T> : IConnectionDB<T>
    {
        #region Constructor estático y variables globales
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private NpgsqlConnection _clsPostgreSqlConnection = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]

        bool _blnConectado = false;
        bool _blnPreparado = false;
        string _nombreProcedimiento = string.Empty;
        DynamicParameters _dynParameters;
        CommandType _commandType;
        int _timeOut = 12000;


        public static PostgreSQL<T> Conectar(string strConnectionString)
        {
            PostgreSQL<T> modSql = new PostgreSQL<T>()
            {
                _clsPostgreSqlConnection = new NpgsqlConnection(strConnectionString)
            };

            try
            {
                modSql._clsPostgreSqlConnection.Open();
                modSql._blnConectado = true;
            }
            catch (NpgsqlException PostgresqlEx)
            {
                throw PostgresqlEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return modSql;
        }
        #endregion


        #region Métodos públicos

        public void PrepararProcedimiento(string nombreProcedimiento, DynamicParameters dynParameters, CommandType enuTipoComando = CommandType.Text)
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
                return _clsPostgreSqlConnection.Execute(_nombreProcedimiento, _dynParameters, null, _timeOut, _commandType);
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
                return _clsPostgreSqlConnection.QueryFirstOrDefault<T>(_nombreProcedimiento, _dynParameters, null, _timeOut, _commandType);
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
                return tipo == Models.TipoDato.Numerico ? _clsPostgreSqlConnection.QueryFirstOrDefault<long>(_nombreProcedimiento, _dynParameters, null, _timeOut, _commandType) : _clsPostgreSqlConnection.QueryFirstOrDefault<string>(_nombreProcedimiento, _dynParameters, null, _timeOut, _commandType);
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
                return _clsPostgreSqlConnection.Query<T>(_nombreProcedimiento, _dynParameters, null, true, _timeOut, _commandType);
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
                _clsPostgreSqlConnection.Dispose();
                _dynParameters = null;
                _blnPreparado = false;
            }
            catch { }
        }
        public void Desconectar()
        {
            _clsPostgreSqlConnection.Close();
        }



        #endregion
    }
}
