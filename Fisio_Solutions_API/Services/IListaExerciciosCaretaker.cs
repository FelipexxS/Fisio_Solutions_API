using Fisio_Solutions_API.Models;

namespace Fisio_Solutions_API.Services
{
    /// <summary>
    /// Caretaker Interface (GoF Memento): Define o contrato para o guardião responsável
    /// por armazenar e recuperar o histórico de snapshots (Mementos) da lista do usuário.
    /// </summary>
    public interface IListaExerciciosCaretaker
    {
        void SaveState(int usuarioId, ListaExerciciosMemento memento);
        ListaExerciciosMemento? PopState(int usuarioId);
        bool HasHistory(int usuarioId);
    }
}

