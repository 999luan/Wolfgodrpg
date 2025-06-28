using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;

namespace Wolfgodrpg.Common.UI
{
    public class QuickStatsUI : UIState
    {
        private UIText _statsText;
        private bool _visible = false;

        public override void OnInitialize()
        {
            _statsText = new UIText("", 1f);
            _statsText.HAlign = 0.5f;
            _statsText.VAlign = 0.5f;
            _statsText.TextColor = Color.White;
            Append(_statsText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_visible) return;

            var player = Main.LocalPlayer;
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            var stats = RPGCalculations.CalculateTotalStats(modPlayer);

            string statsString = "";
            statsString += $"Vida: {player.statLife}/{player.statLifeMax2}\n";
            statsString += $"Mana: {player.statMana}/{player.statManaMax2}\n";
            statsString += $"Defesa: {player.statDefense}\n";
            statsString += $"Velocidade: {player.moveSpeed:F2}x\n";
            statsString += $"Dano: {stats["damage"]:F2}x\n";
            statsString += $"Fome: {modPlayer.CurrentHunger:F0}%\n";
            statsString += $"Sanidade: {modPlayer.CurrentSanity:F0}%\n";

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
