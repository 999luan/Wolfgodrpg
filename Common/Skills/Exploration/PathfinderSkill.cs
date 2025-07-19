using Terraria;
using Wolfgodrpg.Common.Skills;

namespace Wolfgodrpg.Common.Skills.Exploration
{
    public class PathfinderSkill : BaseSkill
    {
        public PathfinderSkill()
        {
            Name = "Pathfinder";
            Description = "Improves movement in unexplored areas.";
            Cooldown = 90;
            StaminaCost = 8f;
            Level = 0;
        }

        protected override bool OnActivate(Player player)
        {
            // TODO: Implementar efeito real
            return true;
        }
    }
} 