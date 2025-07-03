# EXEMPLOS DE CORREÇÕES - WOLFGOD RPG MOD
**Atualizado para Nova Visão 2024: Sistema de 3 Eixos + Afixos**

## 🎯 NOVA VISÃO DO PROJETO (Contexto)

O projeto foi **redesignado em 2024** para implementar:
1. **Sistema de 3 Eixos:** Nível Geral + Classes (10) + Proficiências  
2. **Afixos Aleatórios** em itens com bônus dinâmicos
3. **Proficiência de Armaduras** (Pesada/Leve/Mágicas)
4. **UI de 3 Abas** específicas

## 🚨 CORREÇÃO 1: ASSET LOADING MODERNO

### Arquivo: `Common/UI/RPGTabButton.cs`

**❌ CÓDIGO ATUAL (PROBLEMÁTICO):**
```csharp
public void Initialize()
{
    _normalTexture = ModContent.Request<Texture2D>(_normalTexturePath, AssetRequestMode.ImmediateLoad).Value;
    _selectedTexture = ModContent.Request<Texture2D>(_selectedTexturePath, AssetRequestMode.ImmediateLoad).Value;
    
    Width.Set(_normalTexture.Width, 0f);
    Height.Set(_normalTexture.Height, 0f);
}
```

**✅ CÓDIGO CORRIGIDO (MODERNO):**
```csharp
private Asset<Texture2D> _normalTextureAsset;
private Asset<Texture2D> _selectedTextureAsset;

public void Initialize()
{
    // Request assets without ImmediateLoad
    _normalTextureAsset = ModContent.Request<Texture2D>(_normalTexturePath);
    _selectedTextureAsset = ModContent.Request<Texture2D>(_selectedTexturePath);
    
    // Set dimensions only when loaded
    if (_normalTextureAsset.IsLoaded)
    {
        Width.Set(_normalTextureAsset.Value.Width, 0f);
        Height.Set(_normalTextureAsset.Value.Height, 0f);
    }
    else
    {
        // Fallback dimensions
        Width.Set(32, 0f);
        Height.Set(32, 0f);
    }
}

protected override void DrawSelf(SpriteBatch spriteBatch)
{
    var texture = _isSelected 
        ? (_selectedTextureAsset.IsLoaded ? _selectedTextureAsset.Value : null)
        : (_normalTextureAsset.IsLoaded ? _normalTextureAsset.Value : null);
        
    if (texture != null)
    {
        var position = GetDimensions().Position();
        spriteBatch.Draw(texture, position, Color.White);
    }
}
```

## 🚨 CORREÇÃO 2: MAIN.LOCALPLAYER SEGURO

### Arquivo: `Common/Systems/RPGActionSystem.cs`

**❌ CÓDIGO ATUAL (INSEGURO):**
```csharp
public void RegisterDamageDealt()
{
    var player = Main.LocalPlayer; // PERIGOSO - pode ser null
    var rpgPlayer = player.GetModPlayer<RPGPlayer>();
    rpgPlayer.GainClassExp("melee", 10);
}
```

**✅ CÓDIGO CORRIGIDO (SEGURO):**
```csharp
public void RegisterDamageDealt()
{
    // Verificação segura completa
    if (Main.LocalPlayer == null || !Main.LocalPlayer.active)
        return;
        
    var player = Main.LocalPlayer;
    var rpgPlayer = player.GetModPlayer<RPGPlayer>();
    
    if (rpgPlayer != null)
    {
        rpgPlayer.GainClassExp("melee", 10);
    }
}

// Alternativa usando helper method
private bool TryGetLocalRPGPlayer(out RPGPlayer rpgPlayer)
{
    rpgPlayer = null;
    
    if (Main.LocalPlayer?.active != true)
        return false;
        
    rpgPlayer = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
    return rpgPlayer != null;
}

public void RegisterDamageDealt()
{
    if (TryGetLocalRPGPlayer(out var rpgPlayer))
    {
        rpgPlayer.GainClassExp("melee", 10);
    }
}
```

## 🚨 CORREÇÃO 3: LOCALIZAÇÃO MODERNA

### Arquivo: `Localization/en-US_Mods.Wolfgodrpg.hjson`

**❌ ESTRUTURA ATUAL (PROBLEMÁTICA):**
```hjson
Mods: {
    Wolfgodrpg: {
        Config: {
            ExpMultiplier: {
                Label: Exp Multiplier
                Tooltip: ""  // ❌ Tooltip vazio
            }
            // ... muitas duplicações
        }
        // Estrutura inconsistente
    }
}
```

**✅ ESTRUTURA CORRIGIDA (MODERNA):**
```hjson
Mods: {
    Wolfgodrpg: {
        Config: {
            ExpMultiplier: {
                Label: Experience Multiplier
                Tooltip: Global experience multiplier for all classes (1.0 = normal, 2.0 = double)
            }
            KeepXPOnDeath: {
                Label: Keep XP on Death
                Tooltip: If enabled, players don't lose experience when dying
            }
            EnableHunger: {
                Label: Enable Hunger System
                Tooltip: Enable the hunger mechanic that affects health regeneration
            }
            HungerRate: {
                Label: Hunger Decay Rate
                Tooltip: How fast hunger decreases over time (1.0 = normal, 2.0 = twice as fast)
            }
        }
        
        Items: {
            RPGTestItem: {
                DisplayName: RPG Test Item
                Tooltip: A test item for the RPG system
            }
        }
        
        NPCs: {
            RPGTestNPC: {
                DisplayName: RPG Test NPC
            }
        }
        
        Common: {
            LevelUp: Level Up!
            ClassUnlocked: "New class unlocked: {0}"
            InsufficientLevel: "Requires level {0} to use"
        }
    }
}
```

## 🚨 CORREÇÃO 4: NETWORK SYNC ADEQUADO

### Arquivo: `Common/Players/RPGPlayer.cs`

**❌ IMPLEMENTAÇÃO ATUAL (INCOMPLETA):**
```csharp
// Falta implementação de network sync
public override void SaveData(TagCompound tag) { /* ... */ }
public override void LoadData(TagCompound tag) { /* ... */ }
```

**✅ IMPLEMENTAÇÃO CORRIGIDA (COMPLETA):**
```csharp
// Adicionar campos para tracking de mudanças
private int _lastSyncedHunger;
private int _lastSyncedSanity;
private Dictionary<string, int> _lastSyncedClassLevels = new();

public override void SendClientChanges(ModPlayer clientPlayer)
{
    var client = (RPGPlayer)clientPlayer;
    
    // Sync vitals if changed significantly
    if (Math.Abs(Hunger - client.Hunger) > 5 || 
        Math.Abs(Sanity - client.Sanity) > 5)
    {
        var packet = Mod.GetPacket();
        packet.Write((byte)WolfgodrpgMessageType.SyncVitals);
        packet.Write(Player.whoAmI);
        packet.Write(Hunger);
        packet.Write(Sanity);
        packet.Send();
    }
    
    // Sync class levels if changed
    foreach (var kvp in ClassLevels)
    {
        if (!client.ClassLevels.TryGetValue(kvp.Key, out int clientLevel) || 
            clientLevel != kvp.Value)
        {
            var packet = Mod.GetPacket();
            packet.Write((byte)WolfgodrpgMessageType.SyncClassLevel);
            packet.Write(Player.whoAmI);
            packet.Write(kvp.Key);
            packet.Write(kvp.Value);
            packet.Send();
        }
    }
}

public override void CopyClientState(ModPlayer targetCopy)
{
    var copy = (RPGPlayer)targetCopy;
    
    // Copy network-synced data only
    copy.Hunger = Hunger;
    copy.Sanity = Sanity;
    copy.ClassLevels = new Dictionary<string, int>(ClassLevels);
}

public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
{
    if (Main.netMode == NetmodeID.Server)
    {
        var packet = Mod.GetPacket();
        packet.Write((byte)WolfgodrpgMessageType.SyncPlayerData);
        packet.Write(Player.whoAmI);
        packet.Write(Hunger);
        packet.Write(Sanity);
        packet.Write(ClassLevels.Count);
        foreach (var kvp in ClassLevels)
        {
            packet.Write(kvp.Key);
            packet.Write(kvp.Value);
        }
        packet.Send(toWho, fromWho);
    }
}
```

## 🚨 CORREÇÃO 5: BUILD.TXT ATUALIZADO

### Arquivo: `build.txt`

**❌ ATUAL (INCOMPLETO):**
```
displayName = WolfGod RPG
author = WolfGod
version = 0.7
side = Both
```

**✅ CORRIGIDO (COMPLETO):**
```
displayName = WolfGod RPG
author = WolfGod
version = 0.7.1
side = Both

# Metadata moderna
homepage = https://github.com/[seu-usuario]/Wolfgodrpg
description = Complete RPG system for Terraria featuring classes, levels, hunger, sanity, and progressive items
includeSource = true

# Performance e compatibilidade
includePDB = false
buildIgnore = *.csproj.user, *.suo, bin/, obj/, .vs/, .git/, *.md, *.log

# Opcional: dependências se houver
# weakReferences = SomeOtherMod@1.0
# modReferences = RequiredMod@2.0

# Opcional: versão mínima do tModLoader
# tModLoaderVersion = 2024.1
```

## 🚨 CORREÇÃO 6: ORGANIZAÇÃO DE PASTAS

### Estrutura Atual vs Recomendada

**❌ ATUAL (DESORGANIZADO):**
```
Common/
├── UI/
│   ├── SimpleRPGMenu.cs
│   ├── RPGStatsUI.cs
│   ├── QuickStatsUI.cs
│   ├── RPGTabButton.cs
│   └── ... (misturado)
├── Systems/
│   ├── RPGActionSystem.cs
│   ├── RPGDebugSystem.cs
│   ├── PlayerVitalsSystem.cs
│   └── ... (inconsistente)
└── Players/
    └── RPGPlayer.cs
```

**✅ RECOMENDADO (ORGANIZADO):**
```
Common/
├── Core/
│   ├── WolfgodrpgMod.cs
│   └── Constants.cs
├── Systems/
│   ├── RPGSystem.cs (core logic)
│   ├── VitalsSystem.cs
│   ├── ExperienceSystem.cs
│   └── ConfigSystem.cs
├── Players/
│   ├── RPGPlayer.cs
│   └── RPGPlayerNetwork.cs
├── UI/
│   ├── Base/
│   │   ├── RPGUIElement.cs
│   │   ├── RPGPanel.cs
│   │   └── RPGButton.cs
│   ├── Menus/
│   │   ├── MainRPGMenu.cs
│   │   ├── StatsMenu.cs
│   │   └── ClassesMenu.cs
│   └── HUD/
│       ├── QuickStatsUI.cs
│       ├── VitalsBar.cs
│       └── NotificationSystem.cs
├── GlobalClasses/
│   ├── RPGGlobalItem.cs
│   ├── RPGGlobalNPC.cs
│   └── RPGGlobalTile.cs
├── Network/
│   ├── NetworkHandler.cs
│   └── Messages/
│       ├── SyncVitalsMessage.cs
│       └── SyncClassLevelMessage.cs
└── Utils/
    ├── RPGUtils.cs
    ├── DebugLogger.cs
    └── Extensions.cs
```

## 📋 TEMPLATE PARA NOVOS ARQUIVOS

### Template ModSystem:
```csharp
using Terraria.ModLoader;

namespace Wolfgodrpg.Common.Systems
{
    public class ExampleSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            // Initialization after all mods loaded
        }
        
        public override void PostUpdateEverything()
        {
            // Called every tick
        }
        
        public override void ClearWorld()
        {
            // Reset world-specific data
        }
    }
}
```

### Template UI Element:
```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace Wolfgodrpg.Common.UI.Base
{
    public class ExampleUIElement : UIElement
    {
        private Asset<Texture2D> _textureAsset;
        
        public override void OnInitialize()
        {
            _textureAsset = ModContent.Request<Texture2D>("Wolfgodrpg/Assets/UI/ExampleTexture");
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (_textureAsset.IsLoaded)
            {
                var dimensions = GetDimensions();
                spriteBatch.Draw(_textureAsset.Value, dimensions.Position(), Color.White);
            }
        }
    }
}
```

---
**Nota:** Implemente essas correções uma por vez e teste depois de cada mudança para garantir que não quebre nada existente. 