using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ModLoader.IO;
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
        private UIElement pageContainer; // Container for the pages

        private RPGStatsPageUI _statsPageUI;
        private RPGClassesPageUI _classesPageUI;
        private RPGItemsPageUI _itemsPageUI;
        private RPGProgressPageUI _progressPageUI;
        
        private List<RPGTabButton> _tabButtons;
        private MenuPage _currentPage = MenuPage.Stats;

        public override void OnInitialize()
        {
            DebugLog.UI("OnInitialize", "Inicializando SimpleRPGMenu");
            
            if (Wolfgodrpg.Instance != null)
                Wolfgodrpg.Instance.Logger.Info("[SimpleRPGMenu] OnInitialize called.");
            
            mainPanel = new RPGPanel();
            mainPanel.Width.Set(0, 0.8f);
            mainPanel.Height.Set(0, 0.8f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            mainPanel.SetPadding(12f);
            Append(mainPanel);

            pageTitle = new UIText("", 1.2f, true);
            pageTitle.HAlign = 0.5f;
            pageTitle.Top.Set(10, 0f);
            mainPanel.Append(pageTitle);

            var tabButtonContainer = new UIElement();
            tabButtonContainer.Width.Set(0, 1f);
            tabButtonContainer.Height.Set(30f, 0f);
            tabButtonContainer.Top.Set(40f, 0f);
            mainPanel.Append(tabButtonContainer);
            
            pageContainer = new UIElement();
            pageContainer.Width.Set(0, 1f);
            pageContainer.Height.Set(-80f, 1f); // Fill remaining space
            pageContainer.Top.Set(80f, 0f);
            mainPanel.Append(pageContainer);

            _statsPageUI = new RPGStatsPageUI();
            _statsPageUI.Activate();
            _classesPageUI = new RPGClassesPageUI();
            _classesPageUI.Activate();
            _itemsPageUI = new RPGItemsPageUI();
            _itemsPageUI.Activate();
            _progressPageUI = new RPGProgressPageUI();
            _progressPageUI.Activate();
            
            _tabButtons = new List<RPGTabButton>();
            int tabCount = Enum.GetValues(typeof(MenuPage)).Length;
            float buttonWidth = 120f;
            float totalWidth = tabCount * buttonWidth + (tabCount - 1) * 10f;
            float startX = (mainPanel.Width.Pixels - totalWidth) / 2f;
            
            DebugLog.UI("OnInitialize", $"Criando {tabCount} botões de aba com largura total {totalWidth:F1}");
            
            for (int i = 0; i < tabCount; i++)
            {
                var page = (MenuPage)i;
                var button = new RPGTabButton(GetPageTitle(page), () => SetPage(page));
                button.Left.Set(startX + i * (buttonWidth + 10f), 0f);
                button.Width.Set(buttonWidth, 0f);
                button.Height.Set(30f, 0f);
                tabButtonContainer.Append(button);
                _tabButtons.Add(button);
                
                DebugLog.UI("OnInitialize", $"Botão de aba '{GetPageTitle(page)}' criado na posição {startX + i * (buttonWidth + 10f):F1}");
            }

            // Set initial page
            SetPage(MenuPage.Stats, true);
            
            DebugLog.UI("OnInitialize", "SimpleRPGMenu inicializado com sucesso");
        }

        public override void OnActivate()
        {
            DebugLog.UI("OnActivate", "Menu RPG ativado");
            base.OnActivate();
        }

        public override void OnDeactivate()
        {
            DebugLog.UI("OnDeactivate", "Menu RPG desativado");
            base.OnDeactivate();
        }

        private string GetPageTitle(MenuPage page)
        {
            return page switch
            {
                MenuPage.Stats => "Status",
                MenuPage.Classes => "Classes",
                MenuPage.Items => "Itens",
                MenuPage.Progress => "Progresso",
                _ => "Desconhecido"
            };
        }

        private void SetPage(MenuPage page, bool firstTime = false)
        {
            DebugLog.UI("SetPage", $"Trocando para aba: {page} (firstTime: {firstTime})");
            
            _currentPage = page;
            pageTitle.SetText(GetPageTitle(page));

            // Remove current page content
            pageContainer.RemoveAllChildren();

            // Add new page content
            switch (page)
            {
                case MenuPage.Stats:
                    pageContainer.Append(_statsPageUI);
                    DebugLog.UI("SetPage", "Aba Status carregada");
                    break;
                case MenuPage.Classes:
                    pageContainer.Append(_classesPageUI);
                    DebugLog.UI("SetPage", "Aba Classes carregada");
                    break;
                case MenuPage.Items:
                    pageContainer.Append(_itemsPageUI);
                    DebugLog.UI("SetPage", "Aba Itens carregada");
                    break;
                case MenuPage.Progress:
                    pageContainer.Append(_progressPageUI);
                    DebugLog.UI("SetPage", "Aba Progresso carregada");
                    break;
            }

            // Update tab button states
            for (int i = 0; i < _tabButtons.Count; i++)
            {
                _tabButtons[i].SetSelected((MenuPage)i == page);
            }

            if (!firstTime)
            {
                if (Main.LocalPlayer != null && Main.LocalPlayer.active)
                {
                    try
                    {
                        var modPlayer = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
                        DebugLog.UI("SetPage", $"Atualizando conteúdo da aba {page} para jogador '{Main.LocalPlayer.name}'");
                        
                        _statsPageUI.UpdateStats(modPlayer);
                        _classesPageUI.UpdateClasses(modPlayer);
                        _itemsPageUI.UpdateItems();
                        _progressPageUI.UpdateProgress(modPlayer);
                        
                        DebugLog.UI("SetPage", "Conteúdo da aba atualizado com sucesso");
                    }
                    catch (Exception ex)
                    {
                        DebugLog.Error("UI", "SetPage", $"Erro ao atualizar conteúdo da aba {page}", ex);
                    }
                }
                else
                {
                    DebugLog.UI("SetPage", "Jogador não disponível para atualizar conteúdo da aba");
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // Não atualize as páginas inteiras aqui para evitar reconstrução excessiva da UI.
            // Se precisar atualizar apenas valores dinâmicos, crie métodos específicos para isso.
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
    }
}