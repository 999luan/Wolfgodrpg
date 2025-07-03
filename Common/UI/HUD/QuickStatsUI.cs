using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;
using Wolfgodrpg.Common.Utils;
using Wolfgodrpg.Common.UI.Base;

namespace Wolfgodrpg.Common.UI.HUD
{
    public class QuickStatsUI : UIState
    {
        private RPGPanel _mainPanel;
        private UIText _statsText;
        private bool _visible = false;

        public override void OnInitialize()
        {
            _mainPanel = new RPGPanel();
            _mainPanel.Width.Set(200f, 0f);
            _mainPanel.Height.Set(150f, 0f);
            _mainPanel.HAlign = 0.99f; // Align to right
            _mainPanel.VAlign = 0.01f; // Align to top
            Append(_mainPanel);

            _statsText = new UIText("", 1f);
            _statsText.HAlign = 0.5f;
            _statsText.VAlign = 0.5f;
            _statsText.TextColor = Color.White;
            _mainPanel.Append(_statsText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_visible) return;

            var modPlayer = RPGUtils.GetLocalRPGPlayer();
            if (modPlayer == null) return;

            var player = modPlayer.Player;
            var stats = RPGCalculations.CalculateTotalStats(modPlayer);

            string statsString = "";
            statsString += $"Vida: {player.statLife}/{player.statLifeMax2}{Environment.NewLine}";
            statsString += $"Mana: {player.statMana}/{player.statManaMax2}{Environment.NewLine}";
            statsString += $"Defesa: {player.statDefense}{Environment.NewLine}";
            statsString += $"Velocidade: {player.moveSpeed:F2}x{Environment.NewLine}";
            statsString += $"Dano: {stats["damage"]:F2}x{Environment.NewLine}";
            statsString += $"Fome: {modPlayer.CurrentHunger:F0}%{Environment.NewLine}";
            statsString += $"Sanidade: {modPlayer.CurrentSanity:F0}%{Environment.NewLine}";

            _statsText.SetText(statsString);
        }

        public void ToggleVisibility()
        {
            _visible = !_visible;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!_visible) return;
            base.DrawSelf(spriteBatch);
        }
    }
}