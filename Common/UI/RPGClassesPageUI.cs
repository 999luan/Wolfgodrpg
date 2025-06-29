using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.ModLoader;
using Wolfgodrpg.Common.Players;
using Wolfgodrpg.Common.Systems;
using System.Text;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes;
using System.Linq;

namespace Wolfgodrpg.Common.UI
{
    public class RPGClassesPageUI : UIElement
    {
        private UIList _classesList;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            _classesList = new UIList();
            _classesList.Width.Set(0, 1f);
            _classesList.Height.Set(0, 1f);
            _classesList.ListPadding = 5f;
            Append(_classesList);
        }

        public void UpdateClasses(RPGPlayer modPlayer)
        {
            _classesList.Clear();
            if (modPlayer == null) return;
            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                var classInfo = classEntry.Value;
                float level = modPlayer.GetClassLevel(classEntry.Key);
                float currentExp = 0;
                modPlayer.ClassExperience.TryGetValue(classEntry.Key, out currentExp);
                float nextLevelExp = 100 * (level + 1); // Valor fixo para XP do próximo nível
                ClassEntry entry = new ClassEntry($"Classe {classInfo.Name} lv {level} xp:{currentExp}/{nextLevelExp}", level, currentExp, classInfo.Milestones, modPlayer, classEntry.Key);
                _classesList.Add(entry);
            }
        }

        private class ClassEntry : UIElement
        {
            public ClassEntry(string className, float level, float currentExp, Dictionary<int, string> milestones, RPGPlayer modPlayer, string classKey)
            {
                Width.Set(0, 1f);
                Height.Set(100f, 0f);
                SetPadding(5);

                var text = new UIText($"--- {className} ---");
                Append(text);
                // ... More detailed UI for each class entry can be added here
            }
        }
    }
}
