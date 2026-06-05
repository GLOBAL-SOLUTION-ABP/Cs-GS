using FarolOrbital.Domain;

namespace FarolOrbital.Interfaces;

/// <summary>
/// Contrato para persistência de alertas (em memória no MVP).
/// </summary>
public interface IRepositorioAlertas
{
    void Salvar(AlertaAmbiental alerta);
    IReadOnlyList<AlertaAmbiental> ListarTodos();
    IReadOnlyList<AlertaAmbiental> ListarPorCamera(string cameraId);
    AlertaAmbiental? BuscarPorId(Guid id);
}
