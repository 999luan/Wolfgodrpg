using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Wolfgodrpg.Common.UI.MasterUIElements;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.UI
{
    public class MasterUIState : UIState
    {
        private UIPanel mainPanel;
        private UIElement currentPanel;
        private AttributesPanel attributesPanel;
        private ClassesPanel classesPanel;
        private ProficienciesPanel proficienciesPanel;

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            mainPanel.Width.Set(800, 0);
            mainPanel.Height.Set(500, 0);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            mainPanel.BackgroundColor = new Color(28, 35, 41) * 0.9f;
            mainPanel.BorderColor = new Color(89, 116, 213);
            Append(mainPanel);

            var title = new UIText("Wolfgod RPG Hub", 1.2f, true);
            title.HAlign = 0.5f;
            title.Top.Set(15, 0);
            mainPanel.Append(title);

            var closeButton = new UITextPanel<string>("X", 0.8f, true);
            closeButton.Width.Set(30, 0);
            closeButton.Height.Set(30, 0);
            closeButton.HAlign = 1f;
            closeButton.Top.Set(10, 0);
            closeButton.Left.Set(-40, 0);
            closeButton.BackgroundColor = new Color(180, 40, 40);
            closeButton.OnLeftClick += (evt, elem) => ModContent.GetInstance<Systems.WolfgodUISystem>().HideMasterUI();
            mainPanel.Append(closeButton);

            // Tab buttons
            var attributesTabButton = new UITextPanel<string>("Attributes", 1f, false);
            attributesTabButton.Top.Set(50, 0);
            attributesTabButton.Left.Set(10, 0);
            attributesTabButton.OnLeftClick += (evt, elem) => SetActivePanel(attributesPanel);
            mainPanel.Append(attributesTabButton);

            var classesTabButton = new UITextPanel<string>("Classes", 1f, false);
            classesTabButton.Top.Set(95, 0);
            classesTabButton.Left.Set(10, 0);
            classesTabButton.OnLeftClick += (evt, elem) => SetActivePanel(classesPanel);
            mainPanel.Append(classesTabButton);

            var proficienciesTabButton = new UITextPanel<string>("Proficiencies", 1f, false);
            proficienciesTabButton.Top.Set(140, 0);
            proficienciesTabButton.Left.Set(10, 0);
            proficienciesTabButton.OnLeftClick += (evt, elem) => SetActivePanel(proficienciesPanel);
            mainPanel.Append(proficienciesTabButton);
        }

        public override void OnActivate()
        {
            base.OnActivate();
            var player = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
            if (player == null) return;

            attributesPanel = new AttributesPanel(player);
            attributesPanel.Top.Set(50, 0);
            attributesPanel.Left.Set(150, 0);
            attributesPanel.Width.Set(-160, 1f);
            attributesPanel.Height.Set(-60, 1f);

            classesPanel = new ClassesPanel(player);
            classesPanel.Top.Set(50, 0);
            classesPanel.Left.Set(150, 0);
            classesPanel.Width.Set(-160, 1f);
            classesPanel.Height.Set(-60, 1f);

            proficienciesPanel = new ProficienciesPanel(player);
            proficienciesPanel.Top.Set(50, 0);
            proficienciesPanel.Left.Set(150, 0);
            proficienciesPanel.Width.Set(-160, 1f);
            proficienciesPanel.Height.Set(-60, 1f);

            SetActivePanel(attributesPanel);
        }

        private void SetActivePanel(UIElement panel)
        {
            if (currentPanel != null)
            {
                mainPanel.RemoveChild(currentPanel);
            }
            currentPanel = panel;
            if (currentPanel != null)
            {
                mainPanel.Append(currentPanel);
            }
        }
    }
}
