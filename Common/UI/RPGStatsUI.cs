using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Classes;
using System.Text;

namespace Wolfgodrpg.Common.UI
{
    public class RPGStatsUI : UIState
    {
        

        

        

        private UIPanel BottomPanel;
        private UIPanel RightPanel;

        private Color HeaderColor = new Color(73, 94, 171);
        private Color TextColor = new Color(235, 235, 235);

        // UI Elements for Base Stats
        private List<UIPanel> _baseStatContainers = new List<UIPanel>();
        private List<UIPanel> _baseStatProgressBars = new List<UIPanel>();
        private List<UIText> _baseStatTexts = new List<UIText>();

        // UI Elements for Class Stats
        private Dictionary<string, UIPanel> _classPanels = new Dictionary<string, UIPanel>();
        private Dictionary<string, UIText> _classTexts = new Dictionary<string, UIText>();

        public override void OnInitialize()
        {
            // Painel Inferior (Status e Níveis)
            BottomPanel = new UIPanel();
            BottomPanel.Width.Set(0f, 1f); // 100% da largura da tela
            BottomPanel.Height.Set(150f, 0f); // Altura fixa
            BottomPanel.HAlign = 0.5f;
            BottomPanel.VAlign = 1f; // Alinhar na parte inferior
            BottomPanel.Top.Set(-10f, 0f); // Pequeno espaçamento da borda inferior
            BottomPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            Append(BottomPanel);

            // Painel Direito (Classes e Níveis de Classes)
            RightPanel = new UIPanel();
            RightPanel.Width.Set(300f, 0f); // Largura fixa
            RightPanel.Height.Set(0f, 0.5f); // 50% da altura da tela
            RightPanel.HAlign = 1f; // Alinhar na parte direita
            RightPanel.VAlign = 0f; // Alinhar no topo
            RightPanel.Left.Set(-10f, 0f); // Pequeno espaçamento da borda direita
            RightPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            Append(RightPanel);

            // Initialize Base Stats UI Elements
            float yPositionBaseStats = 10f;
            var baseStatNames = new List<string> { "Vida", "Mana", "Defesa", "Velocidade", "Dano", "Fome", "Sanidade" };
            var baseStatColors = new List<Color> { Color.Red, Color.Blue, Color.Gray, Color.Green, Color.Orange, Color.Brown, Color.Purple };
            var baseStatMaxValues = new List<float> { 100f, 100f, 50f, 2f, 2f, 100f, 100f };

            for (int i = 0; i < baseStatNames.Count; i++)
            {
                var (container, progressBar, text) = AddStatBar(BottomPanel, baseStatNames[i], 0f, baseStatMaxValues[i], baseStatColors[i], ref yPositionBaseStats);
                _baseStatContainers.Add(container);
                _baseStatProgressBars.Add(progressBar);
                _baseStatTexts.Add(text);
            }

            // Initialize Class Stats UI Elements
            float yPositionClassStats = 10f;
            float spacing = 30f;
            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                string className = classEntry.Key;
                var classInfo = classEntry.Value;

                var classPanel = new UIPanel();
                classPanel.Width.Set(250f, 0f);
                classPanel.Height.Set(25f, 0f);
                classPanel.Left.Set(10f, 0f);
                classPanel.Top.Set(yPositionClassStats, 0f);
                classPanel.BackgroundColor = new Color(45, 45, 45);
                RightPanel.Append(classPanel);
                _classPanels[className] = classPanel;

                var classText = new UIText("", 0.9f);
                classText.Left.Set(10f, 0f);
                classText.Top.Set(5f, 0f);
                classText.TextColor = TextColor;
                classPanel.Append(classText);
                _classTexts[className] = classText;

                yPositionClassStats += spacing;
            }
        }

        private void ShowBaseStats()
        {
            var player = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
            var stats = Systems.RPGCalculations.CalculateTotalStats(player);

            // Update Base Stats UI Elements
            var baseStatNames = new List<string> { "Vida", "Mana", "Defesa", "Velocidade", "Dano", "Fome", "Sanidade" };
            var baseStatValues = new List<float> { player.statLife, player.statMana, player.statDefense, player.moveSpeed, stats["damage"], player.CurrentHunger, player.CurrentSanity };
            var baseStatMaxValues = new List<float> { player.statLifeMax2, player.statManaMax2, 50f, 2f, 2f, player.MaxHunger, player.MaxSanity };

            for (int i = 0; i < baseStatNames.Count; i++)
            {
                float value = baseStatValues[i];
                float maxValue = baseStatMaxValues[i];
                float progress = Math.Min(value / maxValue, 1f);

                _baseStatProgressBars[i].Width.Set(_baseStatContainers[i].Width.Pixels * progress, 0f);
                _baseStatTexts[i].SetText($"{baseStatNames[i]}: {value:F1}/{maxValue:F1}");
            }
        }

        private void ShowClassStats()
        {
            var player = Main.LocalPlayer.GetModPlayer<RPGPlayer>();

            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                string className = classEntry.Key;
                var classInfo = classEntry.Value;
                float level = player.GetClassLevel(className);

                _classTexts[className].SetText($"{classInfo.Name} - Nível {level:F0}");

                // Tooltip com bônus
                if (_classPanels[className].IsMouseHovering)
                {
                    string tooltip = $"{classInfo.Description}\n";
                    foreach (var bonus in classInfo.StatBonuses)
                    {
                        tooltip += $"\n{bonus.Key}: +{bonus.Value * level:P1}";
                    }
                    Main.instance.MouseText(tooltip);
                }
            }
        }

        private void ShowAbilities()
        {
            // This method is currently not used in the new UI layout, but keeping it for now.
            var player = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
            float yPosition = 50f;
            float spacing = 30f;

            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                string className = classEntry.Key;
                var classInfo = classEntry.Value;
                float level = player.GetClassLevel(className);

                // Título da classe
                var classTitle = new UIText(classInfo.Name, 1f);
                classTitle.Left.Set(25f, 0f);
                classTitle.Top.Set(yPosition, 0f);
                classTitle.TextColor = HeaderColor;
                RightPanel.Append(classTitle);
                yPosition += 25f;

                // Listar habilidades
                foreach (var milestone in classInfo.Milestones)
                {
                    string abilityId = $"{className}_{milestone.Key}";
                    bool unlocked = player.HasUnlockedAbility(abilityId);
                    Color color = unlocked ? Color.LightGreen : Color.Gray;

                    var abilityText = new UIText($"Nível {milestone.Key}: {milestone.Value}", 0.8f);
                    abilityText.Left.Set(35f, 0f);
                    abilityText.Top.Set(yPosition, 0f);
                    abilityText.TextColor = color;
                    RightPanel.Append(abilityText);

                    yPosition += spacing;
                }

                yPosition += 10f;
            }
        }

        private (UIPanel container, UIPanel progressBar, UIText text) AddStatBar(UIPanel parentPanel, string name, float value, float maxValue, Color color, ref float yPosition)
        {
            // Container da barra
            var container = new UIPanel();
            container.Width.Set(parentPanel.Width.Pixels - 50f, 0f); // Adjusted width for parent panel
            container.Height.Set(25f, 0f);
            container.Left.Set(25f, 0f);
            container.Top.Set(yPosition, 0f);
            container.BackgroundColor = new Color(45, 45, 45);
            parentPanel.Append(container);

            // Barra de progresso
            var progressBar = new UIPanel();
            float progress = Math.Min(value / maxValue, 1f);
            progressBar.Width.Set(container.Width.Pixels - 20f * progress, 0f);
            progressBar.Height.Set(15f, 0f);
            progressBar.Left.Set(10f, 0f);
            progressBar.Top.Set(5f, 0f);
            progressBar.BackgroundColor = color * 0.7f;
            container.Append(progressBar);

            // Texto do status
            var text = new UIText($"{name}: {value:F1}/{maxValue:F1}", 0.8f);
            text.Left.Set(10f, 0f);
            text.Top.Set(5f, 0f);
            text.TextColor = TextColor;
            container.Append(text);

            yPosition += 30f;
            return (container, progressBar, text);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ShowBaseStats();
            ShowClassStats();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
    }
}