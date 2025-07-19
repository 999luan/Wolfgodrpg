using Terraria;
using Wolfgodrpg.Common.Skills;

namespace Wolfgodrpg.Common.Skills.Exploration
{
    public class TreasureHuntSkill : BaseSkill
    {
        public TreasureHuntSkill()
        {
            Name = "Treasure Hunt";
            Description = "Increases chance to find treasures.";
            Cooldown = 120;
            StaminaCost = 10f;
            Level = 0;
        }

        protected override bool OnActivate(Player player)
        {
            // TODO: Implementar efeito real
            return true;
        }
    }
} 