using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Skills;

namespace Wolfgodrpg.Common.UI.MasterUIElements
{
    public class ClassesPanel : UIPanel
    {
        private RPGPlayer _player;
        private UIList _subclassList;
        private UIList _skillsList;
        private UIScrollbar _subclassScrollbar;
        private UIScrollbar _skillsScrollbar;
        private PlayerSubClass _selectedSubclass;

        public ClassesPanel(RPGPlayer player)
        {
            _player = player;
            BackgroundColor = new Color(28, 35, 41) * 0.8f;
            BorderColor = new Color(89, 116, 213);
            SetPadding(10);

            // Title
            var title = new UIText("Subclasses & Skills", 1.1f, true);
            title.HAlign = 0.5f;
            title.Top.Set(5, 0);
            Append(title);

            // Subclasses section
            var subclassTitle = new UIText("Available Subclasses:", 0.9f, true);
            subclassTitle.Top.Set(35, 0);
            subclassTitle.Left.Set(5, 0);
            Append(subclassTitle);

            _subclassList = new UIList();
            _subclassList.Width.Set(0, 1f);
            _subclassList.Height.Set(150, 0);
            _subclassList.Top.Set(60, 0);
            _subclassList.ListPadding = 5f;
            Append(_subclassList);

            _subclassScrollbar = new UIScrollbar();
            _subclassScrollbar.Left.Set(-20, 1f);
            _subclassScrollbar.Top.Set(60, 0);
            _subclassScrollbar.Height.Set(150, 0);
            _subclassList.SetScrollbar(_subclassScrollbar);
            Append(_subclassScrollbar);

            // Skills section
            var skillsTitle = new UIText("Skills:", 0.9f, true);
            skillsTitle.Top.Set(220, 0);
            skillsTitle.Left.Set(5, 0);
            Append(skillsTitle);

            _skillsList = new UIList();
            _skillsList.Width.Set(0, 1f);
            _skillsList.Height.Set(-230, 1f);
            _skillsList.Top.Set(245, 0);
            _skillsList.ListPadding = 5f;
            Append(_skillsList);

            _skillsScrollbar = new UIScrollbar();
            _skillsScrollbar.Left.Set(-20, 1f);
            _skillsScrollbar.Top.Set(245, 0);
            _skillsScrollbar.Height.Set(-245, 1f);
            _skillsList.SetScrollbar(_skillsScrollbar);
            Append(_skillsScrollbar);

            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            _subclassList.Clear();
            _skillsList.Clear();

            if (_player.SubClasses?.SubClasses == null) return;

            // Add all subclasses
            foreach (var subClass in _player.SubClasses.SubClasses)
            {
                var card = new SubclassCard(subClass, _player.SubClasses.ActiveSubClass == subClass);
                card.OnLeftClick += (evt, elem) => SelectSubclass(subClass);
                _subclassList.Add(card);
            }

            // Show skills for active subclass or selected subclass
            var targetSubclass = _selectedSubclass ?? _player.SubClasses?.ActiveSubClass;
            if (targetSubclass != null)
            {
                // Movement skills
                _skillsList.Add(new SkillHeaderCard("Movement Skills"));
                foreach (var skill in _player.MovementSkills)
                {
                    _skillsList.Add(new SkillCard(skill));
                }

                // Subclass skills
                _skillsList.Add(new SkillHeaderCard($"{targetSubclass.Name} Skills"));
                foreach (var skill in targetSubclass.Skills)
                {
                    _skillsList.Add(new SkillCard(skill));
                }
            }
        }

        private void SelectSubclass(PlayerSubClass subClass)
        {
            _selectedSubclass = subClass;
            UpdateDisplay();
        }
    }

    public class SubclassCard : UIPanel
    {
        public SubclassCard(PlayerSubClass subClass, bool isActive)
        {
            Width.Set(0, 1f);
            Height.Set(50, 0);
            SetPadding(5);
            BackgroundColor = isActive ? new Color(70, 120, 200) : new Color(63, 82, 151) * 0.7f;
            BorderColor = isActive ? Color.Gold : Color.Gray;

            var iconText = new UIText(subClass.Icon);
            iconText.Left.Set(5, 0);
            iconText.Top.Set(5, 0);
            Append(iconText);

            var nameText = new UIText(subClass.Name);
            nameText.Left.Set(30, 0);
            nameText.Top.Set(5, 0);
            nameText.TextColor = subClass.GetClassColor();
            Append(nameText);

            var levelText = new UIText($"Level {subClass.Level}");
            levelText.Left.Set(30, 0);
            levelText.Top.Set(25, 0);
            levelText.TextColor = subClass.IsUnlocked ? Color.Green : Color.Gray;
            Append(levelText);

            var statusText = new UIText(isActive ? "Active" : (subClass.IsUnlocked ? "Unlocked" : "Locked"));
            statusText.Left.Set(150, 0);
            statusText.Top.Set(15, 0);
            statusText.TextColor = isActive ? Color.Gold : (subClass.IsUnlocked ? Color.Green : Color.Red);
            Append(statusText);
        }
    }

    public class SkillCard : UIPanel
    {
        public SkillCard(BaseSkill skill)
        {
            Width.Set(0, 1f);
            Height.Set(40, 0);
            SetPadding(5);
            BackgroundColor = skill.IsUnlocked ? new Color(63, 82, 151) * 0.7f : new Color(40, 40, 40);

            var nameText = new UIText(skill.Name);
            nameText.Left.Set(5, 0);
            nameText.Top.Set(5, 0);
            nameText.TextColor = skill.IsUnlocked ? Color.White : Color.Gray;
            Append(nameText);

            var statusText = new UIText(skill.IsUnlocked ? "Available" : "Locked");
            statusText.Left.Set(5, 0);
            statusText.Top.Set(20, 0);
            statusText.TextColor = skill.IsUnlocked ? Color.Green : Color.Red;
            Append(statusText);

            var cooldownText = new UIText($"CD: {skill.Cooldown / 60f:F1}s");
            cooldownText.Left.Set(150, 0);
            cooldownText.Top.Set(5, 0);
            cooldownText.TextColor = Color.Yellow;
            Append(cooldownText);
        }
    }

    public class SkillHeaderCard : UIPanel
    {
        public SkillHeaderCard(string title)
        {
            Width.Set(0, 1f);
            Height.Set(25, 0);
            SetPadding(5);
            BackgroundColor = new Color(70, 120, 200);

            var titleText = new UIText(title);
            titleText.Left.Set(5, 0);
            titleText.Top.Set(5, 0);
            titleText.TextColor = Color.White;
            Append(titleText);
        }
    }
}
