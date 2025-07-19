using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills.Summoning
{
    public class MinionBoostSkill : BaseSkill
    {
        public MinionBoostSkill()
        {
            Name = "Minion Boost";
            Description = "Increases minion damage and count.";
            Cooldown = 180; // 3 seconds
            StaminaCost = 20f;
            Level = 0; // Unlocked at level 3
        }

        protected override bool OnActivate(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer == null) return false;

            // Apply minion boost buff
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