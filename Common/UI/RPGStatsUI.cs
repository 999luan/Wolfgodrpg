using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System; 
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Systems;

namespace Wolfgodrpg.Common.UI
{
    public class RPGStatsUI : UIState
    {
        private UIPanel mainPanel;
        private bool isVisible = false;

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            mainPanel.Width.Set(300f, 0f);
            mainPanel.Height.Set(400f, 0f);
            mainPanel.HAlign = 1f;
            mainPanel.VAlign = 0.5f;
            mainPanel.Left.Set(-50f, 0f);
            mainPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            Append(mainPanel);
        }

        public void ToggleVisibility()
        {
            isVisible = !isVisible;
            if (isVisible)
            {
                UpdateStats();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!isVisible) return;

            // Atualiza as estatísticas em tempo real
            UpdateStats();
        }

        private void UpdateStats()
        {
            mainPanel.RemoveAllChildren(); // Limpa o painel para redesenhar

            var player = Main.LocalPlayer;
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            var totalStats = RPGCalculations.CalculateTotalStats(modPlayer);

            float yPos = 10f;

            // Título
            var title = new UIText("Estatísticas Gerais", 1.1f);
            title.HAlign = 0.5f;
            title.Top.Set(yPos, 0f);
            mainPanel.Append(title);
            yPos += 30f;

            // Status Básicos do Terraria
            AddStatLine($"Vida: {player.statLife} / {player.statLifeMax2}", ref yPos);
            AddStatLine($"Mana: {player.statMana} / {player.statManaMax2}", ref yPos);
            AddStatLine($"Defesa: {player.statDefense}", ref yPos);
            yPos += 10f; // Espaçamento

            // Título de Bônus
            var bonusTitle = new UIText("Bônus Totais (Classes + Itens)", 1f);
            bonusTitle.HAlign = 0.5f;
            bonusTitle.Top.Set(yPos, 0f);
            mainPanel.Append(bonusTitle);
            yPos += 30f;

            // Lista de todos os bônus calculados
            foreach (var stat in totalStats)
            {
                string statName = RPGClassDefinitions.RandomStats.ContainsKey(stat.Key) ? RPGClassDefinitions.RandomStats[stat.Key].Name : stat.Key;
                string valueString = stat.Value < 1 && stat.Value > 0 ? $"{stat.Value:P1}" : $"{stat.Value:F2}";
                AddStatLine($"{statName}: +{valueString}", ref yPos, Color.LightGreen);
            }
        }

        private void AddStatLine(string text, ref float yPos, Color? color = null)
        {
            var statText = new UIText(text, 0.9f);
            statText.Left.Set(15f, 0f);
            statText.Top.Set(yPos, 0f);
            if (color.HasValue) statText.TextColor = color.Value;
            mainPanel.Append(statText);
            yPos += 20f;
        }
    }
}
