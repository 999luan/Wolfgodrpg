using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Utils;
using Wolfgodrpg.Common.Systems;
using Wolfgodrpg.Common.UI.Design;
using Wolfgodrpg.Common.GlobalItems;

namespace Wolfgodrpg.Common.UI.Menus
{
    /// <summary>
    /// Página de UI para mostrar proficiências de armadura.
    /// </summary>
    public class RPGProficienciesPageUI : UIElement
    {
        private UIScrollbar _scrollbar;
        private UIList _proficiencyList;

        public override void OnInitialize()
        {
            DebugLog.UI("OnInitialize", "Inicializando RPGProficienciesPageUI");
            
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            // Lista de proficiências com scrollbar (padrão oficial tModLoader)
            _proficiencyList = new UIList();
            _proficiencyList.Width.Set(-25f, 1f);
            _proficiencyList.Height.Set(0, 1f);
            _proficiencyList.ListPadding = 5f;
            Append(_proficiencyList);

            // Scrollbar acoplado à lista (padrão oficial tModLoader)
            _scrollbar = new UIScrollbar();
            _scrollbar.SetView(100f, 1000f);
            _scrollbar.Height.Set(0, 1f);
            _scrollbar.HAlign = 1f;
            _proficiencyList.SetScrollbar(_scrollbar);
            Append(_scrollbar);
            
            DebugLog.UI("OnInitialize", "RPGProficienciesPageUI inicializado com sucesso");
        }

        /// <summary>
        /// Atualiza as proficiências exibidas.
        /// </summary>
        /// <param name="modPlayer">Jogador RPG</param>
        public void UpdateProficiencies(RPGPlayer modPlayer)
        {
            if (modPlayer == null)
            {
                DebugLog.UI("UpdateProficiencies", "Jogador não disponível");
                return;
            }

            _proficiencyList.Clear();

            float topOffset = 10f;

            // Armaduras
            foreach (ArmorType armorType in System.Enum.GetValues<ArmorType>())
            {
                if (armorType == ArmorType.None) continue;
                var proficiencyCard = CreateArmorProficiencyCard(armorType, modPlayer, topOffset);
                _proficiencyList.Add(proficiencyCard);
                topOffset = 0f;
            }

            // Armas
            foreach (WeaponType weaponType in System.Enum.GetValues<WeaponType>())
            {
                if (weaponType == WeaponType.None) continue;
                var proficiencyCard = CreateWeaponProficiencyCard(weaponType, modPlayer, topOffset);
                _proficiencyList.Add(proficiencyCard);
                topOffset = 0f;
            }

            // Itens progressivos (do inventário) - Comentado até implementar ProgressiveItemCard
            /*
            if (Main.LocalPlayer != null && Main.LocalPlayer.inventory != null)
            {
                foreach (var item in Main.LocalPlayer.inventory)
                {
                    if (item == null || item.IsAir) continue;
                    if (item.TryGetGlobalItem<ProgressiveItem>(out var progressiveItem) && progressiveItem.Experience > 0)
                    {
                        var itemCard = CreateProgressiveItemCard(item, progressiveItem, topOffset);
                        _proficiencyList.Add(itemCard);
                        topOffset = 0f;
                    }
                }
            }
            */

            DebugLog.UI("UpdateProficiencies", "Proficiências de armadura e itens atualizadas com sucesso");
        }

        private ProficiencyCard CreateArmorProficiencyCard(ArmorType armorType, RPGPlayer modPlayer, float topOffset = 0f)
        {
            string armorName = GetArmorTypeName(armorType);
            string armorIcon = GetArmorTypeIcon(armorType);
            Color armorColor = GetArmorTypeColor(armorType);
            
            float level = modPlayer.ArmorProficiencyLevels.TryGetValue(armorType, out var lvl) ? lvl : 0f;
            float experience = modPlayer.ArmorProficiencyExperience.TryGetValue(armorType, out var exp) ? exp : 0f;
            float nextLevelExp = GetProficiencyExperienceForLevel((int)level + 1);
            float progressPercent = nextLevelExp > 0 ? (experience / nextLevelExp) * 100f : 0f;

            return new ProficiencyCard(armorName, armorIcon, armorColor, level, experience, nextLevelExp, progressPercent, topOffset);
        }

        private ProficiencyCard CreateWeaponProficiencyCard(WeaponType weaponType, RPGPlayer modPlayer, float topOffset = 0f)
        {
            string weaponName = GetWeaponTypeName(weaponType);
            string weaponIcon = GetWeaponTypeIcon(weaponType);
            Color weaponColor = GetWeaponTypeColor(weaponType);
            
            float level = modPlayer.WeaponProficiencyLevels.TryGetValue(weaponType, out var lvl) ? lvl : 0f;
            float experience = modPlayer.WeaponProficiencyExperience.TryGetValue(weaponType, out var exp) ? exp : 0f;
            float nextLevelExp = GetProficiencyExperienceForLevel((int)level + 1);
            float progressPercent = nextLevelExp > 0 ? (experience / nextLevelExp) * 100f : 0f;

            return new ProficiencyCard(weaponName, weaponIcon, weaponColor, level, experience, nextLevelExp, progressPercent, topOffset);
        }

        // Comentado até implementar ProgressiveItemCard
        /*
        private ProgressiveItemCard CreateProgressiveItemCard(Item item, ProgressiveItem progressiveItem, float topOffset = 0f)
        {
            int level = progressiveItem.GetItemLevel();
            float nextLevelExp = GetProficiencyExperienceForLevel(level + 1);
            float progressPercent = nextLevelExp > 0 ? (progressiveItem.Experience / nextLevelExp) * 100f : 0f;
            return new ProgressiveItemCard(item, progressiveItem, level, nextLevelExp, progressPercent, topOffset);
        }
        */

        private float GetProficiencyExperienceForLevel(int level)
        {
            return 100f * (float)Math.Pow(level, 1.8f);
        }

        /// <summary>
        /// Obtém o nome de um tipo de armadura.
        /// </summary>
        /// <param name="armorType">Tipo de armadura</param>
        /// <returns>Nome do tipo de armadura</returns>
        private string GetArmorTypeName(ArmorType armorType)
        {
            return armorType switch
            {
                ArmorType.Light => "Armadura Leve",
                ArmorType.Heavy => "Armadura Pesada",
                ArmorType.MagicRobes => "Vestes Mágicas",
                _ => "Desconhecido"
            };
        }

        /// <summary>
        /// Obtém o ícone de um tipo de armadura.
        /// </summary>
        /// <param name="armorType">Tipo de armadura</param>
        /// <returns>Ícone do tipo de armadura</returns>
        private string GetArmorTypeIcon(ArmorType armorType)
        {
            return armorType switch
            {
                ArmorType.Light => "🏃",
                ArmorType.Heavy => "🛡️",
                ArmorType.MagicRobes => "🔮",
                _ => "🛡️"
            };
        }

        /// <summary>
        /// Obtém a cor de um tipo de armadura.
        /// </summary>
        /// <param name="armorType">Tipo de armadura</param>
        /// <returns>Cor do tipo de armadura</returns>
        private Color GetArmorTypeColor(ArmorType armorType)
        {
            return armorType switch
            {
                ArmorType.Light => Color.LightBlue,             // Azul claro para velocidade
                ArmorType.Heavy => Color.Gray,                  // Cinza para defesa
                ArmorType.MagicRobes => Color.Purple,           // Roxo para magia
                _ => Color.White
            };
        }

        /// <summary>
        /// Obtém o nome de um tipo de arma.
        /// </summary>
        /// <param name="weaponType">Tipo de arma</param>
        /// <returns>Nome do tipo de arma</returns>
        private string GetWeaponTypeName(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Melee => "Arma Corpo a Corpo",
                WeaponType.Ranged => "Arma à Distância",
                WeaponType.Magic => "Arma Mágica",
                WeaponType.Summon => "Arma de Invocação",
                _ => "Desconhecido"
            };
        }

        /// <summary>
        /// Obtém o ícone de um tipo de arma.
        /// </summary>
        /// <param name="weaponType">Tipo de arma</param>
        /// <returns>Ícone do tipo de arma</returns>
        private string GetWeaponTypeIcon(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Melee => "⚔️",
                WeaponType.Ranged => "🏹",
                WeaponType.Magic => "✨",
                WeaponType.Summon => "🐾",
                _ => "❓"
            };
        }

        /// <summary>
        /// Obtém a cor de um tipo de arma.
        /// </summary>
        /// <param name="weaponType">Tipo de arma</param>
        /// <returns>Cor do tipo de arma</returns>
        private Color GetWeaponTypeColor(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Melee => Color.OrangeRed,
                WeaponType.Ranged => Color.GreenYellow,
                WeaponType.Magic => Color.DeepSkyBlue,
                WeaponType.Summon => Color.MediumPurple,
                _ => Color.White
            };
        }

        /// <summary>
        /// Card de proficiência de armadura.
        /// </summary>
        private class ProficiencyCard : UIElement
        {
            public ProficiencyCard(string name, string icon, Color color, float level, float experience, float nextLevelExp, float progressPercent, float topOffset = 0f)
            {
                Width.Set(0, 1f);
                Height.Set(110f, 0f);
                Top.Set(topOffset, 0f);

                // Ícone
                var iconText = new UIText(icon, 1.5f);
                iconText.TextColor = color;
                iconText.Left.Set(20f, 0f);
                iconText.Top.Set(20f, 0f);
                Append(iconText);

                // Nome
                var titleText = new UIText(name, 1.0f, true);
                titleText.TextColor = color;
                titleText.Left.Set(80f, 0f);
                titleText.Top.Set(15f, 0f);
                Append(titleText);

                // Nível
                var levelText = new UIText($"Nível {level:F0}", 0.95f);
                levelText.TextColor = Color.White;
                levelText.Left.Set(80f, 0f);
                levelText.Top.Set(45f, 0f);
                Append(levelText);

                // XP
                var expText = new UIText($"XP: {experience:F0}/{nextLevelExp:F0}", 0.85f);
                expText.TextColor = Color.LightGray;
                expText.Left.Set(80f, 0f);
                expText.Top.Set(65f, 0f);
                Append(expText);

                // Progresso
                var progressText = new UIText($"Progresso: {progressPercent:F1}%", 0.8f);
                progressText.TextColor = RPGDesignSystem.GetProgressColor(progressPercent);
                progressText.Left.Set(80f, 0f);
                progressText.Top.Set(85f, 0f);
                Append(progressText);
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                var dimensions = GetInnerDimensions();
                var rect = dimensions.ToRectangle();
                var color = Color.White * 0.08f;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, 2), Color.Gray);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, 2, rect.Height), Color.Gray);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - 2, rect.Y, 2, rect.Height), Color.Gray);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), Color.Gray);
            }
        }

        /// <summary>
        /// Card de proficiência de arma.
        /// </summary>
        private class WeaponProficiencyCard : UIElement
        {
            public WeaponProficiencyCard(string name, string icon, Color color, float level, float experience, float nextLevelExp, float progressPercent, float topOffset = 0f)
            {
                Width.Set(0, 1f);
                Height.Set(110f, 0f);
                Top.Set(topOffset, 0f);

                // Ícone
                var iconText = new UIText(icon, 1.5f);
                iconText.TextColor = color;
                iconText.Left.Set(20f, 0f);
                iconText.Top.Set(20f, 0f);
                Append(iconText);

                // Nome
                var titleText = new UIText(name, 1.0f, true);
                titleText.TextColor = color;
                titleText.Left.Set(80f, 0f);
                titleText.Top.Set(15f, 0f);
                Append(titleText);

                // Nível
                var levelText = new UIText($"Nível {level:F0}", 0.95f);
                levelText.TextColor = Color.White;
                levelText.Left.Set(80f, 0f);
                levelText.Top.Set(45f, 0f);
                Append(levelText);

                // XP
                var expText = new UIText($"XP: {experience:F0}/{nextLevelExp:F0}", 0.85f);
                expText.TextColor = Color.LightGray;
                expText.Left.Set(80f, 0f);
                expText.Top.Set(65f, 0f);
                Append(expText);

                // Progresso
                var progressText = new UIText($"Progresso: {progressPercent:F1}%", 0.8f);
                progressText.TextColor = RPGDesignSystem.GetProgressColor(progressPercent);
                progressText.Left.Set(80f, 0f);
                progressText.Top.Set(85f, 0f);
                Append(progressText);
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                var dimensions = GetInnerDimensions();
                var rect = dimensions.ToRectangle();
                var color = Color.White * 0.08f;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, 2), Color.Gray);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, 2, rect.Height), Color.Gray);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - 2, rect.Y, 2, rect.Height), Color.Gray);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), Color.Gray);
            }
        }
    }
}