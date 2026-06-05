namespace FarolOrbital.Domain;

/// <summary>
/// Registra o feedback de um operador sobre um alerta específico.
/// Compõe o mecanismo de ajuste por feedback operacional.
/// </summary>
public class FeedbackOperacional
{
    public Guid     Id          { get; } = Guid.NewGuid();
    public Guid     AlertaId    { get; }
    public string   OperadorId  { get; }
    public bool     Confirmado  { get; }
    public string   Observacao  { get; }
    public DateTime DataHora    { get; }

    public FeedbackOperacional(Guid alertaId, string operadorId,
                                bool confirmado, string observacao = "")
    {
        AlertaId   = alertaId;
        OperadorId = operadorId;
        Confirmado = confirmado;
        Observacao = observacao;
        DataHora   = DateTime.Now;
    }

    public override string ToString() =>
        $"[{DataHora:dd/MM/yyyy HH:mm:ss}] Alerta {AlertaId.ToString()[..8]}... | " +
        $"{(Confirmado ? "CONFIRMADO" : "NEGADO")} | Op: {OperadorId}" +
        (string.IsNullOrWhiteSpace(Observacao) ? "" : $" | Obs: {Observacao}");
}
