using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FothelCards
{
    /// <summary>
    /// Clase que implementa ICommand para conectar la Vista (Botones) con el ViewModel (Métodos).
    /// En lugar de usar eventos de clic en el code-behind, se enlazan los botones usando un Binding a objetos RelayCommand.
    /// </summary>
    public class RelayCommand : ICommand
    {
        // La acción o método que se ejecutará cuando se pulse el botón
        private readonly Action<object> _execute;
        
        // Condición que determina si el botón debe estar habilitado o deshabilitado
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Constructor del comando.
        /// </summary>
        /// <param name="execute">El método a ejecutar</param>
        /// <param name="canExecute">Condición opcional para habilitar la ejecución</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            // Validamos que la acción a ejecutar no sea nula, si lo es, lanzamos un error que crashearía la app controladamente o el compilador avisaría
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Metodo de la interfaz ICommand. Determina si el comando puede ejecutarse en su estado actual.
        /// Retorna true si canExecute es nulo (siempre ejecutable), o si la condición de canExecute se cumple.
        /// </summary>
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        /// <summary>
        /// Metodo de la interfaz ICommand. Define el método a ser llamado cuando el comando es invocado (botón presionado).
        /// </summary>
        public void Execute(object? parameter) => _execute(parameter);

        /// <summary>
        /// Evento de la interfaz ICommand.
        /// Avisa a los controles de la UI (ej: un botón) que algo ha cambiado y debe reevaluar CanExecute para ver si se activa o desactiva visualmente.
        /// Delega esta tarea al CommandManager de WPF.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
