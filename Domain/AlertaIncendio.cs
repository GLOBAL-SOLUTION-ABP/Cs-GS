using FarolOrbital.Domain.Enums;
using FarolOrbital.Domain.Structs;

namespace FarolOrbital.Domain;

/// <summary>
/// Alerta específico de incêndio florestal ou urbano.
/// Herda de AlertaAmbiental e sobrescreve ObterDescricao() e CalcularPrioridade().
/// </summary>
public class AlertaIncendio : AlertaAmbiental
{
    public double IndicadorCalor { get; }

    public AlertaIncendio(string cameraId, double scoreVisao, double scoreOrbital,
                          CoordenadaGeografica coordenada, NivelRisco nivel,
                          double indicadorCalor = 1.0)
        : base(cameraId, scoreVisao, scoreOrbital, coordenada, nivel)
    {
        IndicadorCalor = Math.Max(0.0, indicadorCalor); // sem limite superior — calor pode ser alto
    }

    // Polimorfismo: descrição específica de incêndio
    public override string ObterDescricao() =>
        $"INCENDIO  | Câm: {CameraId} | Calor: {IndicadorCalor:F2} | {Coordenada}";

    // Polimorfismo: incêndio tem fator extra de prioridade pelo indicador de calor
    public override double CalcularPrioridade() =>
        Math.Min(1.0, ScoreFinal * (1.0 + IndicadorCalor * 0.05));
}
