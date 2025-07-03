# 🌟 ROADMAP OPÇÃO B - NOVA VISÃO COMPLETA
**WolfGod RPG Mod - Sistema de 3 Eixos + Afixos + Armaduras (4-5 semanas)**

## 📋 **VISÃO GERAL**

**Objetivo:** Implementar completamente a nova visão 2024 do projeto  
**Tempo:** 4-5 semanas (30-40 horas trabalho)  
**Prioridade:** Nova arquitetura > Compatibilidade > Performance  
**Resultado:** Sistema RPG moderno e completo conforme gemini.md

### **🎯 Nova Arquitetura (3 Eixos):**
1. **Nível do Jogador (Geral)** - XP por marcos importantes
2. **10 Classes por Ação** - Combate (4) + Utilidade (6) 
3. **Proficiência de Equipamentos** - Armas + Armaduras (3 tipos)

### **⭐ Novos Recursos:**
- **Sistema de Afixos** em itens (bônus aleatórios)
- **UI de 3 Abas** (Atributos, Classes, Proficiências)
- **Proficiência de Armaduras** (Pesada/Leve/Mágicas)
- **Cálculo integrado** de todas as fontes de bônus

---

## 🗓️ **CRONOGRAMA DETALHADO**

### **SEMANA 1-2: NOVA ARQUITETURA (Dias 1-14)**

#### **DIAS 1-3: SISTEMA DE AFIXOS** ⭐ NOVO
**Tempo:** 8-12 horas  
**Prioridade:** ALTA

##### **📚 Referências Técnicas:**
- [Item Modification Guide](https://github.com/tModLoader/tModLoader/wiki/Item-Modification)
- [TagCompound Documentation](https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound)
- [ModifyTooltips Reference](https://github.com/tModLoader/tModLoader/wiki/Basic-Item-Dropping-and-Loot-1.4#modifying-tooltips)

##### **🔧 Implementação:**

**DIA 1: Base do Sistema**
```csharp
// Arquivo: Common/Data/ItemAffix.cs
public class ItemAffix
{
    public AffixType Type { get; set; }
    public string Attribute { get; set; }
    public float Value { get; set; }
    public bool IsPercentage { get; set; }
    
    public static ItemAffix CreateRandom(Item item)
    {
        // Lógica de geração baseada no tipo de item
        // Referência: gemini.md seção 5
    }
    
    public string GetDisplayText()
    {
        // Formatação para tooltip
        // +5 Força, +10% XP Guerreiro, etc.
    }
}

public enum AffixType
{
    PrimaryAttribute,    // +5 Força, Destreza, etc.
    ClassBonus,         // +10% XP Guerreiro, +2 Níveis Acrobata
    WeaponProficiency,  // +15% dano Espadas
    Utility            // +5% velocidade mineração
}
```

**DIA 2: GlobalItem Implementation**
```csharp
// Arquivo: Common/GlobalItems/RPGGlobalItem.cs
public override bool OnCreate(Item item)
{
    if (CanHaveAffixes(item))
    {
        GenerateAffixes(item);
    }
}

public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
{
    // Exibir afixos com cores específicas
    // Azul: Atributos, Verde: Classes, Roxo: Proficiência
}

public override void SaveData(Item item, TagCompound tag)
public override void LoadData(Item item, TagCompound tag)
```

**DIA 3: Integração e Testes**
- Testar geração de afixos
- Validar save/load
- Verificar tooltips
- Performance check

##### **✅ Critérios de Sucesso:**
- [ ] 15% dos itens gerados têm afixos
- [ ] Tooltips exibem corretamente
- [ ] Save/load funcional
- [ ] 4 tipos de afixo implementados

---

#### **DIAS 4-7: PROFICIÊNCIA DE ARMADURAS** ⭐ NOVO
**Tempo:** 6-8 horas

##### **📚 Referências:**
- [Player Hooks](https://github.com/tModLoader/tModLoader/wiki/Player-Class-Documentation)
- [Armor System](https://github.com/tModLoader/tModLoader/wiki/Basic-Armor)

##### **🔧 Implementação:**

**DIA 4-5: Sistema Base**
```csharp
// Arquivo: Common/Players/RPGPlayer.cs (Extensões)
public Dictionary<ArmorType, int> armorProficiencyLevels;
public Dictionary<ArmorType, float> armorProficiencyExperience;

public enum ArmorType
{
    None,
    Light,      // Armadura Leve - velocidade
    Heavy,      // Armadura Pesada - defesa
    MagicRobes  // Vestes Mágicas - mana
}

public override void OnHitByNPC(NPC npc, PlayerDeathReason damageSource, int damage, bool crit)
{
    GainArmorProficiencyXP(GetEquippedArmorType(), damage * 0.1f);
}
```

**DIA 6: Detecção de Tipos**
```csharp
private ArmorType GetEquippedArmorType()
{
    // Lógica para determinar tipo baseado em:
    // - Bônus de mana (Magic)
    // - Total de defesa (Heavy vs Light)
    // - Velocidade de movimento
}

private void GainArmorProficiencyXP(ArmorType type, float xp)
{
    // Sistema de XP e level up
    // Feedback visual
}
```

**DIA 7: Integração**
- Save/Load de proficiências
- UI placeholders
- Testes

##### **✅ Critérios de Sucesso:**
- [ ] 3 tipos de armadura detectados
- [ ] XP ganha ao receber dano
- [ ] Level up funcional
- [ ] Dados persistem

---

#### **DIAS 8-10: REDESIGN DAS 10 CLASSES** ⭐ REDESIGN
**Tempo:** 6-8 horas

##### **📚 Referência de Design:**
- **gemini.md seção 3:** Estrutura das Classes
- [Class System Examples](https://github.com/tModLoader/tModLoader/wiki/ModPlayer#example-usage)

##### **🔧 Nova Estrutura:**

**CLASSES DE COMBATE (4):**
1. **Guerreiro** - Dash, Fúria, Resistência
2. **Arqueiro** - Tiro Certeiro, Precisão, Chuva de Flechas  
3. **Mago** - Amplificação Mágica, Redução de Mana
4. **Invocador** - Minions Aprimorados, Sincronização

**CLASSES DE UTILIDADE (6):**
1. **Acrobata** - Dash Básico → Pulo Duplo → Dash Aéreo
2. **Explorador** - Visão → Detector → Mapa Automático
3. **Engenheiro** - Construção Rápida, Ferramentas
4. **Sobrevivencialista** - Resistência Ambiental
5. **Ferreiro** - Craft Aprimorado, Durabilidade
6. **Alquimista** - Poções Potentes, Transmutação

##### **📋 Implementação:**
```csharp
// Arquivo: Common/Classes/RPGClassDefinitions.cs
public static class RPGClassDefinitions
{
    public static readonly RPGClass Acrobat = new RPGClass
    {
        Name = "Acrobata",
        Category = ClassCategory.Utility,
        Milestones = new Dictionary<int, string>
        {
            { 1, "Dash Básico" },
            { 25, "Pulo Duplo" },
            { 50, "Dash Aprimorado" },
            { 75, "Dash Evasivo" },
            { 100, "Segundo Dash Aéreo" }
        }
    };
    
    // ... todas as 10 classes
}
```

##### **✅ Critérios de Sucesso:**
- [ ] 10 classes implementadas
- [ ] Milestones definidos (1, 25, 50, 75, 100)
- [ ] Sistema de habilidades por nível
- [ ] XP por ações específicas

---

#### **DIAS 11-14: CÁLCULO INTEGRADO** 🧮
**Tempo:** 4-6 horas

##### **🔧 Sistema Unificado:**
```csharp
// Arquivo: Common/Systems/RPGCalculations.cs
public static class RPGCalculations
{
    public static float CalculateTotalBonus(Player player, BonusType type)
    {
        float total = 0f;
        
        // 1. Nível do Jogador
        total += GetPlayerLevelBonus(player, type);
        
        // 2. Classes
        total += GetClassBonus(player, type);
        
        // 3. Proficiências
        total += GetProficiencyBonus(player, type);
        
        // 4. Afixos de Itens ⭐ NOVO
        total += GetAffixBonus(player, type);
        
        return total;
    }
}
```

---

### **SEMANA 3: UI E INTEGRAÇÃO (Dias 15-21)**

#### **DIAS 15-16: UI DE 3 ABAS** 🎨
**Tempo:** 6-8 horas

##### **📚 Referências UI:**
- [UI Development](https://github.com/tModLoader/tModLoader/wiki/UI-Development)
- [UIElement Examples](https://github.com/tModLoader/tModLoader/tree/1.4/ExampleMod/Common/UI)

##### **🔧 Nova Estrutura UI:**
```csharp
// Arquivo: Common/UI/Menus/MainRPGMenu.cs
public class MainRPGMenu : UIState
{
    private RPGTabButton _attributesTab;
    private RPGTabButton _classesTab;
    private RPGTabButton _proficienciesTab;
    
    private AttributesPage _attributesPage;
    private ClassesPage _classesPage;  
    private ProficienciesPage _proficienciesPage;
}

// ABA 1: Atributos (5 primários + distribuição)
// ABA 2: Classes (10 classes + milestones)  
// ABA 3: Proficiências (armas + armaduras)
```

#### **DIAS 17-18: TOOLTIPS DE AFIXOS** 💬
**Tempo:** 3-4 horas

##### **🔧 Sistema de Cores:**
- **Azul:** Atributos primários (+5 Força)
- **Verde:** Bônus de classe (+10% XP Guerreiro)
- **Roxo:** Proficiência (+15% dano Espadas)
- **Amarelo:** Utilidade (+5% velocidade)

#### **DIAS 19-21: INTEGRAÇÃO COMPLETA** 🔗
**Tempo:** 4-6 horas

- Integrar cálculos em tempo real
- Conectar UI com dados
- Sistema de preview
- Validação completa

---

### **SEMANA 4: COMPATIBILIDADE TMODLOADER (Dias 22-28)**

#### **DIAS 22-23: ASSET LOADING MODERNO** ⚡
**Igual à Opção A, mas aplicado ao novo sistema**

#### **DIAS 24-25: NETWORK SYNC COMPLETO** 🌐
**Sincronização de:**
- Afixos de itens
- Proficiências de armadura
- Novas classes
- Estado da UI

#### **DIAS 26-28: LOCALIZAÇÃO NOVA VISÃO** 🌐
**Expandir localização para:**
- 10 classes novas
- 4 tipos de afixos
- 3 tipos de armadura
- Nova UI de 3 abas

---

### **SEMANA 5: TESTES E POLISH (Dias 29-35)**

#### **DIAS 29-31: TESTES EXTENSIVOS** 🧪
- **Teste de afixos:** Geração, save/load, tooltips
- **Teste de armaduras:** Detecção, XP, level up
- **Teste de classes:** XP por ações, milestones
- **Teste de UI:** 3 abas, navegação, responsividade

#### **DIAS 32-33: BALANCEAMENTO** ⚖️
- **Chances de afixos** (ajustar de 15%)
- **XP de proficiências** (balancear ganho)
- **Milestones de classes** (ajustar níveis)
- **Valores dos bônus** (evitar OP)

#### **DIAS 34-35: RELEASE PREPARATION** 🚀
- Documentação completa
- Changelog detalhado  
- Build final
- Testes de compatibilidade

---

## 📊 **MÉTRICAS DE SUCESSO FINAL**

### **Funcionalidades Implementadas:**
- ✅ **Sistema de 3 Eixos** completo
- ✅ **10 Classes** com milestones únicos
- ✅ **Afixos em Itens** com 4 tipos
- ✅ **Proficiência de Armaduras** com 3 tipos
- ✅ **UI de 3 Abas** moderna e intuitiva
- ✅ **Cálculo Integrado** de todas as fontes

### **Compatibilidade:**
- ✅ **tModLoader 2024+** totalmente compatível
- ✅ **Multiplayer** com sync completo
- ✅ **Performance** otimizada
- ✅ **Save/Load** seguro

### **Experiência do Usuário:**
- ✅ **Progressão Rica** em 3 dimensões
- ✅ **Loot Emocionante** com afixos
- ✅ **Especialização** via proficiências
- ✅ **Interface Intuitiva** e organizada

---

## 🛠️ **FERRAMENTAS E RECURSOS**

### **Referências Principais:**
- [tModLoader Wiki Completa](https://github.com/tModLoader/tModLoader/wiki)
- [ExampleMod Source Code](https://github.com/tModLoader/tModLoader/tree/1.4/ExampleMod)
- [Assets Guide](https://github.com/tModLoader/tModLoader/wiki/Assets)
- [UI Development](https://github.com/tModLoader/tModLoader/wiki/UI-Development)
- [Multiplayer Support](https://github.com/tModLoader/tModLoader/wiki/Multiplayer-Support)

### **Comunidade:**
- [tModLoader Discord](https://discord.gg/tmodloader)
- [Official Forums](https://forums.terraria.org/index.php?forums/tmodloader.161/)
- [Reddit Community](https://www.reddit.com/r/tModLoader/)

### **Debugging:**
- [Common Errors Guide](https://github.com/tModLoader/tModLoader/wiki/Common-Errors)
- [Performance Guide](https://github.com/tModLoader/tModLoader/wiki/Performance-Guide)
- [Debugging Tips](https://github.com/tModLoader/tModLoader/wiki/Debugging)

---

## ⚠️ **RISCOS E CONSIDERAÇÕES**

### **Riscos Técnicos:**
- **Complexidade alta** - mais chances de bugs
- **Performance impact** - sistema mais pesado
- **Compatibility issues** - mais pontos de falha

### **Mitigações:**
- **Implementação incremental** - testar cada parte
- **Performance profiling** - monitorar constantemente
- **Extensive testing** - cenários diversos
- **Community feedback** - beta testing

### **Contingência:**
- **Rollback plan** - manter versão estável
- **Feature flags** - poder desabilitar recursos
- **Modular design** - partes independentes

---

## 🎯 **RESULTADO FINAL ESPERADO**

Um mod RPG **completo e moderno** que implementa fielmente a visão do `gemini.md`:

### **Para o Jogador:**
- **Progressão rica** em múltiplas dimensões
- **Loot emocionante** com afixos únicos
- **Especialização profunda** via proficiências
- **Interface moderna** e intuitiva

### **Para o Desenvolvedor:**
- **Arquitetura sólida** para expansões futuras
- **Código limpo** e bem documentado
- **Performance otimizada** 
- **Compatibilidade total** com tModLoader 2024+

### **Para a Comunidade:**
- **Referência** de mod RPG bem feito
- **Base sólida** para colaborações
- **Exemplo** de boas práticas tModLoader

---

**🌟 Esta opção transforma completamente o mod, implementando a visão 2024 e criando uma base sólida para o futuro do projeto.**

---
*Roadmap criado em: 2024-12-28*  
*Baseado no design completo do gemini.md e pesquisa de práticas atuais* 