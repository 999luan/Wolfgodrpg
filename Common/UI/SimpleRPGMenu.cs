using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.GlobalItems;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;

namespace Wolfgodrpg.Common.UI
{
    public enum MenuPage
    {
        Stats = 0,
        Classes = 1,
        Items = 2,
        Progress = 3
    }

    public class SimpleRPGMenu : UIState
    {
        private RPGPanel mainPanel;
        private UIText pageTitle;
        private RPGStatsPageUI _statsPageUI;
        private RPGClassesPageUI _classesPageUI;
        private RPGItemsPageUI _itemsPageUI;
        private RPGProgressPageUI _progressPageUI;
        private List<RPGTabButton> _tabButtons;
        private UIElement _tabButtonContainer;

        private bool isVisible = false;
        private MenuPage currentPage = MenuPage.Stats;

        

        public override void OnInitialize()
        {
            mainPanel = new RPGPanel();
            float panelWidth = Main.screenWidth * 0.8f; // 80% da largura da tela
            float panelHeight = Main.screenHeight - 56f - 20f; // Altura total da tela menos a hotbar e uma margem inferior
            mainPanel.Width.Set(panelWidth, 0f);
            mainPanel.Height.Set(panelHeight, 0f);
            mainPanel.HAlign = 0f; // Alinhado à esquerda
            mainPanel.VAlign = 0f; // Alinhado ao topo
            mainPanel.Top.Set(56f, 0f); // Começa 56 pixels abaixo do topo (abaixo da hotbar)
            mainPanel.Left.Set(50f, 0f); // Adicionar uma margem de 50 pixels à esquerda
            Append(mainPanel);

            pageTitle = new UIText("", 1.2f, true);
            pageTitle.HAlign = 0.5f;
            pageTitle.Top.Set(15f, 0f);
            mainPanel.Append(pageTitle);

            _statsPageUI = new RPGStatsPageUI();
            _statsPageUI.Width.Set(0, 0.9f);
            _statsPageUI.Height.Set(0, 0.8f);
            _statsPageUI.HAlign = 0.5f;
            _statsPageUI.VAlign = 0.5f;
            _statsPageUI.Top.Set(80f, 0f); // Adjusted top position
            mainPanel.Append(_statsPageUI);
            _statsPageUI.Deactivate(); // Ensure it's hidden initially

            _classesPageUI = new RPGClassesPageUI();
            _classesPageUI.Width.Set(0, 0.9f);
            _classesPageUI.Height.Set(0, 0.8f);
            _classesPageUI.HAlign = 0.5f;
            _classesPageUI.VAlign = 0.5f;
            _classesPageUI.Top.Set(80f, 0f); // Adjusted top position
            mainPanel.Append(_classesPageUI);
            _classesPageUI.Deactivate();

            _itemsPageUI = new RPGItemsPageUI();
            _itemsPageUI.Width.Set(0, 0.9f);
            _itemsPageUI.Height.Set(0, 0.8f);
            _itemsPageUI.HAlign = 0.5f;
            _itemsPageUI.VAlign = 0.5f;
            _itemsPageUI.Top.Set(80f, 0f); // Adjusted top position
            mainPanel.Append(_itemsPageUI);
            _itemsPageUI.Deactivate();

            _progressPageUI = new RPGProgressPageUI();
            _progressPageUI.Width.Set(0, 0.9f);
            _progressPageUI.Height.Set(0, 0.8f);
            _progressPageUI.HAlign = 0.5f;
            _progressPageUI.VAlign = 0.5f;
            _progressPageUI.Top.Set(80f, 0f); // Adjusted top position
            mainPanel.Append(_progressPageUI);
            _progressPageUI.Deactivate();

            _progressPageUI.Deactivate();

            _tabButtonContainer = new UIElement();
            _tabButtonContainer.Width.Set(0, 1f);
            _tabButtonContainer.Height.Set(30f, 0f); // Height for tab buttons
            _tabButtonContainer.Top.Set(50f, 0f); // Position below title
            mainPanel.Append(_tabButtonContainer);

            _tabButtons = new List<RPGTabButton>();

            // Create tab buttons
            RPGTabButton statsTab = new RPGTabButton("Stats", "Wolfgodrpg/Assets/UI/ButtonNext", "Wolfgodrpg/Assets/UI/ButtonPrevious");
            statsTab.Left.Set(10f, 0f);
            statsTab.OnClick += (evt, element) => SetPage(MenuPage.Stats);
            _tabButtonContainer.Append(statsTab);
            _tabButtons.Add(statsTab);

            RPGTabButton classesTab = new RPGTabButton("Classes", "Wolfgodrpg/Assets/UI/ButtonNext", "Wolfgodrpg/Assets/UI/ButtonPrevious");
            classesTab.Left.Set(statsTab.Width.Pixels + 20f, 0f); // Position next to statsTab
            classesTab.OnClick += (evt, element) => SetPage(MenuPage.Classes);
            _tabButtonContainer.Append(classesTab);
            _tabButtons.Add(classesTab);

            RPGTabButton itemsTab = new RPGTabButton("Items", "Wolfgodrpg/Assets/UI/ButtonNext", "Wolfgodrpg/Assets/UI/ButtonPrevious");
            itemsTab.Left.Set(statsTab.Width.Pixels + classesTab.Width.Pixels + 30f, 0f); // Position next to classesTab
            itemsTab.OnClick += (evt, element) => SetPage(MenuPage.Items);
            _tabButtonContainer.Append(itemsTab);
            _tabButtons.Add(itemsTab);

            RPGTabButton progressTab = new RPGTabButton("Progress", "Wolfgodrpg/Assets/UI/ButtonNext", "Wolfgodrpg/Assets/UI/ButtonPrevious");
            progressTab.Left.Set(statsTab.Width.Pixels + classesTab.Width.Pixels + itemsTab.Width.Pixels + 40f, 0f); // Position next to itemsTab
            progressTab.OnClick += (evt, element) => SetPage(MenuPage.Progress);
            _tabButtonContainer.Append(progressTab);
            _tabButtons.Add(progressTab);
            
            SetPage(currentPage); // Set initial page and select the correct tab
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!isVisible) return;

            UpdatePageContent();
        }

        private void UpdatePageContent()
        {
            var modPlayer = Main.LocalPlayer.GetModPlayer<RPGPlayer>();

            // Hide all page UIs first
            _statsPageUI.Deactivate();
            _classesPageUI.Deactivate();
            _itemsPageUI.Deactivate();
            _progressPageUI.Deactivate();

            switch (currentPage)
            {
                case MenuPage.Stats:
                    ShowStatsPage(modPlayer);
                    break;
                case MenuPage.Classes:
                    ShowClassesPage(modPlayer);
                    break;
                case MenuPage.Items:
                    ShowItemsPage();
                    break;
                case MenuPage.Progress:
                    ShowProgressPage(modPlayer);
                    break;
            }
        }

        private void ShowStatsPage(RPGPlayer modPlayer)
        {
            pageTitle.SetText("Atributos do Personagem");
            _statsPageUI.UpdateStats(modPlayer);
            _statsPageUI.Activate(); // Activate makes the element visible
        }

        private void ShowClassesPage(RPGPlayer modPlayer)
        {
            pageTitle.SetText("Classes e Habilidades");
            _classesPageUI.UpdateClasses(modPlayer);
            _classesPageUI.Activate();
        }

        private void ShowItemsPage()
        {
            pageTitle.SetText("Itens com Atributos");
            _itemsPageUI.UpdateItems();
            _itemsPageUI.Activate();
        }

        private void ShowProgressPage(RPGPlayer modPlayer)
        {
            pageTitle.SetText("Progresso do Jogo");
            _progressPageUI.UpdateProgress(modPlayer);
            _progressPageUI.Activate();
        }

        public void SetPage(MenuPage page)
        {
            currentPage = page;
            UpdatePageContent();

            // Update tab button selection
            foreach (var tabButton in _tabButtons)
            {
                tabButton.IsSelected = false;
            }

            switch (currentPage)
            {
                case MenuPage.Stats:
                    _tabButtons[0].IsSelected = true;
                    break;
                case MenuPage.Classes:
                    _tabButtons[1].IsSelected = true;
                    break;
                case MenuPage.Items:
                    _tabButtons[2].IsSelected = true;
                    break;
                case MenuPage.Progress:
                    _tabButtons[3].IsSelected = true;
                    break;
            }
        }

        public void Show() { isVisible = true; }
        public void Hide() { isVisible = false; }
        public void Toggle() { isVisible = !isVisible; }
        public bool IsVisible() => isVisible;

        
    }
}