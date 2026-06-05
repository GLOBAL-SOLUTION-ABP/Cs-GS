using FarolOrbital.Domain.Enums;
using FarolOrbital.Domain.Structs;

namespace FarolOrbital.Domain;

/// <summary>
/// Alerta específico de fumaça densa detectada por câmera local.
/// Segundo tipo concreto derivado de AlertaAmbiental — demonstra herança e polimorfismo.
/// Precede o incêndio ativo: detecção precoce com menor score de calor.
/// </summary>
public class AlertaFumaca : AlertaAmbiental
{
    /// <summary>Densidade de fumaça estimada pelo modelo de visão (0.0 a 1.0).</summary>
    public double DensidadeFumaca { get; }

    public AlertaFumaca(string cameraId, double scoreVisao, double scoreOrbital,
                        CoordenadaGeografica coordenada, NivelRisco nivel,
                        double densidadeFumaca = 0.5)
        : base(cameraId, scoreVisao, scoreOrbital, coordenada, nivel)
    {
        DensidadeFumaca = Math.Clamp(densidadeFumaca, 0.0, 1.0);
    }

    // Polimorfismo: descrição específica de fumaça
    public override string ObterDescricao() =>
        $"FUMACA    | Câm: {CameraId} | Densidade: {DensidadeFumaca:F2} | {Coordenada}";

    // Polimorfismo: fumaça tem prioridade ajustada pela densidade
    public override double CalcularPrioridade() =>
        Math.Min(1.0, ScoreFinal + DensidadeFumaca * 0.10);
}
