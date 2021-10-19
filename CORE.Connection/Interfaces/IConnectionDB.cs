using CORE.Connection.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Connection.Interfaces
{
    public interface IConnectionDB<T> : IDisposable
    {
        void PrepararProcedimiento(string nombreProcedimiento, DynamicParameters dynParameters, CommandType enuTipoComando = CommandType.StoredProcedure);
        long ExecuteDapper();
        T QueryFirstOrDefaultDapper();
        object QueryFirstOrDefaultDapper(TipoDato tipo);
        IEnumerable<T> Query();
    }
}
