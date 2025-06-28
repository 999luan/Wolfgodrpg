using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.GlobalItems;
using Wolfgodrpg.Common.Systems;

namespace Wolfgodrpg.Common.UI
{
    public enum MenuPage
    {
        Stats = 0,
        Skills = 1,
        Items = 2,
        Progress = 3,
        Info = 4
    }

    // Main UI State class for the RPG menu
    public class SimpleRPGMenu : UIState
    {
        private UIPanel mainPanel;
        private UIText pageTitle;
        private UIText pageContent;
        private UIImageButton nextButton;
        private UIImageButton prevButton;
        private UIImageButton closeButton;

        private bool visible = false;
        private MenuPage currentPage = MenuPage.Stats;

        public override void OnInitialize()
        {
            mainPanel = new UIPanel();
            mainPanel.Width.Set(600f, 0f);
            mainPanel.Height.Set(400f, 0f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            mainPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
            Append(mainPanel);

            pageTitle = new UIText("", 1.2f);
            pageTitle.HAlign = 0.5f;
            pageTitle.Top.Set(10f, 0f);
            pageTitle.TextColor = Color.White;
            mainPanel.Append(pageTitle);

            pageContent = new UIText("", 1f);
            pageContent.Width.Set(550f, 0f);
            pageContent.Height.Set(300f, 0f);
            pageContent.Left.Set(25f, 0f);
            pageContent.Top.Set(50f, 0f);
            pageContent.TextColor = Color.LightGray;
            mainPanel.Append(pageContent);

            nextButton = new UIImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonNext"));
            nextButton.Width.Set(30f, 0f);
            nextButton.Height.Set(30f, 0f);
            nextButton.HAlign = 0.95f;
            nextButton.VAlign = 0.95f;
            nextButton.OnLeftClick += (evt, element) => { CurrentPage = (MenuPage)(((int)currentPage + 1) % 5); };
            mainPanel.Append(nextButton);

            prevButton = new UIImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonPrevious"));
            prevButton.Width.Set(30f, 0f);
            prevButton.Height.Set(30f, 0f);
            prevButton.HAlign = 0.05f;
            prevButton.VAlign = 0.95f;
            prevButton.OnLeftClick += (evt, element) => { CurrentPage = (MenuPage)(((int)currentPage + 4) % 5); };
            mainPanel.Append(prevButton);

            closeButton = new UIImageButton(ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonClose"));
            closeButton.Width.Set(30f, 0f);
            closeButton.Height.Set(30f, 0f);
            closeButton.HAlign = 0.95f;
            closeButton.Top.Set(10f, 0f);
            closeButton.OnLeftClick += (evt, element) => { Hide(); };
            mainPanel.Append(closeButton);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!visible) return;

            var player = Main.LocalPlayer;
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            var config = ModContent.GetInstance<RPGConfig>();

            UpdatePage(modPlayer, config);
        }

        private void UpdatePage(RPGPlayer modPlayer, RPGConfig config)
        {
            switch (currentPage)
            {
                case MenuPage.Stats:
                    UpdateStatsPage(modPlayer);
                    break;
                case MenuPage.Skills:
                    UpdateSkillsPage(modPlayer);
                    break;
                case MenuPage.Items:
                    UpdateItemsPage();
                    break;
                case MenuPage.Progress:
                    UpdateProgressPage(modPlayer);
                    break;
                case MenuPage.Info:
                    UpdateInfoPage();
                    break;
            }
        }

        private void UpdateStatsPage(RPGPlayer modPlayer)
        {
            pageTitle.SetText("Status do Personagem");
            string stats = "";
            stats += $"Vida: {Main.LocalPlayer.statLife}/{Main.LocalPlayer.statLifeMax2}\n";
            stats += $"Mana: {Main.LocalPlayer.statMana}/{Main.LocalPlayer.statManaMax2}\n";
            stats += $"Defesa: {Main.LocalPlayer.statDefense}\n";
            stats += $"Velocidade: {Main.LocalPlayer.moveSpeed:F2}x\n";
            pageContent.SetText(stats);
        }

        private void UpdateSkillsPage(RPGPlayer modPlayer)
        {
            pageTitle.SetText("Habilidades Desbloqueadas");
            StringBuilder skills = new StringBuilder();
            skills.AppendLine("=== Habilidades Desbloqueadas ===");

            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                string className = classEntry.Key;
                var classInfo = classEntry.Value;
                float level = modPlayer.GetClassLevel(className);

                skills.AppendLine($"\n{classInfo.Name} (Nível {level:F0}):");
                foreach (var milestone in classInfo.Milestones)
                {
                    string abilityId = $"{className}_{milestone.Key}";
                    bool unlocked = modPlayer.HasUnlockedAbility(abilityId);
                    string status = unlocked ? "(Desbloqueado)" : "";
                    skills.AppendLine($"  - Nível {milestone.Key}: {milestone.Value} {status}");
                }
            }
            pageContent.SetText(skills.ToString());
        }

        private void UpdateItemsPage()
        {
            pageTitle.SetText("Itens Equipados");
            string items = "=== Itens Equipados ===\n";
            // TODO: Implement logic to display equipped items and their RPG stats
            items += "(Funcionalidade a ser implementada)";
            pageContent.SetText(items);
        }

        private void UpdateProgressPage(RPGPlayer modPlayer)
        {
            pageTitle.SetText("Progresso Geral");
            string progress = "=== Progresso Geral ===\n";
            // TODO: Implement logic to display overall game progression and RPG stats
            progress += "(Funcionalidade a ser implementada)";
            pageContent.SetText(progress);
        }

        private void UpdateInfoPage()
        {
            pageTitle.SetText("Informações");
            string info = "=== Informações ===\n";
            info += "Pressione 'M' para abrir/fechar o menu.\n";
            info += "Pressione 'Q'/'E' para navegar entre as páginas.\n";
            pageContent.SetText(info);
        }

        public void Show()
        {
            visible = true;
        }

        public void Hide()
        {
            visible = false;
        }

        public void Toggle()
        {
            visible = !visible;
        }

        public bool IsVisible()
        {
            return visible;
        }

        public MenuPage CurrentPage
        {
            get => currentPage;
            set
            {}
                currentPage = value;
                UpdatePage(Main.LocalPlayer.GetModPlayer<RPGPlayer>(), ModContent.GetInstance<RPGConfig>());
            }
        }
    }
}