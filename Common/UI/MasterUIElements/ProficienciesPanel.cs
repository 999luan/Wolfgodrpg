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
        private UIList _mainList;
        private UIScrollbar _mainScrollbar;

        public ProficienciesPanel(RPGPlayer player)
        {
            _player = player;
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _mainList = new UIList();
            _mainList.Width.Set(-20, 1f);
            _mainList.Height.Set(0, 1f);
            _mainList.Left.Set(0, 0);
            _mainList.Top.Set(0, 0);
            _mainList.ListPadding = 5f;
            Append(_mainList);

            _mainScrollbar = new UIScrollbar();
            _mainScrollbar.SetView(100f, 1000f); // Placeholder values
            _mainScrollbar.Height.Set(0, 1f);
            _mainScrollbar.Left.Set(-20, 1f);
            _mainScrollbar.Top.Set(0, 0);
            Append(_mainScrollbar);

            _mainList.SetScrollbar(_mainScrollbar);

            PopulateProficiencies();
        }

        private void PopulateProficiencies()
        {
            _mainList.Clear();

            // Weapon Proficiencies
            var weaponTitle = new UIText("Weapon Proficiencies", 1.1f, true);
            weaponTitle.HAlign = 0.5f;
            _mainList.Add(weaponTitle);

            foreach (var proficiency in _player.WeaponProficiencyLevels)
            {
                _mainList.Add(new ProficiencyRow(proficiency.Key.ToString(), proficiency.Value, _player.WeaponProficiencyExperience[proficiency.Key], 100 + (proficiency.Value * 50)));
            }

            // Armor Proficiencies
            var armorTitle = new UIText("Armor Proficiencies", 1.1f, true);
            armorTitle.HAlign = 0.5f;
            _mainList.Add(armorTitle);

            foreach (var proficiency in _player.ArmorProficiencyLevels)
            {
                _mainList.Add(new ProficiencyRow(proficiency.Key.ToString(), proficiency.Value, _player.ArmorProficiencyExperience[proficiency.Key], 100 + (proficiency.Value * 50)));
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
