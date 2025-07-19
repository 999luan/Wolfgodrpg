using Terraria;
using Wolfgodrpg.Common.Skills;

namespace Wolfgodrpg.Common.Skills.Exploration
{
    public class DiscoverySkill : BaseSkill
    {
        public DiscoverySkill()
        {
            Name = "Discovery";
            Description = "Reveals hidden secrets nearby.";
            Cooldown = 150;
            StaminaCost = 12f;
            Level = 0;
        }

        protected override bool OnActivate(Player player)
        {
            // TODO: Implementar efeito real
            return true;
        }
    }
} 