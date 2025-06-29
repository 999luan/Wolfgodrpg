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
using System.Linq;
using Wolfgodrpg.Common.Classes;

namespace Wolfgodrpg.Common.UI
{
    public class RPGStatsPageUI : UIElement
    {
        private UIElement _statsContainer;

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);

            _statsContainer = new UIElement();
            _statsContainer.Width.Set(0, 1f);
            _statsContainer.Height.Set(0, 1f);
            _statsContainer.SetPadding(10);
            Append(_statsContainer);
        }

        public void UpdateStats(RPGPlayer modPlayer)
        {
            _statsContainer.RemoveAllChildren();
            if (modPlayer == null) return;

            var stats = RPGCalculations.CalculateTotalStats(modPlayer);
            var player = modPlayer.Player;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Health: {player.statLife} / {player.statLifeMax2}");
            sb.AppendLine($"Mana: {player.statMana} / {player.statManaMax2}");
            sb.AppendLine($"Defense: {player.statDefense}");
            sb.AppendLine("");
            foreach (var classEntry in RPGClassDefinitions.ActionClasses)
            {
                string classKey = classEntry.Key;
                var classInfo = classEntry.Value;
                float level = modPlayer.GetClassLevel(classKey);
                float currentExp = 0;
                modPlayer.ClassExperience.TryGetValue(classKey, out currentExp);
                float nextLevelExp = 100 * (level + 1);
                sb.AppendLine($"Classe {classInfo.Name} lv {level} xp:{currentExp}/{nextLevelExp}");
            }
            sb.AppendLine("");
            foreach(var stat in stats)
            {
                sb.AppendLine($"{stat.Key}: {stat.Value:F2}");
            }
            UIText text = new UIText(sb.ToString());
            _statsContainer.Append(text);
        }
    }
}
