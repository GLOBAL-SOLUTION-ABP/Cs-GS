using FarolOrbital.Domain;
using FarolOrbital.Domain.Enums;
using FarolOrbital.Domain.Structs;
using FarolOrbital.Exceptions;
using FarolOrbital.Repositories;
using FarolOrbital.Services;
using FarolOrbital.Utils;

// ═══════════════════════════════════════════════════════════════
// FAROL Orbital — Módulo VIGÍLIA
// Console App .NET 8  |  Global Solution 2026  |  FIAP
// ═══════════════════════════════════════════════════════════════

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine(ConfiguracoesSistema.Banner());

// ── Injeção de dependência manual (DI sem framework) ─────────
var validador    = new ValidacaoOrbitalSimulada();
var classificador = new MotorClassificacaoRisco();
var repositorio   = new RepositorioAlertasMemoria();
var ajustador     = new AjustadorLimiarFeedback();
var motor         = new MotorAlerta(validador, classificador, repositorio, ajustador);

// ── Câmeras cadastradas ───────────────────────────────────────
var cameras = new List<CameraLocal>
{
    new("CAM-001", "Camera Norte — Floresta Amazonica",
        new CoordenadaGeografica(-3.4653, -62.2159, "AM - Zona Norte"), 0.55),

    new("CAM-002", "Camera Sul — Regiao Litoral",
        new CoordenadaGeografica(-25.4284, -49.2733, "PR - Curitiba"), 0.50),

    new("CAM-003", "Camera Centro — Cerrado",
        new CoordenadaGeografica(-15.7801, -47.9292, "DF - Brasilia"), 0.60),

    new("CAM-004", "Camera Leste — Vale do Rio",
        new CoordenadaGeografica(-19.9167, -43.9345, "MG - Belo Horizonte"), 0.50),
};

// ── Operador logado ───────────────────────────────────────────
var operador = new OperadorDefesaCivil("OP-001", "Ana Lima");
AlertaAmbiental? ultimoAlerta = null;

// ── Menu principal ────────────────────────────────────────────
bool rodando = true;
while (rodando)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n\n  ══════════════════════════════════════");
    Console.WriteLine("       MENU PRINCIPAL — MODULO VIGILIA");
    Console.WriteLine("  ══════════════════════════════════════");
    Console.ResetColor();
    Console.WriteLine("  1. Listar cameras cadastradas");
    Console.WriteLine("  2. Simular deteccao (gerar alerta)");
    Console.WriteLine("  3. Listar alertas registrados");
    Console.WriteLine("  4. Registrar feedback do operador");
    Console.WriteLine("  5. Mostrar limiar atual das cameras");
    Console.WriteLine("  6. Demo automatica (10 feedbacks — ajuste de limiar)");
    Console.WriteLine("  0. Sair");
    Console.Write("\n  Opcao: ");

    string entrada = Console.ReadLine() ?? "0";

    try
    {
        switch (entrada.Trim())
        {
            case "1": MenuListarCameras(); break;
            case "2": ultimoAlerta = MenuSimularDeteccao(); break;
            case "3": MenuListarAlertas(); break;
            case "4": MenuRegistrarFeedback(ref ultimoAlerta); break;
            case "5": MenuMostrarLimiares(); break;
            case "6": MenuDemoAutomatica(); break;
            case "0":
                FormatadorConsole.Sucesso("Encerrando FAROL Orbital. Ate logo!");
                rodando = false;
                break;
            default:
                FormatadorConsole.Aviso("Opcao invalida. Tente novamente.");
                break;
        }
    }
    catch (ScoreInvalidoException ex)
    {
        FormatadorConsole.Erro($"Score invalido: {ex.Message}");
    }
    catch (CameraIndisponivelException ex)
    {
        FormatadorConsole.Erro($"Camera indisponivel: {ex.Message}");
    }
    // Nota: ValidacaoOrbitalException é capturada e tratada internamente em
    // MotorAlerta.ProcessarDeteccao() com fallback automático — não propaga até aqui.
    catch (Exception ex)
    {
        FormatadorConsole.Erro($"Erro inesperado: {ex.Message}");
    }
}

// ═══════════════════════════════════════════════════════════════
// METODOS DO MENU
// ═══════════════════════════════════════════════════════════════

void MenuListarCameras()
{
    FormatadorConsole.TituloMenu("CAMERAS CADASTRADAS");
    foreach (var cam in cameras)
    {
        Console.ForegroundColor = cam.Ativa ? ConsoleColor.White : ConsoleColor.DarkGray;
        Console.WriteLine($"  {cam}");
        Console.ResetColor();
    }
}

AlertaAmbiental? MenuSimularDeteccao()
{
    FormatadorConsole.TituloMenu("SIMULAR DETECCAO");

    // Escolher câmera
    for (int i = 0; i < cameras.Count; i++)
        Console.WriteLine($"  {i + 1}. {cameras[i].Nome}");

    Console.Write("  Escolha a camera (1-4): ");
    if (!int.TryParse(Console.ReadLine(), out int idxCam) || idxCam < 1 || idxCam > cameras.Count)
    {
        FormatadorConsole.Aviso("Escolha invalida."); return null;
    }
    var camera = cameras[idxCam - 1];

    // Escolher tipo (MVP: Incendio e Fumaca — ambos detectáveis por câmera local)
    Console.Write("  Tipo de alerta (1-Incendio / 2-Fumaca): ");
    string tipoStr = Console.ReadLine() ?? "1";
    string tipo = tipoStr == "2" ? "fumaca" : "incendio";

    // Score de visão
    Console.Write("  Score de visao computacional (0.0 a 1.0): ");
    if (!double.TryParse(Console.ReadLine()?.Replace(',', '.'),
        System.Globalization.NumberStyles.Float,
        System.Globalization.CultureInfo.InvariantCulture, out double score))
    {
        FormatadorConsole.Aviso("Score invalido."); return null;
    }

    // Dado extra específico por tipo
    Console.Write(tipo == "fumaca"
        ? "  Densidade de fumaca estimada (0.0 a 1.0, ex: 0.7): "
        : "  Indicador de calor (ex: 1.0): ");
    double.TryParse(Console.ReadLine()?.Replace(',', '.'),
        System.Globalization.NumberStyles.Float,
        System.Globalization.CultureInfo.InvariantCulture, out double dadoExtra);
    if (dadoExtra == 0) dadoExtra = tipo == "fumaca" ? 0.5 : 1.0;

    var alerta = motor.ProcessarDeteccao(camera, score, tipo, dadoExtra);

    FormatadorConsole.NivelRisco(alerta.Nivel,
        $"| Score Final: {alerta.ScoreFinal:F3} | Prioridade: {alerta.CalcularPrioridade():F3}");
    FormatadorConsole.Sucesso($"Alerta ID: {alerta.Id.ToString()[..8]}...");

    return alerta;
}

void MenuListarAlertas()
{
    FormatadorConsole.TituloMenu("HISTORICO DE ALERTAS");
    var todos = repositorio.ListarTodos();
    if (todos.Count == 0)
    {
        FormatadorConsole.Aviso("Nenhum alerta registrado ainda."); return;
    }
    foreach (var a in todos)
    {
        Console.ForegroundColor = a.Nivel switch
        {
            NivelRisco.Critico => ConsoleColor.Red,
            NivelRisco.Alto    => ConsoleColor.DarkYellow,
            NivelRisco.Medio   => ConsoleColor.Yellow,
            _                  => ConsoleColor.Green
        };
        Console.WriteLine($"  {a}");
        Console.ResetColor();
    }
    Console.WriteLine($"\n  Total: {todos.Count} alerta(s)");
}

void MenuRegistrarFeedback(ref AlertaAmbiental? alerta)
{
    FormatadorConsole.TituloMenu("REGISTRAR FEEDBACK");

    var todos = repositorio.ListarTodos()
        .Where(a => a.Status == StatusAlerta.Pendente).ToList();

    if (todos.Count == 0)
    {
        FormatadorConsole.Aviso("Nenhum alerta pendente de feedback."); return;
    }

    for (int i = 0; i < todos.Count; i++)
        Console.WriteLine($"  {i + 1}. [{todos[i].Id.ToString()[..8]}] {todos[i].ObterDescricao()} | {todos[i].Nivel}");

    Console.Write("  Escolha o alerta: ");
    if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > todos.Count)
    {
        FormatadorConsole.Aviso("Escolha invalida."); return;
    }

    var alertaSelecionado = todos[idx - 1];
    var cam = cameras.First(c => c.Id == alertaSelecionado.CameraId);

    Console.Write("  Confirmar alerta? (s/n): ");
    bool confirmado = (Console.ReadLine() ?? "n").Trim().ToLower() == "s";

    Console.Write("  Observacao (opcional, Enter para pular): ");
    string obs = Console.ReadLine() ?? "";

    motor.RegistrarFeedback(cam, alertaSelecionado, operador, confirmado, obs);
    FormatadorConsole.Sucesso($"Feedback registrado: {(confirmado ? "CONFIRMADO" : "NEGADO")}");
    alerta = alertaSelecionado;
}

void MenuMostrarLimiares()
{
    FormatadorConsole.TituloMenu("LIMIARES ATUAIS DAS CAMERAS");
    foreach (var cam in cameras)
    {
        Console.Write($"  {cam.Nome,-40} Limiar: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{cam.LimiarConfianca:F2}  | Feedbacks: {cam.TotalFeedbacks}");
        Console.ResetColor();
    }
}

void MenuDemoAutomatica()
{
    FormatadorConsole.TituloMenu("DEMO AUTOMATICA — AJUSTE DE LIMIAR (10 FEEDBACKS)");

    var camDemo  = cameras[0];
    var rng      = new Random(42);
    int gerados  = 0;

    Console.WriteLine($"\n  Gerando 10 deteccoes e feedbacks para: {camDemo.Nome}");
    Console.WriteLine($"  Limiar inicial: {camDemo.LimiarConfianca:F2}\n");

    for (int i = 1; i <= 10; i++)
    {
        double scoreVisao = Math.Round(rng.NextDouble() * 0.6 + 0.3, 3); // 0.30 a 0.90
        string tipoDemo   = i % 3 == 0 ? "fumaca" : "incendio"; // alterna tipos
        double extraDemo  = tipoDemo == "fumaca" ? 0.7 : 1.0;
        // ValidacaoOrbitalException já é tratada internamente por MotorAlerta (fallback)
        // e não propaga para cá — nenhum try/catch extra necessário.
        var alertaDemo = motor.ProcessarDeteccao(camDemo, scoreVisao, tipoDemo, extraDemo);
        bool conf = rng.NextDouble() > 0.65; // ~35% confirmacao (baixa taxa → limiar sobe)
        motor.RegistrarFeedback(camDemo, alertaDemo, operador, conf);
        gerados++;
        Console.WriteLine($"  [{i:D2}] {tipoDemo.ToUpper(),-8} Score: {scoreVisao:F3} | Nivel: {alertaDemo.Nivel,-8} | Feedback: {(conf ? "CONF" : "NEG ")}");
    }

    Console.WriteLine();
    FormatadorConsole.Sucesso($"Demo concluida. {gerados} deteccoes processadas.");
    FormatadorConsole.Sucesso($"Limiar final de {camDemo.Nome}: {camDemo.LimiarConfianca:F2}");
}
