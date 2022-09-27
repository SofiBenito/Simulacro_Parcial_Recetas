using RecetasSLN.Datos.Interfaz;
using RecetasSLN.dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecetasSLN.Datos.Implementacion
{
    public class RecetaDao : IRecetaDao
    {
        public DataTable GetIngredientes()
        {
            return HelperDao.ObtenerInstancia().ConsultaSQL("SP_CONSULTAR_INGREDIENTES");
        }

        public int GetProximaReceta()
        {
            return HelperDao.ObtenerInstancia().ConsultarProximo("SP_PROXIMO_ID", "@next");
        }

        public bool GetReceta(Receta nuevo)
        {
            return HelperDao.ObtenerInstancia().ConfirmarPresupuesto(nuevo);
        }
    }
}
