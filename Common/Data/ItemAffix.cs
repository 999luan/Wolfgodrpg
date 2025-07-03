using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes; // Adicionado para RPGClassDefinitions
using Terraria.ModLoader.IO;
using System;

namespace Wolfgodrpg.Common.Data
{
    /// <summary>
    /// Representa um afixo que pode ser aplicado a itens para fornecer bônus aleatórios.
    /// </summary>
    public class ItemAffix
    {
        /// <summary>
        /// Nome do afixo para exibição.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Descrição do afixo.
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Tipo de estatística que o afixo afeta.
        /// </summary>
        public string StatType { get; set; }
        
        /// <summary>
        /// Valor do bônus fornecido pelo afixo.
        /// </summary>
        public float Value { get; set; }
        
        /// <summary>
        /// Raridade do afixo.
        /// </summary>
        public ItemRarity Rarity { get; set; }
        
        /// <summary>
        /// Se o afixo é aplicável a armas.
        /// </summary>
        public bool AppliesToWeapons { get; set; }
        
        /// <summary>
        /// Se o afixo é aplicável a armaduras.
        /// </summary>
        public bool AppliesToArmor { get; set; }
        
        /// <summary>
        /// Se o afixo é aplicável a acessórios.
        /// </summary>
        public bool AppliesToAccessories { get; set; }

        /// <summary>
        /// Construtor padrão para ItemAffix.
        /// </summary>
        public ItemAffix()
        {
            Name = "";
            Description = "";
            StatType = "";
            Value = 0f;
            Rarity = ItemRarity.Common;
            AppliesToWeapons = false;
            AppliesToArmor = false;
            AppliesToAccessories = false;
        }

        /// <summary>
        /// Construtor com parâmetros para ItemAffix.
        /// </summary>
        /// <param name="name">Nome do afixo</param>
        /// <param name="description">Descrição do afixo</param>
        /// <param name="statType">Tipo de estatística</param>
        /// <param name="value">Valor do bônus</param>
        /// <param name="rarity">Raridade do afixo</param>
        /// <param name="appliesToWeapons">Se aplica a armas</param>
        /// <param name="appliesToArmor">Se aplica a armaduras</param>
        /// <param name="appliesToAccessories">Se aplica a acessórios</param>
        public ItemAffix(string name, string description, string statType, float value, ItemRarity rarity, 
                        bool appliesToWeapons = false, bool appliesToArmor = false, bool appliesToAccessories = false)
        {
            Name = name;
            Description = description;
            StatType = statType;
            Value = value;
            Rarity = rarity;
            AppliesToWeapons = appliesToWeapons;
            AppliesToArmor = appliesToArmor;
            AppliesToAccessories = appliesToAccessories;
        }

        /// <summary>
        /// Salva os dados do afixo usando TagCompound.
        /// </summary>
        /// <param name="tag">TagCompound para salvar os dados</param>
        public void Save(TagCompound tag)
        {
            tag["Name"] = Name;
            tag["Description"] = Description;
            tag["StatType"] = StatType;
            tag["Value"] = Value;
            tag["Rarity"] = (int)Rarity;
            tag["AppliesToWeapons"] = AppliesToWeapons;
            tag["AppliesToArmor"] = AppliesToArmor;
            tag["AppliesToAccessories"] = AppliesToAccessories;
        }

        /// <summary>
        /// Carrega os dados do afixo usando TagCompound.
        /// </summary>
        /// <param name="tag">TagCompound contendo os dados salvos</param>
        public void Load(TagCompound tag)
        {
            if (tag.ContainsKey("Name"))
                Name = tag.GetString("Name");
            
            if (tag.ContainsKey("Description"))
                Description = tag.GetString("Description");
            
            if (tag.ContainsKey("StatType"))
                StatType = tag.GetString("StatType");
            
            if (tag.ContainsKey("Value"))
                Value = tag.GetFloat("Value");
            
            if (tag.ContainsKey("Rarity"))
                Rarity = (ItemRarity)tag.GetInt("Rarity");
            
            if (tag.ContainsKey("AppliesToWeapons"))
                AppliesToWeapons = tag.GetBool("AppliesToWeapons");
            
            if (tag.ContainsKey("AppliesToArmor"))
                AppliesToArmor = tag.GetBool("AppliesToArmor");
            
            if (tag.ContainsKey("AppliesToAccessories"))
                AppliesToAccessories = tag.GetBool("AppliesToAccessories");
        }

        /// <summary>
        /// Retorna uma representação em string do afixo para tooltips.
        /// </summary>
        /// <returns>String formatada do afixo</returns>
        public override string ToString()
        {
            string prefix = Rarity switch
            {
                ItemRarity.Common => "",
                ItemRarity.Uncommon => "[c/00FF00:",
                ItemRarity.Rare => "[c/0080FF:",
                ItemRarity.Epic => "[c/8000FF:",
                ItemRarity.Legendary => "[c/FF8000:",
                _ => ""
            };
            
            string suffix = Rarity == ItemRarity.Common ? "" : "]";
            
            return $"{prefix}{Name}: +{Value:F1} {StatType}{suffix}";
        }

        /// <summary>
        /// Verifica se o afixo pode ser aplicado ao tipo de item especificado.
        /// </summary>
        /// <param name="itemType">Tipo do item (weapon, armor, accessory)</param>
        /// <returns>True se o afixo pode ser aplicado</returns>
        public bool CanApplyTo(string itemType)
        {
            return itemType.ToLower() switch
            {
                "weapon" => AppliesToWeapons,
                "armor" => AppliesToArmor,
                "accessory" => AppliesToAccessories,
                _ => false
            };
        }
    }
}