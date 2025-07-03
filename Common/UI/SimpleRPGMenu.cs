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
        private UIPanel _mainPanel;
        private UIText _pageTitle;
        private UIElement _pageContainer;
        private List<UIElement> _pages;
        private List<UITextPanel<string>> _tabButtons;
        private MenuPage _currentPage = MenuPage.Stats;

        private RPGStatsPageUI _statsPageUI;
        private RPGClassesPageUI _classesPageUI;
        private RPGItemsPageUI _itemsPageUI;
        private RPGProgressPageUI _progressPageUI;

        public override void OnInitialize()
        {
            DebugLog.UI("OnInitialize", "Inicializando SimpleRPGMenu");
            
            if (Wolfgodrpg.Instance != null)
                Wolfgodrpg.Instance.Logger.Info("[SimpleRPGMenu] OnInitialize called.");
            
            Width.Set(0, 0.8f);
            Height.Set(0, 0.8f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            _mainPanel = new UIPanel();
            _mainPanel.Width.Set(0, 1f);
            _mainPanel.Height.Set(0, 1f);
            _mainPanel.SetPadding(12f);
            Append(_mainPanel);

            _pageTitle = new UIText("", 1.2f, true);
            _pageTitle.HAlign = 0.5f;
            _pageTitle.Top.Set(10, 0f);
            _mainPanel.Append(_pageTitle);

            var tabButtonContainer = new UIElement();
            tabButtonContainer.Width.Set(0, 1f);
            tabButtonContainer.Height.Set(30f, 0f);
            tabButtonContainer.Top.Set(40f, 0f);
            _mainPanel.Append(tabButtonContainer);
            
            _pageContainer = new UIElement();
            _pageContainer.Width.Set(0, 1f);
            _pageContainer.Height.Set(-80f, 1f);
            _pageContainer.Top.Set(80f, 0f);
            _mainPanel.Append(_pageContainer);

            _statsPageUI = new RPGStatsPageUI();
            _statsPageUI.Activate();
            _classesPageUI = new RPGClassesPageUI();
            _classesPageUI.Activate();
            _itemsPageUI = new RPGItemsPageUI();
            _itemsPageUI.Activate();
            _progressPageUI = new RPGProgressPageUI();
            _progressPageUI.Activate();
            
            _pages = new List<UIElement> { _statsPageUI, _classesPageUI, _itemsPageUI, _progressPageUI };
            _tabButtons = new List<UITextPanel<string>>();

            string[] tabNames = { "Status", "Classes", "Itens", "Progresso" };
            float buttonWidth = 120f;
            float spacing = 10f;
            for (int i = 0; i < tabNames.Length; i++)
            {
                int pageIndex = i;
                var btn = new UITextPanel<string>(tabNames[i], 0.9f, true);
                btn.Width.Set(buttonWidth, 0f);
                btn.Height.Set(30f, 0f);
                btn.Left.Set(i * (buttonWidth + spacing), 0f);
                btn.OnLeftClick += (evt, elm) => SetPage((MenuPage)pageIndex);
                tabButtonContainer.Append(btn);
                _tabButtons.Add(btn);
                
                DebugLog.UI("OnInitialize", $"Botão de aba '{tabNames[i]}' criado na posição {i * (buttonWidth + spacing):F1}");
            }

            SetPage(MenuPage.Stats);
            
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

        private void SetPage(MenuPage page)
        {
            // Não atualize se já estiver na mesma página (otimização ExampleMod)
            if (_currentPage == page) return;

            _currentPage = page;
            _pageTitle.SetText(_tabButtons[(int)page].Text);
            _pageContainer.RemoveAllChildren();
            _pageContainer.Append(_pages[(int)page]);
            UpdateTabButtonStates();

            // Verificar se o jogador está disponível antes de tentar acessar
            if (Main.LocalPlayer == null || !Main.LocalPlayer.active) 
            {
                DebugLog.UI("SetPage", "Jogador não disponível, pulando atualização");
                return;
            }

            var modPlayer = Main.LocalPlayer.GetModPlayer<RPGPlayer>();
            if (modPlayer == null || modPlayer.Player == null) 
            {
                DebugLog.UI("SetPage", "ModPlayer não disponível, pulando atualização");
                return;
            }

            // Verificar se as classes foram inicializadas
            if (modPlayer.ClassLevels == null || modPlayer.ClassLevels.Count == 0)
            {
                DebugLog.UI("SetPage", "Classes não inicializadas, pulando atualização");
                return;
            }

            // Atualização otimizada: só quando trocar de aba
            switch (_currentPage)
            {
                case MenuPage.Stats:
                    _statsPageUI.UpdateStats(modPlayer);
                    DebugLog.UI("SetPage", "Aba Stats atualizada");
                    break;
                case MenuPage.Classes:
                    _classesPageUI.UpdateClasses(modPlayer);
                    DebugLog.UI("SetPage", "Aba Classes atualizada");
                    break;
                case MenuPage.Items:
                    _itemsPageUI.UpdateItems();
                    DebugLog.UI("SetPage", "Aba Items atualizada");
                    break;
                case MenuPage.Progress:
                    _progressPageUI.UpdateProgress(modPlayer);
                    DebugLog.UI("SetPage", "Aba Progress atualizada");
                    break;
            }
        }

        private void UpdateTabButtonStates()
        {
            for (int i = 0; i < _tabButtons.Count; i++)
            {
                _tabButtons[i].BackgroundColor = (i == (int)_currentPage) ? new Color(70, 120, 200) : new Color(63, 82, 151) * 0.7f;
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

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt); // Propaga o evento para a UI vanilla/jogo
            // Não consome o evento, deixa passar para o jogo
        }
    }
}