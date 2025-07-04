using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Utils;
using Wolfgodrpg.Common.UI.Design;

namespace Wolfgodrpg.Common.UI.Menus
{
    // Aba de Classes focada em detalhes e habilidades
    public class RPGClassesPageUI : UIElement
    {
        private UIList _classesList;
        private UIScrollbar _classesScrollbar;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            // Lista de classes com scrollbar (padrão oficial tModLoader)
            _classesList = new UIList();
            _classesList.Width.Set(-25f, 1f);
            _classesList.Height.Set(0, 1f);
            _classesList.ListPadding = 5f;
            Append(_classesList);

            // Scrollbar acoplado à lista (padrão oficial tModLoader)
            _classesScrollbar = new UIScrollbar();
            _classesScrollbar.SetView(100f, 1000f);
            _classesScrollbar.Height.Set(0, 1f);
            _classesScrollbar.HAlign = 1f;
            _classesList.SetScrollbar(_classesScrollbar);
            Append(_classesScrollbar);
        }

        // Atualiza o conteúdo da aba de classes
        public void UpdateClasses(RPGPlayer modPlayer)
        {
            _classesList.Clear();
            
            if (modPlayer == null || modPlayer.Player == null || !modPlayer.Player.active)
            {
                _classesList.Add(new UIText("Player not available."));
                return;
            }

            if (RPGClassDefinitions.ActionClasses == null || RPGClassDefinitions.ActionClasses.Count == 0)
            {
                _classesList.Add(new UIText("No classes defined."));
                return;
            }

            // Removed: _classesList.Add(new UIText("🎯 CLASS DETAILS", 1.1f, true) { TextColor = Color.Gold });

            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                var classInfo = classEntry.Value;
                if (classInfo == null) continue;

                float level = modPlayer.ClassLevels.TryGetValue(classEntry.Key, out var lvl) ? lvl : 0f;
                float currentExp = modPlayer.ClassExperience.TryGetValue(classEntry.Key, out var exp) ? exp : 0f;
                float nextLevelExp = GetClassExperienceForLevel(classEntry.Key, (int)level + 1);
                float progressPercent = nextLevelExp > 0 ? (currentExp / nextLevelExp) * 100f : 0f;
                
                _classesList.Add(new ClassCard(
                    classInfo.Name,
                    level,
                    currentExp,
                    nextLevelExp,
                    progressPercent,
                    classInfo.Milestones,
                    classInfo.StatBonuses,
                    classEntry.Key));
            }
        }

        private float GetClassExperienceForLevel(string classKey, int level)
        {
            return 100f * (float)Math.Pow(level, 1.8f);
        }

        // Card de classe detalhado
        private class ClassCard : UIElement
        {
            private UIText _titleText;
            private UIText _iconText;
            private UIText _levelText;
            private UIText _expText;
            private UIText _abilitiesHeaderText;
            private UIText _statsHeaderText;
            private Color _color;

            public ClassCard(string className, float level, float currentExp, float nextLevelExp, float progressPercent, 
                           Dictionary<ClassAbility, string> milestones, Dictionary<string, float> statBonuses, string classKey)
            {
                _color = RPGDesignSystem.GetClassColor(classKey);
                Width.Set(-10f, 1f); // padding horizontal
                Height.Set(300f, 0f);
                PaddingTop = 10f;
                PaddingBottom = 10f;
                PaddingLeft = 14f;
                PaddingRight = 14f;
                MarginTop = 10f;
                MarginBottom = 10f;

                // Class title
                _titleText = new UIText($"{className} - Level {level:F0}", 1.2f, true);
                _titleText.TextColor = _color;
                _titleText.Left.Set(20f, 0f);
                _titleText.Top.Set(15f, 0f);
                Append(_titleText);

                // Ícone da classe
                _iconText = new UIText(GetClassIcon(classKey), 2f);
                _iconText.TextColor = _color;
                _iconText.Left.Set(20f, 0f);
                _iconText.Top.Set(50f, 0f);
                Append(_iconText);

                // Level and progress
                _levelText = new UIText($"Level {level:F0}", 1.1f);
                _levelText.TextColor = Color.White;
                _levelText.Left.Set(80f, 0f);
                _levelText.Top.Set(50f, 0f);
                Append(_levelText);

                // Experiência
                _expText = new UIText($"XP: {currentExp:F0}/{nextLevelExp:F0} ({progressPercent:F1}%)", 0.9f);
                _expText.TextColor = Color.LightGray;
                _expText.Left.Set(80f, 0f);
                _expText.Top.Set(75f, 0f);
                Append(_expText);

                float yOffset = 110f;

                // Unlocked abilities
                if (milestones != null && milestones.Count > 0)
                {
                    _abilitiesHeaderText = new UIText("✅ Unlocked Abilities:", 0.9f);
                    _abilitiesHeaderText.TextColor = Color.LightGreen;
                    _abilitiesHeaderText.Left.Set(20f, 0f);
                    _abilitiesHeaderText.Top.Set(yOffset, 0f);
                    Append(_abilitiesHeaderText);
                    yOffset += 20f;

                    var unlockedMilestones = milestones.Where(m => (int)m.Key <= level).OrderBy(m => m.Key).ToList();
                    foreach (var milestone in unlockedMilestones)
                    {
                        var abilityText = new UIText($"  Nv.{(int)milestone.Key}: {milestone.Value}", 0.8f);
                        abilityText.TextColor = Color.LightGreen;
                        abilityText.Left.Set(20f, 0f);
                        abilityText.Top.Set(yOffset, 0f);
                        Append(abilityText);
                        yOffset += 15f;
                    }
                }

                // Attribute bonuses
                if (statBonuses != null && statBonuses.Count > 0)
                {
                    _statsHeaderText = new UIText("📊 Attribute Bonuses:", 0.9f);
                    _statsHeaderText.TextColor = Color.LightBlue;
                    _statsHeaderText.Left.Set(20f, 0f);
                    _statsHeaderText.Top.Set(yOffset, 0f);
                    Append(_statsHeaderText);
                    yOffset += 20f;

                    foreach (var statBonus in statBonuses)
                    {
                        string statName = RPGDisplayUtils.GetStatDisplayName(statBonus.Key);
                        var statText = new UIText($"  {statName}: +{statBonus.Value:F1}", 0.8f);
                        statText.TextColor = Color.LightBlue;
                        statText.Left.Set(20f, 0f);
                        statText.Top.Set(yOffset, 0f);
                        Append(statText);
                        yOffset += 15f;
                    }
                }
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

                // Fundo do card
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, _color * 0.1f);
                
                // Borda
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, 2), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, 2, rect.Height), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - 2, rect.Y, 2, rect.Height), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), _color);
            }

            public override void MouseOver(UIMouseEvent evt)
            {
                base.MouseOver(evt);
                Main.instance.MouseText("Clique para ver mais detalhes");
            }
        }
    }
}
