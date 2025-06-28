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
        private UIPanel mainPanel;
        private UIText pageTitle;
        private UIText pageContent;
        private UIImageButton nextButton;
        private UIImageButton prevButton;
        private UIImageButton closeButton;

        private bool isVisible = false;
        private MenuPage currentPage = MenuPage.Stats;

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            mainPanel.Width.Set(600f, 0f);
            mainPanel.Height.Set(450f, 0f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            mainPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            Append(mainPanel);

            pageTitle = new UIText("", 1.2f, true);
            pageTitle.HAlign = 0.5f;
            pageTitle.Top.Set(15f, 0f);
            mainPanel.Append(pageTitle);

            pageContent = new UIText("", 0.9f);
            pageContent.Width.Set(0, 0.9f);
            pageContent.Height.Set(0, 0.8f);
            pageContent.HAlign = 0.5f;
            pageContent.VAlign = 0.5f;
            pageContent.Top.Set(20f, 0f);
            mainPanel.Append(pageContent);

            closeButton = new UIImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonClose"));
            closeButton.HAlign = 0.95f;
            closeButton.Top.Set(10f, 0f);
            closeButton.OnLeftClick += (evt, elm) => Hide();
            mainPanel.Append(closeButton);

            prevButton = new UIImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPrevious"));
            prevButton.HAlign = 0.1f;
            prevButton.VAlign = 0.9f;
            prevButton.OnLeftClick += (evt, elm) => ChangePage(false);
            mainPanel.Append(prevButton);

            nextButton = new UIImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonNext"));
            nextButton.HAlign = 0.9f;
            nextButton.VAlign = 0.9f;
            nextButton.OnLeftClick += (evt, elm) => ChangePage(true);
            mainPanel.Append(nextButton);
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
            var player = modPlayer.Player;
            var totalStats = RPGCalculations.CalculateTotalStats(modPlayer);
            var sb = new StringBuilder();

            sb.AppendLine($"Vida: {player.statLife} / {player.statLifeMax2}");
            sb.AppendLine($"Mana: {player.statMana} / {player.statManaMax2}");
            sb.AppendLine($"Defesa: {player.statDefense}");
            sb.AppendLine($"
--- Bônus Totais ---");

            foreach (var stat in totalStats)
            {
                string statName = RPGClassDefinitions.RandomStats.ContainsKey(stat.Key) ? RPGClassDefinitions.RandomStats[stat.Key].Name : stat.Key;
                string valueString = stat.Value < 1 && stat.Value > 0 ? $"+{stat.Value:P1}" : $"+{stat.Value:F2}";
                sb.AppendLine($"{statName}: {valueString}");
            }

            pageContent.SetText(sb.ToString());
        }

        private void ShowClassesPage(RPGPlayer modPlayer)
        {
            pageTitle.SetText("Classes e Habilidades");
            var sb = new StringBuilder();

            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                string className = classEntry.Key;
                var classInfo = classEntry.Value;
                float level = modPlayer.GetClassLevel(className);
                float currentExp = modPlayer.ClassExperience.ContainsKey(className) ? modPlayer.ClassExperience[className] : 0;
                
                sb.AppendLine($"--- {classInfo.Name} ---");
                sb.AppendLine($"Nível: {level:F0} (XP: {currentExp:F0})");

                foreach (var milestone in classInfo.Milestones)
                {
                    string abilityId = $"{className}_{milestone.Key}";
                    bool unlocked = modPlayer.HasUnlockedAbility(abilityId);
                    string status = unlocked ? "[Desbloqueado]" : "[Bloqueado]";
                    sb.AppendLine($"  Nível {milestone.Key}: {milestone.Value} {status}");
                }
                sb.AppendLine();
            }

            pageContent.SetText(sb.ToString());
        }

        private void ShowItemsPage()
        {
            pageTitle.SetText("Itens com Atributos");
            var sb = new StringBuilder();
            var player = Main.LocalPlayer;

            // Armadura e Acessórios
            for (int i = 0; i < player.armor.Length; i++)
            {
                Item item = player.armor[i];
                if (item != null && !item.IsAir && item.TryGetGlobalItem(out RPGGlobalItem globalItem) && globalItem.randomStats.Count > 0)
                {
                    sb.AppendLine($"--- {item.Name} ---");
                    foreach (var stat in globalItem.randomStats)
                    {
                        var statInfo = RPGClassDefinitions.RandomStats[stat.Key];
                        string valueString = stat.Value < 1 ? $"+{stat.Value:P1}" : $"+{stat.Value:F0}";
                        sb.AppendLine($"  {valueString} {statInfo.Name}");
                    }
                    sb.AppendLine();
                }
            }

            // Item Segurado
            Item heldItem = player.HeldItem;
            if (heldItem != null && !heldItem.IsAir && heldItem.TryGetGlobalItem(out RPGGlobalItem heldGlobalItem) && heldGlobalItem.randomStats.Count > 0)
            {
                sb.AppendLine($"--- {heldItem.Name} (Segurando) ---");
                foreach (var stat in heldGlobalItem.randomStats)
                {
                    var statInfo = RPGClassDefinitions.RandomStats[stat.Key];
                    string valueString = stat.Value < 1 ? $"+{stat.Value:P1}" : $"+{stat.Value:F0}";
                    sb.AppendLine($"  {valueString} {statInfo.Name}");
                }
            }

            if (sb.Length == 0)
            {
                sb.AppendLine("Nenhum item com atributos equipado.");
            }

            pageContent.SetText(sb.ToString());
        }

        private void ShowProgressPage(RPGPlayer modPlayer)
        {
            pageTitle.SetText("Progresso do Jogo");
            var sb = new StringBuilder();
            
            sb.AppendLine("--- Chefes Derrotados ---");
            sb.AppendLine($"Olho de Cthulhu: {(NPC.downedBoss1 ? "Sim" : "Não")}");
            sb.AppendLine($"Devorador/Cérebro: {(NPC.downedBoss2 ? "Sim" : "Não")}");
            sb.AppendLine($"Esqueletron: {(NPC.downedBoss3 ? "Sim" : "Não")}");
            sb.AppendLine($"Rainha Abelha: {(NPC.downedQueenBee ? "Sim" : "Não")}");
            sb.AppendLine($"Parede de Carne: {(Main.hardMode ? "Sim" : "Não")}");
            sb.AppendLine($"Plantera: {(NPC.downedPlantBoss ? "Sim" : "Não")}");
            sb.AppendLine($"Golem: {(NPC.downedGolemBoss ? "Sim" : "Não")}");
            sb.AppendLine($"Senhor da Lua: {(NPC.downedMoonlord ? "Sim" : "Não")}");

            pageContent.SetText(sb.ToString());
        }

        private void ChangePage(bool forward)
        {
            int pageIndex = (int)currentPage;
            int pageCount = Enum.GetNames(typeof(MenuPage)).Length;
            pageIndex = (pageIndex + (forward ? 1 : -1) + pageCount) % pageCount;
            currentPage = (MenuPage)pageIndex;
        }

        public void Show() { isVisible = true; }
        public void Hide() { isVisible = false; }
        public void Toggle() { isVisible = !isVisible; }
        public bool IsVisible() => isVisible;
    }
}