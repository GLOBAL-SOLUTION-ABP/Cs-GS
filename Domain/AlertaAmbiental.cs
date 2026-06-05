using FarolOrbital.Domain.Enums;
using FarolOrbital.Domain.Structs;
using FarolOrbital.Utils;

namespace FarolOrbital.Domain;

/// <summary>
/// Classe abstrata base para todos os alertas ambientais.
/// Define o contrato comum e permite polimorfismo via métodos virtuais/abstratos.
/// O NivelRisco é recebido externamente — calculado pelo IClassificadorRisco no MotorAlerta.
/// </summary>
public abstract class AlertaAmbiental
{
    // Atributos privados com encapsulamento
    private readonly Guid                _id;
    private readonly string              _cameraId;
    private readonly double              _scoreVisao;
    private readonly double              _scoreOrbital;
    private readonly double              _scoreFinal;

    public Guid              Id           => _id;
    public string            CameraId     => _cameraId;
    public double            ScoreVisao   => _scoreVisao;
    public double            ScoreOrbital => _scoreOrbital;
    public double            ScoreFinal   => _scoreFinal;
    public NivelRisco        Nivel        { get; protected set; }
    public StatusAlerta      Status       { get; set; } = StatusAlerta.Pendente;
    public DateTime          DataHora     { get; }
    public CoordenadaGeografica Coordenada { get; }

    /// <param name="nivel">
    /// Nível de risco classificado externamente pelo IClassificadorRisco,
    /// garantindo que a interface seja genuinamente utilizada pelo MotorAlerta.
    /// </param>
    protected AlertaAmbiental(string cameraId, double scoreVisao, double scoreOrbital,
                               CoordenadaGeografica coordenada, NivelRisco nivel)
    {
        _id           = Guid.NewGuid();
        _cameraId     = cameraId;
        _scoreVisao   = scoreVisao;
        _scoreOrbital = scoreOrbital;
        // Usa as constantes de ConfiguracoesSistema — fonte única de verdade para os pesos
        _scoreFinal   = Math.Round(
            scoreVisao   * ConfiguracoesSistema.PesoVisao +
            scoreOrbital * ConfiguracoesSistema.PesoOrbital, 4);
        DataHora      = DateTime.Now;
        Coordenada    = coordenada;
        Nivel         = nivel;
    }

    // Método abstrato — obriga subclasses a fornecer descrição específica (polimorfismo)
    public abstract string ObterDescricao();

    // Método virtual — pode ser sobrescrito com comportamento específico (polimorfismo)
    public virtual double CalcularPrioridade() => _scoreFinal;

    public override string ToString() =>
        $"[{DataHora:dd/MM/yyyy HH:mm:ss}] {ObterDescricao()} | " +
        $"Score: {_scoreFinal:F3} | Nível: {Nivel} | Status: {Status}";
}
