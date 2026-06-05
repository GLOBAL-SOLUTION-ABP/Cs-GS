namespace FarolOrbital.Exceptions;

/// <summary>
/// Lançada quando um score recebido está fora do intervalo válido [0.0, 1.0].
/// </summary>
public class ScoreInvalidoException : Exception
{
    public double ScoreInformado { get; }

    public ScoreInvalidoException(double score)
        : base($"Score inválido: {score:F3}. O valor deve estar entre 0.0 e 1.0.")
    {
        ScoreInformado = score;
    }
}
