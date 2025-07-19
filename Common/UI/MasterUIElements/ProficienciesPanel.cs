using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Wolfgodrpg.Common.Players;
using System.Collections.Generic;

namespace Wolfgodrpg.Common.UI.MasterUIElements
{
    public class ProficienciesPanel : UIPanel
    {
        private readonly RPGPlayer _player;

        public ProficienciesPanel(RPGPlayer player)
        {
            _player = player;
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            var mainList = new UIList();
            mainList.Width.Set(0, 1f);
            mainList.Height.Set(0, 1f);
            Append(mainList);

            // Weapon Proficiencies
            var weaponTitle = new UIText("Weapon Proficiencies", 1.1f, true);
            weaponTitle.HAlign = 0.5f;
            mainList.Add(weaponTitle);

            foreach (var proficiency in _player.WeaponProficiencyLevels)
            {
                mainList.Add(new ProficiencyRow(proficiency.Key.ToString(), proficiency.Value, _player.WeaponProficiencyExperience[proficiency.Key], 100 + (proficiency.Value * 50)));
            }

            // Armor Proficiencies
            var armorTitle = new UIText("Armor Proficiencies", 1.1f, true);
            armorTitle.HAlign = 0.5f;
            armorTitle.Top.Set(20, 0); // Add some space
            mainList.Add(armorTitle);

            foreach (var proficiency in _player.ArmorProficiencyLevels)
            {
                mainList.Add(new ProficiencyRow(proficiency.Key.ToString(), proficiency.Value, _player.ArmorProficiencyExperience[proficiency.Key], 100 + (proficiency.Value * 50)));
            }
        }
    }

    // Helper class for a single proficiency row
    public class ProficiencyRow : UIPanel
    {
        public ProficiencyRow(string name, int level, float currentXp, float xpToNextLevel)
        {
            Width.Set(0, 1f);
            Height.Set(50, 0);
            BackgroundColor = new Color(40, 50, 60) * 0.8f;
            MarginTop = 5;

            var nameText = new UIText($"{name} - Lv. {level}");
            nameText.Left.Set(10, 0);
            nameText.Top.Set(5, 0);
            Append(nameText);

            var xpBar = new UIProgressBar();
            xpBar.Width.Set(-20, 1f);
            xpBar.Height.Set(15, 0);
            xpBar.Top.Set(25, 0);
            xpBar.HAlign = 0.5f;
            xpBar.SetProgress(currentXp / xpToNextLevel);
            Append(xpBar);
        }
    }
}