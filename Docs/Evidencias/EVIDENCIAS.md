# Evidências de Execução — FAROL Orbital

**Global Solution 2026 · Space Connect · FIAP**

Esta pasta contém os prints reais de execução do projeto. As imagens demonstram que o sistema foi executado corretamente e evidenciam os principais requisitos da rubrica.

## Arquivos de evidência

| Arquivo | O que comprova |
|---|---|
| `01_build_sucesso_+_menu_principal.jpg` | Execução via `dotnet run`, carregamento do JSON orbital, banner do sistema e menu principal com as opções do Módulo VIGÍLIA. |
| `02_cameras_cadastradas.jpg` | Listagem das câmeras cadastradas, com ID, limiar de confiança e coordenadas geográficas. |
| `03_alerta_alto.jpg` | Simulação de alerta com nível ALTO, cálculo de score final e classificação do risco. |
| `04_alerta_critico.jpg` | Simulação de alerta com nível CRÍTICO, evidenciando classificação de risco elevado. |
| `05_alerta_medio.jpg` | Simulação de alerta com nível MÉDIO e registro do evento. |
| `06_alerta_baixo.jpg` | Simulação de alerta com nível BAIXO, incluindo uso de tipo de alerta por fumaça. |
| `07_historico_alertas.jpg` | Histórico de alertas registrados, com data e hora de criação. |
| `08_feedback_confirmado.jpg` | Registro de feedback confirmado pelo operador da Defesa Civil. |
| `09_feedback_negado.jpg` | Registro de feedback negado pelo operador da Defesa Civil. |
| `10_limiares_cameras.jpg` | Consulta dos limiares atuais das câmeras e contador de feedbacks. |
| `11.1_demo_ajuste_limiar.jpg` | Início da demo automática de 10 feedbacks para ajuste de limiar. |
| `11.2_demo_ajuste_limiar.jpg` | Continuação da demo automática, com alertas alternando entre incêndio e fumaça. |
| `11.3_demo_ajuste_limiar.jpg` | Continuação da demo automática e evidência de tratamento/fallback orbital. |
| `11.4_demo_ajuste_limiar.jpg` | Continuação da demo automática antes do fechamento do lote de feedbacks. |
| `11.5_demo_ajuste_limiar_opcao_sair.jpg` | Conclusão da demo automática, ajuste de limiar após lote de 10 feedbacks e encerramento da aplicação. |

## Mapeamento evidência × rubrica

| Requisito da rubrica | Evidenciado em |
|---|---|
| Sistema executa sem quebrar abruptamente | 01 |
| Struct `CoordenadaGeografica` | 02 |
| Níveis de alerta baixo, médio, alto e crítico | 03, 04, 05, 06 |
| Histórico com `DateTime` | 07 |
| Feedback confirmado e negado | 08, 09 |
| Partial class `CameraLocal` e limiar de confiança | 10 |
| Ajuste de limiar por feedback operacional | 11.1 a 11.5 |
| Tratamento de exceção/fallback orbital | 11.3 |
| Herança e polimorfismo em alertas ambientais | 03, 04, 06, 11.2 |
| Interfaces e injeção de dependência no fluxo | Logs de execução em 01, 03 e 11.x |
