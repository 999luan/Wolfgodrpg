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
using Wolfgodrpg.Common.Utils;
using Wolfgodrpg.Common.UI.Base;
using Wolfgodrpg.Common.UI.Menus;

namespace Wolfgodrpg.Common.UI
{
    public enum MenuPage
    {
        Stats = 0,
        Classes = 1,
        Skills = 2,
        Progress = 3,
        Proficiencies = 4
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
        private RPGSkillsPageUI _skillsPageUI;
        private RPGProgressPageUI _progressPageUI;
        private RPGProficienciesPageUI _proficienciesPageUI;

        // Sistema de atualização automática
        private RPGPlayer _lastPlayerData;
        private bool _needsUpdate = true;
        private int _updateTimer = 0;
        private const int UPDATE_INTERVAL = 30; // Atualizar a cada 30 frames (0.5 segundos)

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
            _skillsPageUI = new RPGSkillsPageUI();
            _skillsPageUI.Activate();
            _progressPageUI = new RPGProgressPageUI();
            _progressPageUI.Activate();
            _proficienciesPageUI = new RPGProficienciesPageUI();
            _proficienciesPageUI.Activate();
            
            _pages = new List<UIElement> { _statsPageUI, _classesPageUI, _skillsPageUI, _progressPageUI, _proficienciesPageUI };
            _tabButtons = new List<UITextPanel<string>>();

            string[] tabNames = { "Stats", "Classes", "Skills", "Progress", "Proficiencies" };
            float buttonWidth = 120f;
            float spacing = 0f;
            int tabCount = tabNames.Length;
            float totalWidth = tabCount * buttonWidth;
            float containerWidth = 0.95f; // 95% da largura do container
            float startX = (1f - containerWidth) / 2f;
            float slotWidth = containerWidth / tabCount;
            for (int i = 0; i < tabNames.Length; i++)
            {
                int pageIndex = i;
                var btn = new UITextPanel<string>(tabNames[i], 0.9f, true);
                btn.Width.Set(buttonWidth, 0f);
                btn.Height.Set(30f, 0f);
                btn.HAlign = startX + slotWidth * i + slotWidth / 2f;
                btn.Top.Set(0f, 0f);
                btn.OnLeftClick += (evt, elm) => SetPage((MenuPage)pageIndex);
                tabButtonContainer.Append(btn);
                _tabButtons.Add(btn);
                DebugLog.UI("OnInitialize", $"Tab button '{tabNames[i]}' created at slot {i}");
            }

            SetPage(MenuPage.Stats);
            
            DebugLog.UI("OnInitialize", "SimpleRPGMenu initialized successfully");
        }

        public override void OnActivate()
        {
            DebugLog.UI("OnActivate", "RPG menu activated");
            _needsUpdate = true; // Force update when opening
            base.OnActivate();
        }

        public override void OnDeactivate()
        {
            DebugLog.UI("OnDeactivate", "RPG menu deactivated");
            base.OnDeactivate();
        }

        private void SetPage(MenuPage page)
        {
            // Do not update if already on the same page (optimization ExampleMod)
            if (_currentPage == page) return;

            _currentPage = page;
            _pageTitle.SetText(_tabButtons[(int)page].Text);
            _pageContainer.RemoveAllChildren();
            _pageContainer.Append(_pages[(int)page]);
            UpdateTabButtonStates();

            // Force update when changing pages
            _needsUpdate = true;
            UpdateCurrentPage();
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
            
            // Sistema de atualização automática
            _updateTimer++;
            if (_updateTimer >= UPDATE_INTERVAL)
            {
                _updateTimer = 0;
                CheckForUpdates();
            }
        }

        /// <summary>
        /// Checks if there are changes in player data that require UI updates.
        /// </summary>
        private void CheckForUpdates()
        {
            var modPlayer = RPGUtils.GetLocalRPGPlayer();
            if (modPlayer == null) return;

            // Check if there were significant changes
            if (_lastPlayerData == null || HasSignificantChanges(modPlayer))
            {
                _needsUpdate = true;
                _lastPlayerData = ClonePlayerData(modPlayer);
            }

            // Update if necessary
            if (_needsUpdate)
            {
                UpdateCurrentPage();
                _needsUpdate = false;
            }
        }

        /// <summary>
        /// Checks if there were significant changes in player data.
        /// </summary>
        private bool HasSignificantChanges(RPGPlayer currentPlayer)
        {
            if (_lastPlayerData == null) return true;

            // Verificar mudanças nos níveis das classes
            foreach (var kvp in currentPlayer.ClassLevels)
            {
                if (!_lastPlayerData.ClassLevels.ContainsKey(kvp.Key) || 
                    Math.Abs(_lastPlayerData.ClassLevels[kvp.Key] - kvp.Value) > 0.01f)
                {
                    return true;
                }
            }

            // Verificar mudanças no XP das classes
            foreach (var kvp in currentPlayer.ClassExperience)
            {
                if (!_lastPlayerData.ClassExperience.ContainsKey(kvp.Key) || 
                    Math.Abs(_lastPlayerData.ClassExperience[kvp.Key] - kvp.Value) > 1f)
                {
                    return true;
                }
            }

            // Verificar mudanças nos vitais
            if (Math.Abs(_lastPlayerData.CurrentHunger - currentPlayer.CurrentHunger) > 1f ||
                Math.Abs(_lastPlayerData.CurrentSanity - currentPlayer.CurrentSanity) > 1f ||
                Math.Abs(_lastPlayerData.CurrentStamina - currentPlayer.CurrentStamina) > 1f)
            {
                return true;
            }

            // Verificar mudanças nos níveis de proficiência de armadura
            foreach (var kvp in currentPlayer.ArmorProficiencyLevels)
            {
                if (!_lastPlayerData.ArmorProficiencyLevels.ContainsKey(kvp.Key) ||
                    _lastPlayerData.ArmorProficiencyLevels[kvp.Key] != kvp.Value)
                {
                    return true;
                }
            }

            // Verificar mudanças no XP de proficiência de armadura
            foreach (var kvp in currentPlayer.ArmorProficiencyExperience)
            {
                if (!_lastPlayerData.ArmorProficiencyExperience.ContainsKey(kvp.Key) ||
                    Math.Abs(_lastPlayerData.ArmorProficiencyExperience[kvp.Key] - kvp.Value) > 1f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clona os dados do jogador para comparação.
        /// </summary>
        private RPGPlayer ClonePlayerData(RPGPlayer original)
        {
            var clone = new RPGPlayer();
            
            // Clonar níveis das classes
            foreach (var kvp in original.ClassLevels)
            {
                clone.ClassLevels[kvp.Key] = kvp.Value;
            }
            
            // Clonar XP das classes
            foreach (var kvp in original.ClassExperience)
            {
                clone.ClassExperience[kvp.Key] = kvp.Value;
            }
            
            // Clonar vitais
            clone.CurrentHunger = original.CurrentHunger;
            clone.CurrentSanity = original.CurrentSanity;
            clone.CurrentStamina = original.CurrentStamina;

            // Clonar proficiências de armadura
            foreach (var kvp in original.ArmorProficiencyLevels)
            {
                clone.ArmorProficiencyLevels[kvp.Key] = kvp.Value;
            }

            // Clonar XP de proficiência de armadura
            foreach (var kvp in original.ArmorProficiencyExperience)
            {
                clone.ArmorProficiencyExperience[kvp.Key] = kvp.Value;
            }
            
            return clone;
        }

        /// <summary>
        /// Atualiza a página atual com os dados mais recentes.
        /// </summary>
        private void UpdateCurrentPage()
        {
            // Check if player is available before trying to access
            var modPlayer = RPGUtils.GetLocalRPGPlayer();
            if (modPlayer == null) 
            {
                DebugLog.UI("SetPage", "Player not available, skipping update");
                return;
            }

            // Check if classes were initialized
            if (modPlayer.ClassLevels == null || modPlayer.ClassLevels.Count == 0)
            {
                DebugLog.UI("SetPage", "Classes not initialized, skipping update");
                return;
            }

            // Optimized update: only when changing tabs
            switch (_currentPage)
            {
                case MenuPage.Stats:
                    _statsPageUI.UpdateStats(modPlayer);
                    DebugLog.UI("SetPage", "Stats tab updated");
                    break;
                case MenuPage.Classes:
                    _classesPageUI.UpdateClasses(modPlayer);
                    DebugLog.UI("SetPage", "Classes tab updated");
                    break;
                case MenuPage.Skills:
                    _skillsPageUI.UpdateSkills(modPlayer);
                    DebugLog.UI("SetPage", "Skills tab updated");
                    break;
                case MenuPage.Progress:
                    _progressPageUI.UpdateProgress(modPlayer);
                    DebugLog.UI("SetPage", "Progress tab updated");
                    break;
                case MenuPage.Proficiencies:
                    _proficienciesPageUI.UpdateProficiencies(modPlayer);
                    DebugLog.UI("SetPage", "Proficiencies tab updated");
                    break;
            }
        }

        /// <summary>
        /// Força uma atualização da UI (chamado externamente).
        /// </summary>
        public void ForceUpdate()
        {
            _needsUpdate = true;
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