using FarolOrbital.Domain;
using FarolOrbital.Interfaces;

namespace FarolOrbital.Repositories;

/// <summary>
/// Repositório em memória para AlertaAmbiental.
/// Implementa IRepositorioAlertas — pode ser substituído por EF Core sem alterar o motor.
/// </summary>
public class RepositorioAlertasMemoria : IRepositorioAlertas
{
    private readonly List<AlertaAmbiental> _alertas = new();

    public void Salvar(AlertaAmbiental alerta)
    {
        _alertas.Add(alerta);
    }

    public IReadOnlyList<AlertaAmbiental> ListarTodos() =>
        _alertas.AsReadOnly();

    public IReadOnlyList<AlertaAmbiental> ListarPorCamera(string cameraId) =>
        _alertas.Where(a => a.CameraId == cameraId).ToList().AsReadOnly();

    public AlertaAmbiental? BuscarPorId(Guid id) =>
        _alertas.FirstOrDefault(a => a.Id == id);
}
