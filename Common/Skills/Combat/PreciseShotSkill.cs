using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills.Combat
{
    public class PreciseShotSkill : BaseSkill
    {
        public PreciseShotSkill()
        {
            Name = "Precise Shot";
            Description = "A highly accurate shot that deals bonus damage.";
            Cooldown = 120; // 2 seconds
            StaminaCost = 15f;
            Level = 1; // Unlocked by default
        }

        protected override bool OnActivate(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer == null) return false;

            // Apply precision buff
            player.AddBuff(Terraria.ID.BuffID.AmmoBox, 300); // 5 seconds
            player.AddBuff(Terraria.ID.BuffID.Archery, 300);

            // Visual and sound effects
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.Gold,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, Color.Gold);
            }
            SoundEngine.PlaySound(SoundID.Item5, player.position);

            return true;
        }
    }
} 