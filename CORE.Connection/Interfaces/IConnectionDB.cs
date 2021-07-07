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
        void PrepararProcedimiento(string strNombreProcedimiento, List<Tuple<string, object, int>> tplParametros, CommandType enuTipoComando = CommandType.StoredProcedure);
        int EjecutarProcedimiento();
        object EjecutarScalar();
        DataTableReader EjecutarTableReader();
        DataTable EjecutarTable();
    }
}
