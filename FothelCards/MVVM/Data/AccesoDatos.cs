using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace FothelCards.MVVM.Data
{
    /// <summary>
    /// Capa de Datos del proyecto.
    /// Centraliza toda la comunicación entre la aplicación WPF y la base de datos MySQL.
    /// </summary>
    public class AccesoDatos
    {
        // Almacena la cadena de conexión de forma inmutable
        private readonly string connectionString;

        public AccesoDatos()
        {
            // Ajusta el puerto (3306 o 3309) y el password según tu PC
            // Cadena de conexión para acceder al servidor local y elegir la db 'fothelcards_db'
            connectionString = "datasource=localhost;port=3306;username=root;password=root;database=fothelcards_db;";
        }

        /// <summary>
        /// Método asíncrono que ejecuta un Procedimiento Almacenado que devuelve resultados (ej. SELECT).
        /// </summary>
        /// <param name="nombrePA">Nombre del procedimiento almacenado en MySQL.</param>
        /// <param name="nombresParametros">Lista opcional con los nombres de las variables del PA (sin el @).</param>
        /// <param name="valoresParametros">Lista opcional con los valores a enviar a las variables del PA.</param>
        /// <returns>Retorna un DataTable lleno con las filas y columnas devueltas por la base de datos.</returns>
        public async Task<DataTable> EjecutarProcedimientoAsync(string nombrePA, List<string> nombresParametros = null, List<object> valoresParametros = null)
        {
            // Creamos una tabla virtual en memoria para almacenar los resultados
            DataTable tabla = new DataTable();

            // Bloque using para garantizar que la conexión se cierra y libera recursos incluso si ocurre un error
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                // Abrir la conexión en segundo plano sin congelar la app
                await conn.OpenAsync();
                
                using (MySqlCommand cmd = new MySqlCommand(nombrePA, conn))
                {
                    // Le decimos explícitamente a MySQL que la consulta enviada es un procedimiento almacenado y no SQL puro
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    // Si se enviaron parámetros desde el ViewModel, los emparejamos uno a uno y los añadimos a la orden
                    if (nombresParametros != null && valoresParametros != null)
                    {
                        for (int i = 0; i < nombresParametros.Count; i++)
                            cmd.Parameters.AddWithValue("@" + nombresParametros[i], valoresParametros[i]);
                    }

                    // Usamos un adaptador que recogerá lo que escupa el comando ejecutado
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        // Rellenamos el DataTable en un hilo aparte para no bloquear el hilo de la IU
                        await Task.Run(() => da.Fill(tabla));
                    }
                }
            }
            return tabla; // Devolvemos los datos (Ej: la lista de usuarios)
        }

        /// <summary>
        /// Método asíncrono para ejecutar procedimientos que modifican datos y NO devuelven tablas de resultados (INSERT, UPDATE o DELETE).
        /// </summary>
        /// <returns>Retorna el número de filas que han sido modificadas o afectadas.</returns>
        public async Task<int> EjecutarProcedimientoNonQueryAsync(string nombrePA, List<string> nombresParametros = null, List<object> valoresParametros = null)
        {
            int filasAfectadas;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (MySqlCommand cmd = new MySqlCommand(nombrePA, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    // Agrega los parámetros con valores a la orden
                    if (nombresParametros != null && valoresParametros != null)
                    {
                        for (int i = 0; i < nombresParametros.Count; i++)
                            cmd.Parameters.AddWithValue("@" + nombresParametros[i], valoresParametros[i]);
                    }
                    
                    // Ejecuta el comando asincrónicamente y devuelve directamente el int con las celdas cambiadas
                    filasAfectadas = await cmd.ExecuteNonQueryAsync();
                }
            }
            return filasAfectadas; // Ej: si inserta a 1 usuario, devolverá un 1.
        }
    }
}
