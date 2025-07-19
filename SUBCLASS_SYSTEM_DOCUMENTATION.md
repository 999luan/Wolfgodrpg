# 🎯 **Sistema de Subclasses - WolfGod RPG**

## 📋 **Visão Geral**

O sistema de subclasses permite que o jogador tenha múltiplas especializações, cada uma com suas próprias skills, progressão e modificadores de stats. O nível total é a soma de todas as subclasses desbloqueadas.

## 🏗️ **Arquitetura do Sistema**

### **1. PlayerSubClass (Classe Base Abstrata)**
```csharp
public abstract class PlayerSubClass
{
    // Propriedades básicas
    public string Name { get; protected set; }
    public int Level { get; protected set; } = 1;
    public int XP { get; protected set; } = 0;
    
    // Skills organizadas
    public List<BaseSkill> Skills { get; private set; }
    public List<BaseSkill> PassiveSkills { get; private set; }
    public List<BaseSkill> ActiveSkills { get; private set; }
    
    // Eventos para comunicação
    public event Action OnLevelUp;
    public event Action OnXPChanged;
    public event Action OnSkillUnlocked;
}
```

**Características:**
- ✅ **Progressão independente**: Cada subclasse tem seu próprio XP e nível
- ✅ **Skills organizadas**: Separadas em ativas e passivas
- ✅ **Eventos**: Comunicação eficiente entre sistemas
- ✅ **Validação**: Clamping automático de valores

### **2. SubClassSystem (Gerenciador)**
```csharp
public class SubClassSystem
{
    public List<PlayerSubClass> SubClasses { get; private set; }
    public PlayerSubClass ActiveSubClass { get; private set; }
    
    // Cálculos de nível total
    public int GetTotalLevel() => SubClasses.Sum(sc => sc.Level);
    public int GetTotalXP() => SubClasses.Sum(sc => sc.TotalXP);
    
    // Modificadores combinados
    public Dictionary<string, float> GetCombinedStatModifiers()
}
```

**Funcionalidades:**
- ✅ **Gerenciamento centralizado**: Controle de todas as subclasses
- ✅ **Nível total**: Soma de todas as subclasses
- ✅ **Modificadores combinados**: Stats de todas as subclasses
- ✅ **Subclasse ativa**: Controle de qual está em uso

## 🎮 **Subclasse Exemplo - Guerreiro**

### **Características:**
- **Nome**: Guerreiro
- **Especialização**: Combate corpo a corpo e defesa
- **Cor**: Vermelho
- **Ícone**: ⚔️

### **Skills Implementadas:**

#### **1. Ataque Poderoso (Nível 1)**
```csharp
public class PowerfulStrikeSkill : BaseSkill
{
    protected override bool OnActivate(Player player)
    {
        // Efeito visual: poeira vermelha
        // Efeito sonoro: SoundID.Item1
        // Buff: Rage por 5 segundos
        return true;
    }
}
```

#### **2. Defesa de Ferro (Nível 2)**
```csharp
public class IronDefenseSkill : BaseSkill
{
    protected override bool OnActivate(Player player)
    {
        // Efeito visual: poeira cinza
        // Efeito sonoro: SoundID.Item25
        // Buff: Ironskin por 10 segundos
        return true;
    }
}
```

#### **3. Berserker (Nível 5)**
```csharp
public class BerserkerSkill : BaseSkill
{
    protected override bool OnActivate(Player player)
    {
        // Efeito visual: poeira laranja
        // Buffs: Rage + Swiftness por 15 segundos
        return true;
    }
}
```

#### **4. Escudo de Energia (Nível 8)**
```csharp
public class EnergyShieldSkill : BaseSkill
{
    protected override bool OnActivate(Player player)
    {
        // Efeito visual: poeira ciano
        // Buff: Shine por 7.5 segundos
        return true;
    }
}
```

#### **5. Ataque Final (Nível 10)**
```csharp
public class FinalStrikeSkill : BaseSkill
{
    protected override bool OnActivate(Player player)
    {
        // Efeito visual dramático: poeira vermelha escura
        // Buffs: Rage + Swiftness + Ironskin por 10 segundos
        return true;
    }
}
```

### **Modificadores de Stats:**
```csharp
public override Dictionary<string, float> GetStatModifiers()
{
    var modifiers = new Dictionary<string, float>();
    
    // Bônus baseados no nível
    modifiers["MeleeDamage"] = 1f + (Level * 0.05f); // +5% por nível
    modifiers["Defense"] = Level * 0.5f; // +0.5 defesa por nível
    modifiers["MaxHealth"] = Level * 2f; // +2 HP por nível
    
    // Bônus especiais em níveis altos
    if (Level >= 5)
        modifiers["StaminaRegen"] = 1.2f; // +20% regeneração
    
    if (Level >= 10)
        modifiers["MeleeSpeed"] = 1.1f; // +10% velocidade de ataque
    
    return modifiers;
}
```

## 🔧 **Como Usar o Sistema**

### **1. Inicialização**
```csharp
// Em RPGPlayer.Initialize()
SubClasses = new SubClassSystem();

// O sistema automaticamente:
// - Cria todas as subclasses disponíveis
// - Desbloqueia a primeira subclasse
// - Define a primeira como ativa
```

### **2. Gerenciamento de Subclasses**
```csharp
// Mudar subclasse ativa
SubClasses.SetActiveSubClass(warriorSubClass);

// Desbloquear nova subclasse
SubClasses.UnlockSubClass("Mage");

// Adicionar XP
SubClasses.AddXPToActiveSubClass(100);
SubClasses.AddXPToSubClass("Warrior", 50);
```

### **3. Uso de Skills**
```csharp
// Usar skill da subclasse ativa
SubClasses.UseActiveSubClassSkill(0); // Primeira skill

// Usar skill passiva
SubClasses.UseActiveSubClassPassiveSkill(0);
```

### **4. Cálculos de Stats**
```csharp
// Nível total
int totalLevel = SubClasses.GetTotalLevel();

// XP total
int totalXP = SubClasses.GetTotalXP();

// Modificadores combinados
var modifiers = SubClasses.GetCombinedStatModifiers();

// Modificadores da subclasse ativa
var activeModifiers = SubClasses.GetActiveSubClassModifiers();
```

## 📊 **Progressão e Balanceamento**

### **Fórmula de XP:**
```csharp
protected virtual int XPToNextLevel()
{
    return 100 + (Level * 50) + (Level * Level * 10);
}
```

**Exemplos:**
- Nível 1 → 2: 160 XP
- Nível 2 → 3: 240 XP
- Nível 5 → 6: 500 XP
- Nível 10 → 11: 1,100 XP

### **Desbloqueio de Skills:**
- **Nível 1**: Ataque Poderoso
- **Nível 2**: Defesa de Ferro
- **Nível 5**: Berserker
- **Nível 8**: Escudo de Energia
- **Nível 10**: Ataque Final

## 🎯 **Vantagens da Arquitetura**

### **1. Escalabilidade**
- ✅ **Fácil adição**: Novas subclasses são independentes
- ✅ **Skills modulares**: Cada skill é uma classe separada
- ✅ **Balanceamento**: Cada subclasse pode ser balanceada independentemente

### **2. Performance**
- ✅ **Eventos eficientes**: Só atualiza quando necessário
- ✅ **Cache inteligente**: Comparação antes de recalcular
- ✅ **Updates otimizados**: Skills só atualizam quando ativas

### **3. Manutenibilidade**
- ✅ **Código organizado**: Responsabilidades separadas
- ✅ **Documentação clara**: Comentários explicativos
- ✅ **Padrões consistentes**: Nomenclatura uniforme

### **4. Flexibilidade**
- ✅ **Subclasses independentes**: Cada uma gerencia suas skills
- ✅ **Modificadores dinâmicos**: Stats mudam com o nível
- ✅ **Eventos flexíveis**: Comunicação desacoplada

## 🚀 **Próximas Subclasses Planejadas**

### **1. Mago (Mage)**
- **Especialização**: Dano mágico e controle
- **Cor**: Azul
- **Skills**: Bola de Fogo, Escudo Mágico, Teleporte, Tempestade, Meteoro

### **2. Arqueiro (Archer)**
- **Especialização**: Dano à distância e mobilidade
- **Cor**: Verde
- **Skills**: Tiro Preciso, Flecha Múltipla, Evasão, Chuva de Flechas, Tiro Mortal

### **3. Assassino (Rogue)**
- **Especialização**: Dano crítico e stealth
- **Cor**: Roxo
- **Skills**: Golpe Furtivo, Invisibilidade, Veneno, Backstab, Execução

### **4. Clérigo (Cleric)**
- **Especialização**: Suporte e cura
- **Cor**: Dourado
- **Skills**: Cura Menor, Escudo Divino, Bênção, Ressurreição, Cura Maior

## 🎮 **Integração com UI**

### **1. Aba de Subclasses**
- Lista todas as subclasses disponíveis
- Mostra nível, XP e progresso
- Indica subclasse ativa
- Botões para trocar subclasse

### **2. Aba de Skills**
- Skills organizadas por subclasse
- Mostra cooldowns e custos
- Tooltips detalhados
- Botões para usar skills

### **3. Indicadores de Status**
- Nível total do jogador
- XP total acumulado
- Modificadores ativos
- Buffs das skills

## 🏆 **Resultado Final**

**Status**: **SISTEMA COMPLETO E FUNCIONAL** ✅

### **Arquivos Criados:**
- ✅ `Common/Classes/PlayerSubClass.cs` - Classe base abstrata
- ✅ `Common/Classes/SubClasses/WarriorSubClass.cs` - Subclasse Guerreiro
- ✅ `Common/Systems/SubClassSystem.cs` - Sistema gerenciador
- ✅ `SUBCLASS_SYSTEM_DOCUMENTATION.md` - Documentação completa

### **Funcionalidades Implementadas:**
- ✅ **Sistema modular**: Subclasses independentes
- ✅ **Progressão**: XP e níveis por subclasse
- ✅ **Skills organizadas**: Ativas e passivas
- ✅ **Modificadores dinâmicos**: Stats baseados no nível
- ✅ **Eventos eficientes**: Comunicação otimizada
- ✅ **Nível total**: Soma de todas as subclasses
- ✅ **Subclasse ativa**: Controle de qual está em uso

### **Benefícios Alcançados:**
- ✅ **Escalabilidade**: Fácil adição de novas subclasses
- ✅ **Performance**: Atualização inteligente
- ✅ **Manutenibilidade**: Código organizado
- ✅ **Flexibilidade**: Sistema adaptável

**🎉 Sistema de subclasses completamente implementado e funcional!** 