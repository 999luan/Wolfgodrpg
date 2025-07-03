using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes;
using System.Linq;
using Wolfgodrpg.Common.Systems;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Wolfgodrpg.Common.Data;

namespace Wolfgodrpg.Common.GlobalItems
{
    public class RPGGlobalItem : GlobalItem
    {
        public Dictionary<string, float> randomStats = new Dictionary<string, float>();
        public List<ItemAffix> Affixes = new List<ItemAffix>();

        public override bool InstancePerEntity => true;

        public override GlobalItem Clone(Item item, Item newItem)
        {
            var clone = (RPGGlobalItem)base.Clone(item, newItem);
            clone.randomStats = new Dictionary<string, float>(randomStats);
            clone.Affixes = new List<ItemAffix>(Affixes);
            return clone;
        }

        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (CanHaveAffixes(item))
            {
                GenerateAffixes(item);
            }
        }

        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (CanHaveAffixes(item))
            {
                GenerateAffixes(item);
            }
        }

        private bool CanHaveAffixes(Item item)
        {
            // Itens que podem ter afixos: armas e armaduras
            bool isTool = item.pick > 0 || item.axe > 0 || item.hammer > 0;
            bool isAccessory = item.accessory;
            return (item.damage > 0 && !item.consumable && !isTool) || (item.defense > 0 && !isAccessory);
        }

        private void GenerateAffixes(Item item)
        {
            // Lógica de geração de afixos (simplificada por enquanto)
            // Conforme gemini.md seção 5
            if (Main.rand.NextFloat() < 0.15f) // 15% de chance de ter afixos
            {
                Affixes.Add(ItemAffix.CreateRandom(item));
                
                // Adicionar stats aleatórios baseados nos afixos
                foreach (var affix in Affixes)
                {
                    foreach (var stat in affix.Stats)
                    {
                        if (!randomStats.ContainsKey(stat.Key))
                            randomStats[stat.Key] = 0;
                        randomStats[stat.Key] += stat.Value;
                    }
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (Affixes != null && Affixes.Any())
            {
                foreach (var affix in Affixes)
                {
                    TooltipLine line = new TooltipLine(Mod, "RPGAffix", affix.GetDisplayText());
                    // Definir cor baseada no tipo de afixo
                    switch (affix.Type)
                    {
                        case AffixType.PrimaryAttribute:
                            line.OverrideColor = Color.Blue;
                            break;
                        case AffixType.ClassBonus:
                            line.OverrideColor = Color.Green;
                            break;
                        case AffixType.WeaponProficiency:
                            line.OverrideColor = Color.Purple;
                            break;
                        case AffixType.Utility:
                            line.OverrideColor = Color.Yellow;
                            break;
                    }
                    tooltips.Add(line);
                }
            }
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            if (randomStats.Any())
                tag["RandomStats"] = randomStats.ToDictionary(kv => kv.Key, kv => kv.Value);
            
            if (Affixes.Any())
                tag["Affixes"] = Affixes.Select(a => a.Save()).ToList();
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            if (tag.ContainsKey("RandomStats"))
                randomStats = tag.Get<Dictionary<string, float>>("RandomStats");
            
            if (tag.ContainsKey("Affixes"))
                Affixes = tag.GetList<TagCompound>("Affixes").Select(t => ItemAffix.Load(t)).ToList();
        }
    }
}
