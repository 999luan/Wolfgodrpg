using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.UI.Design;
using Wolfgodrpg.Common.Utils;

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

            // Vitals
            _statsList.Add(new StatCard("Fome", $"{modPlayer.CurrentHunger:F1}%", "🍖", "Coma ou morra de fome!", "Afeta a regeneração de vida e velocidade de movimento.", RPGDesignSystem.GetVitalColor(modPlayer.CurrentHunger)));
            _statsList.Add(new StatCard("Sanidade", $"{modPlayer.CurrentSanity:F1}%", "🧠", "Não enlouqueça!", "Afeta a chance de eventos aleatórios e buffs mentais.", RPGDesignSystem.GetVitalColor(modPlayer.CurrentSanity)));
            _statsList.Add(new StatCard("Stamina", $"{modPlayer.CurrentStamina:F1}%", "⚡", "Fôlego de atleta!", "Afeta a velocidade de dash, pulo e ações físicas.", RPGDesignSystem.GetVitalColor(modPlayer.CurrentStamina)));

            // Base stats
            _statsList.Add(new StatCard("Vida", $"{player.statLife} / {player.statLifeMax2}", "❤️", "Cuidado com os chefes!", "Se chegar a zero, você morre.", RPGDesignSystem.Colors.Success));
            _statsList.Add(new StatCard("Mana", $"{player.statMana} / {player.statManaMax2}", "🔮", "Místicos piram!", "Necessária para magias e habilidades especiais.", RPGDesignSystem.Colors.Info));
            _statsList.Add(new StatCard("Defesa", $"{player.statDefense}", "🛡️", "Torne-se uma muralha!", "Reduz o dano recebido de inimigos.", RPGDesignSystem.Colors.Primary));
            _statsList.Add(new StatCard("Velocidade", $"{player.moveSpeed:F2}", "🏃", "Gotta go fast!", "Afeta o quão rápido você se move.", RPGDesignSystem.Colors.PrimaryLight));

            // Atributos primários
            _statsList.Add(new StatCard("Força", $"{modPlayer.Strength}", "💪", "Hulk smash!", "Afeta dano corpo a corpo e capacidade de carga.", RPGDesignSystem.Colors.Warning));
            _statsList.Add(new StatCard("Destreza", $"{modPlayer.Dexterity}", "🎯", "Precisão de sniper!", "Afeta dano à distância, chance crítica e velocidade de ataque.", RPGDesignSystem.Colors.Info));
            _statsList.Add(new StatCard("Inteligência", $"{modPlayer.Intelligence}", "🧠", "Einstein do pixel!", "Afeta dano mágico, mana máxima e velocidade de conjuração.", RPGDesignSystem.Colors.Primary));
            _statsList.Add(new StatCard("Constituição", $"{modPlayer.Constitution}", "🛡️", "Tank de verdade!", "Afeta vida máxima, defesa e regeneração de vida.", RPGDesignSystem.Colors.Success));
            _statsList.Add(new StatCard("Sabedoria", $"{modPlayer.Wisdom}", "✨", "Sábio como Gandalf!", "Afeta dano de invocação, sorte e resistência a debuffs.", RPGDesignSystem.Colors.PrimaryLight));

            // Nível do jogador
            _statsList.Add(new StatCard("Nível Geral", $"{modPlayer.PlayerLevel}", "👑", "Evolução constante!", $"XP: {modPlayer.PlayerExperience:F0}/{GetPlayerExperienceForLevel(modPlayer.PlayerLevel + 1):F0} | Pontos: {modPlayer.AttributePoints}", RPGDesignSystem.Colors.Warning));

            // Classes
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
                        GetClassIcon(classKey),
                        funny,
                        desc,
                        RPGDesignSystem.GetClassColor(classKey)));
                }
            }
        }

        private float GetPlayerExperienceForLevel(int level)
        {
            return 100f * (float)Math.Pow(level, 1.8f);
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
                Height.Set(220f, 0f);

                // Título
                _titleText = new UIText(title, 1.2f, true);
                _titleText.TextColor = color;
                _titleText.Left.Set(20f, 0f);
                _titleText.Top.Set(15f, 0f);
                Append(_titleText);

                // Ícone
                _iconText = new UIText(icon, 2f);
                _iconText.TextColor = color;
                _iconText.Left.Set(20f, 0f);
                _iconText.Top.Set(50f, 0f);
                Append(_iconText);

                // Valor
                _valueText = new UIText(value, 1.1f);
                _valueText.TextColor = Color.White;
                _valueText.Left.Set(80f, 0f);
                _valueText.Top.Set(50f, 0f);
                Append(_valueText);

                // Texto engraçado
                _funnyText = new UIText(funny, 0.9f);
                _funnyText.TextColor = Color.LightGray;
                _funnyText.Left.Set(20f, 0f);
                _funnyText.Top.Set(90f, 0f);
                Append(_funnyText);

                // Descrição
                _descText = new UIText(description, 0.8f);
                _descText.TextColor = Color.Gray;
                _descText.Left.Set(20f, 0f);
                _descText.Top.Set(120f, 0f);
                Append(_descText);
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
                Main.instance.MouseText(_descText.Text);
            }
        }
    }
} 