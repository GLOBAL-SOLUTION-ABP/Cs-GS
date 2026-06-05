using FarolOrbital.Domain;

namespace FarolOrbital.Interfaces;

/// <summary>
/// Contrato para o mecanismo de ajuste por feedback operacional.
/// </summary>
public interface IAjustadorLimiar
{
    /// <summary>
    /// Avalia a taxa de confirmação do lote e, se necessário, ajusta o limiar da câmera.
    /// Retorna true se o ajuste foi realizado.
    /// </summary>
    bool AjustarComTaxa(CameraLocal camera, double taxaConfirmacao);
}
