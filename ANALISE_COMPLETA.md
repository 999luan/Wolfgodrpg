# Análise Completa do Mod Wolfgod RPG para tModLoader

## 1. Visão Geral do Projeto

O mod Wolfgod RPG para tModLoader visa implementar um sistema de progressão de RPG com níveis de jogador, níveis de classe e proficiências de equipamento, conforme detalhado no `gemini.md`. Esta análise foca na implementação atual do código, identificando inconsistências com o design e oportunidades de refatoração, com ênfase na interface do usuário (UI).

## 2. Análise da Estrutura da UI

A UI do mod é construída usando os componentes padrão do tModLoader (`UIElement`, `UIPanel`, `UIText`, `UIList`, `UIScrollbar`). O `SimpleRPGMenu.cs` atua como o contêiner principal, gerenciando as diferentes abas (Status, Classes, Proficiências, Itens, Progresso).

### 2.1. `SimpleRPGMenu.cs`

- **Função:** Contêiner principal da UI, gerencia a exibição das abas.
- **Inconsistências/Refatorações:**
    - **Correção de Atualização:** O método `HasSignificantChanges` não estava verificando as mudanças nos dados de proficiência de armadura do jogador (`ArmorProficiencyLevels` e `ArmorProficiencyExperience`), o que impedia a atualização correta da UI de proficiências. **Corrigido.**

### 2.2. `RPGStatsPageUI.cs`

- **Função:** Exibe estatísticas vitais (fome, sanidade, stamina, vida, mana, defesa, velocidade) e informações resumidas das classes.
- **Inconsistências/Refatorações:**
    - **Fórmula de XP de Classe:** A fórmula de cálculo de XP de classe na UI estava inconsistente com a lógica em `RPGPlayer.cs`. **Corrigido.**
    - **Atributos Primários:** Os atributos primários (Força, Destreza, Inteligência, Constituição, Sabedoria) mencionados no `gemini.md` **não estão implementados** em `RPGPlayer.cs` e, consequentemente, não são exibidos nesta página. Esta é uma divergência significativa do plano de design e requer refatoração futura.
    - **`GetStatDisplayName`:** Não havia chamadas diretas a uma função `GetStatDisplayName` local neste arquivo.

### 2.3. `RPGClassesPageUI.cs`

- **Função:** Exibe detalhes das classes, incluindo habilidades desbloqueadas e bônus de atributos por nível.
- **Inconsistências/Refatorações:**
    - **`GetStatDisplayName` Duplicado:** A função `GetStatDisplayName` estava definida localmente dentro da classe `DetailedClassEntry`, duplicando a funcionalidade que deveria ser centralizada em `RPGDisplayUtils.cs`. **Corrigido: A definição local foi removida e as chamadas foram atualizadas para usar `RPGDisplayUtils.GetStatDisplayName`.**
    - **Lógica Hardcoded de Habilidades:** Havia uma lógica hardcoded para a habilidade "Dash". **Corrigido: A lógica hardcoded foi removida.**

### 2.4. `RPGItemsPageUI.cs`

- **Função:** Exibe itens com atributos RPG (afixos) e itens progressivos.
- **Inconsistências/Refatorações:**
    - **`GetStatDisplayName` Duplicado:** Similar a `RPGClassesPageUI.cs`, a função `GetStatDisplayName` estava definida localmente. **Corrigido: A definição local foi removida e as chamadas foram atualizadas para usar `Wolfgodrpg.Common.Utils.RPGDisplayUtils.GetStatDisplayName`.**
    - **Bloco de Código Duplicado:** Havia um bloco de código duplicado no final da classe `ItemEntry`. **Corrigido: O bloco duplicado foi removido.**
    - **Fórmula de XP de Item Progressivo:** A fórmula de cálculo de XP para itens progressivos estava inconsistente com a lógica central. **Corrigido.**

### 2.5. `RPGProficienciesPageUI.cs`

- **Função:** Exibe os níveis de proficiência de armas e armaduras.
- **Inconsistências/Refatorações:**
    - Nenhuma inconsistência ou oportunidade de refatoração significativa identificada nesta análise. A página está bem estruturada para exibir os dados de proficiência de armadura.

### 2.6. `RPGProgressPageUI.cs`

- **Função:** Exibe o progresso do jogador em relação a chefes derrotados, eventos especiais e estatísticas gerais de RPG.
- **Inconsistências/Refatorações:**
    - Nenhuma inconsistência ou oportunidade de refatoração significativa identificada nesta análise. A estrutura da UI e o uso de `UIList` e `UIScrollbar` seguem as práticas padrão do tModLoader.

## 3. Análise da Estrutura de Dados e Lógica Central

### 3.1. `RPGPlayer.cs`

- **Função:** Armazena os dados centrais do jogador, incluindo níveis de classe, experiência e proficiências de armadura.
- **Inconsistências/Refatorações:**
    - **Atributos Primários Ausentes:** Os atributos primários (Força, Destreza, Inteligência, Constituição, Sabedoria) definidos no `gemini.md` **não estão implementados** em `RPGPlayer.cs`. Esta é a maior divergência entre o design e a implementação atual e requer uma refatoração substancial para integrar esses atributos ao sistema de progressão e cálculo de status.

### 3.2. `RPGDisplayUtils.cs`

- **Função:** Centraliza funções utilitárias para exibição de dados na UI.
- **Inconsistências/Refatorações:**
    - **Centralização de `GetStatDisplayName`:** Esta classe foi criada para centralizar a função `GetStatDisplayName`, que estava duplicada em várias UIs. **Concluído.**

## 4. Próximos Passos e Recomendações

1.  **Implementação de Atributos Primários:** A prioridade máxima é a implementação dos atributos primários em `RPGPlayer.cs` e a integração deles com o sistema de cálculo de status e a UI. Isso envolverá:
    - Adicionar as variáveis para os atributos em `RPGPlayer.cs`.
    - Modificar a lógica de ganho de nível do jogador para permitir a distribuição de pontos de atributo.
    - Atualizar `RPGStatsPageUI.cs` para exibir e permitir a distribuição desses pontos.
    - Integrar os atributos nos cálculos de dano, defesa, etc., em `RPGCalculations.cs`.

2.  **Verificação Aprofundada da UI (Rolagem e Posicionamento):** Embora as análises pontuais não tenham revelado problemas óbvios, uma verificação mais aprofundada da lógica de rolagem e posicionamento em todas as UIs é recomendada, especialmente se houver relatos de problemas visuais.

3.  **Testes Abrangentes:** Após a implementação dos atributos primários, testes abrangentes de todas as funcionalidades do mod são cruciais para garantir a estabilidade e a correção dos cálculos.

4.  **Documentação Contínua:** Manter o `ANALISE_COMPLETA.md` atualizado com as mudanças e decisões de design.
