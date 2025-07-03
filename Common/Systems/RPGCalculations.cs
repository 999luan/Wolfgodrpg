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

            // 1. Comece com os status base (se houver algum definido aqui)
            // Por enquanto, vamos aplicar os bônus diretamente no jogador.

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