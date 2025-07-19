using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Wolfgodrpg.Common.Players;

namespace Wolfgodrpg.Common.Skills.Combat
{
    public class PowerShotSkill : BaseSkill
    {
        public PowerShotSkill()
        {
            Name = "Power Shot";
            Description = "A powerful shot that deals massive damage.";
            Cooldown = 300; // 5 seconds
            StaminaCost = 25f;
            Level = 0; // Unlocked at level 5
        }

        protected override bool OnActivate(Player player)
        {
            var modPlayer = player.GetModPlayer<RPGPlayer>();
            if (modPlayer == null) return false;

            // Apply power shot buff
            player.AddBuff(Terraria.ID.BuffID.Wrath, 300); // 5 seconds
            player.AddBuff(Terraria.ID.BuffID.Archery, 300);

            // Visual and sound effects
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustDirect(player.position, player.width, player.height, DustID.Gold,
                    Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f), 0, Color.Gold);
            }
            SoundEngine.PlaySound(SoundID.Item1, player.position);

            return true;
        }
    }
} 