namespace FarolOrbital.Domain;

/// <summary>
/// Parte partial de CameraLocal responsável pelo estado de feedback operacional.
/// Uso de partial class para separar responsabilidades em arquivos distintos.
/// </summary>
public partial class CameraLocal
{
    // Estado de feedback — privado, acumulado por câmera
    private int _totalFeedbacks     = 0;
    private int _feedbacksNoLote    = 0;
    private int _confirmacoesNoLote = 0;

    public int TotalFeedbacks     => _totalFeedbacks;
    public int FeedbacksNoLote    => _feedbacksNoLote;
    public int ConfirmacoesNoLote => _confirmacoesNoLote;

    /// <summary>
    /// Registra um feedback e retorna true se o lote de 10 foi completado
    /// (sinal para que o ajustador de limiar seja acionado).
    /// </summary>
    public bool RegistrarFeedback(bool confirmado)
    {
        _totalFeedbacks++;
        _feedbacksNoLote++;
        if (confirmado) _confirmacoesNoLote++;

        if (_feedbacksNoLote >= 10)
        {
            _feedbacksNoLote    = 0;
            _confirmacoesNoLote = 0;
            return true; // lote completo
        }
        return false;
    }

    /// <summary>Taxa de confirmação do lote atual (0.0 a 1.0).</summary>
    public double TaxaConfirmacaoLote()
    {
        if (_feedbacksNoLote == 0) return 0.0;
        return (double)_confirmacoesNoLote / _feedbacksNoLote;
    }
}
