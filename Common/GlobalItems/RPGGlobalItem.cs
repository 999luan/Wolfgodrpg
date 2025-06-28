using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes;
using System.Linq;

namespace Wolfgodrpg.Common.GlobalItems
{
    public class RPGGlobalItem : GlobalItem
    {
        public Dictionary<string, float> randomStats = new Dictionary<string, float>();

        public override bool InstancePerEntity => true;

        public override void OnCreate(Item item, ItemCreationContext context)
        {
            // Não adicionar status a itens vazios ou de quest
            if (item.IsAir || item.questItem) return;

            // Determina a raridade do item para saber quantos status adicionar
            RPGClassDefinitions.ItemRarity rarity = GetItemRarity(item);
            int numberOfStats = RPGClassDefinitions.StatsPerRarity[rarity];
            float statMultiplier = RPGClassDefinitions.StatMultiplierPerRarity[rarity];

            if (numberOfStats > 0)
            {
                // Pega uma lista de todos os status possíveis
                var possibleStats = RPGClassDefinitions.RandomStats.Keys.ToList();
                randomStats = new Dictionary<string, float>();

                for (int i = 0; i < numberOfStats; i++)
                {
                    // Escolhe um status aleatório que ainda não foi adicionado
                    string randomStatName = possibleStats[Main.rand.Next(possibleStats.Count)];
                    if (!randomStats.ContainsKey(randomStatName))
                    {
                        var statInfo = RPGClassDefinitions.RandomStats[randomStatName];
                        
                        // Calcula o valor do status com base na raridade
                        float value = (float)Main.rand.NextDouble() * (statInfo.MaxValue - statInfo.MinValue) + statInfo.MinValue;
                        value *= statMultiplier;

                        randomStats[randomStatName] = value;
                    }
                    else
                    {
                        // Tenta novamente se o status já foi adicionado
                        i--;
                    }
                }
            }
        }

        // Adiciona a descrição dos status na tooltip do item
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        { 
            if (randomStats != null && randomStats.Count > 0)
            {
                foreach (var stat in randomStats)
                {
                    var statInfo = RPGClassDefinitions.RandomStats[stat.Key];
                    string sign = stat.Value > 0 ? "+" : "";
                    string valueString = stat.Value < 1 ? $"{stat.Value:P1}" : $"{stat.Value:F0}";
                    
                    TooltipLine line = new TooltipLine(Mod, "RPGStats", $"{sign}{valueString} {statInfo.Name}");
                    line.OverrideColor = Color.LightGreen;
                    tooltips.Add(line);
                }
            }
        }

        private RPGClassDefinitions.ItemRarity GetItemRarity(Item item)
        {
            if (item.rare >= 9) return RPGClassDefinitions.ItemRarity.Legendary;
            if (item.rare >= 7) return RPGClassDefinitions.ItemRarity.Epic;
            if (item.rare >= 5) return RPGClassDefinitions.ItemRarity.Rare;
            if (item.rare >= 3) return RPGClassDefinitions.ItemRarity.Uncommon;
            return RPGClassDefinitions.ItemRarity.Common;
        }
    }
}
