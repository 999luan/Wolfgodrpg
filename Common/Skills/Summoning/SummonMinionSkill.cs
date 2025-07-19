using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills.Summoning
{
    public class SummonMinionSkill : BaseSkill
    {
        public SummonMinionSkill()
        {
            Name = "Summon Minion";
            Description = "Summons a powerful minion to fight for you.";
            Cooldown = 120; // 2 seconds
            StaminaCost = 20f;
            Level = 1; // Unlocked by default
        }

        protected override bool OnActivate(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer == null) return false;

            // Apply summoning buff
            player.AddBuff(Terraria.ID.BuffID.Summoning, 600); // 10 seconds
            player.AddBuff(Terraria.ID.BuffID.Bewitched, 600);

            // Visual and sound effects
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.PurpleTorch,
                    Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 0, Color.Purple);
            }
            SoundEngine.PlaySound(SoundID.Item6, player.position);

            return true;
        }
    }
} 