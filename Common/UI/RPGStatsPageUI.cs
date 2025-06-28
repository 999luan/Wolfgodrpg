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
using Wolfgodrpg.Common.Classes;

namespace Wolfgodrpg.Common.UI
{
    public class RPGStatsPageUI : UIElement
    {
        private UIElement _statsContainer;
        private bool _visible = false;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            _statsContainer = new UIElement();
            _statsContainer.Width.Set(0, 1f);
            _statsContainer.Height.Set(0, 1f);
            _statsContainer.SetPadding(0);
            Append(_statsContainer);
        }

        public void UpdateStats(RPGPlayer modPlayer)
        {
            _statsContainer.RemoveAllChildren(); // Clear previous stats

            var player = modPlayer.Player;
            var totalStats = RPGCalculations.CalculateTotalStats(modPlayer);
            float currentY = 0f; // Y position for each stat entry

            // Basic Player Stats
            AddStatEntry("Vida", $"{player.statLife} / {player.statLifeMax2}", ref currentY);
            AddStatEntry("Mana", $"{player.statMana} / {player.statManaMax2}", ref currentY);
            AddStatEntry("Defesa", $"{player.statDefense}", ref currentY);
            AddStatEntry("Velocidade", $"{player.moveSpeed:F2}x", ref currentY);

            currentY += 20f; // Add some spacing before bonuses

            AddStatEntry("--- Bônus Totais ---", "", ref currentY);

            foreach (var stat in totalStats)
            {
                string statName = RPGClassDefinitions.RandomStats.ContainsKey(stat.Key) ? RPGClassDefinitions.RandomStats[stat.Key].Name : stat.Key;
                string valueString = stat.Value < 1 && stat.Value > 0 ? $"+{stat.Value:P1}" : $"+{stat.Value:F2}";
                AddStatEntry(statName, valueString, ref currentY);
            }
        }

        private void AddStatEntry(string name, string value, ref float currentY)
        {
            StatEntry entry = new StatEntry(name, value);
            entry.Top.Set(currentY, 0f);
            entry.Left.Set(0, 0.05f); // Small left padding
            entry.Width.Set(0, 0.9f);
            entry.Height.Set(20f, 0f); // Fixed height for each entry
            _statsContainer.Append(entry);
            currentY += 25f; // Increment Y for the next entry
        }

        public new void Activate()
        {
            _visible = true;
        }

        public new void Deactivate()
        {
            _visible = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!_visible) return;
            base.DrawSelf(spriteBatch);
        }

        // Inner class for individual stat entries
        private class StatEntry : UIElement
        {
            private UIText _statNameText;
            private UIText _statValueText;

            public StatEntry(string name, string value)
            {
                Width.Set(0, 1f);
                Height.Set(20f, 0f);

                _statNameText = new UIText(name, 0.9f);
                _statNameText.HAlign = 0f; // Align left
                _statNameText.VAlign = 0.5f;
                Append(_statNameText);

                _statValueText = new UIText(value, 0.9f);
                _statValueText.HAlign = 1f; // Align right
                _statValueText.VAlign = 0.5f;
                Append(_statValueText);
            }
        }
    }
}
