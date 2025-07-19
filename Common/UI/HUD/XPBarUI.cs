using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Utils;
using Wolfgodrpg.Common.UI.Base;

namespace Wolfgodrpg.Common.UI.HUD
{
    public class XPBarUI : UIState
    {
        private RPGPanel mainPanel;
        private UIText levelText;
        private XPProgressBar xpBar;
        private UIText xpText;
        private bool isVisible = true;

        public override void OnInitialize()
        {
            mainPanel = new RPGPanel();
            mainPanel.Width.Set(250f, 0f);
            mainPanel.Height.Set(80f, 0f);
            mainPanel.HAlign = 0.5f; // Centralizar horizontalmente
            mainPanel.VAlign = 0.95f; // Alinhar na parte inferior
            Append(mainPanel);

            // Texto do nível
            levelText = new UIText("Level: 1", 1.2f, true);
            levelText.HAlign = 0.5f;
            levelText.Top.Set(5f, 0f);
            levelText.TextColor = Color.Gold;
            mainPanel.Append(levelText);

            // Barra de fundo (cinza escuro)
            var backgroundBar = new UIElement();
            backgroundBar.Width.Set(220f, 0f);
            backgroundBar.Height.Set(15f, 0f);
            backgroundBar.HAlign = 0.5f;
            backgroundBar.Top.Set(35f, 0f);
            mainPanel.Append(backgroundBar);

            // Barra de XP (azul)
            xpBar = new XPProgressBar(Color.Cyan);
            xpBar.Width.Set(220f, 0f);
            xpBar.Height.Set(15f, 0f);
            xpBar.HAlign = 0.5f;
            xpBar.Top.Set(35f, 0f);
            mainPanel.Append(xpBar);

            // Texto de XP
            xpText = new UIText("XP: 0/100", 0.9f);
            xpText.HAlign = 0.5f;
            xpText.Top.Set(55f, 0f);
            xpText.TextColor = Color.White;
            mainPanel.Append(xpText);
        }

        public void ToggleVisibility() => isVisible = !isVisible;
        public bool IsVisible() => isVisible;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!isVisible) return;

            var rpgPlayer = RPGUtils.GetLocalRPGPlayer();
            if (rpgPlayer == null) return;

            // Atualizar nível
            levelText.SetText($"Level: {rpgPlayer.PlayerLevel}");

            // Calcular XP atual e necessário
            float currentXP = rpgPlayer.PlayerExperience;
            float nextLevelXP = RPGPlayer.GetPlayerExperienceForLevel(rpgPlayer.PlayerLevel + 1);
            float currentLevelXP = RPGPlayer.GetPlayerExperienceForLevel(rpgPlayer.PlayerLevel);
            float xpInCurrentLevel = currentXP - currentLevelXP;
            float xpNeededForNextLevel = nextLevelXP - currentLevelXP;
            float xpProgress = xpNeededForNextLevel > 0 ? xpInCurrentLevel / xpNeededForNextLevel : 0f;

            // Atualizar barra de XP
            xpBar.SetProgress(xpProgress);

            // Atualizar texto de XP
            xpText.SetText($"XP: {xpInCurrentLevel:F0}/{xpNeededForNextLevel:F0}");
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!isVisible) return;
            base.DrawSelf(spriteBatch);
        }
    }

    // Classe auxiliar para barra de progresso de XP
    public class XPProgressBar : UIElement
    {
        private Color barColor;
        private float progress = 0f;

        public XPProgressBar(Color color)
        {
            barColor = color;
        }

        public void SetProgress(float newProgress)
        {
            progress = MathHelper.Clamp(newProgress, 0f, 1f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var dimensions = GetInnerDimensions();
            var rect = dimensions.ToRectangle();

            // Desenhar fundo cinza escuro
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.DarkGray);
            
            // Desenhar barra de progresso
            if (progress > 0f)
            {
                var progressRect = new Rectangle(rect.X, rect.Y, (int)(rect.Width * progress), rect.Height);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, progressRect, barColor);
            }

            // Desenhar borda
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, 1), Color.White);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, 1, rect.Height), Color.White);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - 1, rect.Y, 1, rect.Height), Color.White);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - 1, rect.Width, 1), Color.White);
        }
    }
} 