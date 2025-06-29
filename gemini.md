# WolfGod RPG - Mod para Terraria

## 📋 Índice
1. [Visão Geral](#visão-geral)
2. [Arquitetura do Mod](#arquitetura-do-mod)
3. [Sistemas Principais](#sistemas-principais)
4. [Sistema de Debug e Logging](#sistema-de-debug-e-logging)
5. [UI e Interface](#ui-e-interface)
6. [Progression e Balanceamento](#progression-e-balanceamento)
7. [Vitals e Status](#vitals-e-status)
8. [Item Stats e Evolução](#item-stats-e-evolução)
9. [Compatibilidade e Multiplayer](#compatibilidade-e-multiplayer)
10. [Melhores Práticas](#melhores-práticas)
11. [Debugging e Troubleshooting](#debugging-e-troubleshooting)
12. [Recursos e Links](#recursos-e-links)

---

## 🎯 Visão Geral
WolfGod RPG é um mod para Terraria que adiciona elementos de RPG profundos com progressão baseada em ações e sistemas totalmente configuráveis. O mod foi desenvolvido com foco em **balanceamento** e **progressão natural**, mantendo a essência do Terraria enquanto adiciona profundidade RPG.

**Versão atual:** 0.6  
**Autor:** WolfGod  
**Compatibilidade:** Cliente e Servidor (Both)

### Filosofia de Design
- **Balanceamento Equilibrado**: Multiplicadores de dano máximos (3x) para evitar números astronômicos
- **Progressão Natural**: Baseada em habilidade e uso, não em farming
- **Desafio Mantido**: Scaling dinâmico mantém o desafio em todas as fases
- **Compatibilidade**: Compatível com a progressão natural do Terraria

## 🏗️ Arquitetura do Mod

### Estrutura de Pastas
```
Wolfgodrpg/
├── Common/
│   ├── Classes/          # Definições de classes RPG
│   ├── GlobalItems/      # Modificações globais de itens
│   ├── GlobalNPCs/       # Modificações globais de NPCs
│   ├── Players/          # ModPlayer e lógica do jogador
│   ├── Systems/          # Sistemas centrais (Debug, Config, etc.)
│   └── UI/              # Interface de usuário
├── Assets/              # Texturas e recursos
└── Localization/        # Arquivos de tradução
```

### Componentes Principais
- **RPGPlayer**: Gerencia classes, vitals e progressão do jogador
- **BalancedNPC**: Sistema de scaling dinâmico para NPCs
- **ProgressiveItem**: Sistema de evolução de itens
- **SimpleRPGMenu**: Interface principal do mod
- **DebugLog**: Sistema centralizado de logging

## 🔧 Sistemas Principais

### Sistema de Classes
- **Classes Disponíveis**: Melee, Ranged, Magic, Summoner, Defense
- **Progressão**: XP baseado em ações específicas
- **Habilidades**: Desbloqueadas em milestones
- **Balanceamento**: Multiplicadores configuráveis

### Sistema de Vitals
- **Fome**: Decai com o tempo, afeta regeneração
- **Sanidade**: Afetada por combate e áreas perigosas
- **Stamina**: Consumida por ações especiais (dash, etc.)

### Sistema de Itens
- **Evolução**: Itens ganham XP e evoluem
- **Stats Aleatórios**: Bônus aleatórios em itens
- **Tooltips**: Informações detalhadas de progresso

## 🐛 Sistema de Debug e Logging

### DebugLog - Sistema Centralizado
O mod implementa um sistema robusto de logging para facilitar desenvolvimento e debugging:

#### Métodos Disponíveis
```csharp
// Logs gerais
DebugLog.Info("Player", "GainClassExp", "Mensagem de informação");
DebugLog.Warn("NPC", "OnSpawn", "Aviso importante");
DebugLog.Error("UI", "SetPage", "Erro crítico", exception);

// Logs específicos por área
DebugLog.Player("Initialize", "Jogador inicializado");
DebugLog.NPC("OnKill", "NPC morto, XP distribuído");
DebugLog.Item("GainExperience", "Item ganhou XP");
DebugLog.UI("SetPage", "Aba trocada");
DebugLog.System("SaveData", "Dados salvos");

// Logs de gameplay
DebugLog.Gameplay("Player", "GainClassExp", "LEVEL UP! Classe aumentou");
```

#### Configuração de Logs
```csharp
// Ativar/desativar logs por área
DebugLog.SetLogging("player", true);
DebugLog.SetLogging("ui", false);

// Controles globais
DebugLog.EnableAllLogs();
DebugLog.DisableAllLogs();
```

#### Padrão de Logs
Cada log segue o formato:
```
[WolfGodRPG][Área][Método] Mensagem
[WolfGodRPG][Player][GainClassExp] Classe: melee, XP ganho: 50, Level: 2->3
```

#### Pontos de Log Instrumentados
- **Player**: Inicialização, ganho de XP, level up, mudanças de status
- **NPC**: Spawn, morte, distribuição de XP, scaling
- **Item**: Ganho de XP, level up, modificação de stats
- **UI**: Abertura/fechamento, troca de abas, atualizações
- **System**: Save/load, configurações, eventos globais

## 🖥️ UI e Interface

### Arquitetura da UI
- **SimpleRPGMenu**: Menu principal com sistema de abas, modular e fácil de estender
- **Sub-UIs**: Cada aba é um componente independente (Status, Classes, Itens, Progresso)
- **Inicialização Tardia**: UI carregada em `PostSetupContent`
- **Estado Centralizado**: Gerenciamento de estado no menu principal

### Novos Padrões de UI (tModLoader 1.4+)

#### ✅ Estrutura Recomendada
```csharp
// Menu principal
public class SimpleRPGMenu : UIState {
    // ...
    private List<UIElement> _pages;
    private List<UITextPanel<string>> _tabButtons;
    // ...
    public override void OnInitialize() {
        // Criação dos containers e abas
        // Uso de OnLeftClick para eventos de botão
    }
    private void SetPage(MenuPage page) {
        // Troca de aba sem reconstrução excessiva
        // Atualiza apenas a página ativa
    }
}

// Exemplo de aba modular
public class RPGStatsPageUI : UIElement {
    private UIElement _statsContainer;
    public override void OnInitialize() {
        // Criação do container
    }
    public void UpdateStats(RPGPlayer modPlayer) {
        // Atualização segura, checagem de null
    }
}
```

#### ✅ Boas Práticas para UI
- **Cada aba é um UIElement independente**
- **Atualização só quando necessário** (ex: ao trocar de aba)
- **Checagem de null para jogador e dados**
- **Uso de eventos corretos (`OnLeftClick`) para botões**
- **Sem reconstrução excessiva da UI**
- **Fácil extensão: para adicionar uma nova aba, basta criar um novo UIElement e adicionar à lista**
- **Comentários claros e código limpo**

#### ✅ Checklist de UI
- [x] Menu principal modular (UIState)
- [x] Abas independentes (UIElement)
- [x] Atualização segura (null checks)
- [x] Uso correto de eventos
- [x] Fácil extensão
- [x] Sem reconstrução excessiva

#### Exemplo de extensão:
```csharp
// Para adicionar uma nova aba:
public class RPGQuestsPageUI : UIElement { /* ... */ }
// No SimpleRPGMenu:
_pages.Add(new RPGQuestsPageUI());
_tabButtons.Add(new UITextPanel<string>("Quests", ...));
```

## 📈 Progression e Balanceamento

### Sistema de XP
- **Base**: 100 XP para nível 1
- **Multiplicador**: 1.1x por nível
- **Configuração**: Multiplicadores ajustáveis via config

### Scaling de NPCs
- **Baseado no Jogador**: Nível médio dos jogadores próximos
- **Progressão do Mundo**: Hardmode, bosses derrotados
- **Elite NPCs**: 5% de chance, bônus significativos

### Balanceamento de Itens
- **Evolução**: XP baseado no uso e dano causado
- **Bônus**: Dano, velocidade de ataque, crit
- **Limites**: Máximo de 50 níveis, bônus limitados

## ❤️ Vitals e Status

### Sistema de Fome
- **Decaimento**: 0.01 por segundo (configurável)
- **Efeitos**: Sem regeneração abaixo de 20%
- **Restauração**: Comida e itens específicos

### Sistema de Sanidade
- **Regeneração**: 0.05 por segundo durante o dia
- **Perda**: 0.1 por segundo em combate
- **Efeitos**: Penalidades em valores baixos

### Sistema de Stamina
- **Regeneração**: 15 por segundo
- **Consumo**: Dash e ações especiais
- **Delay**: 1 segundo após consumo

## ⚔️ Item Stats e Evolução

### Stats Aleatórios
- **Geração**: Ao spawn do item
- **Raridade**: Baseada na raridade do item
- **Tipos**: Dano, velocidade, crit, etc.

### Sistema de Evolução
- **XP por Uso**: Baseado no dano causado
- **Multiplicadores**: Crit (1.5x), Boss (3x), Elite (2x)
- **Bônus Progressivos**: Dano, velocidade, crit

### Tooltips Informativos
- Nível atual e progresso
- Bônus aplicados
- Estatísticas de uso

## 🌐 Compatibilidade e Multiplayer

### Hooks Implementados
- **Save/Load**: Dados persistentes
- **Networking**: Sincronização multiplayer
- **Persistence**: Manutenção de estado entre sessões

### Melhores Práticas
- Verificação de `Main.netMode` para operações multiplayer
- Uso de `TagCompound` para serialização
- Tratamento de exceções em operações críticas

## ✅ Melhores Práticas

### Código Limpo
- **Nomes Descritivos**: Variáveis e métodos claros
- **Comentários**: Explicações para lógica complexa
- **Modularização**: Funções pequenas e focadas

### Performance
- **Lazy Loading**: Assets carregados quando necessário
- **Caching**: Evitar recálculos desnecessários
- **Otimização**: Logs condicionais e verificações eficientes

### Debugging
- **Logs Detalhados**: Rastreamento de eventos importantes
- **Tratamento de Erros**: Try-catch em operações críticas
- **Validação**: Verificação de dados antes do uso

## 🔍 Debugging e Troubleshooting

### Problemas Comuns e Soluções

#### 1. UI Não Funciona
**Sintomas**: Menu não abre, botões não respondem
**Causas**: Inicialização prematura, estado incorreto, Activate() manual
**Solução**: 
- Verificar logs de UI
- Garantir inicialização em `PostSetupContent`
- **NÃO chamar Activate() manualmente**
- Seguir padrão correto do tModLoader

#### 2. IndexOutOfRangeException
**Sintomas**: Erro ao acessar `Main.LocalPlayer`
**Causas**: Jogador não inicializado
**Solução**: Sempre verificar `Main.LocalPlayer != null && Main.LocalPlayer.active`

#### 3. KeyNotFoundException
**Sintomas**: Erro ao acessar dicionários
**Causas**: Chave não existe
**Solução**: Usar `TryGetValue` ou inicializar chaves necessárias

#### 4. UI Tremendo/Travando
**Sintomas**: Interface instável, FPS baixo
**Causas**: Reconstrução excessiva da UI, logs excessivos
**Solução**: 
- Atualizar apenas quando necessário, não a cada frame
- Reduzir logs excessivos
- Usar estrutura de UI simples e eficiente

#### 5. Abas Não Carregam
**Sintomas**: Algumas abas não mostram conteúdo
**Causas**: Inicialização incorreta, logs excessivos, verificações complexas
**Solução**:
- Seguir padrão simples das abas funcionais
- Remover logs excessivos
- Simplificar verificações

#### 6. AssetLoadException
**Sintomas**: Erro ao carregar texturas
**Causas**: Arquivo não encontrado, caminho incorreto
**Solução**:
- Verificar se arquivo existe em `Assets/`
- Usar caminho correto: `"Wolfgodrpg/Assets/UI/arquivo"`
- Verificar extensão do arquivo

### Checklist de Debug
- [ ] Verificar logs do sistema DebugLog
- [ ] Confirmar inicialização correta dos sistemas
- [ ] Testar em mundo limpo
- [ ] Verificar compatibilidade com outros mods
- [ ] Validar dados de save/load
- [ ] **Verificar se não há Activate() manual**
- [ ] **Confirmar estrutura de UI simples**
- [ ] **Testar todas as abas individualmente**

### Comandos Úteis
```csharp
// Ativar todos os logs para debug
DebugLog.EnableAllLogs();

// Desativar logs de UI para performance
DebugLog.SetLogging("ui", false);

// Log específico para investigação
DebugLog.Info("System", "Debug", "Verificando estado do sistema");
```

### Script de Debug Automatizado
```bash
# Executar script de debug
./debug_logs.bat

# Ver logs em tempo real
tail -f "C:\Users\tatal\Documents\My Games\Terraria\tModLoader\Logs\client.log" | grep "WolfGodRPG"
```

## 📚 Recursos e Links

### Documentação Oficial
- [tModLoader Wiki](https://github.com/tModLoader/tModLoader/wiki)
- [ExampleMod](https://github.com/tModLoader/tModLoader/tree/master/ExampleMod)
- [tModLoader Discord](https://discord.gg/tmodloader)

### Tutoriais Recomendados
- [Modding Guide](https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide)
- [UI Tutorial](https://github.com/tModLoader/tModLoader/wiki/UI-Development)
- [Multiplayer Guide](https://github.com/tModLoader/tModLoader/wiki/Multiplayer-Support)

### Ferramentas Úteis
- [tModLoader](https://github.com/tModLoader/tModLoader/releases)
- [Visual Studio](https://visualstudio.microsoft.com/)
- [ILSpy](https://github.com/icsharpcode/ILSpy) (para decompilar Terraria)

### Comunidade
- [tModLoader Forums](https://forums.terraria.org/index.php?forums/tmodloader.161/)
- [Reddit r/tModLoader](https://www.reddit.com/r/tModLoader/)
- [GitHub Issues](https://github.com/tModLoader/tModLoader/issues)

## 🚀 Próximos Passos

### Funcionalidades Planejadas
- [ ] Sistema de quests
- [ ] Mais classes e habilidades
- [ ] Sistema de crafting avançado
- [ ] Bosses customizados
- [ ] Sistema de guildas

### Melhorias Técnicas
- [ ] Otimização de performance
- [ ] Mais opções de configuração
- [ ] Sistema de achievements
- [ ] Integração com outros mods

### Manutenção
- [ ] Atualização para novas versões do tModLoader
- [ ] Correção de bugs reportados
- [ ] Melhoria da documentação
- [ ] Testes de compatibilidade

---

*Última atualização: Dezembro 2024*
*Versão do mod: 0.6*
*Compatível com tModLoader: v2023.01.100* 