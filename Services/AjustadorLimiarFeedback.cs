using FarolOrbital.Domain;
using FarolOrbital.Interfaces;
using FarolOrbital.Utils;

namespace FarolOrbital.Services;

/// <summary>
/// Implementação do mecanismo de ajuste por feedback operacional.
/// A cada 10 feedbacks por câmera, recalibra o limiar de confiança.
/// </summary>
public class AjustadorLimiarFeedback : IAjustadorLimiar
{
    /// <summary>
    /// Recebe a taxa de confirmação calculada antes do reset do lote
    /// e aplica a regra de ajuste de limiar.
    /// </summary>
    public bool AjustarComTaxa(CameraLocal camera, double taxaConfirmacao)
    {
        double limiarAtual = camera.LimiarConfianca;
        double novoLimiar  = limiarAtual;

        if (taxaConfirmacao < ConfiguracoesSistema.TaxaConfirmacaoMin)
        {
            novoLimiar = limiarAtual + ConfiguracoesSistema.IncrementoLimiar;
            FormatadorConsole.Log(
                $"[AJUSTE] Taxa {taxaConfirmacao:P0} < 40% → limiar sobe: " +
                $"{limiarAtual:F2} → {Math.Min(novoLimiar, ConfiguracoesSistema.LimiarCameraMax):F2}");
        }
        else if (taxaConfirmacao >= ConfiguracoesSistema.TaxaConfirmacaoAlta)
        {
            novoLimiar = limiarAtual - ConfiguracoesSistema.DecrementoLimiar;
            FormatadorConsole.Log(
                $"[AJUSTE] Taxa {taxaConfirmacao:P0} >= 70% → limiar cai: " +
                $"{limiarAtual:F2} → {Math.Max(novoLimiar, ConfiguracoesSistema.LimiarCameraMin):F2}");
        }
        else
        {
            FormatadorConsole.Log(
                $"[AJUSTE] Taxa {taxaConfirmacao:P0} entre 40%-69% → limiar mantido: {limiarAtual:F2}");
            return false;
        }

        camera.AtualizarLimiar(novoLimiar);
        FormatadorConsole.Log($"[AJUSTE] Novo limiar aplicado: {camera.LimiarConfianca:F2}");
        return true;
    }
}
