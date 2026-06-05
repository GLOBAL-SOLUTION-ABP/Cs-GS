namespace FarolOrbital.Domain;

/// <summary>
/// Representa um operador humano da Defesa Civil que analisa e responde a alertas.
/// </summary>
public class OperadorDefesaCivil
{
    private readonly string _matricula;
    private readonly string _nome;

    public string Matricula  => _matricula;
    public string Nome       => _nome;
    public DateTime RegistradoEm { get; }

    public OperadorDefesaCivil(string matricula, string nome)
    {
        _matricula   = matricula;
        _nome        = nome;
        RegistradoEm = DateTime.Now;
    }

    public override string ToString() => $"Operador: {_nome} (Mat. {_matricula})";
}
