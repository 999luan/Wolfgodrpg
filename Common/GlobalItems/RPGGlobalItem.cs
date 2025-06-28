using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes;
using System.Linq;
using Wolfgodrpg.Common.Systems;
using Terraria.DataStructures; // Adicionado para IEntitySource
using Microsoft.Xna.Framework; // Adicionado para Color

namespace Wolfgodrpg.Common.GlobalItems
{
    public class RPGGlobalItem : GlobalItem
    {
        public Dictionary<string, float> randomStats = new Dictionary<string, float>();

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Item item, IEntitySource source)
        {
            // Não adicionar status a itens vazios, de quest ou já com status
            if (item.IsAir || item.questItem || randomStats.Any()) return;

            // Determina a raridade do item para saber quantos status adicionar
            RPGClassDefinitions.ItemRarity rarity = GetItemRarity(item);
            int numberOfStats = RPGClassDefinitions.StatsPerRarity[rarity];
            float statMultiplier = RPGClassDefinitions.StatMultiplierPerRarity[rarity];

            if (numberOfStats > 0)
            {
                var possibleStats = RPGClassDefinitions.RandomStats.Keys.ToList();
                randomStats = new Dictionary<string, float>();

                for (int i = 0; i < numberOfStats; i++)
                {
                    if (!possibleStats.Any()) break;
                    string randomStatName = possibleStats[Main.rand.Next(possibleStats.Count)];
                    possibleStats.Remove(randomStatName); // Evita status duplicados

                    var statInfo = RPGClassDefinitions.RandomStats[randomStatName];
                    float value = (float)Main.rand.NextDouble() * (statInfo.MaxValue - statInfo.MinValue) + statInfo.MinValue;
                    value *= statMultiplier;
                    randomStats[randomStatName] = value;
                }
            }
        }

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
            if (item.rare >= Terraria.ID.ItemRarityID.Cyan) return RPGClassDefinitions.ItemRarity.Legendary;
            if (item.rare >= Terraria.ID.ItemRarityID.Lime) return RPGClassDefinitions.ItemRarity.Epic;
            if (item.rare >= Terraria.ID.ItemRarityID.Pink) return RPGClassDefinitions.ItemRarity.Rare;
            if (item.rare >= Terraria.ID.ItemRarityID.Orange) return RPGClassDefinitions.ItemRarity.Uncommon;
            return RPGClassDefinitions.ItemRarity.Common;
        }

        
    }
}
