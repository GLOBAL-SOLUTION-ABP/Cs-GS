namespace FarolOrbital.Exceptions;

/// <summary>
/// Lançada quando a validação orbital simulada falha.
/// No MVP, aciona o fallback: score_final = score_visao.
/// </summary>
public class ValidacaoOrbitalException : Exception
{
    public string CameraId { get; }

    public ValidacaoOrbitalException(string cameraId, string motivo = "Falha na consulta orbital")
        : base($"Validação orbital falhou para câmera '{cameraId}': {motivo}. Usando fallback (score_visao).")
    {
        CameraId = cameraId;
    }
}
