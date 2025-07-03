# CHECKLIST DE UI - WOLFGOD RPG MOD
**Baseado na Nova Visão 2024: Sistema de 3 Eixos + Afixos**

## 🎯 NOVA ESTRUTURA DE UI (3 ABAS)

### **ABA 1: ATRIBUTOS** 
**Função:** Nível geral + distribuição de pontos

#### ✅ **Elementos Necessários:**
- [ ] **Display do Nível Geral** (grande, centralizado)
- [ ] **Barra de XP atual** (visual + números)
- [ ] **Pontos disponíveis** para distribuir
- [ ] **5 Atributos Primários:**
  - [ ] Força (+ botão, - botão, valor atual)
  - [ ] Destreza (+ botão, - botão, valor atual)
  - [ ] Inteligência (+ botão, - botão, valor atual)
  - [ ] Constituição (+ botão, - botão, valor atual)
  - [ ] Sabedoria (+ botão, - botão, valor atual)
- [ ] **Confirmação de distribuição** (botão aplicar/resetar)
- [ ] **Preview dos efeitos** de cada atributo

#### 📋 **Tarefas de Implementação:**
```csharp
// Arquivo: Common/UI/RPGStatsPageUI.cs
- [ ] Implementar layout de 5 atributos
- [ ] Adicionar botões +/- para cada atributo
- [ ] Sistema de preview antes de aplicar
- [ ] Validação de pontos disponíveis
- [ ] Animações de hover e click
```

---

### **ABA 2: CLASSES**
**Função:** 10 classes + níveis + habilidades

#### ✅ **Elementos Necessários:**
- [ ] **Seção COMBATE (4 classes):**
  - [ ] Guerreiro (nível, XP, barra)
  - [ ] Arqueiro (nível, XP, barra)
  - [ ] Mago (nível, XP, barra)
  - [ ] Invocador (nível, XP, barra)
  
- [ ] **Seção UTILIDADE (6 classes):**
  - [ ] Acrobata (nível, XP, barra)
  - [ ] Explorador (nível, XP, barra)
  - [ ] Engenheiro (nível, XP, barra)
  - [ ] Sobrevivencialista (nível, XP, barra)
  - [ ] Ferreiro (nível, XP, barra)
  - [ ] Alquimista (nível, XP, barra)

#### 🎯 **Sistema de Habilidades:**
- [ ] **Milestones visuais** (Nv. 1, 25, 50, 75, 100)
- [ ] **Descrição de habilidades** em tooltip
- [ ] **Status desbloqueado/bloqueado** por nível
- [ ] **Preview do próximo milestone**

#### 📋 **Tarefas de Implementação:**
```csharp
// Arquivo: Common/UI/RPGClassesPageUI.cs
- [ ] Layout de 2 colunas (Combate | Utilidade)
- [ ] Sistema de tooltips para habilidades
- [ ] Indicadores visuais de progresso
- [ ] Animações de level up
- [ ] Click para ver detalhes de cada classe
```

---

### **ABA 3: PROFICIÊNCIAS**
**Função:** Armas + Armaduras + Maestria

#### ✅ **Elementos Necessários:**
- [ ] **Seção ARMAS:**
  - [ ] Lista de tipos de arma com níveis
  - [ ] Barras de XP para cada tipo
  - [ ] Bônus atual de cada proficiência
  
- [ ] **Seção ARMADURAS (NOVA):** ⭐
  - [ ] Armadura Pesada (nível, XP, bônus)
  - [ ] Armadura Leve (nível, XP, bônus)
  - [ ] Vestes Mágicas (nível, XP, bônus)

#### 📊 **Sistema de Bônus:**
- [ ] **Display de bônus ativos** de cada proficiência
- [ ] **Comparação** antes/depois de equipar item
- [ ] **Cálculo em tempo real** dos efeitos

#### 📋 **Tarefas de Implementação:**
```csharp
// Arquivo: Common/UI/RPGProgressPageUI.cs
- [ ] Adicionar seção de armaduras
- [ ] Sistema de categorização visual
- [ ] Tooltips explicativos dos bônus
- [ ] Integração com sistema de cálculo
```

---

## 🏗️ **SISTEMA DE AFIXOS - UI**

### **TOOLTIPS DE ITENS** ⭐ NOVO
#### ✅ **Elementos Necessários:**
- [ ] **Exibição de afixos** em tooltips de itens
- [ ] **Cores diferenciadas** por tipo de afixo:
  - [ ] Azul: Atributos primários
  - [ ] Verde: Bônus de classe
  - [ ] Roxo: Proficiência 
  - [ ] Amarelo: Utilidade
- [ ] **Formatting consistente** (+X, +X%, etc.)

#### 📋 **Tarefas de Implementação:**
```csharp
// Arquivo: Common/GlobalItems/RPGGlobalItem.cs
- [ ] Implementar ModifyTooltips()
- [ ] Sistema de cores por categoria
- [ ] Formatting de valores numéricos
- [ ] Suporte a múltiplos afixos por item
```

---

## 🎨 **MODERNIZAÇÃO DE UI**

### **CORREÇÕES TÉCNICAS PRIORITÁRIAS:**
- [ ] **❌ CORRIGIR: AssetRequestMode.ImmediateLoad**
  ```csharp
  // EM: Common/UI/RPGTabButton.cs linhas 39-40
  // SUBSTITUIR por Asset<Texture2D> pattern
  ```

### **MELHORIAS DE UX:**
- [ ] **Animações suaves** entre abas
- [ ] **Feedback visual** em hover/click  
- [ ] **Sons de UI** (level up, click, etc.)
- [ ] **Scaling responsivo** para diferentes resoluções

### **ACESSIBILIDADE:**
- [ ] **Tooltips informativos** em todos os elementos
- [ ] **Teclas de atalho** para abas (1, 2, 3)
- [ ] **Contrast ratio adequado** para leitura
- [ ] **Font sizing** apropriado

---

## 🔧 **INTEGRAÇÃO COM SISTEMAS**

### **CÁLCULO DE STATS:**
- [ ] **Integração** com RPGCalculations.cs
- [ ] **Update em tempo real** quando mudar equipamentos
- [ ] **Preview** de mudanças antes de aplicar
- [ ] **Cache** para otimização de performance

### **SISTEMA DE SAVE/LOAD:**
- [ ] **Persistência** de configurações de UI
- [ ] **Estado das abas** (qual estava aberta)
- [ ] **Posição da janela** se aplicável

### **MULTIPLAYER:**
- [ ] **Sincronização** de dados entre cliente/servidor
- [ ] **Prevenção** de cheats via UI
- [ ] **Feedback** visual de lag/desconexão

---

## 📋 **CRONOGRAMA DE IMPLEMENTAÇÃO**

### **SEMANA 1: Base Técnica**
- [ ] Corrigir asset loading moderno
- [ ] Estrutura base das 3 abas
- [ ] Sistema de navegação entre abas

### **SEMANA 2: Aba de Classes**
- [ ] Implementar 10 classes novas
- [ ] Sistema de milestones
- [ ] Tooltips de habilidades

### **SEMANA 3: Aba de Proficiências**
- [ ] Adicionar proficiência de armaduras
- [ ] Integrar com sistema de cálculo
- [ ] UI de bônus ativos

### **SEMANA 4: Sistema de Afixos**
- [ ] Tooltips de itens com afixos
- [ ] Sistema de cores e formatting
- [ ] Integração com geração aleatória

### **SEMANA 5: Polish e Testes**
- [ ] Animações e feedback visual
- [ ] Testes de multiplayer
- [ ] Optimizações de performance

---

## ✅ **CRITÉRIOS DE ACEITAÇÃO**

### **Funcionalidade:**
- [ ] Todas as 3 abas navegáveis e responsivas
- [ ] Dados persistem entre sessões
- [ ] Performance aceitável (60fps+)
- [ ] Multiplayer funcional sem bugs

### **UX/UI:**
- [ ] Interface intuitiva para novos usuários
- [ ] Tooltips informativos em tudo
- [ ] Animações suaves sem lag
- [ ] Visual consistente com Terraria

### **Técnico:**
- [ ] Código limpo e documentado
- [ ] Sem memory leaks ou performance issues
- [ ] Compatível com tModLoader 2024+
- [ ] Pronto para expansões futuras

---

*Última atualização: 2024-12-28*  
*Baseado no novo design do gemini.md* 