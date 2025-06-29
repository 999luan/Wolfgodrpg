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
    // Aba de Classes do menu RPG
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

        // Atualiza o conteúdo da aba de classes
        public void UpdateClasses(RPGPlayer modPlayer)
        {
            _classesList.Clear();
            if (modPlayer == null || modPlayer.Player == null) return;

            bool foundClasses = false;
            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                var classInfo = classEntry.Value;
                float level = modPlayer.GetClassLevel(classEntry.Key);
                float currentExp = 0;
                modPlayer.ClassExperience.TryGetValue(classEntry.Key, out currentExp);
                float nextLevelExp = 100 * (level + 1);
                
                _classesList.Add(new ClassEntry(classInfo.Name, level, currentExp, nextLevelExp, classInfo.Milestones));
                foundClasses = true;
            }

            if (!foundClasses)
            {
                _classesList.Add(new UIText("Nenhuma classe encontrada."));
            }
        }

        // Elemento visual para cada classe
        private class ClassEntry : UIElement
        {
            public ClassEntry(string className, float level, float currentExp, float nextLevelExp, Dictionary<int, string> milestones)
            {
                Width.Set(0, 1f);
                Height.Set(60f, 0f);
                SetPadding(5);

                var text = new UIText($"{className}: Nv.{level:F0} XP:{currentExp:F0}/{nextLevelExp:F0}");
                Append(text);
                
                // Mostrar próximas milestones se houver
                if (milestones != null && milestones.Count > 0)
                {
                    var nextMilestone = milestones.FirstOrDefault(m => m.Key > level);
                    if (!string.IsNullOrEmpty(nextMilestone.Value))
                    {
                        var milestoneText = new UIText($"Próxima habilidade (Nv.{nextMilestone.Key}): {nextMilestone.Value}", 0.8f);
                        milestoneText.Top.Set(20f, 0f);
                        Append(milestoneText);
                    }
                }
            }
        }
    }
}
