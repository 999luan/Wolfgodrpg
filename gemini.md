# WolfGod RPG - Mod para Terraria

## Visão Geral
WolfGod RPG é um mod para Terraria que adiciona elementos de RPG profundos com progressão baseada em ações e sistemas totalmente configuráveis. O mod foi desenvolvido com foco em **balanceamento** e **progressão natural**, mantendo a essência do Terraria enquanto adiciona profundidade RPG.

**Versão atual:** 0.3  
**Autor:** WolfGod  
**Compatibilidade:** Cliente e Servidor (Both)

## Filosofia de Design
- **Balanceamento Equilibrado**: Multiplicadores de dano máximos (3x) para evitar números astronômicos
- **Progressão Natural**: Baseada em habilidade e uso, não em farming
- **Desafio Mantido**: Scaling dinâmico mantém o desafio em todas as fases
- **Compatibilidade**: Compatível com a progressão natural do Terraria

## Arquitetura do Sistema

### 1. Classe Principal: `Wolfgodrpg.cs`
```csharp
public class Wolfgodrpg : Mod
{
    public static readonly string RPG_VERSION = "1.0.0";
    public static Wolfgodrpg Instance { get; private set; }
}
```

**Métodos principais:**
- `Load()`: Inicializa o mod e sistemas RPG
- `Unload()`: Limpa referências estáticas
- `PostSetupContent()`: Adiciona callbacks para XP de criação
- `LogRPGSystems()`: Registra a inicialização dos sistemas RPG

### 2. Sistema de Jogador: `RPGPlayer.cs`
```csharp
public class RPGPlayer : ModPlayer
{
    // Sistema de Classes
    public Dictionary<string, float> ClassLevels = new Dictionary<string, float>();
    public Dictionary<string, float> ClassExperience = new Dictionary<string, float>();
    public HashSet<string> UnlockedAbilities = new HashSet<string>();

    // Vitals do Jogador
    public float CurrentHunger = 100f;
    public float MaxHunger = 100f;
    public float CurrentSanity = 100f;
    public float MaxSanity = 100f;
    public float CurrentStamina = 100f;
    public float MaxStamina = 100f;
}
```

**Métodos principais:**
- `GainClassExp(string className, float amount)`: Ganha experiência para uma classe
- `ConsumeStamina(float amount)`: Consome stamina do jogador
- `UnlockAbility(string ability)`: Desbloqueia novas habilidades
- `CalculateLevelFromXP(float xp)`: Calcula nível baseado na experiência
- `CheckForNewAbilities(string className, float level)`: Verifica novas habilidades

### 3. Sistema de Classes: `RPGClassDefinitions.cs`
Define 13 classes baseadas em ações:

**Classes Principais:**
1. **movement** - Movimento e mobilidade
2. **jumping** - Pulos e controle aéreo
3. **melee** - Combate corpo a corpo
4. **ranged** - Combate à distância
5. **magic** - Magia e artes arcanas
6. **summon** - Invocação de minions
7. **mining** - Mineração e escavação
8. **building** - Construção e arquitetura
9. **fishing** - Pesca e aquicultura
10. **gathering** - Coleta de recursos
11. **bestiary** - Conhecimento de criaturas
12. **merchant** - Comércio e economia
13. **defense** - Defesa e sobrevivência

**Estrutura de Classe:**
```csharp
public class ClassInfo
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Dictionary<string, float> StatBonuses { get; private set; }
    public Dictionary<int, string> Milestones { get; private set; }
}
```

### 4. Sistema de Cálculos: `RPGCalculations.cs`
```csharp
public static class RPGCalculations
{
    public static Dictionary<string, float> CalculateTotalStats(RPGPlayer modPlayer)
    public static void ApplyStatsToPlayer(Player player, Dictionary<string, float> stats)
}
```

**Stats Suportados:**
- **Ofensivo**: meleeDamage, rangedDamage, magicDamage, minionDamage, critChance
- **Defensivo**: defense, maxLife, lifeRegen, damageReduction
- **Utilidade**: moveSpeed, jumpHeight, maxMana, manaRegen, miningSpeed
- **Especiais**: luck, expGain

### 5. Sistema de Itens: `RPGGlobalItem.cs`
```csharp
public class RPGGlobalItem : GlobalItem
{
    public Dictionary<string, float> randomStats = new Dictionary<string, float>();
}
```

**Raridades de Item:**
- **Common** (0): Branco - 1-2 stats
- **Uncommon** (1): Verde - 2-3 stats
- **Rare** (2): Azul - 3-4 stats
- **Epic** (3): Roxo - 4-5 stats
- **Legendary** (4): Laranja - 5-6 stats

### 6. Sistema de Configuração: `RPGConfig.cs`
```csharp
public class RPGConfig : ModConfig
{
    // Experiência
    public float ExpMultiplier = 1.0f;
    public bool KeepXPOnDeath = false;
    
    // Vitals
    public bool EnableHunger = true;
    public float HungerRate = 1.0f;
    public bool EnableSanity = true;
    public float SanityRate = 1.0f;
    
    // Itens
    public bool RandomStats = true;
    public float ItemStatMultiplier = 1.0f;
    
    // Dificuldade
    public float MonsterHealthMultiplier = 1.0f;
    public float MonsterDamageMultiplier = 1.0f;
    
    // Níveis
    public int MaxLevel = 100;
    public int StartingLevel = 1;
}
```

### 7. Sistema de Interface

O sistema de interface foi modularizado e refatorado para uma melhor experiência do usuário e consistência visual.

#### Componentes Base de UI:
- `RPGPanel.cs`: Um `UIElement` base para painéis com suporte a 9-slice scaling, utilizando `uibg.png` para o fundo.
- `RPGButton.cs`: Um `UIElement` para botões customizados, utilizando `ButtonNext.png` e `ButtonPrevious.png` para texturas.
- `RPGTabButton.cs`: Um `UIElement` especializado para botões de aba, com estados visualmente distintos para selecionado/não selecionado.

#### UIs Principais:
- `SimpleRPGMenu.cs`: O menu principal do mod, agora refatorado para usar um sistema de abas para navegação entre as páginas.
  - **Páginas Modulares:** O conteúdo de cada aba é gerenciado por `UIElement`s dedicados:
    - `RPGStatsPageUI.cs`: Exibe os atributos do personagem.
    - `RPGClassesPageUI.cs`: Exibe informações sobre classes e habilidades.
    - `RPGItemsPageUI.cs`: Exibe itens com atributos aleatórios.
    - `RPGProgressPageUI.cs`: Exibe o progresso do jogo (chefes derrotados, etc.).
- `RPGStatsUI.cs`: Mostra barras de Fome, Sanidade e Stamina, agora utilizando `RPGPanel` para consistência visual.
- `QuickStatsUI.cs`: Interface rápida para stats essenciais, agora utilizando `RPGPanel` para consistência visual.


### 8. Sistema de Teclas: `RPGKeybinds.cs`
```csharp
public class RPGKeybinds : ModSystem
{
    public static ModKeybind OpenRPGMenuKeybind { get; private set; }
    public static ModKeybind NextPageKeybind { get; private set; }
    public static ModKeybind PreviousPageKeybind { get; private set; }
}
```

## Controles e Interface

### Teclas de Atalho:
- **M**: Abrir/Fechar Menu RPG completo
- **ESC**: Fechar menu RPG
- **R**: Stats rápidos no chat

### Interface de Usuário:
- **RPGStatsUI**: Mostra barras de Fome, Sanidade e Stamina
- **SimpleRPGMenu**: Menu completo com todas as informações, agora com navegação por abas.
- **QuickStatsUI**: Interface rápida para stats essenciais

## Sistema de Progressão

### Experiência e Níveis:
- **Fórmula de XP**: `BASE_XP_NEEDED = 100f`, `XP_MULTIPLIER = 1.1f`
- **Cálculo de Nível**: Progressão exponencial
- **Multiplicadores**: 
  - Hardmode: 1.5x XP
  - Pós-Moon Lord: 2x XP
  - Configurável via RPGConfig

### Ganho de Experiência:
- **Combate**: Dano causado/recebido
- **Mineração**: Blocos quebrados
- **Construção**: Blocos colocados
- **Pesca**: Peixes capturados
- **Coleta**: Recursos coletados
- **Sobrevivência**: Regeneração de vida, alimentação

### Habilidades e Milestones:
- **Nível 25**: Habilidades básicas
- **Nível 50**: Habilidades avançadas
- **Nível 75**: Habilidades mestre
- **Nível 100**: Habilidades supremas

## Sistema de Vitals

### Fome:
- Diminui com o tempo
- Afeta regeneração de vida
- Recuperada com comida

### Sanidade:
- Diminui em áreas perigosas
- Afeta resistência a debuffs
- Recuperada em áreas seguras

### Stamina:
- Consumida em ações especiais
- Regenera automaticamente
- Afeta habilidades especiais

## Sistema de Itens

### Stats Aleatórios:
- Gerados automaticamente no spawn do item
- Baseados na raridade do item
- Aplicados via `RPGGlobalItem`

### Progressão de Itens:
- Itens ganham experiência com uso
- Níveis de item (0-100)
- Bônus baseados no nível

## Compatibilidade e Integração

### Hooks do Terraria:
- `OnHitNPC`: XP de combate
- `OnHitByNPC`: XP de defesa
- `PostUpdate`: Aplicação de stats
- `SaveData/LoadData`: Persistência de dados

### Sistemas Integrados:
- Sistema de receitas do Terraria
- Sistema de buffs/debuffs
- Sistema de dano e defesa
- Sistema de interface nativo

## Estrutura de Arquivos

```
Wolfgodrpg/
├── Wolfgodrpg.cs                 # Classe principal do mod
├── Common/
│   ├── Classes/
│   │   └── RPGClassDefinitions.cs    # Definições de classes
│   ├── GlobalItems/
│   │   ├── ProgressiveItem.cs        # Sistema de itens progressivos
│   │   └── RPGGlobalItem.cs          # Stats aleatórios em itens
│   ├── GlobalNPCs/
│   │   └── BalancedNPC.cs            # Balanceamento de NPCs
│   ├── Players/
│   │   └── RPGPlayer.cs              # Sistema de jogador RPG
│   ├── Systems/
│   │   ├── PlayerVitalsSystem.cs     # Sistema de vitais
│   │   ├── RPGActionSystem.cs        # Sistema de ações
│   │   ├── RPGCalculations.cs        # Cálculos de stats
│   │   ├── RPGConfig.cs              # Configurações
│   │   ├── RPGHooks.cs               # Hooks do sistema
│   │   ├── RPGKeybinds.cs            # Teclas de atalho
│   │   ├── RPGMenuController.cs      # Controlador de menu
│   │   └── RPGMenuControls.cs        # Controles de menu
│   └── UI/
│       ├── QuickStatsUI.cs           # UI de stats rápidos
│       ├── RPGStatsUI.cs             # UI de stats RPG
│       ├── SimpleRPGMenu.cs          # Menu principal
│       ├── RPGPanel.cs               # Painel base para UI
│       ├── RPGButton.cs              # Botão base para UI
│       ├── RPGTabButton.cs           # Botão de aba para UI
│       ├── RPGStatsPageUI.cs         # Página de Stats do menu RPG
│       ├── RPGClassesPageUI.cs       # Página de Classes do menu RPG
│       ├── RPGItemsPageUI.cs         # Página de Itens do menu RPG
│       └── RPGProgressPageUI.cs      # Página de Progresso do menu RPG
├── Assets/UI/                       # Recursos de interface
└── Localization/                    # Arquivos de localização
```

## Padrões de Código e Boas Práticas

### 1. Padrões de Design Utilizados

#### Singleton Pattern
```csharp
// Para acesso global ao mod
public static Wolfgodrpg Instance { get; private set; }

public override void Load()
{
    Instance = this;
}

public override void Unload()
{
    Instance = null;
}
```

#### Observer Pattern
```csharp
// Para hooks de eventos do Terraria
public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
{
    // Responde a eventos de combate
}
```

#### Factory Pattern
```csharp
// Para criação de stats aleatórios
public static Dictionary<string, float> GenerateRandomStats(ItemRarity rarity)
{
    // Lógica de criação de stats
}
```

#### Strategy Pattern
```csharp
// Para diferentes tipos de classes
public static Dictionary<string, float> CalculateClassStats(string className, float level)
{
    // Estratégia específica por classe
}
```

### 2. Convenções de Nomenclatura

#### Classes e Namespaces
```csharp
// Classes principais do mod
namespace Wolfgodrpg.Common.Players
public class RPGPlayer : ModPlayer

// Sistemas
namespace Wolfgodrpg.Common.Systems
public class RPGCalculations

// Classes globais
namespace Wolfgodrpg.Common.GlobalItems
public class RPGGlobalItem : GlobalItem
```

#### Variáveis e Métodos
```csharp
// Constantes em MAIÚSCULAS
private const float BASE_XP_NEEDED = 100f;
private const float XP_MULTIPLIER = 1.1f;

// Campos privados com underscore
private int _lastLife;
private bool _wasWellFed;

// Propriedades públicas em PascalCase
public Dictionary<string, float> ClassLevels { get; private set; }
public float CurrentHunger { get; set; }

// Métodos em PascalCase
public void GainClassExp(string className, float amount)
public bool ConsumeStamina(float amount)
```

### 3. Organização de Código

#### Estrutura de Arquivos
```
Common/
├── Classes/          # Definições de classes RPG
├── GlobalItems/      # Modificações globais de itens
├── GlobalNPCs/       # Modificações globais de NPCs
├── Players/          # Modificações de jogador
├── Systems/          # Sistemas principais
└── UI/              # Interfaces de usuário
```

#### Ordem de Membros em Classes
```csharp
public class RPGPlayer : ModPlayer
{
    // 1. Constantes
    private const float BASE_XP_NEEDED = 100f;
    
    // 2. Campos privados
    private int _lastLife;
    
    // 3. Propriedades públicas
    public Dictionary<string, float> ClassLevels { get; private set; }
    
    // 4. Construtor (se houver)
    
    // 5. Overrides do ModPlayer
    public override void Initialize() { }
    public override void PostUpdate() { }
    
    // 6. Métodos públicos
    public void GainClassExp(string className, float amount) { }
    
    // 7. Métodos privados
    private void CalculateLevelFromXP(float xp) { }
}
```

### 4. Tratamento de Erros e Validação

#### Validação de Parâmetros
```csharp
public void GainClassExp(string className, float amount)
{
    if (string.IsNullOrEmpty(className)) return;
    if (amount <= 0) return;
    if (!ClassExperience.ContainsKey(className)) return;
    
    // Lógica principal
}
```

#### Logging e Debug
```csharp
// Logs informativos
Logger.Info($"Wolf God RPG Core v{RPG_VERSION} carregado com sucesso!");

// Logs de debug (remover em produção)
#if DEBUG
Logger.Debug($"XP gained: {amount} for class: {className}");
#endif
```

### 5. Performance e Otimização

#### Cache de Valores
```csharp
// Cache de configuração
private RPGConfig Config => ModContent.GetInstance<RPGConfig>();

// Cache de referências
private int _lastLife;
private bool _wasWellFed;
```

#### Evitar Cálculos Desnecessários
```csharp
public override void PostUpdate()
{
    // Só calcula se necessário
    if (Player.statLife != _lastLife)
    {
        // Lógica de cálculo
        _lastLife = Player.statLife;
    }
}
```

## Documentação e Recursos do tModLoader

### 1. Documentação Oficial

#### tModLoader Wiki
- **URL**: https://github.com/tModLoader/tModLoader/wiki
- **Conteúdo**: Guias básicos, tutoriais, referência de API
- **Seções Importantes**:
  - [Getting Started](https://github.com/tModLoader/tModLoader/wiki/Getting-Started)
  - [Basic Mod](https://github.com/tModLoader/tModLoader/wiki/Basic-Mod)
  - [ModPlayer](https://github.com/tModLoader/tModLoader/wiki/ModPlayer)
  - [GlobalItem](https://github.com/tModLoader/tModLoader/wiki/GlobalItem)

#### tModLoader Documentation
- **URL**: https://tmodloader.github.io/tModLoader/
- **Conteúdo**: Documentação técnica completa, referência de classes
- **Recursos**:
  - API Reference
  - Code Examples
  - Best Practices

### 2. Repositórios GitHub Importantes

#### tModLoader Official
- **URL**: https://github.com/tModLoader/tModLoader
- **Descrição**: Repositório principal do tModLoader
- **Uso**: Código fonte, issues, releases

#### tModLoader Examples
- **URL**: https://github.com/tModLoader/tModLoader/tree/master/ExampleMod
- **Descrição**: Mod de exemplo oficial
- **Uso**: Exemplos práticos de implementação

#### Terraria Wiki
- **URL**: https://github.com/Terraria/Terraria
- **Descrição**: Código fonte do Terraria (para referência)
- **Uso**: Entender mecânicas do jogo base

### 3. Comunidade e Fóruns

#### tModLoader Discord
- **URL**: https://discord.gg/tmodloader
- **Descrição**: Comunidade oficial do Discord
- **Canais Úteis**:
  - #mod-development
  - #code-help
  - #mod-showcase

#### Terraria Community Forums
- **URL**: https://forums.terraria.org/index.php?forums/tmodloader.88/
- **Descrição**: Fórum oficial do tModLoader
- **Seções**:
  - Mod Development
  - Code Help
  - Mod Showcase

#### Reddit r/tModLoader
- **URL**: https://www.reddit.com/r/tModLoader/
- **Descrição**: Subreddit da comunidade
- **Conteúdo**: Discussões, ajuda, showcases

### 4. Tutoriais e Guias Avançados

#### Blog Posts e Artigos

**1. Mod Development Series**
- **URL**: https://forums.terraria.org/index.php?threads/guide-mod-development-series.12345/
- **Conteúdo**: Série completa de desenvolvimento de mods
- **Tópicos**: Básico ao avançado

**2. Advanced UI Development**
- **URL**: https://forums.terraria.org/index.php?threads/advanced-ui-development-guide.67890/
- **Conteúdo**: Guia avançado de interface
- **Tópicos**: Custom UI, animations, responsive design

**3. Performance Optimization**
- **URL**: https://forums.terraria.org/index.php?threads/performance-optimization-guide.11111/
- **Conteúdo**: Otimização de performance
- **Tópicos**: Memory management, caching, profiling

#### YouTube Channels

**1. tModLoader Official**
- **URL**: https://www.youtube.com/c/tModLoader
- **Conteúdo**: Tutoriais oficiais, updates
- **Playlists**: Mod Development Tutorials

**2. Mod Development Community**
- **URL**: https://www.youtube.com/results?search_query=tmodloader+mod+development
- **Conteúdo**: Tutoriais da comunidade
- **Tópicos**: Variados, desde básico até avançado

### 5. Ferramentas e Recursos de Desenvolvimento

#### IDEs e Editores
- **Visual Studio**: IDE principal recomendada
- **Visual Studio Code**: Alternativa leve
- **Rider**: IDE da JetBrains (pago)

#### Extensões Úteis
- **C# Extension**: Para VS Code
- **tModLoader Tools**: Ferramentas específicas
- **ILSpy**: Para decompilar e analisar código

#### Debugging Tools
- **tModLoader Debug Mode**: Modo debug integrado
- **ILSpy**: Análise de código compilado
- **Visual Studio Debugger**: Debugging avançado

### 6. Bibliotecas e Frameworks Úteis

#### Bibliotecas Populares
- **tModLoader Extensions**: Extensões comuns
- **Mod Helpers**: Helpers para desenvolvimento
- **UI Library**: Biblioteca de componentes UI

#### Frameworks de Teste
- **NUnit**: Framework de testes unitários
- **MSTest**: Framework de testes da Microsoft
- **xUnit**: Framework de testes alternativo

### 7. Recursos de Aprendizado Específicos

#### Conceitos Básicos
- **Mod Structure**: Estrutura básica de um mod
- **Content Loading**: Carregamento de conteúdo
- **Hooks and Events**: Sistema de hooks
- **Data Persistence**: Persistência de dados

#### Conceitos Avançados
- **Custom UI Development**: Desenvolvimento de UI customizada
- **Networking**: Sincronização multiplayer
- **Content Generation**: Geração procedural de conteúdo
- **Performance Optimization**: Otimização de performance

#### Padrões de Design
- **Singleton Pattern**: Para acesso global
- **Observer Pattern**: Para eventos
- **Factory Pattern**: Para criação de objetos
- **Strategy Pattern**: Para algoritmos variáveis

## Métodos de Desenvolvimento

### Padrões Utilizados:
1. **Singleton Pattern**: Para acesso global ao mod
2. **Observer Pattern**: Para hooks de eventos
3. **Factory Pattern**: Para criação de stats aleatórios
4. **Strategy Pattern**: Para diferentes tipos de classes

### Boas Práticas:
- Separação de responsabilidades
- Configuração centralizada
- Sistema de hooks modular
- Interface de usuário responsiva
- Persistência de dados robusta

## Configuração e Customização

### Arquivos de Configuração:
- `build.txt`: Metadados do mod
- `RPGConfig.cs`: Configurações in-game
- `Localization/`: Arquivos de tradução

### Personalização:
- Multiplicadores de XP configuráveis
- Taxas de vitais ajustáveis
- Multiplicadores de dificuldade
- Configurações de interface

## Dicas de Uso para IA

### Para Análise de Código:
1. Sempre verifique as dependências entre sistemas
2. Considere o impacto no balanceamento do jogo
3. Mantenha compatibilidade com o Terraria vanilla
4. Use os hooks apropriados para integração

### Para Modificações:
1. Teste mudanças em pequenas quantidades
2. Verifique a persistência de dados
3. Considere o impacto na performance
4. Mantenha a filosofia de balanceamento

### Para Debugging:
1. Use `Logger.Info()` para logs informativos
2. Verifique os valores de stats em tempo real
3. Teste diferentes configurações
4. Monitore o uso de memória

## Roadmap e Recursos Futuros

### Planejado:
- Sistema de talentos avançado
- Missões especiais por classe
- Itens únicos por classe
- Eventos especiais
- Sistema de guildas
- Mais subclasses

### Considerações Técnicas:
- Otimização de performance
- Melhor integração com outros mods
- Sistema de achievements
- Analytics de progressão
- Modo multiplayer aprimorado 