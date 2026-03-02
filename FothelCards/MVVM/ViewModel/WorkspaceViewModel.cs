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
    public class WorkspaceViewModel : BaseViewModel
    {
    
            public ObservableCollection<ProductoModel> ListaProductos { get; set; }

            private ProductoModel _productoSeleccionado;
            public ProductoModel ProductoSeleccionado
            {
                get => _productoSeleccionado;
                set { _productoSeleccionado = value; OnPropertyChanged(); }
            }

            // Parámetros de cuadrícula (Requisito PDF)
            public int Filas { get; set; } = 2;
            public int Columnas { get; set; } = 3;

            // Propiedades para Agregar un nuevo coleccionable
            private string _nuevoNombre;
            public string NuevoNombre { get => _nuevoNombre; set { _nuevoNombre = value; OnPropertyChanged(); } }

            private string _nuevoTipo = "Comic/Manga";
            public string NuevoTipo { get => _nuevoTipo; set { _nuevoTipo = value; OnPropertyChanged(); } }

            private string _nuevaRutaPortada;
            public string NuevaRutaPortada { get => _nuevaRutaPortada; set { _nuevaRutaPortada = value; OnPropertyChanged(); } }

            public ICommand GenerarCommand { get; }
            public ICommand CambiarEstadoCommand { get; }
            public ICommand SeleccionarCommand { get; }
            public ICommand AgregarProductoCommand { get; }
            public ICommand EditarProductoCommand { get; }
            public ICommand EliminarProductoCommand { get; }
        public WorkspaceViewModel()
            {
                ListaProductos = new ObservableCollection<ProductoModel>();
                GenerarCommand = new RelayCommand(GenerarTienda);
                CambiarEstadoCommand = new RelayCommand(CambiarEstadoProducto);
                SeleccionarCommand = new RelayCommand(SeleccionarProducto);
                AgregarProductoCommand = new RelayCommand(AgregarProducto);
                EditarProductoCommand = new RelayCommand(EditarProducto);
                EliminarProductoCommand = new RelayCommand(EliminarProducto);

            GenerarTienda(null);
            }

            private async void GenerarTienda(object parameter)
            {
                ListaProductos.Clear();
                try
                {
                    AccesoDatos db = new AccesoDatos();
                    DataTable dt = await db.EjecutarProcedimientoAsync("sp_ObtenerProductos");

                    foreach (DataRow fila in dt.Rows)
                    {
                        ListaProductos.Add(new ProductoModel
                        {
                            Id = (int)fila["id"],
                            Nombre = fila["nombre"].ToString(),
                            Tipo = fila["tipo"].ToString(),
                            Estado = fila["estado"].ToString(),
                            RutaPortada = fila["ruta_portada"].ToString()
                        });
                    }
                }
                catch { /* Captura de error DB */ }
            }

            private void SeleccionarProducto(object parameter)
            {
                if (parameter is ProductoModel producto)
                {
                    ProductoSeleccionado = producto;
                }
            }

            private async void CambiarEstadoProducto(object parameter)
            {
                if (parameter is ProductoModel producto)
                {
                    string nuevoEstado = producto.Estado == "Disponible" ? "Agotado" : "Disponible";
                    AccesoDatos db = new AccesoDatos();
                    await db.EjecutarProcedimientoNonQueryAsync(
                        "sp_ActualizarProducto",
                        new List<string> { "p_id", "p_estado" },
                        new List<object> { producto.Id, nuevoEstado }
                    );
                    producto.Estado = nuevoEstado;
                }
            }

            private async void AgregarProducto(object parameter)
            {
                if (string.IsNullOrWhiteSpace(NuevoNombre)) return;

                AccesoDatos db = new AccesoDatos();
                await db.EjecutarProcedimientoNonQueryAsync(
                    "sp_InsertarProducto",
                    new List<string> { "p_nombre", "p_tipo", "p_estado", "p_ruta" },
                    new List<object> { NuevoNombre, NuevoTipo, "Disponible", NuevaRutaPortada ?? "" }
                );

                NuevoNombre = string.Empty;
                NuevaRutaPortada = null;
                GenerarTienda(null); // Refresca el panel
            }
        private async void EditarProducto(object parameter)
        {
            if (ProductoSeleccionado == null) return;

            try
            {
                AccesoDatos db = new AccesoDatos();
                await db.EjecutarProcedimientoNonQueryAsync(
                    "sp_EditarProducto", // Nombre del procedimiento almacenado
                    new List<string> { "p_id", "p_nombre", "p_tipo" },
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

        private async void EliminarProducto(object parameter)
        {
            if (ProductoSeleccionado == null) return;

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

                    // Eliminar de la lista visible y limpiar la selección actual
                    ListaProductos.Remove(ProductoSeleccionado);
                    ProductoSeleccionado = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
    }

