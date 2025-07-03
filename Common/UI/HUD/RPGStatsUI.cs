using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Utils;
using Wolfgodrpg.Common.UI.Base;

namespace Wolfgodrpg.Common.UI.HUD
{
    public class RPGStatsUI : UIState
    {
        private RPGPanel mainPanel;
        private bool isVisible = false;

        // Elementos da UI para os Vitals
        private UIProgressBar hungerBar;
        private UIProgressBar sanityBar;
        private UIProgressBar staminaBar;

        public override void OnInitialize()
        {
            mainPanel = new RPGPanel();
            mainPanel.Width.Set(220f, 0f);
            mainPanel.Height.Set(120f, 0f);
            mainPanel.HAlign = 0.01f; // Alinhar à esquerda
            mainPanel.VAlign = 0.9f;  // Alinhar na parte inferior
            Append(mainPanel);

            // Inicializar as barras de status
            hungerBar = new UIProgressBar(Color.Orange, "Fome");
            hungerBar.Width.Set(0, 0.9f);
            hungerBar.Height.Set(20f, 0f);
            hungerBar.HAlign = 0.5f;
            hungerBar.Top.Set(10f, 0f);
            mainPanel.Append(hungerBar);

            sanityBar = new UIProgressBar(Color.Purple, "Sanidade");
            sanityBar.Width.Set(0, 0.9f);
            sanityBar.Height.Set(20f, 0f);
            sanityBar.HAlign = 0.5f;
            sanityBar.Top.Set(40f, 0f);
            mainPanel.Append(sanityBar);

            staminaBar = new UIProgressBar(Color.Yellow, "Stamina");
            staminaBar.Width.Set(0, 0.9f);
            staminaBar.Height.Set(20f, 0f);
            staminaBar.HAlign = 0.5f;
            staminaBar.Top.Set(70f, 0f);
            mainPanel.Append(staminaBar);
        }

        public void ToggleVisibility() => isVisible = !isVisible;
        public bool IsVisible() => isVisible;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!isVisible) return;

            var rpgPlayer = RPGUtils.GetLocalRPGPlayer();
            if (rpgPlayer == null) return;

            // Atualizar os valores das barras
            hungerBar.SetProgress(rpgPlayer.CurrentHunger / rpgPlayer.MaxHunger);
            sanityBar.SetProgress(rpgPlayer.CurrentSanity / rpgPlayer.MaxSanity);
            staminaBar.SetProgress(rpgPlayer.CurrentStamina / rpgPlayer.MaxStamina);
        }
    }

    // Classe auxiliar para a barra de progresso
    public class UIProgressBar : UIElement
    {
        private UIElement progressBar;
        private UIText text;
        private Color color;

        public UIProgressBar(Color barColor, string label)
        {
            color = barColor;

            progressBar = new UIElement();
            progressBar.Height.Set(0, 1f);
            Append(progressBar);

            text = new UIText(label, 0.8f);
            text.HAlign = 0.5f;
            text.VAlign = 0.5f;
            Append(text);
        }

        public void SetProgress(float progress)
        {
            progress = MathHelper.Clamp(progress, 0f, 1f);
            progressBar.Width.Set(0, progress);
        }
    }
}