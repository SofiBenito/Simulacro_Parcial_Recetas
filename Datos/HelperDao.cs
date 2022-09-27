using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using RecetasSLN.dominio;

namespace RecetasSLN.Datos.Interfaz
{
    public class HelperDao
    {
        private static HelperDao instancia;
        private string conexion;
        public HelperDao()
        {
            conexion = Properties.Resources.CnnString;
        }
        public static HelperDao ObtenerInstancia()
        {
            if (instancia == null)
            {
                instancia = new HelperDao();
            }
            return instancia;
        }
        public DataTable ConsultaSQL(string sp)
        {
            SqlConnection cnn = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            DataTable tabla = new DataTable();
            cnn.ConnectionString = conexion;
            cnn.Open();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = sp;
            tabla.Load(cmd.ExecuteReader());
            cnn.Close();
            return tabla;
        }
        public int ConsultarProximo(string sp, string Parametro)
        {
            SqlConnection cnn = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            cnn.ConnectionString = conexion;
            cnn.Open();
            cmd.Connection = cnn;
            cmd.CommandText = sp;
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pOut = new SqlParameter();
            pOut.ParameterName = Parametro;
            pOut.DbType = DbType.Int32;
            pOut.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(pOut);
            cmd.ExecuteNonQuery();
            cnn.Close();
            return (int)pOut.Value;
        }
        public bool ConfirmarPresupuesto(Receta oReceta)
        {
            SqlTransaction transaction = null;
            SqlConnection cnn= new SqlConnection();
            bool ok = true;
            try
            {
                cnn.ConnectionString = conexion;
                cnn.Open();
                transaction = cnn.BeginTransaction();

                //Se inserta Receta
                SqlCommand cmdMaestro = new SqlCommand("SP_INSERTAR_RECETA", cnn, transaction);
                cmdMaestro.CommandType = CommandType.StoredProcedure;
                cmdMaestro.Parameters.AddWithValue("@tipo_receta", oReceta.TipoReceta);
                cmdMaestro.Parameters.AddWithValue("@nombre", oReceta.Nombre);
                if (oReceta.Chef != null)
                    cmdMaestro.Parameters.AddWithValue("@cheff", oReceta.Chef);
                else
                    cmdMaestro.Parameters.AddWithValue("@cheff", DBNull.Value);

                //parámetro de salida:
                SqlParameter pOut = new SqlParameter();
                pOut.ParameterName = "@receta_nro";
                pOut.DbType = DbType.Int32;
                pOut.Direction = ParameterDirection.Output;
                cmdMaestro.Parameters.Add(pOut);
                cmdMaestro.ExecuteNonQuery();
                cmdMaestro.Parameters.Clear();
                int recetaNro = (int)pOut.Value;
               
                int count = 1;
                //Se inserta Detalle Receta 
                foreach (DetalleReceta detalle in oReceta.DetallesRecetas)
                {
                    SqlCommand cmdDetalle = new SqlCommand("InsertarDetalle", cnn, transaction);
                    cmdDetalle.CommandType = CommandType.StoredProcedure;
                    cmdDetalle.Parameters.AddWithValue("@receta_nro", recetaNro);
                    cmdDetalle.Parameters.AddWithValue("@id_ingrediente", detalle.Ingrediente.IdIngrediente);
                    cmdDetalle.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
                    count++;
                    cmdDetalle.ExecuteNonQuery();
                    cmdDetalle.Parameters.Clear();

                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ok = false;
            }
            finally
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
            return ok;
        }


        


      
    }
}
