using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.UI.Design;
using System;

namespace Wolfgodrpg.Common.UI.Menus
{
    // Aba de Status com cards dinâmicos
    public class RPGStatsPageUI : UIElement
    {
        private UIPanel _contentPanel;
        private UIElement _cardGrid;
        private UIScrollbar _cardScrollbar;
        private float _scrollOffset = 0f;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            // Painel de conteúdo fixo (janela da aba)
            _contentPanel = new UIPanel();
            _contentPanel.Width.Set(-40f, 1f); // margem para scrollbar
            _contentPanel.Height.Set(-60f, 1f); // altura limitada
            _contentPanel.Left.Set(20f, 0f);
            _contentPanel.Top.Set(50f, 0f);
            _contentPanel.OverflowHidden = true;
            Append(_contentPanel);

            // Grid de cards
            _cardGrid = new UIElement();
            _cardGrid.Width.Set(0, 1f);
            _cardGrid.Height.Set(0, 0f); // altura dinâmica
            _contentPanel.Append(_cardGrid);

            // Scrollbar
            _cardScrollbar = new UIScrollbar();
            _cardScrollbar.SetView(100f, 1000f);
            _cardScrollbar.Height.Set(-60f, 1f);
            _cardScrollbar.Left.Set(-20f, 1f);
            _cardScrollbar.Top.Set(50f, 0f);
            Append(_cardScrollbar);
        }

        public void UpdateStats(RPGPlayer modPlayer)
        {
            _cardGrid.RemoveAllChildren();
            if (modPlayer == null || modPlayer.Player == null || !modPlayer.Player.active)
            {
                _cardGrid.Append(new UIText("Jogador não disponível."));
                return;
            }
            var player = modPlayer.Player;

            // Monta lista de cards
            var cards = new List<UIElement>();
            cards.Add(new StatCard(
                "Fome", $"{modPlayer.CurrentHunger:F1}%", "🍖", "Coma ou morra de fome!",
                "Afeta a regeneração de vida e velocidade de movimento.",
                RPGDesignSystem.GetVitalColor(modPlayer.CurrentHunger)));
            cards.Add(new StatCard(
                "Sanidade", $"{modPlayer.CurrentSanity:F1}%", "🧠", "Não enlouqueça!",
                "Afeta a chance de eventos aleatórios e buffs mentais.",
                RPGDesignSystem.GetVitalColor(modPlayer.CurrentSanity)));
            cards.Add(new StatCard(
                "Stamina", $"{modPlayer.CurrentStamina:F1}%", "⚡", "Fôlego de atleta!",
                "Afeta a velocidade de dash, pulo e ações físicas.",
                RPGDesignSystem.GetVitalColor(modPlayer.CurrentStamina)));
            cards.Add(new StatCard(
                "Vida", $"{player.statLife} / {player.statLifeMax2}", "❤️", "Cuidado com os chefes!",
                "Se chegar a zero, você morre.",
                RPGDesignSystem.Colors.Success));
            cards.Add(new StatCard(
                "Mana", $"{player.statMana} / {player.statManaMax2}", "🔮", "Místicos piram!",
                "Necessária para magias e habilidades especiais.",
                RPGDesignSystem.Colors.Info));
            cards.Add(new StatCard(
                "Defesa", $"{player.statDefense}", "🛡️", "Torne-se uma muralha!",
                "Reduz o dano recebido de inimigos.",
                RPGDesignSystem.Colors.Primary));
            cards.Add(new StatCard(
                "Velocidade", $"{player.moveSpeed:F2}", "🏃", "Gotta go fast!",
                "Afeta o quão rápido você se move.",
                RPGDesignSystem.Colors.PrimaryLight));

            // Cards de classes
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
                    float nextLevelExp = 100 * (level + 1);
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
                    cards.Add(new StatCard(
                        classInfo.Name + $" (Nv.{level:F0})",
                        $"XP: {currentExp:F0}/{nextLevelExp:F0} ({progressPercent:F1}%)",
                        "⭐",
                        funny,
                        desc,
                        RPGDesignSystem.GetClassColor(classKey)));
                }
            }

            // Grid: 3 colunas
            int columns = 3;
            float cardW = 400f;
            float cardH = 220f;
            float spacing = 24f;
            int rows = (int)Math.Ceiling(cards.Count / (float)columns);
            float gridHeight = rows * cardH + (rows - 1) * spacing;
            _cardGrid.Height.Set(gridHeight, 0f);

            for (int i = 0; i < cards.Count; i++)
            {
                int col = i % columns;
                int row = i / columns;
                var card = cards[i];
                card.Left.Set(col * (cardW + spacing), 0f);
                card.Top.Set(row * (cardH + spacing), 0f);
                _cardGrid.Append(card);
            }

            // Scroll: calcula máximo e atualiza offset
            float panelHeight = _contentPanel.GetInnerDimensions().Height;
            float maxScroll = Math.Max(0, gridHeight - panelHeight);
            _cardScrollbar.SetView(panelHeight, gridHeight);
            _scrollOffset = _cardScrollbar.ViewPosition * maxScroll;
            _cardGrid.Top.Set(-_scrollOffset, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // Scroll: atualiza offset do grid
            if (_cardGrid != null && _contentPanel != null && _cardScrollbar != null)
            {
                float gridHeight = _cardGrid.GetInnerDimensions().Height;
                float panelHeight = _contentPanel.GetInnerDimensions().Height;
                float maxScroll = Math.Max(0, gridHeight - panelHeight);
                _scrollOffset = _cardScrollbar.ViewPosition * maxScroll;
                _cardGrid.Top.Set(-_scrollOffset, 0f);
            }
        }

        // Card universal para status, vitais e classes
        private class StatCard : UIElement
        {
            private string _tooltip;
            public StatCard(string title, string value, string icon, string funny, string description, Color color)
            {
                // Largura fixa, centralizado
                Width.Set(400f, 0f);
                MinWidth.Set(400f, 0f);
                MaxWidth.Set(400f, 0f);
                HAlign = 0.5f;
                // Dobrar altura mínima
                MinHeight.Set(220f, 0f);
                SetPadding(RPGDesignSystem.Spacing.M);
                _tooltip = description;

                var bg = new UIPanel();
                bg.Width.Set(0, 1f);
                bg.Height.Set(0, 1f);
                bg.BackgroundColor = RPGDesignSystem.Colors.Surface;
                bg.BorderColor = color;
                bg.PaddingTop = bg.PaddingBottom = RPGDesignSystem.Spacing.M;
                bg.PaddingLeft = bg.PaddingRight = RPGDesignSystem.Spacing.L;
                Append(bg);

                float y = 0f;

                // Ícone pequeno
                var iconText = new UIText(icon, 0.6f, true) { HAlign = 0.5f };
                iconText.Top.Set(y, 0f);
                bg.Append(iconText);
                y += iconText.MinHeight.Pixels + RPGDesignSystem.Spacing.XS;

                // Título
                var titleText = new UIText(title, 0.7f, true) { HAlign = 0.5f };
                titleText.TextColor = color;
                titleText.Top.Set(y, 0f);
                bg.Append(titleText);
                y += titleText.MinHeight.Pixels + RPGDesignSystem.Spacing.XS;

                // Valor
                var valueText = new UIText(value, 0.9f, true) { HAlign = 0.5f };
                valueText.TextColor = RPGDesignSystem.Colors.Text;
                valueText.Top.Set(y, 0f);
                bg.Append(valueText);
                y += valueText.MinHeight.Pixels + RPGDesignSystem.Spacing.XS;

                // Frase engraçada (word wrap/truncamento)
                string funnyShort = funny.Length > 32 ? funny.Substring(0, 29) + "..." : funny;
                var funnyText = new UIText(funnyShort, 0.6f, true) { HAlign = 0.5f };
                funnyText.TextColor = RPGDesignSystem.Colors.TextSecondary;
                funnyText.Top.Set(y, 0f);
                bg.Append(funnyText);
                y += funnyText.MinHeight.Pixels + RPGDesignSystem.Spacing.XS;

                // Explicação (word wrap/truncamento)
                string descShort = description.Length > 48 ? description.Substring(0, 45) + "..." : description;
                var descText = new UIText(descShort, 0.5f, false) { HAlign = 0.5f };
                descText.TextColor = RPGDesignSystem.Colors.TextMuted;
                descText.Top.Set(y, 0f);
                bg.Append(descText);
                y += descText.MinHeight.Pixels + RPGDesignSystem.Spacing.S;

                // Garante altura mínima dobrada
                float finalHeight = Math.Max(y, 220f);
                Height.Set(finalHeight, 0f);
                bg.Height.Set(finalHeight, 0f);
                Recalculate();
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                base.DrawSelf(spriteBatch);
                if (IsMouseHovering)
                {
                    Main.hoverItemName = _tooltip;
                    Main.LocalPlayer.mouseInterface = true;
                }
            }
        }
    }
}
