using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes; // Adicionado para RPGClassDefinitions
using Terraria.ModLoader.IO;

namespace Wolfgodrpg.Common.Data
{
    public class ItemAffix
    {
        public AffixType Type { get; set; }
        public string Attribute { get; set; }
        public float Value { get; set; }
        public bool IsPercentage { get; set; }

        public Dictionary<string, float> Stats
        {
            get
            {
                var stats = new Dictionary<string, float>();
                switch (Type)
                {
                    case AffixType.PrimaryAttribute:
                        stats[Attribute.ToLower()] = Value;
                        break;
                    case AffixType.ClassBonus:
                        stats[$"{Attribute.ToLower()}XPBonus"] = Value;
                        break;
                    case AffixType.WeaponProficiency:
                        stats[$"{Attribute.ToLower()}Damage"] = Value;
                        break;
                    case AffixType.Utility:
                        stats[Attribute.ToLower()] = Value;
                        break;
                }
                return stats;
            }
        }

        public TagCompound Save()
        {
            return new TagCompound {
                ["Type"] = (int)Type,
                ["Attribute"] = Attribute,
                ["Value"] = Value,
                ["IsPercentage"] = IsPercentage
            };
        }

        public static ItemAffix Load(TagCompound tag)
        {
            return new ItemAffix {
                Type = (AffixType)tag.GetInt("Type"),
                Attribute = tag.GetString("Attribute"),
                Value = tag.GetFloat("Value"),
                IsPercentage = tag.GetBool("IsPercentage")
            };
        }

        public static ItemAffix CreateRandom(Item item)
        {
            // Lógica de geração simplificada para teste
            // Em uma implementação real, seria mais complexo e baseado no item
            var affix = new ItemAffix();
            int randType = Main.rand.Next(4);
            switch (randType)
            {
                case 0:
                    affix.Type = AffixType.PrimaryAttribute;
                    affix.Attribute = "Strength";
                    affix.Value = Main.rand.Next(1, 6);
                    affix.IsPercentage = false;
                    break;
                case 1:
                    affix.Type = AffixType.ClassBonus;
                    affix.Attribute = "Guerreiro";
                    affix.Value = Main.rand.Next(5, 21);
                    affix.IsPercentage = true;
                    break;
                case 2:
                    affix.Type = AffixType.WeaponProficiency;
                    affix.Attribute = "Espadas";
                    affix.Value = Main.rand.Next(1, 11);
                    affix.IsPercentage = true;
                    break;
                case 3:
                    affix.Type = AffixType.Utility;
                    affix.Attribute = "MiningSpeed";
                    affix.Value = Main.rand.Next(1, 6);
                    affix.IsPercentage = true;
                    break;
            }
            return affix;
        }

        public string GetDisplayText()
        {
            string sign = Value > 0 ? "+" : "";
            string valueString = IsPercentage ? $"{Value:F0}%" : $"{Value:F0}";

            return Type switch
            {
                AffixType.PrimaryAttribute => $"{sign}{valueString} {Attribute}",
                AffixType.ClassBonus => $"{sign}{valueString} XP {Attribute}",
                AffixType.WeaponProficiency => $"{sign}{valueString} Dano {Attribute}",
                AffixType.Utility => $"{sign}{valueString} {Attribute}",
                _ => ""
            };
        }
    }

    public enum AffixType
    {
        PrimaryAttribute,    // +5 Força, Destreza, etc.
        ClassBonus,         // +10% XP Guerreiro, +2 Níveis Acrobata
        WeaponProficiency,  // +15% dano Espadas
        Utility            // +5% velocidade mineração
    }
}