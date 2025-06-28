using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Systems
{
    public static class RPGCalculations
    {
        // Constantes de XP
        private const float BASE_XP_NEEDED = 100f;
        private const float XP_MULTIPLIER = 1.1f;      // Cada nível requer 10% mais XP
        private const float ACTION_XP_BASE = 10f;      // XP base por ação
        private const float BOSS_XP_MULTIPLIER = 5f;   // Multiplicador de XP para bosses
        private const float ELITE_XP_MULTIPLIER = 2f;  // Multiplicador de XP para elites

        // Constantes de Status
        private const float BASE_HEALTH = 20f;
        private const float BASE_MANA = 10f;
        private const float BASE_DEFENSE = 0f;
        private const float BASE_DAMAGE = 0.5f;
        private const float BASE_SPEED = 0.8f;

        // XP ganho por ação
        public static Dictionary<string, float> GetActionXP(string action, RPGPlayer player, bool isBoss = false, bool isElite = false)
        {
            var xpGains = new Dictionary<string, float>();
            float baseXP = ACTION_XP_BASE;
            
            // Multiplicadores especiais
            if (isBoss) baseXP *= BOSS_XP_MULTIPLIER;
            if (isElite) baseXP *= ELITE_XP_MULTIPLIER;

            // Aplicar bônus de XP do jogador
            baseXP *= (1f + player.GetExpBonus());

            // Distribuir XP baseado na ação
            switch (action.ToLower())
            {
                case "walk":
                    xpGains["movement"] = baseXP * 0.5f;
                    break;
                case "jump":
                    xpGains["jumping"] = baseXP * 0.5f;
                    break;
                case "dash":
                    xpGains["movement"] = baseXP * 0.8f;
                    break;
                case "melee_hit":
                    xpGains["melee"] = baseXP * 1.0f;
                    break;
                case "ranged_hit":
                    xpGains["ranged"] = baseXP * 1.0f;
                    break;
                case "magic_hit":
                    xpGains["magic"] = baseXP * 1.0f;
                    break;
                case "summon_hit":
                    xpGains["summon"] = baseXP * 1.0f;
                    break;
                case "mine":
                    xpGains["mining"] = baseXP * 1.0f;
                    break;
                case "build":
                    xpGains["building"] = baseXP * 1.0f;
                    break;
                case "fish":
                    xpGains["fishing"] = baseXP * 1.0f;
                    break;
                case "gather":
                    xpGains["gathering"] = baseXP * 1.0f;
                    break;
                case "kill":
                    xpGains["bestiary"] = baseXP * 0.5f;
                    break;
                case "shop":
                    xpGains["merchant"] = baseXP * 1.0f;
                    break;
                case "take_damage":
                    xpGains["defense"] = baseXP * 0.5f;
                    break;
            }

            return xpGains;
        }

        // Gerar status aleatórios para item baseado na raridade
        public static Dictionary<string, float> GenerateRandomStats(RPGClassDefinitions.ItemRarity rarity)
        {
            var stats = new Dictionary<string, float>();
            var random = new Random();

            // Número de status baseado na raridade
            int numStats = RPGClassDefinitions.StatsPerRarity[rarity];
            float multiplier = RPGClassDefinitions.StatMultiplierPerRarity[rarity];

            // Lista de status possíveis
            var possibleStats = RPGClassDefinitions.RandomStats.Keys.ToList();

            // Selecionar status aleatórios
            for (int i = 0; i < numStats; i++)
            {
                if (possibleStats.Count == 0) break;

                // Escolher um status aleatório
                int index = random.Next(possibleStats.Count);
                string statKey = possibleStats[index];
                var statInfo = RPGClassDefinitions.RandomStats[statKey];

                // Gerar valor aleatório
                float value = statInfo.MinValue + (float)(random.NextDouble() * (statInfo.MaxValue - statInfo.MinValue));
                value *= multiplier;

                stats[statKey] = value;
                possibleStats.RemoveAt(index);
            }

            return stats;
        }

        // Calcular status total do jogador
        public static Dictionary<string, float> CalculateTotalStats(RPGPlayer player)
        {
            var stats = new Dictionary<string, float>
            {
                {"health", BASE_HEALTH},
                {"mana", BASE_MANA},
                {"defense", BASE_DEFENSE},
                {"damage", BASE_DAMAGE},
                {"speed", BASE_SPEED}
            };

            // Adicionar bônus das classes
            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                string className = classEntry.Key;
                var classInfo = classEntry.Value;
                float classLevel = player.GetClassLevel(className);

                foreach (var bonus in classInfo.StatBonuses)
                {
                    if (!stats.ContainsKey(bonus.Key))
                        stats[bonus.Key] = 0f;
                    
                    stats[bonus.Key] += bonus.Value * classLevel;
                }
            }

            // Adicionar bônus dos itens equipados
            foreach (var itemStats in player.GetEquippedItemStats())
            {
                foreach (var stat in itemStats)
                {
                    if (!stats.ContainsKey(stat.Key))
                        stats[stat.Key] = 0f;
                    
                    stats[stat.Key] += stat.Value;
                }
            }

            // Aplicar modificadores de status secundários
            ApplySecondaryStats(stats);

            return stats;
        }

        // Aplicar efeitos de status secundários
        private static void ApplySecondaryStats(Dictionary<string, float> stats)
        {
            // Movimento
            if (stats.ContainsKey("moveSpeed"))
                stats["speed"] *= (1f + stats["moveSpeed"]);

            // Pulo
            if (stats.ContainsKey("jumpHeight"))
                stats["jumpBoost"] = stats["jumpHeight"];

            // Dano
            if (stats.ContainsKey("meleeDamage"))
                stats["damage"] *= (1f + stats["meleeDamage"]);
            if (stats.ContainsKey("rangedDamage"))
                stats["damage"] *= (1f + stats["rangedDamage"]);
            if (stats.ContainsKey("magicDamage"))
                stats["damage"] *= (1f + stats["magicDamage"]);
            if (stats.ContainsKey("minionDamage"))
                stats["damage"] *= (1f + stats["minionDamage"]);

            // Defesa
            if (stats.ContainsKey("damageReduction"))
                stats["defense"] *= (1f + stats["damageReduction"]);

            // Vida e Mana
            if (stats.ContainsKey("maxLife"))
                stats["health"] *= (1f + stats["maxLife"]);
            if (stats.ContainsKey("maxMana"))
                stats["mana"] *= (1f + stats["maxMana"]);
        }

        // Verificar desbloqueio de habilidades
        public static List<string> CheckUnlockedAbilities(RPGPlayer player)
        {
            var unlockedAbilities = new List<string>();

            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                string className = classEntry.Key;
                var classInfo = classEntry.Value;
                float classLevel = player.GetClassLevel(className);

                foreach (var milestone in classInfo.Milestones)
                {
                    if (classLevel >= milestone.Key && !player.HasUnlockedAbility($"{className}_{milestone.Key}"))
                    {
                        unlockedAbilities.Add($"{className}_{milestone.Key}");
                    }
                }
            }

            return unlockedAbilities;
        }

        // Calcular dano final
        public static float CalculateFinalDamage(float baseDamage, Dictionary<string, float> stats, string damageType)
        {
            float damage = baseDamage;
            float critChance = stats.ContainsKey("critChance") ? stats["critChance"] : 0f;
            float critMultiplier = 2f;

            switch (damageType.ToLower())
            {
                case "melee":
                    damage *= (1f + (stats.ContainsKey("meleeDamage") ? stats["meleeDamage"] : 0f));
                    break;
                case "ranged":
                    damage *= (1f + (stats.ContainsKey("rangedDamage") ? stats["rangedDamage"] : 0f));
                    break;
                case "magic":
                    damage *= (1f + (stats.ContainsKey("magicDamage") ? stats["magicDamage"] : 0f));
                    break;
                case "summon":
                    damage *= (1f + (stats.ContainsKey("minionDamage") ? stats["minionDamage"] : 0f));
                    break;
            }

            // Aplicar crítico
            if (Main.rand.NextFloat() < critChance)
            {
                damage *= critMultiplier;
            }

            return damage;
        }

        // Calcular defesa final
        public static float CalculateFinalDefense(float baseDefense, Dictionary<string, float> stats)
        {
            float defense = baseDefense;
            
            if (stats.ContainsKey("defense"))
                defense += stats["defense"];

            if (stats.ContainsKey("damageReduction"))
                defense *= (1f + stats["damageReduction"]);

            return defense;
        }

        // Calcular chance de drop baseado na sorte
        public static float CalculateDropChance(float baseChance, Dictionary<string, float> stats)
        {
            float chance = baseChance;
            
            if (stats.ContainsKey("luck"))
                chance *= (1f + stats["luck"]);

            if (stats.ContainsKey("rareDrop"))
                chance *= (1f + stats["rareDrop"]);

            return chance;
        }
    }
} 