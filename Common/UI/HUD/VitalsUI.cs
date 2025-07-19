using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;

namespace Wolfgodrpg.Common.UI.HUD
{
    /// <summary>
    /// UI simples para exibir vitais do jogador na tela
    /// </summary>
    public class VitalsUI : UIState
    {
        private const float BAR_WIDTH = 200f;
        private const float BAR_HEIGHT = 20f;
        private const float MARGIN = 5f;
        
        public override void OnInitialize()
        {
            // Posicionar no canto inferior esquerdo
            Left.Set(20f, 0f);
            Top.Set(Main.screenHeight - (BAR_HEIGHT * 3 + MARGIN * 2) - 20f, 0f);
            Width.Set(BAR_WIDTH, 0f);
            Height.Set(BAR_HEIGHT * 3 + MARGIN * 2, 0f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.gameMenu || Main.LocalPlayer == null) return;

            var player = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
            if (player?.Vitals == null) return;

            // Posição base
            Vector2 position = GetDimensions().Position();
            
            // Desenhar fundo
            DrawBarBackground(spriteBatch, position, "Hunger");
            DrawBarBackground(spriteBatch, position + new Vector2(0, BAR_HEIGHT + MARGIN), "Sanity");
            DrawBarBackground(spriteBatch, position + new Vector2(0, (BAR_HEIGHT + MARGIN) * 2), "Stamina");
            
            // Desenhar barras de progresso
            DrawProgressBar(spriteBatch, position, player.CurrentHunger, VitalsSystem.MAX_HUNGER, Color.Orange);
            DrawProgressBar(spriteBatch, position + new Vector2(0, BAR_HEIGHT + MARGIN), player.CurrentSanity, VitalsSystem.MAX_SANITY, Color.Purple);
            DrawProgressBar(spriteBatch, position + new Vector2(0, (BAR_HEIGHT + MARGIN) * 2), player.CurrentStamina, VitalsSystem.MAX_STAMINA, Color.Yellow);
            
            // Desenhar texto
            DrawBarText(spriteBatch, position, "Hunger", player.CurrentHunger, VitalsSystem.MAX_HUNGER);
            DrawBarText(spriteBatch, position + new Vector2(0, BAR_HEIGHT + MARGIN), "Sanity", player.CurrentSanity, VitalsSystem.MAX_SANITY);
            DrawBarText(spriteBatch, position + new Vector2(0, (BAR_HEIGHT + MARGIN) * 2), "Stamina", player.CurrentStamina, VitalsSystem.MAX_STAMINA);
        }

        private void DrawBarBackground(SpriteBatch spriteBatch, Vector2 position, string label)
        {
            // Fundo escuro
            Rectangle backgroundRect = new Rectangle((int)position.X, (int)position.Y, (int)BAR_WIDTH, (int)BAR_HEIGHT);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, backgroundRect, Color.Black * 0.7f);
            
            // Borda
            Rectangle borderRect = new Rectangle((int)position.X - 1, (int)position.Y - 1, (int)BAR_WIDTH + 2, (int)BAR_HEIGHT + 2);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, borderRect, Color.Gray);
        }

        private void DrawProgressBar(SpriteBatch spriteBatch, Vector2 position, float current, float max, Color color)
        {
            float progress = current / max;
            float barWidth = BAR_WIDTH * progress;
            
            if (barWidth > 0)
            {
                Rectangle progressRect = new Rectangle((int)position.X, (int)position.Y, (int)barWidth, (int)BAR_HEIGHT);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, progressRect, color);
            }
        }

        private void DrawBarText(SpriteBatch spriteBatch, Vector2 position, string label, float current, float max)
        {
            string text = $"{label}: {(int)current}/{(int)max}";
            Vector2 textPosition = position + new Vector2(5f, 2f);
            
            // Sombra
            Terraria.Utils.DrawBorderString(spriteBatch, text, textPosition + new Vector2(1f, 1f), Color.Black);
            // Texto principal
            Terraria.Utils.DrawBorderString(spriteBatch, text, textPosition, Color.White);
        }
    }
} 