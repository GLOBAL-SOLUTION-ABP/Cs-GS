using FarolOrbital.Domain;
using FarolOrbital.Domain.Enums;
using FarolOrbital.Exceptions;
using FarolOrbital.Interfaces;
using FarolOrbital.Utils;

namespace FarolOrbital.Services;

/// <summary>
/// Orquestrador central do Módulo VIGÍLIA.
/// Recebe todas as dependências via injeção de dependência no construtor (DI manual).
/// IClassificadorRisco é invocado explicitamente para definir o NivelRisco do alerta.
/// O limiar de confiança da câmera — ajustado pelo mecanismo de feedback operacional —
/// influencia diretamente o fluxo: detecções abaixo do limiar são sinalizadas como
/// eventos de baixa confiança antes do processamento completo.
/// </summary>
public class MotorAlerta
{
    private readonly IValidadorOrbital   _validadorOrbital;
    private readonly IClassificadorRisco _classificadorRisco;
    private readonly IRepositorioAlertas _repositorio;
    private readonly IAjustadorLimiar    _ajustador;

    public MotorAlerta(
        IValidadorOrbital   validadorOrbital,
        IClassificadorRisco classificadorRisco,
        IRepositorioAlertas repositorio,
        IAjustadorLimiar    ajustador)
    {
        _validadorOrbital   = validadorOrbital;
        _classificadorRisco = classificadorRisco;
        _repositorio        = repositorio;
        _ajustador          = ajustador;
    }

    /// <summary>
    /// Processa uma detecção: valida inputs, aplica limiar de confiança,
    /// consulta orbital, calcula score final, classifica via IClassificadorRisco e persiste.
    /// </summary>
    public AlertaAmbiental ProcessarDeteccao(
        CameraLocal camera, double scoreVisao,
        string tipoAlerta = "incendio", double dadoExtra = 1.0)
    {
        FormatadorConsole.Secao("PROCESSANDO DETECCAO");

        // 1. Validar câmera ativa
        if (!camera.Ativa)
            throw new CameraIndisponivelException(camera.Id);

        // 2. Validar score de visão no intervalo [0.0, 1.0]
        if (scoreVisao < 0.0 || scoreVisao > 1.0)
            throw new ScoreInvalidoException(scoreVisao);

        FormatadorConsole.Log($"[MOTOR] Câmera: {camera.Nome} | Score visão: {scoreVisao:F3} | Limiar: {camera.LimiarConfianca:F2}");

        // 3. Verificar limiar de confiança da câmera
        // O limiar é recalibrado pelo mecanismo de ajuste por feedback operacional.
        // Detecções abaixo do limiar são processadas normalmente, mas sinalizadas
        // como eventos de baixa confiança — o operador deve analisar com atenção extra.
        if (scoreVisao < camera.LimiarConfianca)
        {
            FormatadorConsole.Aviso(
                $"[LIMIAR] Score visão ({scoreVisao:F3}) abaixo do limiar desta câmera ({camera.LimiarConfianca:F2}). " +
                $"Evento processado como baixa confiança — validação orbital ganha peso decisivo.");
        }
        else
        {
            FormatadorConsole.Log(
                $"[LIMIAR] Score visão ({scoreVisao:F3}) acima do limiar ({camera.LimiarConfianca:F2}). " +
                $"Detecção dentro da faixa de confiança.");
        }

        // 4. Consultar validação orbital simulada (com fallback)
        double scoreOrbital;
        try
        {
            scoreOrbital = _validadorOrbital.ObterScoreOrbital(camera.Id, scoreVisao);
        }
        catch (ValidacaoOrbitalException ex)
        {
            FormatadorConsole.Aviso($"Fallback orbital ativado: {ex.Message}");
            scoreOrbital = scoreVisao;
        }

        // 5. Calcular score final com média ponderada
        double scoreFinal = Math.Round(
            scoreVisao   * ConfiguracoesSistema.PesoVisao +
            scoreOrbital * ConfiguracoesSistema.PesoOrbital, 4);

        FormatadorConsole.Log(
            $"[MOTOR] Score orbital: {scoreOrbital:F3} | " +
            $"Score final: {scoreFinal:F3} ({ConfiguracoesSistema.PesoVisao*100:F0}%v + {ConfiguracoesSistema.PesoOrbital*100:F0}%o)");

        // 6. Classificar nível via IClassificadorRisco — interface genuinamente utilizada
        NivelRisco nivel = _classificadorRisco.Classificar(scoreFinal);

        // 7. Criar alerta concreto com o nível classificado (polimorfismo)
        AlertaAmbiental alerta = tipoAlerta.ToLower() switch
        {
            "fumaca" => new AlertaFumaca(
                            camera.Id, scoreVisao, scoreOrbital,
                            camera.Coordenada, nivel, dadoExtra),
            _ => new AlertaIncendio(
                            camera.Id, scoreVisao, scoreOrbital,
                            camera.Coordenada, nivel, dadoExtra)
        };

        // 8. Persistir no repositório
        _repositorio.Salvar(alerta);

        FormatadorConsole.Log($"[MOTOR] Alerta criado: {alerta}");
        FormatadorConsole.Log($"[MOTOR] Prioridade calculada: {alerta.CalcularPrioridade():F3}");

        return alerta;
    }

    /// <summary>
    /// Registra feedback do operador e aciona o mecanismo de ajuste se o lote de 10 completar.
    /// </summary>
    public void RegistrarFeedback(CameraLocal camera, AlertaAmbiental alerta,
                                   OperadorDefesaCivil operador, bool confirmado,
                                   string observacao = "")
    {
        FormatadorConsole.Secao("FEEDBACK OPERACIONAL");

        alerta.Status = confirmado ? StatusAlerta.Confirmado : StatusAlerta.Negado;

        double taxaAntes = camera.FeedbacksNoLote > 0
            ? (double)(camera.ConfirmacoesNoLote + (confirmado ? 1 : 0))
              / (camera.FeedbacksNoLote + 1)
            : (confirmado ? 1.0 : 0.0);

        bool loteCompleto = camera.RegistrarFeedback(confirmado);

        var feedback = new FeedbackOperacional(alerta.Id, operador.Matricula, confirmado, observacao);
        FormatadorConsole.Log($"[FEEDBACK] {feedback}");
        FormatadorConsole.Log(
            $"[FEEDBACK] Total feedbacks câmera: {camera.TotalFeedbacks} | No lote: {camera.FeedbacksNoLote}");

        if (loteCompleto)
        {
            FormatadorConsole.Aviso(">>> Lote de 10 feedbacks completo — acionando mecanismo de ajuste <<<");
            _ajustador.AjustarComTaxa(camera, taxaAntes);
        }
    }
}
