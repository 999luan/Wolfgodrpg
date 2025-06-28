using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.UI
{
    public class RPGStatsUI : UIState
    {
        private UIPanel mainPanel;
        private bool isVisible = false;

        // Elementos da UI para os Vitals
        private UIProgressBar hungerBar;
        private UIProgressBar sanityBar;
        private UIProgressBar staminaBar;

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            mainPanel.Width.Set(220f, 0f);
            mainPanel.Height.Set(120f, 0f);
            mainPanel.HAlign = 0.01f; // Alinhar à esquerda
            mainPanel.VAlign = 0.9f;  // Alinhar na parte inferior
            mainPanel.BackgroundColor = new Color(44, 57, 101) * 0.8f;
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

            var rpgPlayer = Main.LocalPlayer.GetModPlayer<RPGPlayer>();

            // Atualizar os valores das barras
            hungerBar.SetProgress(rpgPlayer.CurrentHunger / rpgPlayer.MaxHunger);
            sanityBar.SetProgress(rpgPlayer.CurrentSanity / rpgPlayer.MaxSanity);
            staminaBar.SetProgress(rpgPlayer.CurrentStamina / rpgPlayer.MaxStamina);
        }
    }

    // Classe auxiliar para a barra de progresso
    public class UIProgressBar : UIPanel
    {
        private UIPanel progressBar;
        private UIText text;
        private Color color;

        public UIProgressBar(Color barColor, string label)
        {
            BackgroundColor = new Color(20, 20, 20) * 0.7f;
            BorderColor = Color.Black;
            color = barColor;

            progressBar = new UIPanel();
            progressBar.Height.Set(0, 1f);
            progressBar.BackgroundColor = color;
            progressBar.BorderColor = Color.Transparent;
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

