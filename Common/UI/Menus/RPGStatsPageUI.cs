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
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;
using Wolfgodrpg.Common.UI.Design;
using Wolfgodrpg.Common.Utils;
using Terraria.Audio;
using Terraria.ID;

namespace Wolfgodrpg.Common.UI.Menus
{
    public class RPGStatsPageUI : UIElement
    {
        private UIList _statsList;
        private UIScrollbar _statsScrollbar;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            // Lista de stats com scrollbar (padrão oficial tModLoader)
            _statsList = new UIList();
            _statsList.Width.Set(-25f, 1f);
            _statsList.Height.Set(0, 1f);
            _statsList.ListPadding = 5f;
            Append(_statsList);

            // Scrollbar acoplado à lista (padrão oficial tModLoader)
            _statsScrollbar = new UIScrollbar();
            _statsScrollbar.SetView(100f, 1000f);
            _statsScrollbar.Height.Set(0, 1f);
            _statsScrollbar.HAlign = 1f;
            _statsList.SetScrollbar(_statsScrollbar);
            Append(_statsScrollbar);
        }

        public void UpdateStats(RPGPlayer modPlayer)
        {
            _statsList.Clear();
            
            if (modPlayer == null || modPlayer.Player == null || !modPlayer.Player.active)
            {
                _statsList.Add(new UIText("Jogador não disponível."));
                return;
            }
            
            var player = modPlayer.Player;

            // Player Level and Attribute Points
            _statsList.Add(new StatCard(
                "Nível Geral",
                $"{modPlayer.PlayerLevel}",
                "👑",
                "Evolução constante!",
                $"XP: {modPlayer.PlayerExperience:F0}/{RPGPlayer.GetPlayerExperienceForLevel(modPlayer.PlayerLevel + 1):F0} | Pontos: {modPlayer.AttributePoints}",
                RPGDesignSystem.Colors.Warning
            ));

            // Primary Attributes with distribution buttons
            _statsList.Add(new AttributeStatCard(modPlayer, "Força", modPlayer.Strength, "💪", "Poder bruto!", "Afeta dano corpo a corpo e capacidade de carga.\nClasses relacionadas: Guerreiro, Blacksmith", RPGDesignSystem.Colors.Warning, "Strength"));
            _statsList.Add(new AttributeStatCard(modPlayer, "Destreza", modPlayer.Dexterity, "🎯", "Precisão de sniper!", "Afeta dano à distância, chance crítica e velocidade de ataque.\nClasses relacionadas: Arqueiro, Acrobata", RPGDesignSystem.Colors.Info, "Dexterity"));
            _statsList.Add(new AttributeStatCard(modPlayer, "Inteligência", modPlayer.Intelligence, "🧠", "Einstein do pixel!", "Afeta dano mágico, mana máxima e velocidade de conjuração.\nClasses relacionadas: Mago, Alquimista, Místico", RPGDesignSystem.Colors.Primary, "Intelligence"));
            _statsList.Add(new AttributeStatCard(modPlayer, "Constituição", modPlayer.Constitution, "🛡️", "Tank de verdade!", "Afeta vida máxima, defesa e regeneração de vida.\nClasses relacionadas: Guerreiro, Sobrevivente", RPGDesignSystem.Colors.Success, "Constitution"));
            _statsList.Add(new AttributeStatCard(modPlayer, "Sabedoria", modPlayer.Wisdom, "🦉", "Conhecimento ancestral!", "Afeta dano de invocação, sorte e resistência a debuffs.\nClasses relacionadas: Místico, Invocador", RPGDesignSystem.Colors.PrimaryLight, "Wisdom"));

            // Vital Stats
            _statsList.Add(new StatCard(
                "Fome",
                $"{modPlayer.CurrentHunger:F1}%",
                "🍖",
                "Coma ou morra de fome!",
                "Afeta a regeneração de vida e velocidade de movimento.",
                RPGDesignSystem.GetVitalColor(modPlayer.CurrentHunger)
            ));
            _statsList.Add(new StatCard(
                "Sanidade",
                $"{modPlayer.CurrentSanity:F1}%",
                "🧠",
                "Não enlouqueça!",
                "Afeta a chance de eventos aleatórios e buffs mentais.",
                RPGDesignSystem.GetVitalColor(modPlayer.CurrentSanity)
            ));
            _statsList.Add(new StatCard(
                "Stamina",
                $"{modPlayer.CurrentStamina:F1}%",
                "⚡",
                "Fôlego de atleta!",
                "Afeta a velocidade de dash, pulo e ações físicas.",
                RPGDesignSystem.GetVitalColor(modPlayer.CurrentStamina)
            ));

            // Player Stats (Vida, Mana, Defesa, Velocidade)
            _statsList.Add(new StatCard(
                "Vida",
                $"{player.statLife} / {player.statLifeMax2}",
                "❤️",
                "Cuidado com os chefes!",
                "Se chegar a zero, você morre.",
                RPGDesignSystem.Colors.Success
            ));
            _statsList.Add(new StatCard(
                "Mana",
                $"{player.statMana} / {player.statManaMax2}",
                "🔮",
                "Místicos piram!",
                "Necessária para magias e habilidades especiais.",
                RPGDesignSystem.Colors.Info
            ));
            _statsList.Add(new StatCard(
                "Defesa",
                $"{player.statDefense}",
                "🛡️",
                "Torne-se uma muralha!",
                "Reduz o dano recebido de inimigos.",
                RPGDesignSystem.Colors.Primary
            ));
            _statsList.Add(new StatCard(
                "Velocidade",
                $"{player.moveSpeed:F2}",
                "🏃",
                "Gotta go fast!",
                "Afeta o quão rápido você se move.",
                RPGDesignSystem.Colors.PrimaryLight
            ));

            // Class Levels (existing logic)
            if (RPGClassDefinitions.ActionClasses != null)
            {
                foreach (var classEntry in RPGClassDefinitions.ActionClasses)
                {
                    string classKey = classEntry.Key;
                    var classInfo = classEntry.Value;
                    if (classInfo == null) continue;
                    float level = modPlayer.ClassLevels.TryGetValue(classKey, out var lvl) ? lvl : 0f;
                    float currentExp = 0;
                    modPlayer.ClassExperience.TryGetValue(classKey, out currentExp);
                    float nextLevelExp = 100f * (float)Math.Pow(level + 1, 1.5);
                    float progressPercent = nextLevelExp > 0 ? (currentExp / nextLevelExp * 100f) : 0f;
                    var nextMilestone = classInfo.Milestones?.FirstOrDefault(m => (int)m.Key > level) ?? default;
                    string desc = !nextMilestone.Equals(default(KeyValuePair<ClassAbility, string>)) && !string.IsNullOrEmpty(nextMilestone.Value)
                        ? $"Próxima habilidade: {nextMilestone.Value} (Nv.{(int)nextMilestone.Key})"
                        : "Todas habilidades desbloqueadas!";
                    string funny = classKey switch {
                        "warrior" => "Só vai na porrada!",
                        "archer" => "Robin Hood wannabe.",
                        "mage" => "Harry Potter feelings.",
                        "summoner" => "Pokémon master.",
                        "acrobat" => "Salto mortal carpado!",
                        "explorer" => "Dora, a aventureira.",
                        "engineer" => "Faz gambiarra até no Terraria.",
                        "survivalist" => "Bear Grylls do pixel.",
                        "blacksmith" => "Martela até item quebrar.",
                        "alchemist" => "Transforma água em XP.",
                        "mystic" => "Vidente de plantão.",
                        _ => "Classe misteriosa..."
                    };
                    _statsList.Add(new StatCard(
                        classInfo.Name + $" (Nv.{level:F0})",
                        $"XP: {currentExp:F0}/{nextLevelExp:F0} ({progressPercent:F1}%)",
                        "⭐",
                        funny,
                        desc,
                        RPGDesignSystem.GetClassColor(classKey)
                    ));
                }
            }
        }

        private float GetPlayerExperienceForLevel(int level)
        {
            return 100f * (float)Math.Pow(level, 1.8f);
        }

        private class StatCard : UIElement
        {
            private UIText _titleText;
            private UIText _iconText;
            private UIText _valueText;
            private UIText _funnyText;
            private UIText _descText;
            private Color _color;

            public StatCard(string title, string value, string icon, string funny, string description, Color color)
            {
                _color = color;
                Width.Set(0, 1f);
                Height.Set(110f, 0f); // Adjusted height for consistency

                // Background panel
                UIPanel bg = new UIPanel();
                bg.Width.Set(0, 1f);
                bg.Height.Set(0, 1f);
                bg.BackgroundColor = color * 0.1f;
                bg.BorderColor = color;
                Append(bg);

                // Icon
                _iconText = new UIText(icon, 1.5f);
                _iconText.TextColor = color;
                _iconText.Left.Set(10f, 0f);
                _iconText.Top.Set(10f, 0f);
                bg.Append(_iconText);

                // Title
                _titleText = new UIText(title, 1.0f, true);
                _titleText.TextColor = color;
                _titleText.Left.Set(60f, 0f);
                _titleText.Top.Set(10f, 0f);
                bg.Append(_titleText);

                // Value
                _valueText = new UIText(value, 1.1f);
                _valueText.TextColor = Color.White;
                _valueText.Left.Set(60f, 0f);
                _valueText.Top.Set(40f, 0f);
                bg.Append(_valueText);

                // Funny text
                _funnyText = new UIText(funny, 0.9f);
                _funnyText.TextColor = Color.LightGray;
                _funnyText.Left.Set(10f, 0f);
                _funnyText.Top.Set(70f, 0f);
                bg.Append(_funnyText);

                // Description
                _descText = new UIText(description, 0.8f);
                _descText.TextColor = Color.Gray;
                _descText.Left.Set(10f, 0f);
                _descText.Top.Set(90f, 0f); // Adjusted position
                bg.Append(_descText);
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                base.DrawSelf(spriteBatch);
                var dimensions = GetInnerDimensions();
                var rect = dimensions.ToRectangle();
                var color = _color * 0.08f;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);
                
                // Border
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, 2), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, 2, rect.Height), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - 2, rect.Y, 2, rect.Height), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), _color);
            }

            public override void MouseOver(UIMouseEvent evt)
            {
                base.MouseOver(evt);
                Main.instance.MouseText(_descText.Text);
            }
        }

        private class AttributeStatCard : UIElement
        {
            private RPGPlayer _modPlayer;
            private string _attributeName;
            private string _attributeKey; // e.g., "Strength", "Dexterity"
            private UIText _valueText;
            private Color _color;
            private UIText _descText; // Added for tooltip

            public AttributeStatCard(RPGPlayer modPlayer, string title, int value, string icon, string funny, string description, Color color, string attributeKey)
            {
                _modPlayer = modPlayer;
                _attributeName = title;
                _attributeKey = attributeKey;
                _color = color;

                Width.Set(0, 1f);
                Height.Set(110f, 0f); // Adjust height as needed for buttons

                // Background panel
                UIPanel bg = new UIPanel();
                bg.Width.Set(0, 1f);
                bg.Height.Set(0, 1f);
                bg.BackgroundColor = color * 0.1f;
                bg.BorderColor = color;
                Append(bg);

                // Icon
                UIText iconText = new UIText(icon, 1.5f);
                iconText.TextColor = color;
                iconText.Left.Set(10f, 0f);
                iconText.Top.Set(10f, 0f);
                bg.Append(iconText);

                // Title
                UIText titleText = new UIText(title, 1.0f, true);
                titleText.TextColor = color;
                titleText.Left.Set(60f, 0f);
                titleText.Top.Set(10f, 0f);
                bg.Append(titleText);

                // Value
                _valueText = new UIText(value.ToString(), 1.1f);
                _valueText.TextColor = Color.White;
                _valueText.Left.Set(60f, 0f);
                _valueText.Top.Set(40f, 0f);
                bg.Append(_valueText);

                // Funny text
                UIText funnyText = new UIText(funny, 0.9f);
                funnyText.TextColor = Color.LightGray;
                funnyText.Left.Set(10f, 0f);
                funnyText.Top.Set(70f, 0f);
                bg.Append(funnyText);

                // Description (tooltip)
                _descText = new UIText(description, 0.8f);
                _descText.TextColor = Color.Gray;
                _descText.Left.Set(10f, 0f);
                _descText.Top.Set(90f, 0f); // Adjusted position
                bg.Append(_descText);

                // Plus button
                UIPanel plusButton = new UIPanel();
                plusButton.Width.Set(30f, 0f);
                plusButton.Height.Set(30f, 0f);
                plusButton.Left.Set(-40f, 1f);
                plusButton.Top.Set(10f, 0f);
                plusButton.BackgroundColor = RPGDesignSystem.Colors.Success;
                plusButton.BorderColor = Color.Green;
                plusButton.OnLeftClick += PlusButton_OnLeftClick;
                bg.Append(plusButton);

                UIText plusText = new UIText("+", 1.2f, true);
                plusText.HAlign = 0.5f;
                plusText.VAlign = 0.5f;
                plusButton.Append(plusText);
            }

            private void PlusButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
            {
                if (_modPlayer.AttributePoints > 0)
                {
                    _modPlayer.AttributePoints--;
                    switch (_attributeKey)
                    {
                        case "Strength": _modPlayer.Strength++; break;
                        case "Dexterity": _modPlayer.Dexterity++; break;
                        case "Intelligence": _modPlayer.Intelligence++; break;
                        case "Constitution": _modPlayer.Constitution++; break;
                        case "Wisdom": _modPlayer.Wisdom++; break;
                    }
                    _valueText.SetText(GetAttributeValue().ToString());
                    SoundEngine.PlaySound(SoundID.MenuTick); // Generic sound
                    // Trigger a full UI update to reflect changes in AttributePoints and other stats
                    Main.LocalPlayer.GetModPlayer<RPGPlayer>().Player.statLifeMax2 += 1; // Dummy change to trigger UI update
                }
                else
                {
                    Main.NewText("Você não tem pontos de atributo disponíveis!", Color.Red);
                }
            }

            private int GetAttributeValue()
            {
                return _attributeKey switch
                {
                    "Strength" => _modPlayer.Strength,
                    "Dexterity" => _modPlayer.Dexterity,
                    "Intelligence" => _modPlayer.Intelligence,
                    "Constitution" => _modPlayer.Constitution,
                    "Wisdom" => _modPlayer.Wisdom,
                    _ => 0
                };
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                base.DrawSelf(spriteBatch);
                // Custom drawing for the card background and border
                var dimensions = GetInnerDimensions();
                var rect = dimensions.ToRectangle();
                var color = _color * 0.08f;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, 2), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, 2, rect.Height), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - 2, rect.Y, 2, rect.Height), _color);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), _color);
            }

            public override void MouseOver(UIMouseEvent evt)
            {
                base.MouseOver(evt);
                Main.instance.MouseText(_descText.Text);
            }
        }
    }
}