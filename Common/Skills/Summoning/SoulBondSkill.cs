using Terraria;
using Wolfgodrpg.Common.Skills;

namespace Wolfgodrpg.Common.Skills.Summoning
{
    public class SoulBondSkill : BaseSkill
    {
        public SoulBondSkill()
        {
            Name = "Soul Bond";
            Description = "Links your soul to your minions, sharing damage.";
            Cooldown = 180;
            StaminaCost = 20f;
            Level = 0;
        }

        protected override bool OnActivate(Player player)
        {
            // TODO: Implementar efeito real
            return true;
        }
    }
} 