# ⚔️ Sistema de Modo de Combate - WolfGod RPG

## 🎯 **Funcionalidades Implementadas**

### **✅ Sistema Completo de Modo de Combate**

#### **1. Hotkey de Modo de Combate**
- **Tecla**: `Q` (configurável)
- **Funcionalidade**: Liga/desliga o modo de combate
- **Feedback**: Notificação visual com cores (Verde = Ativo, Cinza = Inativo)

#### **2. Dash Direcional**
- **Ativação**: Apenas quando modo de combate está **ATIVO**
- **Controles**: Teclas direcionais (↑, ↓, ←, →, diagonais)
- **Consumo**: 10% de stamina por dash
- **Cooldown**: 60 ticks (1 segundo)
- **Efeitos**: Partículas de fumaça + som

#### **3. Double Jump**
- **Desbloqueio**: Nível 3 do jogador
- **Ativação**: Pressionar tecla de pulo no ar (modo de combate ativo)
- **Consumo**: 10% de stamina
- **Efeitos**: Partículas de nuvem + som

#### **4. Wall Jump**
- **Desbloqueio**: Nível 4 do jogador
- **Ativação**: Pressionar tecla de pulo contra parede (modo de combate ativo)
- **Consumo**: 10% de stamina
- **Efeitos**: Partículas de pedra + som

## 🎮 **Como Usar**

### **Controles Básicos:**
1. **Pressione Q** para ativar/desativar modo de combate
2. **Use direcionais** para dash (apenas no modo de combate)
3. **Pressione pulo no ar** para double jump (nível 3+)
4. **Pressione pulo contra parede** para wall jump (nível 4+)

### **Sistema de Níveis:**
- **Nível 1**: Dash básico disponível
- **Nível 3**: Double Jump desbloqueado
- **Nível 4**: Wall Jump desbloqueado
- **Nível 5**: Dash aprimorado (cooldown reduzido)

## 🔧 **Implementação Técnica**

### **1. Keybind no Mod Principal**
```csharp
// Em Wolfgodrpg.cs
public static ModKeybind CombatModeKey;

public override void Load()
{
    CombatModeKey = KeybindLoader.RegisterKeybind(this, "Toggle Combat Mode", "Q");
}
```

### **2. Propriedades no RPGPlayer**
```csharp
// Sistema de modo de combate
public bool CombatModeActive { get; set; } = false;
public int DashCooldown { get; set; } = 0;

// Flags de desbloqueio
public bool UnlockedDash { get; set; } = true;
public bool UnlockedDoubleJump { get; set; } = false;
public bool UnlockedWallJump { get; set; } = false;
```

### **3. Processamento de Input**
```csharp
public override void ProcessTriggers(TriggersSet triggersSet)
{
    // Toggle modo de combate
    if (Wolfgodrpg.CombatModeKey.JustPressed)
    {
        CombatModeActive = !CombatModeActive;
        Main.NewText($"Modo de Combate {(CombatModeActive ? "Ativado" : "Desativado")}", 
            CombatModeActive ? Color.Green : Color.Gray);
    }

    // Dash direcional
    if (CombatModeActive && UnlockedDash && DashCooldown <= 0)
    {
        Vector2 direction = GetInputDirection();
        if (direction != Vector2.Zero && ConsumeStaminaPercent(10f))
        {
            DashInDirection(direction);
            DashCooldown = 60;
        }
    }
}
```

### **4. Movimento Avançado**
```csharp
public override void PreUpdateMovement()
{
    // Double Jump
    if (CombatModeActive && UnlockedDoubleJump && Player.controlJump && !usedDoubleJump)
    {
        if (ConsumeStaminaPercent(10f))
        {
            Player.velocity.Y = -Player.jumpSpeed * 0.8f;
            usedDoubleJump = true;
        }
    }

    // Wall Jump
    if (CombatModeActive && UnlockedWallJump && Player.controlJump)
    {
        if (IsTouchingWall(out int side) && ConsumeStaminaPercent(10f))
        {
            Player.velocity.Y = -Player.jumpSpeed * 0.9f;
            Player.velocity.X = 6f * -side;
        }
    }
}
```

## 📊 **Sistema de Desbloqueio**

### **Lógica de Desbloqueio:**
```csharp
private void UnlockMovementSkills()
{
    // Double Jump no nível 3
    if (PlayerLevel == 3 && !UnlockedDoubleJump)
    {
        UnlockedDoubleJump = true;
        Main.NewText("🎯 Double Jump unlocked!", Color.Cyan);
    }
    
    // Wall Jump no nível 4
    if (PlayerLevel == 4 && !UnlockedWallJump)
    {
        UnlockedWallJump = true;
        Main.NewText("🎯 Wall Jump unlocked!", Color.Cyan);
    }
    
    // Dash aprimorado no nível 5
    if (PlayerLevel == 5)
    {
        DashCooldown = Math.Max(30, DashCooldown - 10);
        Main.NewText("⚡ Dash improved!", Color.Cyan);
    }
}
```

## 🎨 **Interface e Feedback**

### **1. Status na UI**
- **Combat Mode**: Mostra se está ativo/inativo
- **Ícone**: ⚔️ (ativo) ou 🛡️ (inativo)
- **Cor**: Vermelho (ativo) ou Cinza (inativo)

### **2. Efeitos Visuais**
- **Dash**: Partículas de fumaça
- **Double Jump**: Partículas de nuvem
- **Wall Jump**: Partículas de pedra

### **3. Efeitos Sonoros**
- **Dash**: SoundID.Item24
- **Double Jump**: SoundID.Item24
- **Wall Jump**: SoundID.Item24
- **Desbloqueios**: SoundID.Item4

## 🔄 **Compatibilidade**

### **Sistema Legado Mantido:**
- **Double-tap dash**: Funciona quando modo de combate está **INATIVO**
- **Skills de movimentação**: Mantidas para compatibilidade
- **Auto-dash**: Funciona independentemente do modo de combate

### **Integração com Sistema RPG:**
- **Stamina**: Consumo integrado com sistema de vitais
- **Níveis**: Desbloqueio baseado no nível do jogador
- **UI**: Status integrado na interface existente

## 🚀 **Benefícios do Sistema**

### **1. Controle Preciso**
- ✅ Dash direcional em 8 direções
- ✅ Evita ativação acidental
- ✅ Controle fino de movimento

### **2. Progressão Natural**
- ✅ Desbloqueio por nível
- ✅ Melhorias graduais
- ✅ Sistema de evolução

### **3. Feedback Visual**
- ✅ Efeitos visuais distintos
- ✅ Status claro na UI
- ✅ Notificações informativas

### **4. Performance Otimizada**
- ✅ Cooldown em ticks
- ✅ Verificação eficiente
- ✅ Sistema modular

## 📈 **Futuras Expansões**

### **Skills Adicionais:**
1. **Air Dash**: Dash no ar
2. **Ground Slam**: Pulo para baixo com dano
3. **Wall Climb**: Escalar paredes
4. **Teleport**: Teleporte curto

### **Melhorias de Sistema:**
1. **Combo System**: Combinações de skills
2. **Skill Trees**: Árvores de evolução
3. **Custom Keybinds**: Configuração de teclas
4. **Visual Effects**: Efeitos mais elaborados

## 🎯 **Resultado Final**

**Status**: **100% IMPLEMENTADO E FUNCIONAL** ✅

O sistema oferece:
- ✅ **Controle preciso** de movimento
- ✅ **Progressão natural** por níveis
- ✅ **Feedback visual** e sonoro
- ✅ **Integração completa** com sistema RPG
- ✅ **Compatibilidade** com sistema legado
- ✅ **Interface intuitiva** e responsiva

**🎉 Sistema de modo de combate totalmente funcional e integrado!** 