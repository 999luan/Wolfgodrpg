using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;
using System.Text;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes;

namespace Wolfgodrpg.Common.UI
{
    public class RPGClassesPageUI : UIElement
    {
        private UIElement _classesContainer;
        private bool _visible = false;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            _classesContainer = new UIElement();
            _classesContainer.Width.Set(0, 1f);
            _classesContainer.Height.Set(0, 1f);
            _classesContainer.SetPadding(0);
            Append(_classesContainer);
        }

        public void UpdateClasses(RPGPlayer modPlayer)
        {
            _classesContainer.RemoveAllChildren(); // Clear previous classes

            float currentY = 0f;

            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                string className = classEntry.Key;
                var classInfo = classEntry.Value;
                float level = modPlayer.GetClassLevel(className);
                float currentExp = modPlayer.ClassExperience.ContainsKey(className) ? modPlayer.ClassExperience[className] : 0;

                ClassEntry classUI = new ClassEntry(classInfo.Name, level, currentExp, classInfo.Milestones, modPlayer, className);
                classUI.Top.Set(currentY, 0f);
                classUI.Left.Set(0, 0.05f);
                classUI.Width.Set(0, 0.9f);
                classUI.Height.Set(100f, 0f); // Adjust height as needed
                _classesContainer.Append(classUI);

                currentY += classUI.Height.Pixels + 10f; // Spacing between classes
            }
        }

        public void Activate()
        {
            _visible = true;
        }

        public void Deactivate()
        {
            _visible = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!_visible) return;
            base.DrawSelf(spriteBatch);
        }

        private class ClassEntry : UIElement
        {
            private UIText _classNameText;
            private UIText _levelText;
            private UIText _expText;
            private UIElement _milestonesContainer;

            public ClassEntry(string className, float level, float currentExp, Dictionary<int, string> milestones, RPGPlayer modPlayer, string classKey)
            {
                Width.Set(0, 1f);
                Height.Set(100f, 0f); // Initial height, will be adjusted

                _classNameText = new UIText($"--- {className} ---", 1f, true);
                _classNameText.HAlign = 0.5f;
                _classNameText.Top.Set(0f, 0f);
                Append(_classNameText);

                _levelText = new UIText($"Nível: {level:F0}", 0.9f);
                _levelText.HAlign = 0.5f;
                _levelText.Top.Set(20f, 0f);
                Append(_levelText);

                _expText = new UIText($"XP: {currentExp:F0}", 0.9f);
                _expText.HAlign = 0.5f;
                _expText.Top.Set(40f, 0f);
                Append(_expText);

                _milestonesContainer = new UIElement();
                _milestonesContainer.Width.Set(0, 1f);
                _milestonesContainer.Height.Set(0, 1f);
                _milestonesContainer.Top.Set(60f, 0f);
                _milestonesContainer.SetPadding(0);
                Append(_milestonesContainer);

                float milestoneY = 0f;
                foreach (var milestone in milestones)
                {
                    string abilityId = $"{classKey}_{milestone.Key}";
                    bool unlocked = modPlayer.HasUnlockedAbility(abilityId);
                    string status = unlocked ? "[Desbloqueado]" : "[Bloqueado]";
                    UIText milestoneText = new UIText($"  Nível {milestone.Key}: {milestone.Value} {status}", 0.8f);
                    milestoneText.Top.Set(milestoneY, 0f);
                    milestoneText.Left.Set(20f, 0f);
                    _milestonesContainer.Append(milestoneText);
                    milestoneY += 15f;
                }

                // Adjust height of the ClassEntry based on content
                Height.Set(60f + milestoneY, 0f);
            }
        }
    }
}
