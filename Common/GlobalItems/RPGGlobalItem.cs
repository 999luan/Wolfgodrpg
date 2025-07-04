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
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.GlobalItems
{
    /// <summary>
    /// Classe global que gerencia afixos e estatísticas aleatórias em itens.
    /// Herda de GlobalItem para integrar com o sistema de itens do tModLoader.
    /// </summary>
    public class RPGGlobalItem : GlobalItem
    {
        /// <summary>
        /// Estatísticas aleatórias aplicadas ao item.
        /// </summary>
        public Dictionary<string, float> RandomStats = new Dictionary<string, float>();
        
        /// <summary>
        /// Lista de afixos aplicados ao item.
        /// </summary>
        public List<ItemAffix> Affixes = new List<ItemAffix>();

        /// <summary>
        /// Indica que cada instância do item terá seus próprios dados.
        /// </summary>
        public override bool InstancePerEntity => true;

        /// <summary>
        /// Clona os dados do item para uma nova instância.
        /// </summary>
        /// <param name="item">Item original</param>
        /// <param name="newItem">Nova instância do item</param>
        /// <returns>Clone do GlobalItem</returns>
        public override GlobalItem Clone(Item item, Item newItem)
        {
            var clone = (RPGGlobalItem)base.Clone(item, newItem);
            clone.RandomStats = new Dictionary<string, float>(RandomStats);
            clone.Affixes = new List<ItemAffix>(Affixes);
            return clone;
        }

        /// <summary>
        /// Chamado quando o item é criado (craft, etc.).
        /// </summary>
        /// <param name="item">Item criado</param>
        /// <param name="context">Contexto de criação</param>
        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (CanHaveAffixes(item))
            {
                GenerateAffixes(item);
            }
        }

        /// <summary>
        /// Chamado quando o item é gerado no mundo (drop, etc.).
        /// </summary>
        /// <param name="item">Item gerado</param>
        /// <param name="source">Fonte da geração</param>
        public override void OnSpawn(Item item, IEntitySource source)
        {
            if (CanHaveAffixes(item))
            {
                GenerateAffixes(item);
            }
        }

        /// <summary>
        /// Verifica se o item pode ter afixos aplicados.
        /// </summary>
        /// <param name="item">Item a verificar</param>
        /// <returns>True se o item pode ter afixos</returns>
        private bool CanHaveAffixes(Item item)
        {
            // Itens que podem ter afixos: armas e armaduras
            bool isTool = item.pick > 0 || item.axe > 0 || item.hammer > 0;
            bool isAccessory = item.accessory;
            return (item.damage > 0 && !item.consumable && !isTool) || (item.defense > 0 && !isAccessory);
        }

        /// <summary>
        /// Gera afixos aleatórios para o item.
        /// </summary>
        /// <param name="item">Item para gerar afixos</param>
        private void GenerateAffixes(Item item)
        {
            // Limpar afixos existentes
            Affixes.Clear();
            RandomStats.Clear();
            
            // 15% de chance de ter afixos
            if (Main.rand.NextFloat() < 0.15f)
            {
                // Gerar 1-3 afixos
                int affixCount = Main.rand.Next(1, 4);
                
                for (int i = 0; i < affixCount; i++)
                {
                    var affix = GenerateRandomAffix(item);
                    if (affix != null)
                    {
                        Affixes.Add(affix);
                        
                        // Aplicar estatísticas do afixo
                        if (!RandomStats.ContainsKey(affix.StatType))
                            RandomStats[affix.StatType] = 0f;
                        RandomStats[affix.StatType] += affix.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Gera um afixo aleatório baseado no tipo de item.
        /// </summary>
        /// <param name="item">Item para gerar o afixo</param>
        /// <returns>ItemAffix gerado ou null</returns>
        private ItemAffix GenerateRandomAffix(Item item)
        {
            // Determinar tipo de item
            string itemType = item.damage > 0 ? "weapon" : "armor";
            
            // Selecionar afixo aleatório da lista de afixos disponíveis
            var availableAffixes = RPGClassDefinitions.RandomStats
                .Where(kvp => CanApplyAffixToItem(kvp.Key, itemType))
                .ToList();
            
            if (!availableAffixes.Any())
                return null;
            
            var selectedStat = availableAffixes[Main.rand.Next(availableAffixes.Count)];
            var statInfo = selectedStat.Value;
            
            // Gerar valor aleatório
            float value = Main.rand.NextFloat(statInfo.MinValue, statInfo.MaxValue);
            
            // Determinar raridade baseada no valor
            ItemRarity rarity = DetermineRarity(value, statInfo.MaxValue);
            
            return new ItemAffix(
                selectedStat.Key,
                $"Aumenta {selectedStat.Key}",
                selectedStat.Key,
                value,
                rarity,
                itemType == "weapon",
                itemType == "armor",
                false
            );
        }

        /// <summary>
        /// Verifica se um afixo pode ser aplicado ao tipo de item.
        /// </summary>
        /// <param name="statName">Nome da estatística</param>
        /// <param name="itemType">Tipo do item</param>
        /// <returns>True se pode ser aplicado</returns>
        private bool CanApplyAffixToItem(string statName, string itemType)
        {
            // Lógica simplificada - pode ser expandida
            return itemType switch
            {
                "weapon" => statName.Contains("damage") || statName.Contains("speed") || statName.Contains("crit"),
                "armor" => statName.Contains("defense") || statName.Contains("health") || statName.Contains("regen"),
                _ => false
            };
        }

        /// <summary>
        /// Determina a raridade do afixo baseada no valor.
        /// </summary>
        /// <param name="value">Valor do afixo</param>
        /// <param name="maxValue">Valor máximo possível</param>
        /// <returns>Raridade determinada</returns>
        private ItemRarity DetermineRarity(float value, float maxValue)
        {
            float percentage = value / maxValue;
            
            return percentage switch
            {
                >= 0.9f => ItemRarity.Legendary,
                >= 0.7f => ItemRarity.Epic,
                >= 0.5f => ItemRarity.Rare,
                >= 0.3f => ItemRarity.Uncommon,
                _ => ItemRarity.Common
            };
        }

        /// <summary>
        /// Modifica os tooltips do item para mostrar afixos.
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="tooltips">Lista de tooltips</param>
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (Affixes != null && Affixes.Any())
            {
                // Adicionar linha separadora
                tooltips.Add(new TooltipLine(Mod, "RPGAffixSeparator", ""));
                
                foreach (var affix in Affixes)
                {
                    TooltipLine line = new TooltipLine(Mod, "RPGAffix", affix.ToString());
                    
                    // Definir cor baseada na raridade
                    line.OverrideColor = affix.Rarity switch
                    {
                        ItemRarity.Common => Color.White,
                        ItemRarity.Uncommon => Color.Lime,
                        ItemRarity.Rare => Color.Cyan,
                        ItemRarity.Epic => Color.Magenta,
                        ItemRarity.Legendary => Color.Orange,
                        _ => Color.White
                    };
                    
                    tooltips.Add(line);
                }
            }

            // Apenas para o jogador local
            if (Main.LocalPlayer.GetModPlayer<RPGPlayer>() is not RPGPlayer modPlayer)
                return;

            // Determinar tipo de arma/armadura
            WeaponType weaponType = GetWeaponType(item);
            ArmorType armorType = GetArmorType(item);

            // Adicionar tooltip de proficiência de arma
            if (weaponType != WeaponType.None)
            {
                int level = modPlayer.WeaponProficiencyLevels.GetValueOrDefault(weaponType, 0);
                float xp = modPlayer.WeaponProficiencyExperience.GetValueOrDefault(weaponType, 0f);
                float xpNeeded = GetWeaponXPNeeded(level);
                
                string weaponTypeName = GetWeaponTypeDisplayName(weaponType);
                Color color = level > 0 ? Color.Gold : Color.Gray;
                
                tooltips.Add(new TooltipLine(Mod, "WeaponProficiency", 
                    $"[c/{color.Hex3()}:Proficiência {weaponTypeName}: Nível {level}]"));
                
                if (level > 0)
                {
                    float progress = (xp / xpNeeded) * 100f;
                    tooltips.Add(new TooltipLine(Mod, "WeaponProgress", 
                        $"[c/7F7F7F:Progresso: {progress:F1}% ({xp:F0}/{xpNeeded:F0} XP)]"));
                }
            }

            // Adicionar tooltip de proficiência de armadura
            if (armorType != ArmorType.None)
            {
                int level = modPlayer.ArmorProficiencyLevels.GetValueOrDefault(armorType, 0);
                float xp = modPlayer.ArmorProficiencyExperience.GetValueOrDefault(armorType, 0f);
                float xpNeeded = GetArmorXPNeeded(level);
                
                string armorTypeName = GetArmorTypeDisplayName(armorType);
                Color color = level > 0 ? Color.Gold : Color.Gray;
                
                tooltips.Add(new TooltipLine(Mod, "ArmorProficiency", 
                    $"[c/{color.Hex3()}:Proficiência {armorTypeName}: Nível {level}]"));
                
                if (level > 0)
                {
                    float progress = (xp / xpNeeded) * 100f;
                    tooltips.Add(new TooltipLine(Mod, "ArmorProgress", 
                        $"[c/7F7F7F:Progresso: {progress:F1}% ({xp:F0}/{xpNeeded:F0} XP)]"));
                }
            }
        }

        private WeaponType GetWeaponType(Item item)
        {
            // Verificar por DamageType primeiro
            if (item.DamageType == DamageClass.Melee)
                return WeaponType.Melee;
            if (item.DamageType == DamageClass.Ranged)
                return WeaponType.Ranged;
            if (item.DamageType == DamageClass.Magic)
                return WeaponType.Magic;
            if (item.DamageType == DamageClass.Summon)
                return WeaponType.Summon;
            
            // Fallback: verificar por nome do item
            string itemName = item.Name.ToLower();
            
            // Melee weapons
            if (itemName.Contains("sword") || itemName.Contains("axe") || itemName.Contains("hammer") || 
                itemName.Contains("spear") || itemName.Contains("lance") || itemName.Contains("dagger") ||
                itemName.Contains("knife") || itemName.Contains("mace") || itemName.Contains("flail"))
                return WeaponType.Melee;
            
            // Ranged weapons
            if (itemName.Contains("bow") || itemName.Contains("gun") || itemName.Contains("rifle") ||
                itemName.Contains("pistol") || itemName.Contains("revolver") || itemName.Contains("crossbow") ||
                itemName.Contains("blowgun") || itemName.Contains("dart") || itemName.Contains("arrow"))
                return WeaponType.Ranged;
            
            // Magic weapons
            if (itemName.Contains("staff") || itemName.Contains("wand") || itemName.Contains("book") ||
                itemName.Contains("spell") || itemName.Contains("magic") || itemName.Contains("crystal") ||
                itemName.Contains("orb") || itemName.Contains("tome"))
                return WeaponType.Magic;
            
            // Summon weapons
            if (itemName.Contains("whip") || itemName.Contains("summon") || itemName.Contains("staff") ||
                itemName.Contains("rod") || itemName.Contains("crystal") || itemName.Contains("minion"))
                return WeaponType.Summon;
            
            return WeaponType.None;
        }

        private ArmorType GetArmorType(Item item)
        {
            // Verificar se é armadura mágica
            if (item.manaIncrease > 0 || 
                item.Name.ToLower().Contains("robe") || 
                item.Name.ToLower().Contains("wizard") || 
                item.Name.ToLower().Contains("mage"))
                return ArmorType.MagicRobes;
            
            // Verificar se é armadura pesada
            if (item.defense >= 8 || 
                item.Name.ToLower().Contains("plate") || 
                item.Name.ToLower().Contains("heavy") ||
                item.Name.ToLower().Contains("titanium") ||
                item.Name.ToLower().Contains("adamantite"))
                return ArmorType.Heavy;
            
            // Verificar se é armadura leve
            if (item.defense > 0)
                return ArmorType.Light;
            
            return ArmorType.None;
        }

        private string GetWeaponTypeDisplayName(WeaponType type)
        {
            return type switch
            {
                WeaponType.Melee => "Corpo a Corpo",
                WeaponType.Ranged => "À Distância",
                WeaponType.Magic => "Mágica",
                WeaponType.Summon => "Invocação",
                _ => "Desconhecido"
            };
        }

        private string GetArmorTypeDisplayName(ArmorType type)
        {
            return type switch
            {
                ArmorType.Light => "Leve",
                ArmorType.Heavy => "Pesada",
                ArmorType.MagicRobes => "Mágica",
                _ => "Desconhecida"
            };
        }

        private float GetWeaponXPNeeded(int level)
        {
            return 100f + (level * 50f);
        }

        private float GetArmorXPNeeded(int level)
        {
            return 100f + (level * 50f);
        }

        /// <summary>
        /// Salva os dados do item usando TagCompound.
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="tag">TagCompound para salvar</param>
        public override void SaveData(Item item, TagCompound tag)
        {
            if (RandomStats.Any())
            {
                tag["RandomStats"] = RandomStats;
            }
            
            if (Affixes.Any())
            {
                var affixTags = new List<TagCompound>();
                foreach (var affix in Affixes)
                {
                    var affixTag = new TagCompound();
                    affix.Save(affixTag);
                    affixTags.Add(affixTag);
                }
                tag["Affixes"] = affixTags;
            }
        }

        /// <summary>
        /// Carrega os dados do item usando TagCompound.
        /// </summary>
        /// <param name="item">Item</param>
        /// <param name="tag">TagCompound contendo os dados</param>
        public override void LoadData(Item item, TagCompound tag)
        {
            if (tag.ContainsKey("RandomStats"))
            {
                RandomStats = tag.Get<Dictionary<string, float>>("RandomStats");
            }
            
            if (tag.ContainsKey("Affixes"))
            {
                var affixTags = tag.GetList<TagCompound>("Affixes");
                Affixes.Clear();
                
                foreach (var affixTag in affixTags)
                {
                    var affix = new ItemAffix();
                    affix.Load(affixTag);
                    Affixes.Add(affix);
                }
            }
        }
    }
}
