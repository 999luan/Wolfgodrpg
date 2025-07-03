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

## ⭐ CORREÇÃO 6: SISTEMA DE AFIXOS (NOVA FUNCIONALIDADE)

### **Implementação do Sistema de Afixos em Itens**

**✅ ARQUIVO: `Common/GlobalItems/RPGGlobalItem.cs` (Adições)**
```csharp
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

public class RPGGlobalItem : GlobalItem
{
    // Armazenar afixos no item
    private List<ItemAffix> affixes = new List<ItemAffix>();
    
    public override bool OnCreate(Item item)
    {
        // Gerar afixos apenas para itens que podem ter (armas/armaduras)
        if (CanHaveAffixes(item))
        {
            GenerateAffixes(item);
        }
        return base.OnCreate(item);
    }
    
    private bool CanHaveAffixes(Item item)
    {
        return item.damage > 0 || item.defense > 0 || item.pick > 0 || item.axe > 0;
    }
    
    private void GenerateAffixes(Item item)
    {
        // Chance base de 15% de ter afixos
        if (Main.rand.NextFloat() < 0.15f)
        {
            int affixCount = Main.rand.Next(1, 4); // 1-3 afixos
            
            for (int i = 0; i < affixCount; i++)
            {
                ItemAffix newAffix = GenerateRandomAffix(item);
                if (newAffix != null && !HasAffixType(newAffix.Type))
                {
                    affixes.Add(newAffix);
                }
            }
        }
    }
    
    private bool HasAffixType(AffixType type)
    {
        return affixes.Any(a => a.Type == type);
    }
    
    private ItemAffix GenerateRandomAffix(Item item)
    {
        var affixTypes = new[]
        {
            AffixType.PrimaryAttribute,  // +5 Força
            AffixType.ClassBonus,        // +10% XP Guerreiro
            AffixType.WeaponProficiency, // +15% dano Espadas
            AffixType.Utility           // +5% velocidade mineração
        };
        
        AffixType type = Main.rand.Next(affixTypes);
        return ItemAffix.Create(type, item);
    }
    
    public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
    {
        if (affixes.Count > 0)
        {
            // Adicionar linha separadora
            tooltips.Add(new TooltipLine(Mod, "AffixSeparator", "--- Afixos ---"));
            
            // Adicionar cada afixo com cor específica
            foreach (var affix in affixes)
            {
                Color color = GetAffixColor(affix.Type);
                tooltips.Add(new TooltipLine(Mod, $"Affix_{affix.Type}", 
                    affix.GetDisplayText()) { OverrideColor = color });
            }
        }
    }
    
    private Color GetAffixColor(AffixType type)
    {
        return type switch
        {
            AffixType.PrimaryAttribute => Color.CornflowerBlue,
            AffixType.ClassBonus => Color.LimeGreen,
            AffixType.WeaponProficiency => Color.MediumPurple,
            AffixType.Utility => Color.Gold,
            _ => Color.White
        };
    }
    
    // Salvar/Carregar afixos
    public override void SaveData(Item item, TagCompound tag)
    {
        if (affixes.Count > 0)
        {
            tag["RPGAffixes"] = affixes.Select(a => a.Save()).ToList();
        }
    }
    
    public override void LoadData(Item item, TagCompound tag)
    {
        if (tag.ContainsKey("RPGAffixes"))
        {
            var affixData = tag.GetList<TagCompound>("RPGAffixes");
            affixes = affixData.Select(ItemAffix.Load).ToList();
        }
    }
}

// Classes auxiliares para afixos
public class ItemAffix
{
    public AffixType Type { get; set; }
    public string Attribute { get; set; }
    public float Value { get; set; }
    public bool IsPercentage { get; set; }
    
    public string GetDisplayText()
    {
        string valueText = IsPercentage ? $"+{Value:F1}%" : $"+{Value:F0}";
        return $"{valueText} {Attribute}";
    }
    
    public static ItemAffix Create(AffixType type, Item item)
    {
        return type switch
        {
            AffixType.PrimaryAttribute => new ItemAffix
            {
                Type = type,
                Attribute = GetRandomAttribute(),
                Value = Main.rand.Next(3, 8),
                IsPercentage = false
            },
            AffixType.ClassBonus => new ItemAffix
            {
                Type = type,
                Attribute = $"XP {GetRandomClass()}",
                Value = Main.rand.Next(10, 25),
                IsPercentage = true
            },
            _ => null
        };
    }
    
    private static string GetRandomAttribute()
    {
        var attrs = new[] { "Força", "Destreza", "Inteligência", "Constituição", "Sabedoria" };
        return attrs[Main.rand.Next(attrs.Length)];
    }
    
    private static string GetRandomClass()
    {
        var classes = new[] { "Guerreiro", "Arqueiro", "Mago", "Acrobata", "Explorador" };
        return classes[Main.rand.Next(classes.Length)];
    }
    
    public TagCompound Save()
    {
        return new TagCompound
        {
            ["Type"] = (int)Type,
            ["Attribute"] = Attribute,
            ["Value"] = Value,
            ["IsPercentage"] = IsPercentage
        };
    }
    
    public static ItemAffix Load(TagCompound tag)
    {
        return new ItemAffix
        {
            Type = (AffixType)tag.GetInt("Type"),
            Attribute = tag.GetString("Attribute"),
            Value = tag.GetFloat("Value"),
            IsPercentage = tag.GetBool("IsPercentage")
        };
    }
}

public enum AffixType
{
    PrimaryAttribute,
    ClassBonus,
    WeaponProficiency,
    Utility
}
```

---

## ⭐ CORREÇÃO 7: PROFICIÊNCIA DE ARMADURAS (NOVA FUNCIONALIDADE)

### **Adição do Sistema de Proficiência de Armaduras**

**✅ ARQUIVO: `Common/Players/RPGPlayer.cs` (Adições ao final da classe)**
```csharp
// Adicionar aos campos existentes no topo da classe:
public Dictionary<ArmorType, int> armorProficiencyLevels = new Dictionary<ArmorType, int>();
public Dictionary<ArmorType, float> armorProficiencyExperience = new Dictionary<ArmorType, float>();

// Adicionar aos métodos existentes:
public override void Initialize()
{
    base.Initialize();
    
    // Inicializar proficiências de armadura
    foreach (ArmorType armorType in System.Enum.GetValues<ArmorType>())
    {
        armorProficiencyLevels[armorType] = 1;
        armorProficiencyExperience[armorType] = 0f;
    }
}

public override void OnHitByNPC(NPC npc, PlayerDeathReason damageSource, int damage, bool crit)
{
    // Sistema de XP por receber dano
    if (damage > 0)
    {
        ArmorType currentArmorType = GetEquippedArmorType();
        if (currentArmorType != ArmorType.None)
        {
            GainArmorProficiencyXP(currentArmorType, damage * 0.1f);
        }
    }
    
    base.OnHitByNPC(npc, damageSource, damage, crit);
}

public override void OnHitByProjectile(Projectile proj, PlayerDeathReason damageSource, int damage, bool crit)
{
    // Sistema de XP por receber dano de projéteis
    if (damage > 0)
    {
        ArmorType currentArmorType = GetEquippedArmorType();
        if (currentArmorType != ArmorType.None)
        {
            GainArmorProficiencyXP(currentArmorType, damage * 0.1f);
        }
    }
    
    base.OnHitByProjectile(proj, damageSource, damage, crit);
}

private ArmorType GetEquippedArmorType()
{
    // Determinar tipo baseado na armadura equipada
    Item helmet = Player.armor[0];
    Item chestplate = Player.armor[1];
    Item leggings = Player.armor[2];
    
    if (!helmet.IsAir || !chestplate.IsAir || !leggings.IsAir)
    {
        // Lógica para determinar tipo (simplificada)
        if (IsMagicArmor(helmet, chestplate, leggings))
            return ArmorType.MagicRobes;
        else if (IsHeavyArmor(helmet, chestplate, leggings))
            return ArmorType.Heavy;
        else
            return ArmorType.Light;
    }
    
    return ArmorType.None;
}

private void GainArmorProficiencyXP(ArmorType armorType, float xp)
{
    armorProficiencyExperience[armorType] += xp;
    
    // Verificar level up
    float xpNeeded = GetArmorXPNeeded(armorProficiencyLevels[armorType]);
    if (armorProficiencyExperience[armorType] >= xpNeeded)
    {
        armorProficiencyLevels[armorType]++;
        armorProficiencyExperience[armorType] -= xpNeeded;
        
        // Feedback visual de level up
        ShowArmorLevelUpEffect(armorType);
    }
}

private float GetArmorXPNeeded(int level)
{
    return 100f + (level * 50f); // XP cresce com o nível
}

private bool IsMagicArmor(Item helmet, Item chest, Item legs)
{
    // Verificar se é armadura mágica (Mana bonus, etc.)
    return helmet.manaIncrease > 0 || chest.manaIncrease > 0 || legs.manaIncrease > 0;
}

private bool IsHeavyArmor(Item helmet, Item chest, Item legs)
{
    // Verificar se é armadura pesada (alta defesa)
    int totalDefense = helmet.defense + chest.defense + legs.defense;
    return totalDefense >= 20; // Threshold para armadura pesada
}

private void ShowArmorLevelUpEffect(ArmorType armorType)
{
    // Efeito visual e som de level up
    Main.NewText($"Proficiência com {armorType} aumentou para nível {armorProficiencyLevels[armorType]}!", 
                 Color.Gold);
}

// Adicionar ao SaveData existente:
public override void SaveData(TagCompound tag)
{
    // ... código existente ...
    
    // Salvar proficiências de armadura
    tag["ArmorProficiencyLevels"] = armorProficiencyLevels.ToDictionary(
        kvp => kvp.Key.ToString(), 
        kvp => kvp.Value);
    tag["ArmorProficiencyExperience"] = armorProficiencyExperience.ToDictionary(
        kvp => kvp.Key.ToString(), 
        kvp => kvp.Value);
}

// Adicionar ao LoadData existente:
public override void LoadData(TagCompound tag)
{
    // ... código existente ...
    
    // Carregar proficiências de armadura
    if (tag.ContainsKey("ArmorProficiencyLevels"))
    {
        var levels = tag.GetStringIntDictionary("ArmorProficiencyLevels");
        foreach (var kvp in levels)
        {
            if (System.Enum.TryParse<ArmorType>(kvp.Key, out ArmorType type))
            {
                armorProficiencyLevels[type] = kvp.Value;
            }
        }
    }
    
    if (tag.ContainsKey("ArmorProficiencyExperience"))
    {
        var experience = tag.GetStringFloatDictionary("ArmorProficiencyExperience");
        foreach (var kvp in experience)
        {
            if (System.Enum.TryParse<ArmorType>(kvp.Key, out ArmorType type))
            {
                armorProficiencyExperience[type] = kvp.Value;
            }
        }
    }
}

// Enum para tipos de armadura (adicionar ao final do arquivo)
public enum ArmorType
{
    None,
    Light,      // Armadura Leve
    Heavy,      // Armadura Pesada  
    MagicRobes  // Vestes Mágicas
}
```

---

**Nota:** Implemente essas correções uma por vez e teste depois de cada mudança para garantir que não quebre nada existente. 