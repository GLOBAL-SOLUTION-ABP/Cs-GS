using FarolOrbital.Domain.Enums;
using FarolOrbital.Interfaces;
using FarolOrbital.Utils;

namespace FarolOrbital.Services;

/// <summary>
/// Implementação de IClassificadorRisco.
/// Chamado explicitamente pelo MotorAlerta para classificar o nível de risco
/// a partir do score_final calculado. Garante que a interface seja genuinamente utilizada.
/// </summary>
public class MotorClassificacaoRisco : IClassificadorRisco
{
    public NivelRisco Classificar(double scoreFinal)
    {
        // Usa constantes de ConfiguracoesSistema — ponto único de verdade
        NivelRisco nivel = scoreFinal switch
        {
            _ when scoreFinal < ConfiguracoesSistema.LimiarMedio   => NivelRisco.Baixo,
            _ when scoreFinal < ConfiguracoesSistema.LimiarAlto    => NivelRisco.Medio,
            _ when scoreFinal < ConfiguracoesSistema.LimiarCritico => NivelRisco.Alto,
            _                                                       => NivelRisco.Critico
        };

        FormatadorConsole.Log($"[CLASSIF] Score {scoreFinal:F3} classificado como: {nivel}");
        return nivel;
    }
}
