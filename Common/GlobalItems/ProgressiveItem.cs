using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Wolfgodrpg.Common.Systems;
using Wolfgodrpg.Common.GlobalClasses;

namespace Wolfgodrpg.Common.GlobalItems
{
    public class ProgressiveItem : GlobalItem
    {
        // === PROPRIEDADES ===
        public float Experience { get; set; } = 0f;
        public int UsageCount { get; set; } = 0;
        public bool HasRandomStats { get; set; } = false;
        
        // === CONSTANTES ===
        private const int MAX_LEVEL = 50;
        private const float DAMAGE_BONUS_PER_LEVEL = 0.02f;
        private const float ATTACK_SPEED_BONUS_PER_LEVEL = 0.01f;
        private const float MAX_DAMAGE_BONUS = 1.5f;
        private const float MAX_ATTACK_SPEED_BONUS = 0.5f;

        public override bool InstancePerEntity => true;

        // === CALCULAR NÍVEL ===
        public int GetItemLevel()
        {
            if (Experience <= 0) return 1;
            
            // Fórmula mais consistente: nível = raiz quadrada da experiência / 50 + 1
            int level = (int)Math.Floor(Math.Sqrt(Experience / 50.0f)) + 1;
            return Math.Min(level, MAX_LEVEL);
        }

        // === EXPERIÊNCIA PARA PRÓXIMO NÍVEL ===
        private float ExperienceForLevel(int level)
        {
            // Fórmula inversa: XP = (nível - 1)² * 50
            return (level - 1) * (level - 1) * 50f;
        }

        // === EXPERIÊNCIA ATUAL NO NÍVEL ===
        private float ExperienceInCurrentLevel(int level)
        {
            float currentLevelExp = ExperienceForLevel(level);
            return Experience - currentLevelExp;
        }

        // === EXPERIÊNCIA NECESSÁRIA PARA PRÓXIMO NÍVEL ===
        private float ExperienceNeededForNextLevel(int level)
        {
            float nextLevelExp = ExperienceForLevel(level + 1);
            float currentLevelExp = ExperienceForLevel(level);
            return nextLevelExp - currentLevelExp;
        }

        // === GANHAR EXPERIÊNCIA ===
        public void GainExperience(Item item, float amount, string reason = "usage")
        {
            if (!ShouldGainExperience(item)) return;
            
            float multiplier = reason.ToLower() switch
            {
                "crit" => 1.5f,
                "boss" => 3.0f,
                "elite" => 2.0f,
                _ => 1.0f
            };
            
            multiplier *= GetRarityMultiplier(item.rare);
            
            int oldLevel = GetItemLevel();
            float oldExp = Experience;
            Experience += amount * multiplier;
            UsageCount++;
            
            int newLevel = GetItemLevel();
            
            DebugLog.Item("GainExperience", $"Item '{item.Name}' gained {amount * multiplier:F1} XP (base: {amount:F1}, mult: {multiplier:F2}, reason: {reason}) - Level: {oldLevel}->{newLevel}, Total XP: {oldExp:F1}->{Experience:F1}");
            
            if (newLevel > oldLevel && Main.myPlayer >= 0 && Main.myPlayer < Main.player.Length)
            {
                Player player = Main.player[Main.myPlayer];
                if (player.active)
                {
                    DebugLog.Gameplay("Item", "GainExperience", $"LEVEL UP! Item '{item.Name}' leveled up to level {newLevel}");
                    // Notificação global para todos
                    Main.NewText($"{item.Name} leveled up to level {newLevel}!", Color.Gold);
                }
            }
        }

        // === MULTIPLICADOR DE RARIDADE ===
        private float GetRarityMultiplier(int rare)
        {
            return rare switch
            {
                -1 => 0.5f,  // Gray
                0 => 1.0f,   // White
                1 => 1.2f,   // Blue
                2 => 1.4f,   // Green
                3 => 1.6f,   // Orange
                4 => 1.8f,   // Light Red
                5 => 2.0f,   // Pink
                6 => 2.2f,   // Light Purple
                7 => 2.4f,   // Lime
                8 => 2.6f,   // Yellow
                9 => 2.8f,   // Cyan
                10 => 3.0f,  // Red
                11 => 3.5f,  // Purple
                _ => 1.0f
            };
        }

        // === VERIFICAR SE DEVE GANHAR EXPERIÊNCIA ===
        private bool ShouldGainExperience(Item item)
        {
            // Log de debug para entender o que está acontecendo
            DebugLog.Item("ShouldGainExperience", $"Checking item '{item.Name}' - damage: {item.damage}, defense: {item.defense}, consumable: {item.consumable}, DamageType: {item.DamageType}");

            // Itens que NÃO devem ganhar experiência
            if (item.damage <= 0 && item.defense <= 0) 
            {
                DebugLog.Item("ShouldGainExperience", $"Item '{item.Name}' rejected - no damage or defense");
                return false; // Sem dano nem defesa
            }
            if (item.axe > 0 || item.hammer > 0 || item.pick > 0) 
            {
                DebugLog.Item("ShouldGainExperience", $"Item '{item.Name}' rejected - is tool");
                return false; // Ferramentas
            }
            if (item.fishingPole > 0) 
            {
                DebugLog.Item("ShouldGainExperience", $"Item '{item.Name}' rejected - is fishing rod");
                return false; // Varas de pesca
            }
            if (item.consumable) 
            {
                DebugLog.Item("ShouldGainExperience", $"Item '{item.Name}' rejected - is consumable");
                return false; // Consumíveis
            }

            // Só armas (melee, ranged, magic, summon) e armaduras (defense > 0) ganham XP
            bool isWeapon = item.DamageType == DamageClass.Melee || 
                           item.DamageType == DamageClass.Ranged || 
                           item.DamageType == DamageClass.Magic || 
                           item.DamageType == DamageClass.Summon;
            bool isArmor = item.defense > 0;
            if ((isWeapon && item.damage > 0) || isArmor)
            {
                DebugLog.Item("ShouldGainExperience", $"Item '{item.Name}' APPROVED - is {(isWeapon ? "weapon" : "armor")}");
                return true;
            }
            // Nenhum outro tipo de item ganha XP
            return false;
        }

        // === APLICAR BÔNUS DE NÍVEL ===
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (!ShouldGainExperience(item)) return;
            
            int level = GetItemLevel();
            if (level > 1)
            {
                float damageBonus = (level - 1) * DAMAGE_BONUS_PER_LEVEL;
                damageBonus = Math.Min(damageBonus, MAX_DAMAGE_BONUS);
                
                if (damageBonus > 0)
                {
                    damage += damageBonus;
                    DebugLog.Item("ModifyWeaponDamage", $"Item '{item.Name}' (Level {level}) - Damage bonus applied: +{damageBonus:P0}");
                }
            }
        }

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            if (!ShouldGainExperience(item)) return;
            
            int level = GetItemLevel();
            if (level > 10) // Bônus de crit a partir do nível 10
            {
                float critBonus = (level - 10) * 0.5f; // +0.5% crit por nível após 10
                crit += critBonus;
                
                DebugLog.Item("ModifyWeaponCrit", $"Item '{item.Name}' (Level {level}) - Crit bonus applied: +{critBonus:F1}%");
            }
        }

        // === BÔNUS DE DEFESA PARA ARMADURAS ===
        // Nota: O bônus de defesa será aplicado via tooltips e cálculos manuais
        // pois o tModLoader não tem um hook direto para modificar defesa de itens

        // === HIT NPC ===
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!ShouldGainExperience(item)) return;
            
            float expGain = damageDone * 0.2f;
            
            string reason = "usage";
            if (hit.Crit) reason = "crit";
            if (target.boss) reason = "boss";
            
            // Bônus de XP para inimigos elite
            var balancedNPC = target.GetGlobalNPC<BalancedNPC>();
            if (balancedNPC.IsElite)
            {
                reason = "elite";
            }
            
            DebugLog.Item("OnHitNPC", $"Item '{item.Name}' hit '{target.FullName}' - Damage: {damageDone}, XP: {expGain:F1}, Reason: {reason}");
            
            GainExperience(item, expGain, reason);
        }

        // === TESTE DE USO DE ITEM ===
        public override void OnConsumeItem(Item item, Player player)
        {
            if (!ShouldGainExperience(item)) return;
            
            DebugLog.Item("OnConsumeItem", $"Item '{item.Name}' was used by player '{player.name}'");
        }

        // === MODIFICAR TOOLTIPS ===
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!ShouldGainExperience(item)) return;
            
            int level = GetItemLevel();
            
            // Linha de nível
            tooltips.Add(new TooltipLine(Mod, "ItemLevel", 
                $"Level: {level}") { OverrideColor = Color.Gold });
            
            // Linha de XP e progresso (só se não for nível máximo)
            if (level < MAX_LEVEL)
            {
                float currentExpInLevel = ExperienceInCurrentLevel(level);
                float neededExp = ExperienceNeededForNextLevel(level);
                float progressPercent = neededExp > 0 ? (currentExpInLevel / neededExp) * 100f : 0f;
                
                tooltips.Add(new TooltipLine(Mod, "ItemXP", 
                    $"XP: {currentExpInLevel:F0}/{neededExp:F0} ({progressPercent:F1}%)") { OverrideColor = Color.LightBlue });
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "ItemXP", 
                    "XP: MAX") { OverrideColor = Color.Purple });
            }
            
            // Linha de uso
            if (UsageCount > 0)
            {
                tooltips.Add(new TooltipLine(Mod, "ItemUsage", 
                    $"Used {UsageCount} times") { OverrideColor = Color.Gray });
            }
            
            // Bônus aplicados
            if (level > 1)
            {
                // Bônus de dano para armas
                if (item.damage > 0)
                {
                    float damageBonus = (level - 1) * DAMAGE_BONUS_PER_LEVEL;
                    damageBonus = Math.Min(damageBonus, MAX_DAMAGE_BONUS);
                    
                    if (damageBonus > 0)
                    {
                        tooltips.Add(new TooltipLine(Mod, "DamageBonus", 
                            $"Damage Bonus: +{damageBonus:P0}") { OverrideColor = Color.LightGreen });
                    }
                }
                
                // Bônus de defesa para armaduras
                if (item.defense > 0)
                {
                    float defenseBonus = (level - 1) * 0.5f; // +0.5 defesa por nível
                    tooltips.Add(new TooltipLine(Mod, "DefenseBonus", 
                        $"Defense Bonus: +{defenseBonus:F1}") { OverrideColor = Color.LightBlue });
                }
                
                if (level > 10) // Bônus de crit a partir do nível 10
                {
                    float critBonus = (level - 10) * 0.5f;
                    tooltips.Add(new TooltipLine(Mod, "CritBonus", 
                        $"Crit Bonus: +{critBonus:F1}%") { OverrideColor = Color.Yellow });
                }
            }
            
            // Dica de como ganhar XP
            if (level == 1)
            {
                tooltips.Add(new TooltipLine(Mod, "XPHint", 
                    "Use this item to gain XP!") { OverrideColor = Color.Cyan });
            }
        }

        // === SAVE/LOAD ===
        public override void SaveData(Item item, TagCompound tag)
        {
            if (Experience > 0 || UsageCount > 0 || HasRandomStats)
            {
                tag["experience"] = Experience;
                tag["usageCount"] = UsageCount;
                tag["hasRandomStats"] = HasRandomStats;
                
                DebugLog.Item("SaveData", $"Item '{item.Name}' saved - XP: {Experience:F1}, Uses: {UsageCount}, Initialized: {HasRandomStats}");
            }
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            try
            {
                Experience = tag.GetFloat("experience");
                UsageCount = tag.GetInt("usageCount");
                HasRandomStats = tag.GetBool("hasRandomStats");
                
                // Se o item não foi inicializado ainda, inicializar agora
                if (!HasRandomStats)
                {
                    InitializeItemIfNeeded(item);
                }
                
                DebugLog.Item("LoadData", $"Item '{item.Name}' loaded - XP: {Experience:F1}, Uses: {UsageCount}, Level: {GetItemLevel()}, Initialized: {HasRandomStats}");
            }
            catch (Exception ex)
            {
                DebugLog.Error("Item", "LoadData", $"Error loading item data for '{item.Name}'", ex);
                Experience = 0f;
                UsageCount = 0;
                HasRandomStats = false;
                InitializeItemIfNeeded(item);
            }
        }

        // === CLONE ===
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            var clone = (ProgressiveItem)base.Clone(item, itemClone);
            clone.Experience = Experience;
            clone.UsageCount = UsageCount;
            clone.HasRandomStats = HasRandomStats;
            
            DebugLog.Item("Clone", $"Item '{item.Name}' cloned - XP: {Experience:F1}, Uses: {UsageCount}, Initialized: {HasRandomStats}");
            
            return clone;
        }

        // === INICIALIZAÇÃO DE ITEM ===
        public override void PostReforge(Item item)
        {
            // Quando um item é reforjado, manter os stats RPG
            InitializeItemIfNeeded(item);
        }

        public override void OnCreated(Item item, ItemCreationContext context)
        {
            // Inicializar para todos os contextos de criação válidos
            DebugLog.Item("OnCreated", $"Item '{item.Name}' created via {context.GetType().Name}");
            InitializeItemIfNeeded(item);
        }

        public override void OnSpawn(Item item, IEntitySource source)
        {
            // Hook adicional para quando um item spawna (crafting, drop, etc)
            InitializeItemIfNeeded(item);
        }

        private void InitializeItemIfNeeded(Item item)
        {
            if (!ShouldGainExperience(item)) 
            {
                DebugLog.Item("InitializeItemIfNeeded", $"Item '{item.Name}' should not gain XP (damage: {item.damage}, consumable: {item.consumable})");
                return;
            }
            if (HasRandomStats) 
            {
                DebugLog.Item("InitializeItemIfNeeded", $"Item '{item.Name}' already initialized");
                return; // Já foi inicializado
            }
            
            // Inicializar com nível 1 e sem XP
            Experience = 0f;
            UsageCount = 0;
            HasRandomStats = true;
            
            // Dar um pouco de XP inicial baseado na raridade (opcional)
            float initialExp = GetRarityMultiplier(item.rare) * 10f;
            if (initialExp > 0)
            {
                Experience = initialExp;
                DebugLog.Item("InitializeItemIfNeeded", $"Item '{item.Name}' initialized with {initialExp:F1} initial XP (rarity: {item.rare})");
            }
            else
            {
                DebugLog.Item("InitializeItemIfNeeded", $"Item '{item.Name}' initialized at level 1");
            }
        }
    }
}
