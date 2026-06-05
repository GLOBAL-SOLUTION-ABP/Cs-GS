namespace FarolOrbital.Exceptions;

/// <summary>
/// Lançada quando se tenta operar sobre uma câmera inativa ou inexistente.
/// </summary>
public class CameraIndisponivelException : Exception
{
    public string CameraId { get; }

    public CameraIndisponivelException(string cameraId)
        : base($"Câmera '{cameraId}' está indisponível ou inativa.")
    {
        CameraId = cameraId;
    }
}
