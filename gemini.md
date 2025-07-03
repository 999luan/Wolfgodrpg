# WolfGod RPG - Plano de Design e Implementação

## 1. Visão Geral e Filosofia

O sistema se baseia em três eixos de progressão interconectados:

1.  **Nível do Jogador (Geral):** Representa a evolução fundamental do personagem.
2.  **Níveis de Classe (Ação):** Recompensa o jogador por *fazer* coisas no mundo.
3.  **Níveis de Proficiência (Maestria):** Recompensa a prática com tipos de equipamentos específicos.

---

## 2. Detalhamento do Nível do Jogador (Player Level)

*   **Como Ganhar XP:** O jogador ganha XP para o Nível Geral ao atingir marcos importantes, não por farm direto.
*   **Recompensas por Nível:** Pontos de Atributo para distribuir e aumentos passivos de Vida/Mana.
*   **Atributos Primários:** Força, Destreza, Inteligência, Constituição, Sabedoria.

---

## 3. Detalhamento dos Níveis de Classe (Class Levels)

Classes são definidas por ações. O jogador pode evoluir todas elas simultaneamente.

*   **Estrutura das Classes:**
    *   **Combate:** Guerreiro, Arqueiro, Mago, Invocador.
    *   **Utilidade/Aventura:** Acrobata, Explorador, Engenheiro, Sobrevivencialista, Ferreiro, Alquimista.
*   **Recompensas:** Bônus passivos em atributos e Habilidades desbloqueadas em níveis chave (milestones).
    *   **Exemplo (Acrobata):** Nv. 1 (Dash Básico), Nv. 25 (Pulo Duplo), Nv. 50 (Dash Aprimorado), Nv. 75 (Dash Evasivo), Nv. 100 (Segundo Dash Aéreo).

---

## 4. Detalhamento da Proficiência de Equipamentos (Equipment Proficiency)

Este sistema recompensa o uso contínuo de tipos específicos de armas e armaduras.

*   **Proficiência com Armas:**
    *   **Como Ganhar XP:** Causar dano com um tipo de arma (Espadas, Arcos, etc.).
    *   **Recompensas:** Bônus passivos para aquele tipo de arma (dano, velocidade, etc.).

*   **Proficiência com Armaduras:**
    *   **Como Ganhar XP:** Receber dano enquanto veste um tipo de armadura.
    *   **Tipos:** Armadura Pesada, Armadura Leve, Vestes Mágicas.
    *   **Recompensas:** Bônus passivos para aquela categoria (defesa, redução de dano, etc.).

---

## 5. Sistema de Atributos em Itens (Afixos)

Para tornar o loot mais emocionante, armas e armaduras podem ser geradas com bônus aleatórios (afixos) que interagem com os sistemas do mod.

*   **Como Funciona:** Ao ser gerado (drop/craft), um item tem chance de receber um ou mais afixos.
*   **Exemplos de Afixos:**
    *   **Atributo Primário:** `+5 de Força`
    *   **Bônus de Classe:** `+10% de XP para a classe Guerreiro`, `+2 Níveis para a classe Acrobata (enquanto equipado)`
    *   **Bônus de Proficiência:** `+15% de dano com Espadas`
    *   **Utilidade:** `+5% de velocidade de mineração`
*   **Exibição:** Os bônus serão listados na tooltip do item.

---

## 6. Planejamento da Interface (UI)

*   **Aba 1: Atributos:** Nível geral, distribuição de pontos.
*   **Aba 2: Classes:** Níveis de classe, XP, descrição de habilidades.
*   **Aba 3: Proficiências:** Níveis de proficiência de armas e armaduras.

---

## 7. Plano de Implementação (Passos Técnicos)

1.  **Estrutura de Dados (`RPGPlayer.cs`):**
    *   Adicionar variáveis para níveis, XP, atributos, etc.
    *   Adicionar dicionários para proficiência de armaduras: `armorProficiencyLevels` e `armorProficiencyExperience`.
    *   Implementar `SaveData`/`LoadData`.

2.  **Estrutura de Dados dos Itens (`RPGGlobalItem.cs`):**
    *   Adicionar um `TagCompound` para armazenar os afixos de cada item.
    *   Implementar a lógica de geração de afixos em `OnCreate` ou fonte de entidade customizada.

3.  **Sistema de Ações e Ganchos (Hooks):**
    *   **Proficiência de Armadura:** Usar `OnHitByNPC`/`OnHitByProjectile` em `RPGPlayer.cs` para conceder XP.

4.  **Cálculos de Status (`RPGCalculations.cs`):**
    *   O sistema de cálculo deve somar bônus de todas as fontes: Nível do Jogador -> Classes -> Proficiências -> **Afixos de Itens Equipados**.
    *   Aplicar o total nos hooks de stats do tModLoader.

5.  **Interface (UI):**
    *   **Tooltips:** Implementar `ModifyTooltips` em `RPGGlobalItem.cs` para exibir os afixos.
    *   **Menu:** Atualizar a UI de Proficiências para incluir armaduras.
