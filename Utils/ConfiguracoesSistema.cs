namespace FarolOrbital.Utils;

/// <summary>
/// Classe estática com constantes e configurações globais do sistema.
/// Uso de static: não precisa de instância, dados compartilhados por todo o sistema.
/// </summary>
public static class ConfiguracoesSistema
{
    public const string NomeSistema   = "FAROL Orbital — Módulo VIGÍLIA";
    public const string VersaoSistema = "1.0.0-MVP";
    public const string Disciplina    = "Advanced Business Development with .NET";
    public const string Instituicao   = "FIAP";
    public const string GlobalSolution = "Global Solution 2026 — Space Connect";

    // Pesos da fórmula de score final
    public const double PesoVisao   = 0.70;
    public const double PesoOrbital = 0.30;

    // Limiares de nível
    public const double LimiarMedio   = 0.40;
    public const double LimiarAlto    = 0.60;
    public const double LimiarCritico = 0.80;

    // Limites do limiar de câmera
    public const double LimiarCameraMin = 0.40;
    public const double LimiarCameraMax = 0.90;

    // Ajuste de limiar
    public const int    LoteFeedback          = 10;
    public const double TaxaConfirmacaoMin     = 0.40;
    public const double TaxaConfirmacaoAlta    = 0.70;
    public const double IncrementoLimiar       = 0.05;
    public const double DecrementoLimiar       = 0.03;

    public static string Banner()
    {
        string linha = new string('=', 60);
        return $"\n{linha}\n  {NomeSistema}\n  {GlobalSolution}\n  Versão: {VersaoSistema}\n{linha}\n";
    }
}
