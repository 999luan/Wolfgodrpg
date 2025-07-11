using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader; // Adicionado para DamageClass
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.GlobalItems;
using System.Linq;

namespace Wolfgodrpg.Common.Systems
{
    public static class RPGCalculations
    {
        // Este é o método central para calcular todos os status do jogador.
        public static Dictionary<string, float> CalculateTotalStats(RPGPlayer modPlayer)
        {
            var totalStats = new Dictionary<string, float>();
            var player = modPlayer.Player;

            // 1. Aplicar bônus dos atributos primários ⭐ NOVO
            ApplyPrimaryAttributes(modPlayer, totalStats);

            // 2. Adicione os bônus de todas as classes ativas
            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                string className = classEntry.Key;
                float level = modPlayer.ClassLevels.TryGetValue(className, out var lvl) ? lvl : 0f;

                if (level > 1)
                {
                    foreach (var statBonus in classEntry.Value.StatBonuses)
                    {
                        if (!totalStats.ContainsKey(statBonus.Key))
                        {
                            totalStats[statBonus.Key] = 0;
                        }
                        // O bônus é o valor base multiplicado pelo nível da classe
                        totalStats[statBonus.Key] += statBonus.Value * level;
                    }
                }
                
                // 2.1. Adicione os bônus das milestones desbloqueadas
                var milestoneBonuses = RPGClassMilestones.CalculateMilestoneBonuses(className, level);
                foreach (var bonus in milestoneBonuses)
                {
                    if (!totalStats.ContainsKey(bonus.Key))
                    {
                        totalStats[bonus.Key] = 0;
                    }
                    totalStats[bonus.Key] += bonus.Value;
                }
            }

            // 3. Adicione os bônus de proficiência de armadura ⭐ NOVO
            ArmorType currentArmorType = GetCurrentArmorType(player);
            if (currentArmorType != ArmorType.None)
            {
                int armorLevel = modPlayer.ArmorProficiencyLevels.TryGetValue(currentArmorType, out var level) ? level : 1;
                float armorBonus = (armorLevel - 1) * 0.02f; // +2% por nível de proficiência
                
                switch (currentArmorType)
                {
                    case ArmorType.Light:
                        totalStats["moveSpeed"] = (totalStats.TryGetValue("moveSpeed", out var speed) ? speed : 0f) + armorBonus;
                        break;
                    case ArmorType.Heavy:
                        totalStats["defense"] = (totalStats.TryGetValue("defense", out var def) ? def : 0f) + (armorLevel * 0.5f);
                        break;
                    case ArmorType.MagicRobes:
                        totalStats["manaRegen"] = (totalStats.TryGetValue("manaRegen", out var mana) ? mana : 0f) + (armorLevel * 0.5f);
                        totalStats["maxMana"] = (totalStats.TryGetValue("maxMana", out var maxMana) ? maxMana : 0f) + (armorLevel * 2f);
                        break;
                }
            }

            // 3. Adicione os status de todos os itens equipados (armaduras e acessórios)
            for (int i = 0; i < player.armor.Length; i++)
            {
                Item item = player.armor[i];
                if (item != null && !item.IsAir && item.TryGetGlobalItem(out RPGGlobalItem globalItem))
                {
                    if (globalItem.RandomStats != null)
                    {
                        foreach (var stat in globalItem.RandomStats)
                        {
                            if (!totalStats.ContainsKey(stat.Key))
                            {
                                totalStats[stat.Key] = 0;
                            }
                            totalStats[stat.Key] += stat.Value;
                        }
                    }
                }
            }
            
            // Adiciona o item segurado
            Item heldItem = player.HeldItem;
            if (heldItem != null && !heldItem.IsAir && heldItem.TryGetGlobalItem(out RPGGlobalItem heldGlobalItem))
            {
                if (heldGlobalItem.RandomStats != null)
                {
                    foreach (var stat in heldGlobalItem.RandomStats)
                    {
                        if (!totalStats.ContainsKey(stat.Key))
                        {
                            totalStats[stat.Key] = 0;
                        }
                        totalStats[stat.Key] += stat.Value;
                    }
                }
            }

            return totalStats;
        }

        // Aplica os status calculados ao jogador do Terraria
        public static void ApplyStatsToPlayer(Player player, Dictionary<string, float> stats)
        {
            // Resetar modificadores antes de aplicar os novos para evitar acúmulo
            player.GetDamage(DamageClass.Generic).Base = 1f;
            player.GetCritChance(DamageClass.Generic) = 0;
            player.moveSpeed = 1f;
            player.lifeRegen = 0;
            player.manaRegen = 0;

            // Aplicar cada stat
            foreach (var stat in stats)
            {
                switch (stat.Key)
                {
                    // Ofensivo
                    case "meleeDamage":
                        player.GetDamage(DamageClass.Melee).Base += stat.Value;
                        break;
                    case "rangedDamage":
                        player.GetDamage(DamageClass.Ranged).Base += stat.Value;
                        break;
                    case "magicDamage":
                        player.GetDamage(DamageClass.Magic).Base += stat.Value;
                        break;
                    case "minionDamage":
                        player.GetDamage(DamageClass.Summon).Base += stat.Value;
                        break;
                    case "critChance":
                    case "meleeCrit":
                    case "rangedCrit":
                    case "magicCrit":
                        player.GetCritChance(DamageClass.Generic) += (int)stat.Value;
                        break;
                    case "meleeSpeed":
                        player.GetAttackSpeed(DamageClass.Melee) += stat.Value;
                        break;

                    // Defensivo
                    case "defense":
                        player.statDefense += (int)stat.Value;
                        break;
                    case "maxLife":
                        player.statLifeMax2 += (int)stat.Value;
                        break;
                    case "lifeRegen":
                        player.lifeRegen += (int)stat.Value;
                        break;
                    case "damageReduction":
                        player.endurance += stat.Value;
                        break;

                    // Utilidade
                    case "moveSpeed":
                        player.moveSpeed += stat.Value;
                        break;
                    case "jumpHeight":
                         player.jumpSpeedBoost += stat.Value;
                        break;
                    case "maxMana":
                        player.statManaMax2 += (int)stat.Value;
                        break;
                    case "manaRegen":
                        player.manaRegen += (int)stat.Value;
                        break;
                    case "manaCost":
                        player.manaCost -= stat.Value;
                        break;
                    case "minionSlots":
                        player.maxMinions += (int)stat.Value;
                        break;
                    case "miningSpeed":
                        player.pickSpeed -= stat.Value; // Menor é mais rápido
                        break;
                    
                    // Stats das Milestones
                    case "summonDamage":
                        player.GetDamage(DamageClass.Summon).Base += stat.Value;
                        break;
                    case "summonSpeed":
                        // Velocidade de invocação (implementar no futuro)
                        break;
                    case "extraJumps":
                        // Pulos extras (implementar no futuro)
                        break;
                    case "extraDashes":
                        // Dash extras (já implementado no RPGPlayer)
                        break;
                    case "projectileSpeed":
                        // Velocidade de projéteis (implementar no futuro)
                        break;
                    case "reloadSpeed":
                        // Velocidade de recarga (implementar no futuro)
                        break;
                    case "castSpeed":
                        // Velocidade de conjuração (implementar no futuro)
                        break;
                    case "lightRadius":
                        // Raio de luz (implementar no futuro)
                        break;
                    case "rareLootChance":
                        // Chance de loot raro (implementar no futuro)
                        break;
                    case "trapResistance":
                        // Resistência a armadilhas (implementar no futuro)
                        break;
                    case "inventorySlots":
                        // Slots de inventário (implementar no futuro)
                        break;
                    case "trapDamage":
                        // Dano de armadilhas (implementar no futuro)
                        break;
                    case "turretSlots":
                        // Slots de torretas (implementar no futuro)
                        break;
                    case "repairSpeed":
                        // Velocidade de reparo (implementar no futuro)
                        break;
                    case "machineEfficiency":
                        // Eficiência de máquinas (implementar no futuro)
                        break;
                    case "buildSpeed":
                        // Velocidade de construção (implementar no futuro)
                        break;
                    case "damageResistance":
                        // Resistência a dano (implementar no futuro)
                        break;
                    case "extraLootChance":
                        // Chance de loot extra (implementar no futuro)
                        break;
                    case "rareResourceChance":
                        // Chance de recursos raros (implementar no futuro)
                        break;
                    case "hungerRate":
                        // Taxa de fome (implementar no futuro)
                        break;
                    case "debuffResistance":
                        // Resistência a debuffs (implementar no futuro)
                        break;
                    case "sanityRate":
                        // Taxa de sanidade (implementar no futuro)
                        break;
                    case "forgeQuality":
                        // Qualidade de forja (implementar no futuro)
                        break;
                    case "toolDamage":
                        // Dano de ferramentas (implementar no futuro)
                        break;
                    case "forgedWeaponDamage":
                        // Dano de armas forjadas (implementar no futuro)
                        break;
                    case "rareItemChance":
                        // Chance de itens raros (implementar no futuro)
                        break;
                    case "potionHealing":
                        // Cura de poções (implementar no futuro)
                        break;
                    case "buffDuration":
                        // Duração de buffs (implementar no futuro)
                        break;
                    case "buffSlots":
                        // Slots de buff (implementar no futuro)
                        break;
                    case "potionUseSpeed":
                        // Velocidade de uso de poções (implementar no futuro)
                        break;
                    case "minionHealth":
                        // Vida de minions (implementar no futuro)
                        break;
                    case "sentinelSlots":
                        // Slots de sentinelas (implementar no futuro)
                        break;
                    case "allyLifeRegen":
                        // Regeneração de aliados (implementar no futuro)
                        break;
                    
                    // Outros
                    case "luck":
                        player.luck += stat.Value;
                        break;
                    case "expGain":
                        // Este é um bônus, precisa ser pego pelo RPGPlayer
                        break;
                }
            }
        }

        /// <summary>
        /// Aplica os bônus dos atributos primários aos status do jogador.
        /// </summary>
        /// <param name="modPlayer">Jogador do mod</param>
        /// <param name="totalStats">Dicionário de status para acumular os bônus</param>
        private static void ApplyPrimaryAttributes(RPGPlayer modPlayer, Dictionary<string, float> totalStats)
        {
            // Força: Afeta dano corpo a corpo e capacidade de carga
            totalStats["meleeDamage"] = (totalStats.TryGetValue("meleeDamage", out var melee) ? melee : 0f) + (modPlayer.Strength - 10) * 0.05f;
            
            // Destreza: Afeta dano à distância, chance crítica e velocidade de ataque
            totalStats["rangedDamage"] = (totalStats.TryGetValue("rangedDamage", out var ranged) ? ranged : 0f) + (modPlayer.Dexterity - 10) * 0.04f;
            totalStats["critChance"] = (totalStats.TryGetValue("critChance", out var crit) ? crit : 0f) + (modPlayer.Dexterity - 10) * 0.5f;
            totalStats["meleeSpeed"] = (totalStats.TryGetValue("meleeSpeed", out var speed) ? speed : 0f) + (modPlayer.Dexterity - 10) * 0.02f;
            
            // Inteligência: Afeta dano mágico, mana máxima e velocidade de conjuração
            totalStats["magicDamage"] = (totalStats.TryGetValue("magicDamage", out var magic) ? magic : 0f) + (modPlayer.Intelligence - 10) * 0.06f;
            totalStats["maxMana"] = (totalStats.TryGetValue("maxMana", out var maxMana) ? maxMana : 0f) + (modPlayer.Intelligence - 10) * 2f;
            totalStats["manaRegen"] = (totalStats.TryGetValue("manaRegen", out var manaRegen) ? manaRegen : 0f) + (modPlayer.Intelligence - 10) * 0.1f;
            
            // Constituição: Afeta vida máxima, defesa e regeneração de vida
            totalStats["maxLife"] = (totalStats.TryGetValue("maxLife", out var maxLife) ? maxLife : 0f) + (modPlayer.Constitution - 10) * 5f;
            totalStats["defense"] = (totalStats.TryGetValue("defense", out var defense) ? defense : 0f) + (modPlayer.Constitution - 10) * 0.5f;
            totalStats["lifeRegen"] = (totalStats.TryGetValue("lifeRegen", out var lifeRegen) ? lifeRegen : 0f) + (modPlayer.Constitution - 10) * 0.2f;
            
            // Sabedoria: Afeta dano de invocação, sorte e resistência a debuffs
            totalStats["minionDamage"] = (totalStats.TryGetValue("minionDamage", out var minion) ? minion : 0f) + (modPlayer.Wisdom - 10) * 0.05f;
            totalStats["luck"] = (totalStats.TryGetValue("luck", out var luck) ? luck : 0f) + (modPlayer.Wisdom - 10) * 0.1f;
        }

        /// <summary>
        /// Determina o tipo de armadura equipada pelo jogador.
        /// </summary>
        /// <param name="player">Jogador</param>
        /// <returns>Tipo de armadura equipada</returns>
        private static ArmorType GetCurrentArmorType(Player player)
        {
            // Determinar tipo baseado na armadura equipada
            Item helmet = player.armor[0];
            Item chestplate = player.armor[1];
            Item leggings = player.armor[2];
            
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

        /// <summary>
        /// Verifica se a armadura equipada é do tipo mágico.
        /// </summary>
        /// <param name="helmet">Capacete</param>
        /// <param name="chest">Peitoral</param>
        /// <param name="legs">Calças</param>
        /// <returns>True se é armadura mágica</returns>
        private static bool IsMagicArmor(Item helmet, Item chest, Item legs)
        {
            // Verificar se é armadura mágica (Mana bonus, etc.)
            return helmet.manaIncrease > 0 || chest.manaIncrease > 0 || legs.manaIncrease > 0;
        }

        /// <summary>
        /// Verifica se a armadura equipada é do tipo pesado.
        /// </summary>
        /// <param name="helmet">Capacete</param>
        /// <param name="chest">Peitoral</param>
        /// <param name="legs">Calças</param>
        /// <returns>True se é armadura pesada</returns>
        private static bool IsHeavyArmor(Item helmet, Item chest, Item legs)
        {
            // Verificar se é armadura pesada (alta defesa)
            int totalDefense = helmet.defense + chest.defense + legs.defense;
            return totalDefense >= 20; // Threshold para armadura pesada
        }
    }
}