using FothelCards.MVVM.Data;
using FothelCards.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FothelCards.MVVM.ViewModel
{
    /// <summary>
    /// ViewModel principal del inventario. Gestiona el catálogo de cartas/productos.
    /// Incluye todas las operaciones CRUD (Crear, Leer, Actualizar, Eliminar).
    /// </summary>
    public class WorkspaceViewModel : BaseViewModel
    {
        // Colección observable: si añadimos o borramos un producto de aquí, la interfaz (ej. un DataGrid) se actualiza sola al instante
        public ObservableCollection<ProductoModel> ListaProductos { get; set; }

        // Mantiene track de la carta que el usuario ha clicado en la tabla
        private ProductoModel _productoSeleccionado;
        public ProductoModel ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set { _productoSeleccionado = value; OnPropertyChanged(); }
        }

        // Parámetros de cuadrícula para definir cómo se muestran las cartas (Requisito PDF)
        public int Filas { get; set; } = 2;
        public int Columnas { get; set; } = 3;

        // Propiedades de la UI preparadas para que el usuario escriba los datos de Agregar/Editar un producto
        private string _nuevoNombre;
        public string NuevoNombre { get => _nuevoNombre; set { _nuevoNombre = value; OnPropertyChanged(); } }

        private string _nuevoTipo = "Comic/Manga"; // Desplegable por defecto
        public string NuevoTipo { get => _nuevoTipo; set { _nuevoTipo = value; OnPropertyChanged(); } }

        private string _nuevaRutaPortada;
        public string NuevaRutaPortada { get => _nuevaRutaPortada; set { _nuevaRutaPortada = value; OnPropertyChanged(); } }

        // Listado de Botones (Bindings)
        public ICommand GenerarCommand { get; }
        public ICommand CambiarEstadoCommand { get; }
        public ICommand SeleccionarCommand { get; }
        public ICommand AgregarProductoCommand { get; }
        public ICommand EditarProductoCommand { get; }
        public ICommand EliminarProductoCommand { get; }
        
        public WorkspaceViewModel()
        {
            // Instanciar la lista vacía de cartas
            ListaProductos = new ObservableCollection<ProductoModel>();
            
            // Asignar qué método arranca cada botón de la UI
            GenerarCommand = new RelayCommand(GenerarTienda);
            CambiarEstadoCommand = new RelayCommand(CambiarEstadoProducto);
            SeleccionarCommand = new RelayCommand(SeleccionarProducto);
            AgregarProductoCommand = new RelayCommand(AgregarProducto);
            EditarProductoCommand = new RelayCommand(EditarProducto);
            EliminarProductoCommand = new RelayCommand(EliminarProducto);

            // Cargar los productos automáticamente al abrir esta pestaña
            GenerarTienda(null);
        }

        /// <summary>
        /// (LEER) Trae todos los productos de la Base de Datos y rellena la lista observable.
        /// </summary>
        private async void GenerarTienda(object parameter)
        {
            ListaProductos.Clear(); // Limpiamos la lista visual antes de recargar
            try
            {
                AccesoDatos db = new AccesoDatos();
                DataTable dt = await db.EjecutarProcedimientoAsync("sp_ObtenerProductos");

                // Recorremos la tabla obtenida traduciendo cada fila (row) a un objeto modelo ProductoModel en C#
                foreach (DataRow fila in dt.Rows)
                {
                    ListaProductos.Add(new ProductoModel
                    {
                        Id = (int)fila["id"],
                        Nombre = fila["nombre"].ToString(),
                        Tipo = fila["tipo"].ToString(),
                        Estado = fila["estado"].ToString(),
                        RutaPortada = fila["ruta_portada"].ToString() // Imagen de la carta
                    });
                }
            }
            catch { /* Captura de error silenciosa (en producción aquí iría un aviso o log) */ }
        }

        /// <summary>
        /// Marca un producto de la tabla como el producto "Objetivo" para poder borrarlo o editarlo luego.
        /// </summary>
        private void SeleccionarProducto(object parameter)
        {
            if (parameter is ProductoModel producto)
            {
                ProductoSeleccionado = producto;
            }
        }

        /// <summary>
        /// Alterna el estado de stock (Disponible <-> Agotado) con un simple clic.
        /// </summary>
        private async void CambiarEstadoProducto(object parameter)
        {
            if (parameter is ProductoModel producto)
            {
                // Operador ternario: si es Disponible, ponlo Agotado, sino, ponlo Disponible
                string nuevoEstado = producto.Estado == "Disponible" ? "Agotado" : "Disponible";
                AccesoDatos db = new AccesoDatos();
                await db.EjecutarProcedimientoNonQueryAsync(
                    "sp_ActualizarProducto",
                    new List<string> { "p_id", "p_estado" },
                    new List<object> { producto.Id, nuevoEstado }
                );
                
                // Actualiza visualmente el modelo local para no tener que recargar toda la base de datos entera
                producto.Estado = nuevoEstado;
            }
        }

        /// <summary>
        /// (CREAR) Añade un nuevo producto a la tienda.
        /// </summary>
        private async void AgregarProducto(object parameter)
        {
            // Mínimo exigir un nombre, no insertar productos en blanco
            if (string.IsNullOrWhiteSpace(NuevoNombre)) return;

            AccesoDatos db = new AccesoDatos();
            await db.EjecutarProcedimientoNonQueryAsync(
                "sp_InsertarProducto",
                new List<string> { "p_nombre", "p_tipo", "p_estado", "p_ruta" },
                new List<object> { NuevoNombre, NuevoTipo, "Disponible", NuevaRutaPortada ?? "" }
            );

            // Una vez guardado, vaciar el campo de texto de la UI para dejarlo limpito
            NuevoNombre = string.Empty;
            NuevaRutaPortada = null;
            GenerarTienda(null); // Refresca el panel y la lista visual de Cartas
        }
        
        /// <summary>
        /// (ACTUALIZAR) Modifica el nombre o tipo de la carta que tenemos seleccionada en la tabla.
        /// </summary>
        private async void EditarProducto(object parameter)
        {
            // Seguridad por si pulsamos Editar sin haber clicado a nadie primero en la tabla
            if (ProductoSeleccionado == null) return;

            try
            {
                AccesoDatos db = new AccesoDatos();
                await db.EjecutarProcedimientoNonQueryAsync(
                    "sp_EditarProducto", // Nombre del procedimiento almacenado
                    new List<string> { "p_id", "p_nombre", "p_tipo" },
                    // Usamos las propiedades del ProductoSeleccionado modificado por el Binding bidireccional de la tabla
                    new List<object> { ProductoSeleccionado.Id, ProductoSeleccionado.Nombre, ProductoSeleccionado.Tipo }
                );

                MessageBox.Show("Producto actualizado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                GenerarTienda(null); // Refresca la lista por si hubo cambios visuales importantes
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al editar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// (ELIMINAR) Borra de forma definitiva una carta de la base de datos tras confirmación del usuario.
        /// </summary>
        private async void EliminarProducto(object parameter)
        {
            if (ProductoSeleccionado == null) return;

            // Alerta nativa de Windows pidiendo confirmación (Sí / No) para evitar borrados accidentales
            var resultado = MessageBox.Show($"¿Estás seguro de que deseas eliminar '{ProductoSeleccionado.Nombre}'?",
                                            "Confirmar Eliminación",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    AccesoDatos db = new AccesoDatos();
                    await db.EjecutarProcedimientoNonQueryAsync(
                        "sp_EliminarProducto", // Nombre del procedimiento almacenado
                        new List<string> { "p_id" },
                        new List<object> { ProductoSeleccionado.Id }
                    );

                    // Pequeña optimización: Eliminar de la lista visible de C# manualmente
                    // sin tener que llamar a la BD y pedir 'GenerarTienda' otra vez ralentizando todo
                    ListaProductos.Remove(ProductoSeleccionado);
                    ProductoSeleccionado = null; // Quitar la selección virtual
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

