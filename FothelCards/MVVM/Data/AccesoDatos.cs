using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace FothelCards.MVVM.Data
{
    public class AccesoDatos
    {
        private readonly string connectionString;

        public AccesoDatos()
        {
            // Ajusta el puerto (3306 o 3309) y el password según tu PC
            connectionString = "datasource=localhost;port=3306;username=root;password=1234;database=fothelcards_db;";
        }

        // Procedimiento que devuelve datos (SELECT)
        public async Task<DataTable> EjecutarProcedimientoAsync(string nombrePA, List<string> nombresParametros = null, List<object> valoresParametros = null)
        {
            DataTable tabla = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (MySqlCommand cmd = new MySqlCommand(nombrePA, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (nombresParametros != null && valoresParametros != null)
                    {
                        for (int i = 0; i < nombresParametros.Count; i++)
                            cmd.Parameters.AddWithValue("@" + nombresParametros[i], valoresParametros[i]);
                    }

                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        await Task.Run(() => da.Fill(tabla));
                    }
                }
            }
            return tabla;
        }

        // Procedimiento sin resultado (INSERT / UPDATE / DELETE)
        public async Task<int> EjecutarProcedimientoNonQueryAsync(string nombrePA, List<string> nombresParametros = null, List<object> valoresParametros = null)
        {
            int filasAfectadas;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (MySqlCommand cmd = new MySqlCommand(nombrePA, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (nombresParametros != null && valoresParametros != null)
                    {
                        for (int i = 0; i < nombresParametros.Count; i++)
                            cmd.Parameters.AddWithValue("@" + nombresParametros[i], valoresParametros[i]);
                    }
                    filasAfectadas = await cmd.ExecuteNonQueryAsync();
                }
            }
            return filasAfectadas;
        }
    }
}
