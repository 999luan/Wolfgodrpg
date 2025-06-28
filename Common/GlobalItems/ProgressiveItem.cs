using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Wolfgodrpg.Common.GlobalItems
{
    public class ProgressiveItem : GlobalItem
    {
        // === PROPRIEDADES ===
        public float Experience { get; set; } = 0f;
        public int UsageCount { get; set; } = 0;
        
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
            
            int level = (int)Math.Floor(Math.Sqrt(Experience / 50.0f)) + 1;
            return Math.Min(level, MAX_LEVEL);
        }

        // === EXPERIÊNCIA PARA PRÓXIMO NÍVEL ===
        private float ExperienceForLevel(int level)
        {
            return (level - 1) * (level - 1) * 50f;
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
            Experience += amount * multiplier;
            UsageCount++;
            
            int newLevel = GetItemLevel();
            
            if (newLevel > oldLevel && Main.myPlayer >= 0 && Main.myPlayer < Main.player.Length)
            {
                Player player = Main.player[Main.myPlayer];
                if (player.active)
                {
                    Main.NewText($"{item.Name} subiu para nível {newLevel}!", Color.Gold);
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
            return item.damage > 0 || item.healLife > 0 || item.healMana > 0 || 
                   item.pick > 0 || item.axe > 0 || item.hammer > 0;
        }

        // === MODIFICAR DANO ===
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (!ShouldGainExperience(item)) return;
            
            int level = GetItemLevel();
            if (level <= 1) return;
            
            float damageBonus = (level - 1) * DAMAGE_BONUS_PER_LEVEL;
            damageBonus = Math.Min(damageBonus, MAX_DAMAGE_BONUS);
            
            damage += damageBonus;
        }

        // === MODIFICAR KNOCKBACK ===
        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            if (!ShouldGainExperience(item)) return;
            
            int level = GetItemLevel();
            if (level <= 1) return;
            
            float knockbackBonus = (level - 1) * 0.005f;
            knockbackBonus = Math.Min(knockbackBonus, 0.25f);
            
            knockback += knockbackBonus;
        }

        // === USO DO ITEM ===
        public override bool? UseItem(Item item, Player player)
        {
            if (ShouldGainExperience(item))
            {
                float baseExp = 1.0f;
                
                if (item.damage > 0)
                {
                    baseExp += item.damage * 0.1f;
                }
                
                GainExperience(item, baseExp, "usage");
            }
            
            return null;
        }

        // === HIT NPC ===
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!ShouldGainExperience(item)) return;
            
            float expGain = damageDone * 0.2f;
            
            string reason = "usage";
            if (hit.Crit) reason = "crit";
            if (target.boss) reason = "boss";
            
            var balancedNPC = target.GetGlobalNPC<GlobalNPCs.BalancedNPC>();
            if (balancedNPC.IsElite) reason = "elite";
            
            GainExperience(item, expGain, reason);
        }

        // === MODIFICAR TOOLTIPS ===
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!ShouldGainExperience(item)) return;
            
            int level = GetItemLevel();
            
            tooltips.Add(new TooltipLine(Mod, "ItemLevel", 
                $"Nível: {level}") { OverrideColor = Color.Gold });
            
            if (level > 1)
            {
                float damageBonus = (level - 1) * DAMAGE_BONUS_PER_LEVEL;
                damageBonus = Math.Min(damageBonus, MAX_DAMAGE_BONUS);
                
                if (damageBonus > 0)
                {
                    tooltips.Add(new TooltipLine(Mod, "DamageBonus", 
                        $"+{damageBonus:P0} dano") { OverrideColor = Color.LightBlue });
                }
                
                float speedBonus = (level - 1) * ATTACK_SPEED_BONUS_PER_LEVEL;
                speedBonus = Math.Min(speedBonus, MAX_ATTACK_SPEED_BONUS);
                
                if (speedBonus > 0)
                {
                    tooltips.Add(new TooltipLine(Mod, "SpeedBonus", 
                        $"+{speedBonus:P0} velocidade de ataque") { OverrideColor = Color.LightGreen });
                }
            }
            
            if (level < MAX_LEVEL)
            {
                float currentExp = Experience;
                float expForNext = ExperienceForLevel(level + 1);
                float expForCurrent = ExperienceForLevel(level);
                float progress = (currentExp - expForCurrent) / (expForNext - expForCurrent);
                
                tooltips.Add(new TooltipLine(Mod, "ExpProgress", 
                    $"Progresso: {progress:P1} para nível {level + 1}") { OverrideColor = Color.Yellow });
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "MaxLevel", 
                    "NÍVEL MÁXIMO") { OverrideColor = Color.Red });
            }
            
            tooltips.Add(new TooltipLine(Mod, "UsageStats", 
                $"Usado {UsageCount} vezes | {Experience:F0} EXP") { OverrideColor = Color.Gray });
        }

        // === SAVE/LOAD ===
        public override void SaveData(Item item, TagCompound tag)
        {
            if (Experience > 0 || UsageCount > 0)
            {
                tag["experience"] = Experience;
                tag["usageCount"] = UsageCount;
            }
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            Experience = tag.GetFloat("experience");
            UsageCount = tag.GetInt("usageCount");
        }

        // === CLONE ===
        public override GlobalItem Clone(Item item, Item itemClone)
        {
            var clone = (ProgressiveItem)base.Clone(item, itemClone);
            clone.Experience = Experience;
            clone.UsageCount = UsageCount;
            return clone;
        }
    }
} 