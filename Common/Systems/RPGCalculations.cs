using System.Collections.Generic;
using Terraria;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.GlobalItems;

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
                float level = modPlayer.GetClassLevel(className);

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

            // 3. Adicione os status de todos os itens equipados (armaduras e acessórios)
            for (int i = 0; i < player.armor.Length; i++)
            {
                Item item = player.armor[i];
                if (item != null && !item.IsAir && item.TryGetGlobalItem(out RPGGlobalItem globalItem))
                {
                    if (globalItem.randomStats != null)
                    {
                        foreach (var stat in globalItem.randomStats)
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
                if (heldGlobalItem.randomStats != null)
                {
                    foreach (var stat in heldGlobalItem.randomStats)
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
            player.GetDamage(DamageClass.Generic) = 1f;
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
                        player.GetDamage(DamageClass.Melee) += stat.Value;
                        break;
                    case "rangedDamage":
                        player.GetDamage(DamageClass.Ranged) += stat.Value;
                        break;
                    case "magicDamage":
                        player.GetDamage(DamageClass.Magic) += stat.Value;
                        break;
                    case "minionDamage":
                        player.GetDamage(DamageClass.Summon) += stat.Value;
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
    }
}