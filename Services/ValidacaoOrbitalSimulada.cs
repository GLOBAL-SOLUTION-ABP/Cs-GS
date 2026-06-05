using System.Text.Json;
using FarolOrbital.Exceptions;
using FarolOrbital.Interfaces;
using FarolOrbital.Utils;

namespace FarolOrbital.Services;

/// <summary>
/// Implementação mockada de IValidadorOrbital.
/// Lê scores orbitais do arquivo Data/dados_orbitais_mock.json,
/// simulando a resposta de uma API satelital real (INPE/Copernicus/Planet Labs).
/// No roadmap: substituir a leitura do JSON por chamada HTTP à API real.
/// </summary>
public class ValidacaoOrbitalSimulada : IValidadorOrbital
{
    private static readonly Random _rng = new();
    private readonly Dictionary<string, double> _dadosOrbitais;

    // ── Classe privada interna ─────────────────────────────────────
    // DTO (Data Transfer Object) utilizado exclusivamente nesta classe
    // para mapear cada entrada do arquivo JSON em um objeto tipado.
    // 'sealed' impede herança desnecessária; 'private' restringe ao escopo da classe pai.
    private sealed class CameraOrbitalMock
    {
        public string CameraId         { get; set; } = string.Empty;
        public double ScoreOrbitalBase { get; set; }
    }
    // ───────────────────────────────────────────────────────────────

    public ValidacaoOrbitalSimulada()
    {
        _dadosOrbitais = CarregarJsonMock();
    }

    /// <summary>
    /// Carrega o arquivo JSON mockado de dados orbitais.
    /// Fallback para valores padrão se o arquivo não for encontrado.
    /// </summary>
    private static Dictionary<string, double> CarregarJsonMock()
    {
        string caminhoJson = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Data", "dados_orbitais_mock.json");

        if (!File.Exists(caminhoJson))
            caminhoJson = Path.Combine(
                Directory.GetCurrentDirectory(), "Data", "dados_orbitais_mock.json");

        if (!File.Exists(caminhoJson))
        {
            FormatadorConsole.Aviso("[ORBITAL] JSON mock não encontrado — usando valores embutidos.");
            return Fallback();
        }

        try
        {
            string json   = File.ReadAllText(caminhoJson);
            using var doc = JsonDocument.Parse(json);

            var mocks = doc.RootElement
                .GetProperty("cameras")
                .EnumerateArray()
                .Select(cam => new CameraOrbitalMock           // uso da classe privada
                {
                    CameraId         = cam.GetProperty("cameraId").GetString()!,
                    ScoreOrbitalBase = cam.GetProperty("scoreOrbitalBase").GetDouble()
                })
                .ToList();

            FormatadorConsole.Log($"[ORBITAL] JSON mock carregado: {mocks.Count} câmera(s).");
            return mocks.ToDictionary(m => m.CameraId, m => m.ScoreOrbitalBase);
        }
        catch (Exception ex)
        {
            FormatadorConsole.Aviso($"[ORBITAL] Falha ao ler JSON: {ex.Message}. Usando valores embutidos.");
            return Fallback();
        }
    }

    private static Dictionary<string, double> Fallback() => new()
    {
        { "CAM-001", 0.72 }, { "CAM-002", 0.55 },
        { "CAM-003", 0.88 }, { "CAM-004", 0.40 }
    };

    public double ObterScoreOrbital(string cameraId, double scoreVisao)
    {
        FormatadorConsole.Log($"[ORBITAL] Consultando dados orbitais para câmera {cameraId}...");

        // Simula falha ocasional (5% de chance) para demonstrar tratamento de exceção
        if (_rng.NextDouble() < 0.05)
            throw new ValidacaoOrbitalException(cameraId, "timeout simulado na consulta orbital");

        double baseOrbital = _dadosOrbitais.TryGetValue(cameraId, out double val)
            ? val : scoreVisao;

        double variacao  = (_rng.NextDouble() - 0.5) * 0.10;
        double resultado = Math.Clamp(baseOrbital + variacao, 0.0, 1.0);

        FormatadorConsole.Log($"[ORBITAL] Score orbital retornado: {resultado:F3}");
        return Math.Round(resultado, 4);
    }
}
