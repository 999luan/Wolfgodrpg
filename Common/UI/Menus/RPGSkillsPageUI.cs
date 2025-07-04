using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.UI.Design;

namespace Wolfgodrpg.Common.UI.Menus
{
    public class RPGSkillsPageUI : UIElement
    {
        private UIList _skillsList;
        private UIScrollbar _skillsScrollbar;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            _skillsList = new UIList();
            _skillsList.Width.Set(-25f, 1f);
            _skillsList.Height.Set(0, 1f);
            _skillsList.ListPadding = 5f;
            Append(_skillsList);

            _skillsScrollbar = new UIScrollbar();
            _skillsScrollbar.SetView(100f, 1000f);
            _skillsScrollbar.Height.Set(0, 1f);
            _skillsScrollbar.HAlign = 1f;
            _skillsList.SetScrollbar(_skillsScrollbar);
            Append(_skillsScrollbar);
        }

        public void UpdateSkills(RPGPlayer modPlayer)
        {
            _skillsList.Clear();
            float topOffset = 10f;

            if (modPlayer == null || modPlayer.Player == null || !modPlayer.Player.active)
            {
                _skillsList.Add(new UIText("Jogador não disponível."));
                return;
            }

            // Listar todas as habilidades desbloqueadas por classe
            foreach (var classEntry in modPlayer.ClassLevels.OrderByDescending(kv => kv.Value))
            {
                string classKey = classEntry.Key;
                float classLevel = classEntry.Value;
                if (!RPGClassDefinitions.ActionClasses.TryGetValue(classKey, out var classInfo)) continue;

                // Título da classe
                var classTitle = new UIText($"{classInfo.Name} (Nv. {classLevel:F0})", 1.1f, true);
                classTitle.TextColor = RPGDesignSystem.GetClassColor(classKey);
                classTitle.Top.Set(topOffset, 0f);
                _skillsList.Add(classTitle);
                topOffset = 0f;

                // Habilidades desbloqueadas
                var unlocked = classInfo.Milestones?.Where(m => (int)m.Key <= classLevel).ToList();
                if (unlocked != null && unlocked.Count > 0)
                {
                    foreach (var milestone in unlocked)
                    {
                        _skillsList.Add(new SkillCard(milestone.Value, classInfo.Name, classKey, milestone.Key.ToString(), "Passiva/Ativa", RPGDesignSystem.GetClassColor(classKey)));
                    }
                }

                // Regra especial: Acrobata ganha +1 dash a cada 10 níveis
                if (classKey == "acrobat" && classLevel >= 10)
                {
                    int extraDashes = (int)(classLevel / 10);
                    for (int i = 1; i <= extraDashes; i++)
                    {
                        _skillsList.Add(new SkillCard($"+1 Dash Extra (Total: {i})", classInfo.Name, classKey, $"Nv. {i * 10}", "Especial", RPGDesignSystem.GetClassColor(classKey)));
                    }
                }
            }
        }

        private class SkillCard : UIElement
        {
            public SkillCard(string skillName, string className, string classKey, string unlockLevel, string type, Color color)
            {
                Width.Set(-10f, 1f); // padding horizontal
                Height.Set(80f, 0f); // altura maior
                PaddingTop = 8f;
                PaddingBottom = 8f;
                PaddingLeft = 12f;
                PaddingRight = 12f;
                MarginTop = 8f;
                MarginBottom = 8f;

                // Ícone da classe
                var iconText = new UIText(GetClassIcon(classKey), 1.5f);
                iconText.TextColor = color;
                iconText.Left.Set(20f, 0f);
                iconText.Top.Set(15f, 0f);
                Append(iconText);

                // Nome da habilidade
                var skillText = new UIText(skillName, 1.0f, true);
                skillText.TextColor = color;
                skillText.Left.Set(80f, 0f);
                skillText.Top.Set(10f, 0f);
                skillText.Width.Set(-100f, 1f); // limitar largura
                skillText.OverflowHidden = true;
                Append(skillText);

                // Nível de desbloqueio
                var levelText = new UIText(unlockLevel, 0.85f);
                levelText.TextColor = Color.LightGray;
                levelText.Left.Set(80f, 0f);
                levelText.Top.Set(35f, 0f);
                Append(levelText);

                // Tipo
                var typeText = new UIText(type, 0.8f);
                typeText.TextColor = Color.Gray;
                typeText.Left.Set(200f, 0f);
                typeText.Top.Set(35f, 0f);
                Append(typeText);
            }

            private string GetClassIcon(string classKey)
            {
                return classKey switch
                {
                    "warrior" => "⚔️",
                    "archer" => "🏹",
                    "mage" => "🔮",
                    "summoner" => "👾",
                    "acrobat" => "🤸",
                    "explorer" => "🧭",
                    "engineer" => "🔧",
                    "survivalist" => "🌲",
                    "blacksmith" => "🛠️",
                    "alchemist" => "⚗️",
                    "mystic" => "✨",
                    _ => "⭐"
                };
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                var dimensions = GetInnerDimensions();
                var rect = dimensions.ToRectangle();
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.White * 0.06f);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, 2), Color.Gray);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, 2, rect.Height), Color.Gray);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - 2, rect.Y, 2, rect.Height), Color.Gray);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), Color.Gray);
            }
        }
    }
} 