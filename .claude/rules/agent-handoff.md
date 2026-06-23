# Protocolo de Handoff entre Agentes

Adaptado do aiox-core, simplificado para este repo. Objetivo: ao trocar de agente (`@lpd-dev` → `@lpd-integrator` → `@lpd-docs`), **compactar** o contexto do agente que sai em um artefato pequeno, em vez de carregar a persona inteira.

## Quando se aplica
Quando o usuário invoca um novo agente enquanto outro já estava ativo, OU quando um agente conclui sua parte e o trabalho precisa continuar em outro.

## Artefato de handoff (≤ ~400 tokens)
O agente que **sai** gera um bloco YAML mental e o registra em `.claude/handoffs/`:

```yaml
handoff:
  from_agent: lpd-integrator
  to_agent: lpd-dev
  context:
    task: "Mover validação de exit code para X"
    branch: master
  decisions:        # máx 5
    - "Manter contrato de CLI; só adicionar log extra"
  files_to_touch:   # máx 10
    - Program.cs
  blockers: []      # máx 3
  next_action: "Implementar e rodar /decrypt-roundtrip"
```

## O agente que entra recebe
1. Seu próprio perfil completo (`.claude/agents/<nome>.md`) + sua memória.
2. O artefato compacto acima.
3. **NÃO** recebe a persona/ferramentas completas do agente anterior.

## Sempre preservar
- O objetivo da tarefa, decisões já tomadas, arquivos-alvo, próximo passo, blockers.

## Nunca carregar adiante
- A persona inteira do agente anterior, histórico de tool calls, raciocínio intermediário.

## Armazenamento
- `.claude/handoffs/handoff-{from}-to-{to}-{timestamp}.yaml` (runtime, gitignored).
- O agente que entra marca o artefato como `consumed: true` após ler.

## Fluxos típicos
- `@lpd-integrator` (design) → `@lpd-dev` (implementa) → `@lpd-docs` (documenta).
- `@lpd-docs` acha inconsistência → `@lpd-dev` (corrige).
