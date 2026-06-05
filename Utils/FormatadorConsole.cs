using FarolOrbital.Domain;
using FarolOrbital.Domain.Enums;

namespace FarolOrbital.Utils;

/// <summary>
/// Classe estática utilitária para saída formatada no console.
/// Centraliza mensagens padronizadas e cores — garante visual consistente em todo o sistema.
/// </summary>
public static class FormatadorConsole
{
    public static void Log(string mensagem)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  {mensagem}");
        Console.ResetColor();
    }

    public static void Aviso(string mensagem)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  [!] {mensagem}");
        Console.ResetColor();
    }

    public static void Erro(string mensagem)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  [ERRO] {mensagem}");
        Console.ResetColor();
    }

    public static void Sucesso(string mensagem)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  [OK] {mensagem}");
        Console.ResetColor();
    }

    public static void Secao(string titulo)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n  ── {titulo.ToUpper()} {new string('─', Math.Max(0, 44 - titulo.Length))}");
        Console.ResetColor();
    }

    public static void NivelRisco(NivelRisco nivel, string extra = "")
    {
        Console.ForegroundColor = nivel switch
        {
            Domain.Enums.NivelRisco.Baixo   => ConsoleColor.Green,
            Domain.Enums.NivelRisco.Medio   => ConsoleColor.Yellow,
            Domain.Enums.NivelRisco.Alto    => ConsoleColor.DarkYellow,
            Domain.Enums.NivelRisco.Critico => ConsoleColor.Red,
            _                               => ConsoleColor.White
        };
        Console.WriteLine($"\n  *** NÍVEL: {nivel.ToString().ToUpper()} {extra} ***");
        Console.ResetColor();
    }

    public static void Linha() =>
        Console.WriteLine(new string('─', 60));

    public static void TituloMenu(string titulo)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"\n  {titulo}");
        Console.ResetColor();
        Linha();
    }
}
