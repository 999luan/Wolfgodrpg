using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Utils;
using Wolfgodrpg.Common.Systems;

namespace Wolfgodrpg.Common.UI.Menus
{
    /// <summary>
    /// Página de UI para mostrar proficiências de armadura.
    /// </summary>
    public class RPGProficienciesPageUI : UIElement
    {
        private List<UIElement> _proficiencyElements = new List<UIElement>();
        private UIScrollbar _scrollbar;
        private UIList _proficiencyList;

        public override void OnInitialize()
        {
            DebugLog.UI("OnInitialize", "Inicializando RPGProficienciesPageUI");
            
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            // Título da página
            var title = new UIText("Proficiências de Armadura", 1.2f, true);
            title.HAlign = 0.5f;
            title.Top.Set(10f, 0f);
            Append(title);

            // Lista de proficiências com scrollbar
            _proficiencyList = new UIList();
            _proficiencyList.Width.Set(-25f, 1f);
            _proficiencyList.Height.Set(-60f, 1f);
            _proficiencyList.Top.Set(50f, 0f);
            _proficiencyList.ListPadding = 5f;
            Append(_proficiencyList);

            _scrollbar = new UIScrollbar();
            _scrollbar.Width.Set(20f, 0f);
            _scrollbar.Height.Set(-60f, 1f);
            _scrollbar.Top.Set(50f, 0f);
            _scrollbar.Left.Set(-25f, 1f);
            _scrollbar.SetView(100f, 1000f);
            Append(_scrollbar);

            _proficiencyList.SetScrollbar(_scrollbar);
            
            DebugLog.UI("OnInitialize", "RPGProficienciesPageUI inicializado com sucesso");
        }

        /// <summary>
        /// Atualiza as proficiências exibidas.
        /// </summary>
        /// <param name="modPlayer">Jogador RPG</param>
        public void UpdateProficiencies(RPGPlayer modPlayer)
        {
            if (modPlayer == null)
            {
                DebugLog.UI("UpdateProficiencies", "Jogador não disponível");
                return;
            }

            _proficiencyList.Clear();
            _proficiencyElements.Clear();

            // Adicionar proficiências de armadura
            foreach (ArmorType armorType in System.Enum.GetValues<ArmorType>())
            {
                if (armorType == ArmorType.None) continue;

                var proficiencyPanel = CreateProficiencyPanel(armorType, modPlayer);
                _proficiencyList.Add(proficiencyPanel);
                _proficiencyElements.Add(proficiencyPanel);
            }

            DebugLog.UI("UpdateProficiencies", $"Atualizadas {_proficiencyElements.Count} proficiências");
        }

        /// <summary>
        /// Cria um painel para uma proficiência específica.
        /// </summary>
        /// <param name="armorType">Tipo de armadura</param>
        /// <param name="modPlayer">Jogador RPG</param>
        /// <returns>Painel da proficiência</returns>
        private UIPanel CreateProficiencyPanel(ArmorType armorType, RPGPlayer modPlayer)
        {
            var panel = new UIPanel();
            panel.Width.Set(0, 1f);
            panel.Height.Set(80f, 0f);
            panel.SetPadding(8f);

            // Nome do tipo de armadura
            string armorName = GetArmorTypeName(armorType);
            var nameText = new UIText(armorName, 1.1f, true);
            nameText.Top.Set(5f, 0f);
            panel.Append(nameText);

            // Nível atual
            int currentLevel = modPlayer.ArmorProficiencyLevels.TryGetValue(armorType, out var level) ? level : 1;
            var levelText = new UIText($"Nível: {currentLevel}", 0.9f);
            levelText.Top.Set(25f, 0f);
            levelText.Left.Set(10f, 0f);
            panel.Append(levelText);

            // XP atual
            float currentXP = modPlayer.ArmorProficiencyExperience.TryGetValue(armorType, out var xp) ? xp : 0f;
            float xpNeeded = GetXPNeeded(currentLevel);
            var xpText = new UIText($"XP: {currentXP:F0}/{xpNeeded:F0}", 0.9f);
            xpText.Top.Set(25f, 0f);
            xpText.Left.Set(150f, 0f);
            panel.Append(xpText);

            // Barra de progresso
            var progressBar = CreateProgressBar(currentXP, xpNeeded);
            progressBar.Top.Set(45f, 0f);
            progressBar.Left.Set(10f, 0f);
            progressBar.Width.Set(-20f, 1f);
            progressBar.Height.Set(15f, 0f);
            panel.Append(progressBar);

            // Bônus atual
            string bonusText = GetBonusText(armorType, currentLevel);
            var bonusLabel = new UIText(bonusText, 0.8f);
            bonusLabel.Top.Set(25f, 0f);
            bonusLabel.Left.Set(300f, 0f);
            panel.Append(bonusLabel);

            return panel;
        }

        /// <summary>
        /// Cria uma barra de progresso para o XP.
        /// </summary>
        /// <param name="currentXP">XP atual</param>
        /// <param name="xpNeeded">XP necessário</param>
        /// <returns>Barra de progresso</returns>
        private UIElement CreateProgressBar(float currentXP, float xpNeeded)
        {
            var container = new UIPanel();
            
            // Fundo da barra
            var background = new UIPanel();
            background.Width.Set(0, 1f);
            background.Height.Set(0, 1f);
            background.BackgroundColor = new Color(50, 50, 50);
            container.Append(background);

            // Barra de progresso
            float progress = xpNeeded > 0 ? currentXP / xpNeeded : 0f;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            
            var progressBar = new UIPanel();
            progressBar.Width.Set(progress, 1f);
            progressBar.Height.Set(0, 1f);
            progressBar.BackgroundColor = new Color(0, 150, 255);
            container.Append(progressBar);

            return container;
        }

        /// <summary>
        /// Obtém o nome amigável do tipo de armadura.
        /// </summary>
        /// <param name="armorType">Tipo de armadura</param>
        /// <returns>Nome amigável</returns>
        private string GetArmorTypeName(ArmorType armorType)
        {
            return armorType switch
            {
                ArmorType.Light => "Armadura Leve",
                ArmorType.Heavy => "Armadura Pesada",
                ArmorType.MagicRobes => "Vestes Mágicas",
                _ => "Desconhecido"
            };
        }

        /// <summary>
        /// Calcula o XP necessário para o próximo nível.
        /// </summary>
        /// <param name="level">Nível atual</param>
        /// <returns>XP necessário</returns>
        private float GetXPNeeded(int level)
        {
            return 100f + (level * 50f);
        }

        /// <summary>
        /// Obtém o texto do bônus atual.
        /// </summary>
        /// <param name="armorType">Tipo de armadura</param>
        /// <param name="level">Nível atual</param>
        /// <returns>Texto do bônus</returns>
        private string GetBonusText(ArmorType armorType, int level)
        {
            float bonus = (level - 1) * 0.02f; // +2% por nível
            
            return armorType switch
            {
                ArmorType.Light => $"Velocidade: +{bonus:P0}",
                ArmorType.Heavy => $"Defesa: +{level * 0.5f:F1}",
                ArmorType.MagicRobes => $"Mana: +{level * 2f:F0}",
                _ => "Sem bônus"
            };
        }
    }
} 