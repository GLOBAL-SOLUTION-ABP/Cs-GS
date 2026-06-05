using FarolOrbital.Domain.Enums;

namespace FarolOrbital.Interfaces;

/// <summary>
/// Contrato para classificação de risco a partir de um score final.
/// </summary>
public interface IClassificadorRisco
{
    NivelRisco Classificar(double scoreFinal);
}
