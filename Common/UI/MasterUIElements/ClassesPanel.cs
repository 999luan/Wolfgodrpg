using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Classes;
using System.Collections.Generic;
using System.Linq;
using Wolfgodrpg.Common.Skills;

namespace Wolfgodrpg.Common.UI.MasterUIElements
{
    public class ClassesPanel : UIPanel
    {
        private readonly RPGPlayer _player;
        private UIList subclassTabsList;
        private UIPanel skillsPanel;

        public ClassesPanel(RPGPlayer player)
        {
            _player = player;
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            // Container for the two main sections
            var mainContainer = new UIElement();
            mainContainer.Width.Set(0, 1f);
            mainContainer.Height.Set(0, 1f);
            Append(mainContainer);

            // Subclass list on the left
            subclassTabsList = new UIList();
            subclassTabsList.Width.Set(180, 0);
            subclassTabsList.Height.Set(0, 1f);
            subclassTabsList.ListPadding = 8;
            mainContainer.Append(subclassTabsList);

            // Panel for skills on the right
            skillsPanel = new UIPanel();
            skillsPanel.Width.Set(-190, 1f);
            skillsPanel.Height.Set(0, 1f);
            skillsPanel.Left.Set(190, 0);
            skillsPanel.BackgroundColor = new Color(40, 50, 60) * 0.8f;
            mainContainer.Append(skillsPanel);

            PopulateSubclassList();
            // Select the first subclass by default
            if (_player.SubClasses.SubClasses.Any(s => s.IsUnlocked))
            {
                ShowSkillsFor(_player.SubClasses.SubClasses.First(s => s.IsUnlocked));
            }
        }

        private void PopulateSubclassList()
        {
            subclassTabsList.Clear();
            var unlockedSubclasses = _player.SubClasses.SubClasses.Where(s => s.IsUnlocked).ToList();

            foreach (var subclass in unlockedSubclasses)
            {
                var tab = new UITextPanel<string>($"{subclass.Name} (Lv {subclass.Level})", 1f, false);
                tab.Width.Set(0, 1f);
                tab.Height.Set(40, 0);
                tab.OnLeftClick += (evt, elem) => ShowSkillsFor(subclass);
                subclassTabsList.Add(tab);
            }
        }

        private void ShowSkillsFor(PlayerSubClass subclass)
        {
            skillsPanel.RemoveAllChildren();

            var title = new UIText($"{subclass.Name} - Level {subclass.Level}", 1.1f, true);
            title.HAlign = 0.5f;
            skillsPanel.Append(title);

            var xpBar = new UI.UIProgressBar(); // Using the progress bar from the old UI
            xpBar.Width.Set(-20, 1f);
            xpBar.Height.Set(20, 0);
            xpBar.Top.Set(40, 0);
            xpBar.HAlign = 0.5f;
            xpBar.SetProgress(subclass.GetXPProgress());
            skillsPanel.Append(xpBar);

            var skillsList = new UIList();
            skillsList.Width.Set(-25, 1f);
            skillsList.Height.Set(-80, 1f);
            skillsList.Top.Set(70, 0);
            skillsList.ListPadding = 5;
            skillsPanel.Append(skillsList);

            foreach (var skill in subclass.Skills)
            {
                skillsList.Add(new SkillItemUI(skill)); // Using the skill item from the old UI
            }
        }
    }
}