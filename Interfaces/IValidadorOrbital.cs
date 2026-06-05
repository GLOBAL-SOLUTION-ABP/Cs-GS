namespace FarolOrbital.Interfaces;

/// <summary>
/// Contrato para o validador orbital. Permite substituir a implementação
/// mockada por uma real (satélite INPE, Copernicus, etc.) sem alterar o motor.
/// </summary>
public interface IValidadorOrbital
{
    /// <summary>
    /// Retorna um score orbital entre 0.0 e 1.0 para a câmera e score de visão informados.
    /// </summary>
    double ObterScoreOrbital(string cameraId, double scoreVisao);
}
