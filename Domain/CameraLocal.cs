using FarolOrbital.Domain.Structs;

namespace FarolOrbital.Domain;

/// <summary>
/// Representa uma câmera de monitoramento local.
/// Classe partial — a lógica de feedback está em CameraLocal.Feedback.cs.
/// Encapsula estado interno com atributos privados.
/// </summary>
public partial class CameraLocal
{
    // Atributos privados
    private readonly string             _id;
    private readonly string             _nome;
    private readonly CoordenadaGeografica _coordenada;
    private bool                        _ativa;
    private double                      _limiarConfianca;

    // Propriedades públicas controladas
    public string              Id              => _id;
    public string              Nome            => _nome;
    public CoordenadaGeografica Coordenada     => _coordenada;
    public bool                Ativa           => _ativa;
    public double              LimiarConfianca => _limiarConfianca;
    public DateTime            CadastradaEm    { get; }

    public CameraLocal(string id, string nome, CoordenadaGeografica coordenada,
                       double limiarInicial = 0.50)
    {
        _id              = id;
        _nome            = nome;
        _coordenada      = coordenada;
        _ativa           = true;
        _limiarConfianca = limiarInicial;
        CadastradaEm     = DateTime.Now;
    }

    public void Ativar()   => _ativa = true;
    public void Desativar() => _ativa = false;

    /// <summary>Ajusta o limiar mantendo-o dentro dos limites [0.40, 0.90].</summary>
    public void AtualizarLimiar(double novoLimiar)
    {
        _limiarConfianca = Math.Clamp(novoLimiar, 0.40, 0.90);
    }

    public override string ToString() =>
        $"[{Id}] {Nome} | Limiar: {_limiarConfianca:F2} | Ativa: {_ativa} | {_coordenada}";
}
